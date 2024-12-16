using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal class SDLXCleaner
	{
		private static Dictionary<string, string> _TagTypeMappings;

		static SDLXCleaner()
		{
			_TagTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			string[] array = new string[18]
			{
				"Bold",
				"Italic",
				"Underline",
				"Subscript",
				"Superscript",
				"SmallCaps",
				"SoftBreak",
				"LeftToRight",
				"RightToLeft",
				"EmDash",
				"EnDash",
				"NonBreakingHyphen",
				"OptionalHyphen",
				"NonBreakingSpace",
				"Euro",
				"Copyright",
				"Registered",
				"Trademark"
			};
			foreach (string text in array)
			{
				_TagTypeMappings.Add(text, text);
			}
			_TagTypeMappings.Add("ulined", "Underline");
		}

		public static bool ApplyExtendedCleanupHeuristics(TranslationUnit tu)
		{
			bool result = false;
			if (ApplyExtendedCleanupHeuristics(tu.SourceSegment))
			{
				result = true;
			}
			if (ApplyExtendedCleanupHeuristics(tu.TargetSegment))
			{
				result = true;
			}
			return result;
		}

		public static void MapSDLXTagTypes(Tag t)
		{
			if (t != null && !string.IsNullOrEmpty(t.TagID) && _TagTypeMappings.TryGetValue(t.TagID, out string value))
			{
				t.TagID = value;
			}
		}

		private static bool ApplyExtendedCleanupHeuristics(Segment segment)
		{
			bool flag = false;
			bool result = false;
			do
			{
				flag = false;
				if (HandleListsAndBullets(segment))
				{
					flag = true;
				}
				else if (segment.DeleteEmptyTagPairs(onlyInPeripheralPositions: true))
				{
					flag = true;
				}
				if (flag)
				{
					result = true;
				}
			}
			while (flag);
			return result;
		}

		public static bool ApplyStandardCleanupHeuristics(Segment segment)
		{
			bool result = false;
			if (HandleDanglingTags(segment))
			{
				result = true;
			}
			if (segment.DeleteEmptyTagPairs(onlyInPeripheralPositions: true))
			{
				result = true;
			}
			return result;
		}

		public static bool HandleDanglingTags(Segment segment)
		{
			bool result = false;
			if (segment != null && segment.HasUnmatchedStartOrEndTags())
			{
				if (segment.RemoveUnmatchedStartAndEndTags(peripheralPositionsOnly: true))
				{
					result = true;
				}
				if (segment.FillUnmatchedStartAndEndTags())
				{
					result = true;
				}
				if (segment.RemoveUnmatchedStartAndEndTags())
				{
					result = true;
				}
			}
			return result;
		}

		private static bool HandleListsAndBullets(Segment segment)
		{
			if (segment?.Elements == null || segment.Elements.Count < 3)
			{
				return false;
			}
			Tag tag = segment.Elements[0] as Tag;
			if (tag == null)
			{
				return false;
			}
			Text text = segment.Elements[1] as Text;
			if (text == null || string.IsNullOrEmpty(text.Value) || !text.Value.EndsWith("\t"))
			{
				return false;
			}
			Tag tag2 = segment.Elements[2] as Tag;
			if (tag2 == null)
			{
				return false;
			}
			if (tag.Type != TagType.Start || tag2.Type != TagType.End)
			{
				return false;
			}
			if (tag.Anchor != tag2.Anchor)
			{
				return false;
			}
			bool flag = false;
			string value = text.Value;
			int length = text.Value.Length;
			char c = value[0];
			if (char.IsDigit(c))
			{
				int num = 0;
				bool flag2 = false;
				do
				{
					flag2 = false;
					while (num < length && char.IsDigit(value, num))
					{
						num++;
						flag2 = true;
					}
					if (flag2 && num < length && value[num] == '.')
					{
						num++;
					}
				}
				while (flag2);
				if (num == length - 1)
				{
					flag = true;
				}
			}
			else if (c > '\0' && c <= 'ÿ')
			{
				if (length == 2)
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				int num2 = 3;
				if (segment.Elements.Count >= 4)
				{
					Tag tag3 = segment.Elements[3] as Tag;
					if (tag3 != null && tag3.Type == TagType.Standalone)
					{
						num2++;
					}
				}
				segment.Elements.RemoveRange(0, num2);
				segment.Trim();
			}
			return flag;
		}
	}
}
