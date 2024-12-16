using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	public class Segment
	{
		[Flags]
		public enum DeleteTagsAction
		{
			DeleteNone = 0x0,
			DeleteTextPlaceholders = 0x1,
			DeleteStandaloneTags = 0x2,
			DeletePairedTags = 0x4,
			DeleteAll = 0x7,
			KeepTextPlaceholders = 0x6
		}

		public enum ValidationMode
		{
			ReportAllErrors,
			IgnorePairedTagErrors
		}

		public bool IsEmpty
		{
			get
			{
				if (Elements != null)
				{
					return Elements.Count == 0;
				}
				return true;
			}
		}

		[XmlArrayItem(typeof(Text))]
		[XmlArrayItem(typeof(Tag))]
		[XmlArrayItem(typeof(Token))]
		[DataMember]
		public List<SegmentElement> Elements
		{
			get;
			set;
		}

		[XmlIgnore]
		public SegmentElement LastElement
		{
			get
			{
				if (Elements != null && Elements.Count != 0)
				{
					return Elements[Elements.Count - 1];
				}
				return null;
			}
			set
			{
				Elements[Elements.Count - 1] = value;
			}
		}

		[XmlIgnore]
		[DataMember]
		public List<Token> Tokens
		{
			get;
			set;
		}

		public bool HasPlaceables
		{
			get
			{
				if (!Elements.Any((SegmentElement se) => se is Tag))
				{
					if (Tokens != null)
					{
						return Tokens.Any((Token t) => t.IsPlaceable);
					}
					return false;
				}
				return true;
			}
		}

		public bool HasTags => Elements.Any((SegmentElement se) => se is Tag);

		public bool HasPairedTags => Elements.Any(delegate(SegmentElement se)
		{
			Tag tag = se as Tag;
			return tag != null && tag.Type == TagType.Start;
		});

		[DataMember]
		public string CultureName
		{
			get
			{
				return Culture.Name;
			}
			set
			{
				Culture = CultureInfoExtensions.GetCultureInfo(value);
			}
		}

		[XmlIgnore]
		public CultureInfo Culture
		{
			get;
			set;
		}

		public Segment()
		{
			Elements = new List<SegmentElement>();
			Culture = CultureInfo.InvariantCulture;
		}

		public Segment(CultureInfo culture)
		{
			Elements = new List<SegmentElement>();
			Culture = culture;
		}

		public bool Equals(Segment other)
		{
			if (!Culture.Equals(other.Culture))
			{
				return false;
			}
			if (Elements == null || other.Elements == null)
			{
				return false;
			}
			if (Elements.Count != other.Elements.Count)
			{
				return false;
			}
			return !Elements.Where((SegmentElement t, int i) => !t.Equals(other.Elements[i])).Any();
		}

		public bool WeakEquals(Segment other)
		{
			if (Elements == null || other.Elements == null)
			{
				return false;
			}
			if (Elements.Count != other.Elements.Count)
			{
				return false;
			}
			return !Elements.Where((SegmentElement t, int i) => t.GetSimilarity(other.Elements[i]) == SegmentElement.Similarity.None).Any();
		}

		public override int GetHashCode()
		{
			if (Culture == null || Elements == null)
			{
				return -1;
			}
			int num = Culture.GetHashCode();
			foreach (SegmentElement element in Elements)
			{
				num = ((num << 1) ^ element.GetHashCode());
			}
			return num;
		}

		public int GetWeakHashCode()
		{
			int num = Culture.GetPlatformAgnosticHashCode();
			foreach (SegmentElement element in Elements)
			{
				if (element != null)
				{
					num = ((num << 1) ^ element.GetWeakHashCode());
				}
			}
			return num;
		}

		public Segment Duplicate()
		{
			Segment segment = new Segment(Culture);
			if (Elements != null)
			{
				for (int i = 0; i < Elements.Count; i++)
				{
					segment.Elements.Add(Elements[i].Duplicate());
				}
			}
			if (Tokens == null)
			{
				return segment;
			}
			segment.Tokens = new List<Token>();
			for (int j = 0; j < Tokens.Count; j++)
			{
				segment.Tokens.Add((Token)Tokens[j].Duplicate());
			}
			return segment;
		}

		public ErrorCode Validate()
		{
			return Validate(ValidationMode.ReportAllErrors);
		}

		public bool HasPeripheralWhitespace()
		{
			if (Elements == null || Elements.Count == 0)
			{
				return false;
			}
			Text text = Elements[0] as Text;
			if (text != null && text.Value != null && StringUtilities.StartWithAny(text.Value, CharacterProperties.WhitespaceCharacters))
			{
				return true;
			}
			text = (Elements[Elements.Count - 1] as Text);
			if (text != null && text.Value != null)
			{
				return StringUtilities.EndsWithAny(text.Value, CharacterProperties.WhitespaceCharacters);
			}
			return false;
		}

		public ErrorCode Validate(ValidationMode mode)
		{
			if (Culture == null || Culture.Equals(CultureInfo.InvariantCulture))
			{
				return ErrorCode.UndefinedOrInvalidLanguage;
			}
			if (Culture.IsNeutralCulture)
			{
				return ErrorCode.NeutralLanguage;
			}
			if (IsEmpty)
			{
				return ErrorCode.EmptySegment;
			}
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			Stack<int> stack = new Stack<int>();
			bool flag = false;
			foreach (SegmentElement element in Elements)
			{
				Tag tag = element as Tag;
				if (tag != null)
				{
					if (tag.Anchor <= 0)
					{
						return ErrorCode.TagInvalidTagAnchor;
					}
					switch (tag.Type)
					{
					case TagType.Start:
						if (mode != ValidationMode.IgnorePairedTagErrors)
						{
							if (dictionary.ContainsKey(tag.Anchor))
							{
								return ErrorCode.TagAnchorAlreadyUsed;
							}
							dictionary.Add(tag.Anchor, value: false);
							stack.Push(tag.Anchor);
						}
						break;
					case TagType.End:
						if (mode != ValidationMode.IgnorePairedTagErrors)
						{
							if (!dictionary.TryGetValue(tag.Anchor, out bool value))
							{
								return ErrorCode.TagAnchorNotOpen;
							}
							if (value)
							{
								return ErrorCode.TagAnchorAlreadyClosed;
							}
							if (stack.Count == 0 || stack.Peek() != tag.Anchor)
							{
								return ErrorCode.TagAnchorNotOpen;
							}
							dictionary[tag.Anchor] = true;
							stack.Pop();
						}
						break;
					case TagType.Standalone:
					case TagType.TextPlaceholder:
					case TagType.LockedContent:
						if (dictionary.ContainsKey(tag.Anchor))
						{
							return ErrorCode.TagAnchorAlreadyUsed;
						}
						flag = true;
						dictionary.Add(tag.Anchor, value: true);
						break;
					default:
						return ErrorCode.Other;
					}
				}
				else if (!flag)
				{
					Text text = element as Text;
					if (text != null && text.Value != null && text.Value.Length > 0 && text.Value.Trim().Length > 0)
					{
						flag = true;
					}
				}
			}
			if (mode == ValidationMode.IgnorePairedTagErrors)
			{
				if (!flag)
				{
					return ErrorCode.NoTextInSegment;
				}
				return ErrorCode.OK;
			}
			foreach (KeyValuePair<int, bool> item in dictionary)
			{
				if (!item.Value)
				{
					return ErrorCode.TagAnchorNotClosed;
				}
			}
			if (!flag)
			{
				return ErrorCode.NoTextInSegment;
			}
			return ErrorCode.OK;
		}

		public void Add(SegmentElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException();
			}
			if (Elements == null)
			{
				Elements = new List<SegmentElement>();
			}
			if (Elements.Count > 0 && element is Text && Elements[Elements.Count - 1] is Text)
			{
				((Text)Elements[Elements.Count - 1]).Value += ((Text)element).Value;
			}
			else
			{
				Elements.Add(element);
			}
		}

		public void Add(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException();
			}
			if (Elements.Count > 0 && Elements[Elements.Count - 1] is Text)
			{
				((Text)Elements[Elements.Count - 1]).Value += text;
			}
			else
			{
				Add(new Text(text));
			}
		}

		public void AddRange(IEnumerable<SegmentElement> elements)
		{
			if (elements != null)
			{
				foreach (SegmentElement element in elements)
				{
					Add(element);
				}
			}
		}

		public void Clear()
		{
			Elements?.Clear();
		}

		public bool IsValid()
		{
			return Validate() == ErrorCode.OK;
		}

		public void Trim()
		{
			if (Elements != null && Elements.Count != 0)
			{
				RemoveAll(Elements, (SegmentElement e) => e == null);
				TrimStart();
				TrimEnd();
			}
		}

		public string TrimStart()
		{
			string text = null;
			bool flag;
			do
			{
				flag = false;
				if (Elements.Count > 0)
				{
					flag = TrimElement(Elements[0] as Text, trailing: false, out string trimmed);
					if (trimmed != null)
					{
						text = ((text != null) ? (text + trimmed) : trimmed);
					}
					if (flag)
					{
						Elements.RemoveAt(0);
					}
				}
			}
			while (flag);
			return text;
		}

		public string TrimEnd()
		{
			string text = null;
			bool flag;
			do
			{
				flag = false;
				if (Elements.Count > 0)
				{
					flag = TrimElement(Elements[Elements.Count - 1] as Text, trailing: true, out string trimmed);
					if (trimmed != null)
					{
						text = ((text != null) ? (trimmed + text) : trimmed);
					}
					if (flag)
					{
						Elements.RemoveAt(Elements.Count - 1);
					}
				}
			}
			while (flag);
			return text;
		}

		public bool HasTokenBundles()
		{
			if (Tokens != null)
			{
				return Tokens.Any((Token x) => x is TokenBundle);
			}
			return false;
		}

		public bool RemoveTokenBundles()
		{
			bool result = false;
			if (Tokens == null)
			{
				return false;
			}
			for (int i = 0; i < Tokens.Count; i++)
			{
				TokenBundle tokenBundle = Tokens[i] as TokenBundle;
				if (tokenBundle != null)
				{
					if (tokenBundle.Count == 0)
					{
						throw new InvalidOperationException();
					}
					Tokens[i] = tokenBundle.GetBest();
					Tokens[i].Span = tokenBundle.Span;
					result = true;
				}
			}
			return result;
		}

		public bool RemoveUnmatchedStartAndEndTags()
		{
			return RemoveUnmatchedStartAndEndTags(peripheralPositionsOnly: false);
		}

		public bool RemoveUnmatchedStartAndEndTags(bool peripheralPositionsOnly)
		{
			if (Elements == null || Elements.Count == 0)
			{
				return false;
			}
			int num = 0;
			if (peripheralPositionsOnly)
			{
				int num2 = 0;
				foreach (SegmentElement element in Elements)
				{
					Tag tag = element as Tag;
					if (tag == null || tag.Type != TagType.UnmatchedEnd)
					{
						break;
					}
					num2++;
				}
				if (num2 > 0)
				{
					Elements.RemoveRange(0, num2);
				}
				num2 = 0;
				for (int num3 = Elements.Count - 1; num3 >= 0; num3--)
				{
					Tag tag2 = Elements[num3] as Tag;
					if (tag2 == null || tag2.Type != TagType.UnmatchedStart)
					{
						break;
					}
					num2++;
				}
				if (num2 > 0)
				{
					Elements.RemoveRange(Elements.Count - num2, num2);
				}
			}
			else
			{
				num = RemoveAll(Elements, delegate(SegmentElement se)
				{
					if (!(se is Tag))
					{
						return false;
					}
					Tag tag3 = (Tag)se;
					return tag3.Type == TagType.UnmatchedStart || tag3.Type == TagType.UnmatchedEnd;
				});
			}
			return num > 0;
		}

		public bool HasUnmatchedStartOrEndTags()
		{
			if (Elements == null || Elements.Count == 0)
			{
				return false;
			}
			return Elements.Any(delegate(SegmentElement se)
			{
				Tag tag = se as Tag;
				if (tag == null)
				{
					return false;
				}
				return tag.Type == TagType.UnmatchedStart || tag.Type == TagType.UnmatchedEnd;
			});
		}

		public bool FillUnmatchedStartAndEndTags()
		{
			if (Elements == null || Elements.Count == 0)
			{
				return false;
			}
			bool result = false;
			Stack<int> stack = new Stack<int>();
			List<SegmentElement> list = null;
			List<SegmentElement> list2 = null;
			bool flag = false;
			for (int i = 0; i < Elements.Count; i++)
			{
				if (flag)
				{
					break;
				}
				Tag tag = Elements[i] as Tag;
				if (tag == null)
				{
					continue;
				}
				switch (tag.Type)
				{
				case TagType.Start:
					stack.Push(tag.Anchor);
					break;
				case TagType.End:
					if (stack.Count > 0 && tag.Anchor == stack.Peek())
					{
						stack.Pop();
					}
					else
					{
						flag = true;
					}
					break;
				case TagType.UnmatchedStart:
					if (stack.Count == 0)
					{
						Tag tag3 = new Tag(tag);
						tag.Type = TagType.Start;
						tag3.Type = TagType.End;
						tag3.AlignmentAnchor = 0;
						if (list2 == null)
						{
							list2 = new List<SegmentElement>();
						}
						list2.Insert(0, tag3);
					}
					break;
				case TagType.UnmatchedEnd:
					if (stack.Count == 0 && list2 == null)
					{
						Tag tag2 = new Tag(tag);
						tag.Type = TagType.End;
						tag.AlignmentAnchor = 0;
						tag2.Type = TagType.Start;
						if (list == null)
						{
							list = new List<SegmentElement>();
						}
						list.Insert(0, tag2);
					}
					break;
				}
			}
			if (list != null)
			{
				Elements.InsertRange(0, list);
				result = true;
			}
			if (list2 == null)
			{
				return result;
			}
			Elements.AddRange(list2);
			return true;
		}

		private static bool TrimElement(Text t, bool trailing, out string trimmed)
		{
			trimmed = null;
			if (t == null)
			{
				return false;
			}
			t.Value = (trailing ? StringUtilities.TrimEnd(t.Value, CharacterProperties.WhitespaceCharacters, out trimmed) : StringUtilities.TrimStart(t.Value, CharacterProperties.WhitespaceCharacters, out trimmed));
			return t.Value.Length == 0;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Elements == null)
			{
				return stringBuilder.ToString();
			}
			foreach (SegmentElement element in Elements)
			{
				if (element != null)
				{
					stringBuilder.Append(element);
				}
			}
			return stringBuilder.ToString();
		}

		public string ToPlain()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Elements == null)
			{
				return stringBuilder.ToString();
			}
			foreach (SegmentElement element in Elements)
			{
				if (element != null)
				{
					Text text = element as Text;
					if (text == null)
					{
						Tag tag = element as Tag;
						if (tag != null && (tag.Type == TagType.TextPlaceholder || tag.Type == TagType.LockedContent))
						{
							stringBuilder.Append(tag.TextEquivalent);
						}
					}
					else
					{
						stringBuilder.Append(text.Value);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public string ToPlain(bool tolower, bool tobase, out List<SegmentPosition> ranges)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ranges = new List<SegmentPosition>();
			if (Elements == null)
			{
				return string.Empty;
			}
			for (int i = 0; i < Elements.Count; i++)
			{
				if (Elements[i] == null)
				{
					continue;
				}
				Text text = Elements[i] as Text;
				if (text == null)
				{
					continue;
				}
				string value = text.Value;
				for (int j = 0; j < value.Length; j++)
				{
					char c = value[j];
					if (tolower)
					{
						c = char.ToLower(c, CultureInfo.InvariantCulture);
					}
					if (tobase)
					{
						c = CharacterProperties.ToBase(c);
					}
					stringBuilder.Append(c);
					ranges.Add(new SegmentPosition(i, j));
				}
			}
			return stringBuilder.ToString();
		}

		public string ToPlain(SegmentRange range)
		{
			if (range == null)
			{
				throw new ArgumentNullException();
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = range.From.Index; i <= range.Into.Index; i++)
			{
				Text text = Elements[i] as Text;
				if (text != null)
				{
					string value = text.Value;
					int num = (i == range.From.Index) ? range.From.Position : 0;
					int num2 = (i == range.Into.Index) ? (range.Into.Position + 1) : value.Length;
					stringBuilder.Append(value.Substring(num, num2 - num));
				}
			}
			return stringBuilder.ToString();
		}

		public int GetTokenIndex(SegmentPosition p)
		{
			if (p == null)
			{
				throw new ArgumentNullException();
			}
			if (Tokens == null)
			{
				return -1;
			}
			for (int i = 0; i < Tokens.Count; i++)
			{
				Token token = Tokens[i];
				if (p.Index == token.Span.From.Index && p.Position >= token.Span.From.Position && p.Position <= token.Span.Into.Position)
				{
					return i;
				}
			}
			return -1;
		}

		public void MergeAdjacentTextRuns()
		{
			if (Elements == null || Elements.Count < 2)
			{
				return;
			}
			bool flag = false;
			Text text = null;
			for (int i = 0; i < Elements.Count; i++)
			{
				Text text2 = Elements[i] as Text;
				if (text2 == null)
				{
					text = null;
				}
				else if (text != null)
				{
					text.Value += text2.Value;
					Elements[i] = null;
					flag = true;
				}
				else
				{
					text = text2;
				}
			}
			if (flag)
			{
				RemoveAll(Elements, (SegmentElement e) => e == null);
			}
		}

		public bool VerifyTokenSpans()
		{
			if (Tokens == null)
			{
				return true;
			}
			bool result = true;
			foreach (Token token in Tokens)
			{
				if (!(token is TagToken) && !(token is TokenBundle))
				{
					string a = ToPlain(token.Span);
					string text = token.Text;
					if (!string.Equals(a, text, StringComparison.Ordinal))
					{
						result = false;
					}
				}
			}
			return result;
		}

		public string ToPlain(int fromToken, int intoToken)
		{
			if (Tokens == null)
			{
				throw new LanguagePlatformException(ErrorCode.SegmentNotTokenized);
			}
			if (fromToken < 0 || fromToken >= Tokens.Count)
			{
				return string.Empty;
			}
			if (intoToken < 0 || intoToken >= Tokens.Count)
			{
				return string.Empty;
			}
			if (fromToken > intoToken)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = -1;
			int num2 = -1;
			for (int i = Tokens[fromToken].Span.From.Index; i <= Tokens[intoToken].Span.From.Index; i++)
			{
				Text text = Elements[i] as Text;
				if (text != null)
				{
					if (i == Tokens[fromToken].Span.From.Index)
					{
						num = Tokens[fromToken].Span.From.Position;
					}
					if (i == Tokens[intoToken].Span.Into.Index)
					{
						num2 = stringBuilder.Length + Tokens[intoToken].Span.Into.Position;
					}
					stringBuilder.Append(text.Value);
				}
			}
			if (num < 0 || num2 < 0)
			{
				return null;
			}
			return stringBuilder.ToString().Substring(num, num2 - num + 1);
		}

		public bool DeleteTags()
		{
			return DeleteTags(DeleteTagsAction.DeleteAll);
		}

		public bool DeleteTags(DeleteTagsAction mode)
		{
			if (Elements == null)
			{
				return false;
			}
			int count = Elements.Count;
			RemoveAll(Elements, delegate(SegmentElement e)
			{
				Tag tag = e as Tag;
				if (tag == null)
				{
					return false;
				}
				switch (tag.Type)
				{
				case TagType.Standalone:
					return (mode & DeleteTagsAction.DeleteStandaloneTags) != 0;
				case TagType.TextPlaceholder:
				case TagType.LockedContent:
					return (mode & DeleteTagsAction.DeleteTextPlaceholders) != 0;
				case TagType.Start:
				case TagType.End:
					return (mode & DeleteTagsAction.DeletePairedTags) != 0;
				default:
					return false;
				}
			});
			if (count != Elements.Count)
			{
				MergeAdjacentTextRuns();
				return true;
			}
			return false;
		}

		private static int RemoveAll(IList<SegmentElement> elements, Func<SegmentElement, bool> d)
		{
			int num = 0;
			int num2 = 0;
			while (num < elements.Count)
			{
				if (d(elements[num]))
				{
					elements.RemoveAt(num);
					num2++;
				}
				else
				{
					num++;
				}
			}
			return num2;
		}

		public void AnchorDanglingTags()
		{
			int num = 0;
			bool flag = false;
			foreach (SegmentElement element in Elements)
			{
				Tag tag;
				if ((tag = (element as Tag)) != null)
				{
					num = Math.Max(num, tag.Anchor);
					flag = (tag.Anchor <= 0);
				}
			}
			if (flag)
			{
				Stack<Tag> stack = null;
				foreach (SegmentElement element2 in Elements)
				{
					Tag tag2 = element2 as Tag;
					if (tag2 != null && tag2.Anchor <= 0)
					{
						switch (tag2.Type)
						{
						case TagType.Start:
							num = (tag2.Anchor = num + 1);
							if (stack == null)
							{
								stack = new Stack<Tag>();
							}
							stack.Push(tag2);
							break;
						case TagType.End:
							if (stack != null && stack.Count != 0)
							{
								Tag tag3 = stack.Pop();
								tag2.Anchor = tag3.Anchor;
							}
							break;
						case TagType.Standalone:
						case TagType.TextPlaceholder:
						case TagType.LockedContent:
						case TagType.UnmatchedStart:
						case TagType.UnmatchedEnd:
							num = (tag2.Anchor = num + 1);
							break;
						}
					}
				}
			}
		}

		public int GetMaxTagAnchor()
		{
			int num = 0;
			foreach (SegmentElement element in Elements)
			{
				if (element is Tag)
				{
					Tag tag = (Tag)element;
					num = Math.Max(num, tag.Anchor);
				}
			}
			return num;
		}

		public Tag FindTag(TagType type, int anchor)
		{
			if (Elements == null || Elements.Count == 0)
			{
				return null;
			}
			foreach (SegmentElement element in Elements)
			{
				Tag tag = element as Tag;
				if (tag != null && tag.Type == type && tag.Anchor == anchor)
				{
					return tag;
				}
			}
			return null;
		}

		public void GetMinMaxTagAnchor(out int min, out int max)
		{
			min = 0;
			max = 0;
			foreach (SegmentElement element in Elements)
			{
				if (element is Tag)
				{
					Tag tag = (Tag)element;
					min = Math.Min(tag.Anchor, min);
					max = Math.Max(tag.Anchor, max);
				}
			}
		}

		public bool DeleteEmptyTagPairs(bool onlyInPeripheralPositions)
		{
			bool flag = false;
			if (onlyInPeripheralPositions)
			{
				while (Elements.Count >= 2)
				{
					Tag tag = Elements[0] as Tag;
					Tag tag2 = Elements[1] as Tag;
					if (tag == null || tag2 == null || tag.Anchor != tag2.Anchor)
					{
						break;
					}
					Elements.RemoveRange(0, 2);
					flag = true;
				}
				while (Elements.Count >= 2)
				{
					int num = Elements.Count - 1;
					Tag tag = Elements[num - 1] as Tag;
					Tag tag2 = Elements[num] as Tag;
					if (tag == null || tag2 == null || tag.Anchor != tag2.Anchor)
					{
						break;
					}
					Elements.RemoveRange(num - 1, 2);
					flag = true;
				}
			}
			else
			{
				bool flag2;
				do
				{
					flag2 = false;
					for (int num2 = Elements.Count - 1; num2 > 0; num2--)
					{
						Tag tag2 = Elements[num2] as Tag;
						if (tag2 != null && tag2.Type == TagType.End)
						{
							Tag tag = Elements[num2 - 1] as Tag;
							if (tag != null && tag.Anchor == tag2.Anchor)
							{
								Elements.RemoveRange(num2 - 1, 2);
								num2--;
								flag = true;
								flag2 = true;
							}
						}
					}
				}
				while (flag2);
			}
			if (!flag)
			{
				return false;
			}
			MergeAdjacentTextRuns();
			Trim();
			return true;
		}

		public bool RenumberTagAnchors(ref int maxAlignmentAnchor)
		{
			return RenumberTagAnchors(1, ref maxAlignmentAnchor);
		}

		public bool RenumberTagAnchors(int nextTagAnchor, ref int maxAlignmentAnchor)
		{
			if (nextTagAnchor <= 0)
			{
				throw new ArgumentOutOfRangeException("nextTagAnchor");
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			bool flag = false;
			int num = 0;
			foreach (SegmentElement element in Elements)
			{
				if (element is Tag)
				{
					Tag tag = (Tag)element;
					int value;
					switch (tag.Type)
					{
					case TagType.Start:
						if (dictionary.TryGetValue(tag.Anchor, out value))
						{
							flag |= (tag.Anchor != value);
							tag.Anchor = value;
						}
						else
						{
							dictionary.Add(tag.Anchor, nextTagAnchor);
							flag |= (tag.Anchor != nextTagAnchor);
							tag.Anchor = nextTagAnchor;
							nextTagAnchor++;
						}
						break;
					case TagType.End:
						if (dictionary.TryGetValue(tag.Anchor, out value))
						{
							flag |= (tag.Anchor != value);
							tag.Anchor = value;
						}
						else
						{
							dictionary.Add(tag.Anchor, nextTagAnchor);
							flag |= (tag.Anchor != nextTagAnchor);
							tag.Anchor = nextTagAnchor;
							nextTagAnchor++;
						}
						break;
					case TagType.Standalone:
					case TagType.TextPlaceholder:
					case TagType.LockedContent:
						flag |= (tag.Anchor != nextTagAnchor);
						tag.Anchor = nextTagAnchor;
						nextTagAnchor++;
						break;
					default:
						throw new Exception("Unexpected");
					}
					if (tag.Anchor > num)
					{
						num = tag.Anchor;
					}
					if (tag.AlignmentAnchor > maxAlignmentAnchor)
					{
						maxAlignmentAnchor = tag.AlignmentAnchor;
					}
				}
			}
			return flag;
		}

		public bool UpdateFromTokenIndices(ICollection<int> tokenIndices)
		{
			if (Tokens == null)
			{
				throw new InvalidOperationException("Segment needs to be tokenized");
			}
			List<int> list = tokenIndices.ToList();
			list.Sort((int a, int b) => b - a);
			bool flag = false;
			foreach (int item in list)
			{
				if (item < 0 || item >= Tokens.Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				Token t = Tokens[item];
				flag |= UpdateFromToken(t);
			}
			return flag;
		}

		public Dictionary<int, int> GetTagPairings()
		{
			if (Tokens == null)
			{
				throw new InvalidOperationException("Segment needs to be tokenized");
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int num = Tokens.Count - 1; num >= 0; num--)
			{
				TagToken tagToken = Tokens[num] as TagToken;
				if (tagToken != null && tagToken.Tag.Type == TagType.End)
				{
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						TagToken tagToken2 = Tokens[num2] as TagToken;
						if (tagToken2 != null && tagToken2.Tag.Type == TagType.Start && tagToken.Tag.Anchor == tagToken2.Tag.Anchor)
						{
							dictionary.Add(num2, num);
							break;
						}
					}
				}
			}
			return dictionary;
		}

		public int GetTagCount()
		{
			int num = 0;
			foreach (SegmentElement element in Elements)
			{
				Tag tag = element as Tag;
				if (tag != null && tag.Type != 0 && tag.Type != TagType.End)
				{
					num++;
				}
			}
			return num;
		}

		public Dictionary<int, string> GetTagIdGroups()
		{
			if (Tokens == null)
			{
				throw new InvalidOperationException("Segment needs to be tokenized");
			}
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			for (int i = 0; i < Tokens.Count; i++)
			{
				TagToken tagToken = Tokens[i] as TagToken;
				if (tagToken?.Tag != null && (tagToken.Tag.Type == TagType.Start || tagToken.Tag.Type == TagType.Standalone || tagToken.Tag.Type == TagType.TextPlaceholder || tagToken.Tag.Type == TagType.LockedContent) && !string.IsNullOrEmpty(tagToken.Tag.TagID))
				{
					dictionary.Add(i, tagToken.Tag.TagID);
				}
			}
			return dictionary;
		}

		private bool UpdateFromToken(Token t)
		{
			if (t.Span.From.Index < 0 || t.Span.From.Index >= Elements.Count)
			{
				throw new ArgumentException("Token is not inside segment");
			}
			if (t.Span.From.Index != t.Span.Into.Index)
			{
				throw new ArgumentException("Invalid token");
			}
			if (t.Span.From.Position > t.Span.Into.Position)
			{
				throw new ArgumentException("Invalid token");
			}
			if (Tokens == null)
			{
				throw new InvalidOperationException("Segment is not yet tokenized");
			}
			int num = -1;
			for (int i = 0; i < Tokens.Count; i++)
			{
				if (Tokens[i] == t)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				throw new ArgumentException("Token is not inside segment");
			}
			TagToken tagToken = t as TagToken;
			if (tagToken != null)
			{
				if (!(Elements[tagToken.Span.From.Index] is Tag))
				{
					return false;
				}
				Elements[tagToken.Span.From.Index] = tagToken.Tag;
			}
			else
			{
				Text text = Elements[t.Span.From.Index] as Text;
				if (text == null)
				{
					return false;
				}
				if (t.Span.From.Position < 0 || t.Span.From.Position >= text.Value.Length || t.Span.Into.Position < 0 || t.Span.Into.Position >= text.Value.Length)
				{
					return false;
				}
				int position = t.Span.From.Position;
				int position2 = t.Span.Into.Position;
				int num2 = position2 - position + 1;
				int num3 = t.Text.Length - num2;
				if (num3 != 0)
				{
					foreach (Token token in Tokens)
					{
						if (token != t && token.Span.From.Index == t.Span.From.Index && token.Span.From.Position > t.Span.Into.Position)
						{
							token.Span.From.Position += num3;
							token.Span.Into.Position += num3;
						}
					}
				}
				string text3 = text.Value = text.Value.Insert(position, t.Text).Remove(position + t.Text.Length, num2);
				t.Span.Into.Position = t.Span.Into.Position + num3;
			}
			return true;
		}
	}
}
