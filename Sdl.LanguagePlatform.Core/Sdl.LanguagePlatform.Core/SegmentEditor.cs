using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Core
{
	public class SegmentEditor
	{
		public static void CleanSegment(Segment segment)
		{
			if (segment.Tokens == null)
			{
				List<SegmentElement> list = new List<SegmentElement>(segment.Elements);
				segment.Elements.Clear();
				foreach (SegmentElement item in list)
				{
					if (item != null)
					{
						Text text = item as Text;
						if (text == null || !string.IsNullOrEmpty(text.Value))
						{
							segment.Add(item);
						}
					}
				}
			}
			else
			{
				List<Token> list2 = new List<Token>(segment.Tokens);
				list2 = list2.FindAll((Token x) => x != null);
				segment.Elements.Clear();
				segment.Tokens.Clear();
				InsertTokens(segment, list2, 0);
			}
		}

		public static bool ChangeTokens(Segment segment, List<Token> tokens, short startTokenIndex, short length)
		{
			ValidateArgs(segment, startTokenIndex);
			if (startTokenIndex >= segment.Tokens.Count)
			{
				throw new Exception("startTokenIndex >= proposalTarget.Tokens.Count");
			}
			if (length < 1)
			{
				throw new Exception("length < 1");
			}
			if (DeleteTokens(segment, startTokenIndex, length))
			{
				return InsertTokens(segment, tokens, startTokenIndex);
			}
			return false;
		}

		public static bool DeleteTokens(Segment segment, short startTokenIndex, short length)
		{
			ValidateArgs(segment, startTokenIndex);
			if (startTokenIndex >= segment.Tokens.Count)
			{
				throw new Exception("startTokenIndex >= proposalTarget.Tokens.Count");
			}
			if (length < 0)
			{
				throw new Exception("length < 0");
			}
			if (startTokenIndex + length > segment.Tokens.Count)
			{
				throw new Exception("(startTokenIndex + length) > segment.Tokens.Count");
			}
			while (length-- > 0)
			{
				DeleteToken(segment, startTokenIndex);
			}
			return true;
		}

		private static void DeleteToken(Segment segment, short tokenIndex)
		{
			Token token = segment.Tokens[tokenIndex];
			int index = token.Span.From.Index;
			Text text = segment.Elements[index] as Text;
			int num = token.Span.Into.Position - token.Span.From.Position + 1;
			if (text == null || (token.Span.From.Position == 0 && num == text.Value.Length))
			{
				segment.Elements.RemoveAt(index);
				segment.Tokens.RemoveAt(tokenIndex);
				for (int i = tokenIndex; i < segment.Tokens.Count; i++)
				{
					segment.Tokens[i].Span.From.Index--;
					segment.Tokens[i].Span.Into.Index--;
				}
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(text.Value.Substring(0, token.Span.From.Position));
			stringBuilder.Append(text.Value.Substring(token.Span.Into.Position + 1));
			text.Value = stringBuilder.ToString();
			segment.Tokens.RemoveAt(tokenIndex);
			for (int j = tokenIndex; j < segment.Tokens.Count && segment.Tokens[j].Span.From.Index == index; j++)
			{
				segment.Tokens[j].Span.From.Position -= num;
				segment.Tokens[j].Span.Into.Position -= num;
			}
		}

		private static void ValidateArgs(Segment segment, short startTokenIndex)
		{
			if (segment.Tokens == null)
			{
				throw new ArgumentNullException("Tokens");
			}
			if (startTokenIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startTokenIndex");
			}
		}

		public static Segment Clone(Segment other)
		{
			Segment segment = new Segment(other.Culture);
			foreach (SegmentElement element in other.Elements)
			{
				segment.Elements.Add(element?.Duplicate());
			}
			if (other.Tokens == null)
			{
				return segment;
			}
			segment.Tokens = new List<Token>();
			foreach (Token token in other.Tokens)
			{
				Token item = token.Duplicate() as Token;
				segment.Tokens.Add(item);
			}
			return segment;
		}

		public static bool AppendTokens(Segment segment, List<Token> tokens)
		{
			return InsertTokens(segment, tokens, (short)segment.Tokens.Count);
		}

		public static bool InsertTokens(Segment segment, List<Token> tokens, short startTokenIndex)
		{
			return InsertTokens(segment, tokens, startTokenIndex, null);
		}

		public static bool InsertTokens(Segment segment, List<Token> tokens, short startTokenIndex, List<Token> tokensInserted)
		{
			int num = 0;
			if (tokens.Count == 0)
			{
				return false;
			}
			do
			{
				List<Token> list = new List<Token>();
				while (num < tokens.Count && !(tokens[num] is TagToken))
				{
					list.Add(tokens[num++]);
				}
				if (list.Count > 0 && !InsertTokensInternal(segment, list, startTokenIndex, tokensInserted))
				{
					return false;
				}
				startTokenIndex = (short)(startTokenIndex + (short)list.Count);
				if (num == tokens.Count)
				{
					return true;
				}
				while (num < tokens.Count && tokens[num] is TagToken)
				{
					int num2 = 0;
					TagToken tagToken = tokens[num].Duplicate() as TagToken;
					tokensInserted?.Add(tagToken);
					int num3 = 0;
					if (startTokenIndex > 0)
					{
						num3 = segment.Tokens[startTokenIndex - 1].Span.Into.Index + 1;
						Text text = segment.Elements[num3 - 1] as Text;
						if (text != null)
						{
							num2 = segment.Tokens[startTokenIndex - 1].Span.Into.Position + 1;
							string text2 = text.Value.Substring(num2);
							if (text2.Length > 0)
							{
								text.Value = text.Value.Substring(0, num2);
								segment.Elements.Insert(num3, new Text(text2));
							}
							else
							{
								num2 = 0;
							}
						}
					}
					tagToken.Span.From.Index = num3;
					tagToken.Span.Into.Index = num3;
					segment.Elements.Insert(num3, tagToken.Tag);
					segment.Tokens.Insert(startTokenIndex, tagToken);
					for (int i = startTokenIndex + 1; i < segment.Tokens.Count; i++)
					{
						segment.Tokens[i].Span.From.Index++;
						segment.Tokens[i].Span.Into.Index++;
						if (num2 > 0)
						{
							segment.Tokens[i].Span.From.Index++;
							segment.Tokens[i].Span.Into.Index++;
							if (segment.Tokens[i].Span.From.Index == num3 + 1)
							{
								segment.Tokens[i].Span.From.Position -= num2;
								segment.Tokens[i].Span.Into.Position -= num2;
							}
						}
					}
					num++;
					startTokenIndex = (short)(startTokenIndex + 1);
				}
			}
			while (num != tokens.Count);
			return true;
		}

		private static bool InsertTokensInternal(Segment segment, List<Token> tokens, short startTokenIndex, List<Token> tokensInserted)
		{
			ValidateArgs(segment, startTokenIndex);
			if (startTokenIndex > segment.Tokens.Count)
			{
				throw new Exception("startTokenIndex > proposalTarget.Tokens.Count");
			}
			List<Token> list = new List<Token>();
			foreach (Token token3 in tokens)
			{
				list.Add(token3.Duplicate() as Token);
			}
			tokens = list;
			tokensInserted?.AddRange(tokens);
			List<Token> list2 = tokens;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token item in list2)
			{
				stringBuilder.Append(item.Text);
			}
			if (startTokenIndex == segment.Tokens.Count)
			{
				segment.Tokens.AddRange(list2);
				bool flag = segment.Elements.Any() && segment.Elements[segment.Elements.Count - 1] is Text;
				int num = 0;
				if (flag)
				{
					Text text = segment.Elements[segment.Elements.Count - 1] as Text;
					if (text != null)
					{
						num = text.Value.Length;
					}
				}
				segment.Add(new Text(stringBuilder.ToString()));
				for (int i = 0; i < list2.Count; i++)
				{
					list2[i].Span = new SegmentRange(segment.Elements.Count - 1, num, num + list2[i].Text.Length - 1);
					num += list2[i].Text.Length;
				}
			}
			else if (startTokenIndex == 0)
			{
				segment.Tokens.InsertRange(0, list2);
				segment.Elements.Insert(0, new Text(stringBuilder.ToString()));
				int num2 = 0;
				foreach (Token item2 in list2)
				{
					item2.Span = new SegmentRange(0, num2, num2 + item2.Text.Length - 1);
					num2 += item2.Text.Length;
				}
				for (int j = list2.Count; j < segment.Tokens.Count; j++)
				{
					segment.Tokens[j].Span.From.Index++;
					segment.Tokens[j].Span.Into.Index++;
				}
			}
			else
			{
				segment.Tokens.InsertRange(startTokenIndex, list2);
				Token token = segment.Tokens[startTokenIndex - 1];
				Token token2 = segment.Tokens[startTokenIndex + list2.Count];
				int index = token2.Span.From.Index;
				if (token2.Span.From.Index != token2.Span.Into.Index)
				{
					return false;
				}
				if (token.Span.From.Index != token.Span.Into.Index)
				{
					return false;
				}
				if (token.Span.Into.Index == index)
				{
					Text text2 = segment.Elements[index] as Text;
					if (text2 == null)
					{
						return false;
					}
					string text3 = text2.Value.Substring(0, token.Span.Into.Position + 1);
					string str = text2.Value.Substring(text3.Length);
					text2.Value = text3 + stringBuilder?.ToString() + str;
					int num3 = token.Span.Into.Position + 1;
					foreach (Token item3 in list2)
					{
						item3.Span = new SegmentRange(token.Span.Into.Index, num3, num3 + item3.Text.Length - 1);
						num3 += item3.Text.Length;
					}
					int k = startTokenIndex + list2.Count;
					int length = stringBuilder.ToString().Length;
					for (; k < segment.Tokens.Count && segment.Tokens[k].Span.From.Index == index; k++)
					{
						segment.Tokens[k].Span.From.Position += length;
						segment.Tokens[k].Span.Into.Position += length;
					}
					return true;
				}
				segment.Elements.Insert(index, new Text(stringBuilder.ToString()));
				int num4 = 0;
				foreach (Token item4 in list2)
				{
					item4.Span = new SegmentRange(index, num4, num4 + item4.Text.Length - 1);
					num4 += item4.Text.Length;
				}
				for (int l = startTokenIndex + list2.Count; l < segment.Tokens.Count; l++)
				{
					segment.Tokens[l].Span.From.Index++;
					segment.Tokens[l].Span.Into.Index++;
				}
			}
			return true;
		}
	}
}
