using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class WorkbenchCleaner
	{
		private struct TagProperties
		{
			public string Name;

			public int Position;

			public int Anchor;

			public TagProperties(string n, int p, int a)
			{
				Name = n;
				Position = p;
				Anchor = a;
			}
		}

		private struct TagInformation
		{
			public int Position;

			public int Anchor;

			public bool IsMIF;

			public string Id;

			public TagInformation(int p, int a, bool isMif, string id)
			{
				Position = p;
				Anchor = a;
				Id = id;
				IsMIF = isMif;
			}
		}

		private static HashSet<string> _FixableMifTags = new HashSet<string>
		{
			"b",
			"bi",
			"c",
			"c1",
			"c2",
			"i",
			"s",
			"cns",
			"cs",
			"fc",
			"ti",
			"el",
			"elf"
		};

		private static Regex _contentElementNameRegex = new Regex("\\s*</?([^>\n\r\t\f/ ]+).*>\\s*");

		private static bool DeletePeripheralBookmarks(Segment segment)
		{
			bool flag = false;
			bool result = false;
			do
			{
				flag = false;
				if (IsPairedEmptyBookmark(segment, 0))
				{
					segment.Elements.RemoveRange(0, 2);
					flag = true;
				}
				else if (IsPairedEmptyBookmark(segment, segment.Elements.Count - 2))
				{
					segment.Elements.RemoveRange(segment.Elements.Count - 2, 2);
					flag = true;
				}
				if (flag)
				{
					segment.Trim();
					result = true;
				}
			}
			while (flag && segment.Elements.Count >= 2);
			return result;
		}

		private static void ChangeToText(Segment segment, int p, string s)
		{
			segment.Elements[p] = new Text(s);
		}

		private static bool IsCommentMarker(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			if (s.EndsWith("-->"))
			{
				return true;
			}
			int length = s.Length;
			int num = 0;
			for (num = 0; num < length && char.IsWhiteSpace(s, num); num++)
			{
			}
			if (num + 3 >= length)
			{
				return false;
			}
			if (s[num] == '<' && s[num + 1] == '!' && s[num + 2] == '-')
			{
				return s[num + 3] == '-';
			}
			return false;
		}

		private static bool HandlePlaceholders(Segment segment)
		{
			bool flag = false;
			int num = -1;
			int num2 = segment.GetMaxTagAnchor();
			int count = segment.Elements.Count;
			for (int num3 = count - 1; num3 >= 0; num3--)
			{
				ContentBearingTag contentBearingTag = segment.Elements[num3] as ContentBearingTag;
				if (contentBearingTag != null)
				{
					if (num < 0)
					{
						num = num3;
					}
					if (contentBearingTag.Type == TagType.Standalone)
					{
						string content = contentBearingTag.Content;
						int length = content.Length;
						if (length > 2 && content[0] == '&' && content[length - 1] == ';')
						{
							char unicodeEquivalent;
							if (content[1] == '#')
							{
								bool flag2 = false;
								if ((content[2] != 'x') ? int.TryParse(content.Substring(2, length - 3), NumberStyles.None, CultureInfo.InvariantCulture, out int result) : int.TryParse(content.Substring(3, length - 4), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result))
								{
									char c = (char)result;
									ChangeToText(segment, num3, char.ToString(c));
									flag = true;
								}
							}
							else if (EntityMappings.GetMapping(content.Substring(1, length - 2), out unicodeEquivalent))
							{
								ChangeToText(segment, num3, unicodeEquivalent.ToString());
								flag = true;
							}
						}
						else if (content.Equals("\\-", StringComparison.Ordinal))
						{
							ChangeToText(segment, num3, "­");
							flag = true;
						}
						else if (content.Equals("<br/>", StringComparison.Ordinal))
						{
							ChangeToText(segment, num3, "\n");
							flag = true;
						}
						else if (content.StartsWith("<bookmarkstart ") || content.StartsWith("<bookmarkend "))
						{
							if (content.IndexOf(" name=\"_") > 0)
							{
								segment.Elements.RemoveAt(num3);
								flag = true;
							}
						}
						else if (content.StartsWith("<cf "))
						{
							if (num3 == 0 || num3 == count - 1)
							{
								segment.Elements.RemoveAt(num3);
								flag = true;
							}
							else if (num3 == num)
							{
								contentBearingTag.AlignmentAnchor = 0;
								contentBearingTag.Type = TagType.Start;
								num2 = (contentBearingTag.Anchor = num2 + 1);
								Tag item = new Tag(TagType.End, null, contentBearingTag.Anchor);
								segment.Elements.Add(item);
								flag = true;
							}
						}
						else if ((num3 == 0 || num3 == count - 1) && IsCommentMarker(content))
						{
							segment.Elements.RemoveAt(num3);
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				segment.MergeAdjacentTextRuns();
				segment.Trim();
			}
			return flag;
		}

		public static bool ApplyCleanupHeuristics(TranslationUnit tu)
		{
			bool result = false;
			if (ApplyCleanupHeuristics(tu.SourceSegment))
			{
				result = true;
			}
			if (ApplyCleanupHeuristics(tu.TargetSegment))
			{
				result = true;
			}
			return result;
		}

		private static string ExtractTagName(string contents, out TagType tt)
		{
			tt = TagType.Undefined;
			int num = 0;
			int length = contents.Length;
			if (length == 0)
			{
				return null;
			}
			if (contents[num] != '<')
			{
				return null;
			}
			num++;
			if (num == length)
			{
				return null;
			}
			if (contents[num] == '/')
			{
				tt = TagType.End;
				num++;
			}
			else
			{
				tt = TagType.Start;
			}
			for (; num < length && char.IsWhiteSpace(contents[num]); num++)
			{
			}
			int i;
			for (i = num; i < length && (char.IsLetterOrDigit(contents[i]) || contents[i] == '_'); i++)
			{
			}
			if (contents.EndsWith("/>", StringComparison.Ordinal))
			{
				tt = TagType.Standalone;
			}
			if (i == num)
			{
				return null;
			}
			return contents.Substring(num, i - num);
		}

		public static bool RebracketMIFPlaceholders(Segment segment)
		{
			bool result = false;
			Stack<TagInformation> stack = new Stack<TagInformation>();
			for (int i = 0; i < segment.Elements.Count; i++)
			{
				Tag tag = segment.Elements[i] as Tag;
				if (tag == null)
				{
					continue;
				}
				ContentBearingTag contentBearingTag = segment.Elements[i] as ContentBearingTag;
				if (tag.Type == TagType.Start)
				{
					stack.Push(new TagInformation(i, tag.Anchor, isMif: false, null));
				}
				else if (tag.Type == TagType.End)
				{
					if (stack.Count > 0 && stack.Peek().Anchor == tag.Anchor)
					{
						stack.Pop();
					}
				}
				else
				{
					if (tag.Type != TagType.Standalone || contentBearingTag == null || !ExtractMifTag(contentBearingTag.Content, out string tagId, out bool isEnd) || !_FixableMifTags.Contains(tagId))
					{
						continue;
					}
					if (!isEnd)
					{
						stack.Push(new TagInformation(i, tag.Anchor, isMif: true, tagId));
					}
					else if (stack.Count > 0)
					{
						TagInformation tagInformation = stack.Peek();
						if (tagInformation.IsMIF && string.Equals(tagInformation.Id, tagId))
						{
							tagInformation = stack.Pop();
							Tag tag2 = segment.Elements[tagInformation.Position] as Tag;
							tag2.Type = TagType.Start;
							tag.Type = TagType.End;
							tag.Anchor = tag2.Anchor;
							result = true;
						}
					}
				}
			}
			return result;
		}

		private static bool ExtractMifTag(string content, out string tagId, out bool isEnd)
		{
			tagId = null;
			isEnd = false;
			if (string.IsNullOrEmpty(content))
			{
				return false;
			}
			int length = content.Length;
			if (length < 2)
			{
				return false;
			}
			int num = 0;
			if (num >= length || content[num] != '<')
			{
				return false;
			}
			num++;
			if (num >= length || content[num] != ':')
			{
				return false;
			}
			num++;
			if (num < length && content[num] == '/')
			{
				isEnd = true;
				num++;
			}
			int i;
			for (i = num; i < length && char.IsLetterOrDigit(content[i]); i++)
			{
			}
			if (i == num || i >= length)
			{
				return false;
			}
			tagId = content.Substring(num, i - num);
			return true;
		}

		public static bool FixMistaggings(Segment segment)
		{
			return FixMistaggingsFromWordTTXDocuments(segment) | FixMistaggingsFromWorkbench_ph_it(segment) | ChangeUnmatchedTagsToPlaceholderTags(segment);
		}

		private static bool FixMistaggingsFromWordTTXDocuments(Segment segment)
		{
			bool result = false;
			for (int i = 0; i < segment.Elements.Count - 2; i++)
			{
				ContentBearingTag contentBearingTag = segment.Elements[i] as ContentBearingTag;
				if (contentBearingTag == null || contentBearingTag.Type != TagType.Start)
				{
					continue;
				}
				ContentBearingTag contentBearingTag2 = segment.Elements[i + 1] as ContentBearingTag;
				if (contentBearingTag2 == null)
				{
					continue;
				}
				ContentBearingTag contentBearingTag3 = segment.Elements[i + 2] as ContentBearingTag;
				if (contentBearingTag3 != null && contentBearingTag2.Anchor == contentBearingTag.Anchor && ((contentBearingTag.Content.StartsWith("<csf ", StringComparison.Ordinal) && !contentBearingTag2.Content.StartsWith("</csf", StringComparison.Ordinal) && contentBearingTag3.Content.Equals("</csf>", StringComparison.Ordinal)) || (contentBearingTag.Content.Equals("<footnotereference>", StringComparison.Ordinal) && !contentBearingTag2.Content.StartsWith("</footnotereference", StringComparison.Ordinal) && contentBearingTag3.Content.Equals("</footnotereference>", StringComparison.Ordinal)) || (contentBearingTag.Content.Equals("<endnotereference>", StringComparison.Ordinal) && !contentBearingTag2.Content.StartsWith("</endnotereference", StringComparison.Ordinal) && contentBearingTag3.Content.Equals("</endnotereference>", StringComparison.Ordinal)) || (contentBearingTag.Content.StartsWith("{\\cs8") && contentBearingTag3.Content.Equals("}"))))
				{
					bool num = contentBearingTag.Content.StartsWith("{\\cs8");
					contentBearingTag3.Anchor = contentBearingTag.Anchor;
					contentBearingTag3.Type = TagType.End;
					contentBearingTag2.Type = TagType.Standalone;
					contentBearingTag2.Anchor = 0;
					contentBearingTag2.AlignmentAnchor = 0;
					if (num)
					{
						contentBearingTag.Type = TagType.UnmatchedStart;
						contentBearingTag3.Type = TagType.UnmatchedEnd;
					}
					result = true;
					i += 2;
				}
			}
			return result;
		}

		internal static bool FixMistaggingsFromWorkbench_ph_it(Segment segment)
		{
			bool result = false;
			Stack<Tag> stack = new Stack<Tag>();
			foreach (SegmentElement element in segment.Elements)
			{
				Tag tag = element as Tag;
				if (tag != null)
				{
					switch (tag.Type)
					{
					case TagType.Standalone:
						stack.Push(tag);
						break;
					case TagType.UnmatchedStart:
						stack.Push(tag);
						break;
					case TagType.UnmatchedEnd:
					{
						Tag tag2 = tag;
						while (stack.Count > 0)
						{
							Tag tag3 = stack.Pop();
							if (HasSameContent(tag3, tag2))
							{
								if (tag3.Type == TagType.Standalone)
								{
									tag3.Type = TagType.Start;
									tag2.Type = TagType.End;
									result = true;
								}
								break;
							}
						}
						break;
					}
					}
				}
			}
			return result;
		}

		private static bool HasSameContent(Tag startTag, Tag endTag)
		{
			string content = GetContent(startTag);
			if (content != null)
			{
				string content2 = GetContent(endTag);
				if (content2 != null)
				{
					if (object.Equals(content, content2))
					{
						return true;
					}
					string contentElementName = GetContentElementName(content);
					if (contentElementName != null)
					{
						string contentElementName2 = GetContentElementName(content2);
						if (contentElementName2 != null)
						{
							return object.Equals(contentElementName, contentElementName2);
						}
					}
				}
			}
			return false;
		}

		private static string GetContent(Tag tag)
		{
			return (tag as ContentBearingTag)?.Content;
		}

		private static string GetContentElementName(string content)
		{
			Match match = _contentElementNameRegex.Match(content);
			if (!match.Success || match.Groups.Count <= 0)
			{
				return null;
			}
			return match.Groups[1].Value;
		}

		internal static bool ChangeUnmatchedTagsToPlaceholderTags(Segment segment)
		{
			bool result = false;
			foreach (SegmentElement element in segment.Elements)
			{
				Tag tag = element as Tag;
				if (tag != null && (tag.Type == TagType.UnmatchedStart || tag.Type == TagType.UnmatchedEnd))
				{
					tag.Type = TagType.Standalone;
					result = true;
				}
			}
			return result;
		}

		private static bool ApplyCleanupHeuristics(Segment segment)
		{
			bool flag = false;
			bool result = false;
			if (HandlePlaceholders(segment))
			{
				result = true;
			}
			do
			{
				flag = false;
				if (segment.DeleteEmptyTagPairs(onlyInPeripheralPositions: true))
				{
					flag = true;
					result = true;
				}
				else if (DeletePeripheralBookmarks(segment))
				{
					flag = true;
					result = true;
				}
			}
			while (flag);
			return result;
		}

		public static bool ConvertEmptyTagPairsToLockedContentPlaceholders(TranslationUnit tu)
		{
			bool result = false;
			if (ConvertEmptyTagPairsToLockedContentPlaceholders(tu.SourceSegment))
			{
				result = true;
			}
			if (ConvertEmptyTagPairsToLockedContentPlaceholders(tu.TargetSegment))
			{
				result = true;
			}
			return result;
		}

		private static bool ConvertEmptyTagPairsToLockedContentPlaceholders(Segment segment)
		{
			bool result = false;
			for (int num = segment.Elements.Count - 1; num > 0; num--)
			{
				Tag tag = segment.Elements[num] as Tag;
				if (tag != null && tag.Type == TagType.End)
				{
					Tag tag2 = segment.Elements[num - 1] as Tag;
					if (tag2 != null && tag2.Anchor == tag.Anchor)
					{
						segment.Elements.RemoveAt(num);
						tag2.Type = TagType.LockedContent;
						num--;
						result = true;
					}
				}
			}
			return result;
		}

		private static bool IsPairedEmptyBookmark(Segment segment, int p)
		{
			if (segment == null || p < 0 || p + 1 >= segment.Elements.Count)
			{
				return false;
			}
			ContentBearingTag contentBearingTag = segment.Elements[p] as ContentBearingTag;
			if (contentBearingTag != null)
			{
				ContentBearingTag contentBearingTag2 = segment.Elements[p + 1] as ContentBearingTag;
				if (contentBearingTag2 != null && contentBearingTag.Type == TagType.Standalone && contentBearingTag2.Type == TagType.Standalone)
				{
					string content = contentBearingTag.Content;
					string content2 = contentBearingTag2.Content;
					if (!content.StartsWith("<bookmarkstart ") || !content2.StartsWith("<bookmarkend "))
					{
						return false;
					}
					string text = ExtractBookmarkName(content);
					string text2 = ExtractBookmarkName(content2);
					if (text == null || text2 == null)
					{
						return false;
					}
					return text.Equals(text2);
				}
			}
			return false;
		}

		private static string ExtractBookmarkName(string s)
		{
			int num;
			if (s == null || (num = s.IndexOf(" name=\"")) < 0)
			{
				return null;
			}
			num += " name=\"".Length;
			int num2 = s.IndexOf('"', num);
			if (num2 < 0)
			{
				return null;
			}
			return s.Substring(num, num2 - num);
		}
	}
}
