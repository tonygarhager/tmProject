using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sdl.LanguagePlatform.Lingua.Segmentation.T8
{
	public class TWBRuleConverter
	{
		private enum Condition
		{
			Ignore,
			Forbid,
			Require
		}

		private struct TwbRule
		{
			public string Description;

			public bool IsException;

			public bool IsFullStopRule;

			public string StopCharacters;

			public int MinSegmentCharacters;

			public int MinSegmentWords;

			public int MinTrailingWhitespace;

			public int MinFullStops;

			public Condition LetterDot;

			public Condition NumberStop;

			public Condition OrdinalDot;

			public Condition FoundAbbreviation;

			public Condition GuessedAbbreviation;

			public Condition TabFollower;

			public Condition LowerFollower;

			public Condition OrdinalFollower;
		}

		private const string DefaultTrailingPunctuationRx = "[\\p{Pe}\\p{Pf}\\p{Po}\"-[\\u002C\\u003A\\u003B\\u055D\\u060C\\u061B\\u0703\\u0704\\u0705\\u0706\\u0707\\u0708\\u0709\\u07F8\\u1363\\u1364\\u1365\\u1366\\u1802\\u1804\\u1808\\u204F\\u205D\\u3001\\uA60D\\uFE10\\uFE11\\uFE13\\uFE14\\uFE50\\uFE51\\uFE54\\uFE55\\uFF0C\\uFF1A\\uFF1B\\uFF64]]*";

		public static SegmentationRules Convert(string twbRulesString, CultureInfo culture, string tmName, out Wordlist ordinalFollowers, out Wordlist abbreviations)
		{
			ordinalFollowers = null;
			abbreviations = null;
			if (twbRulesString == null)
			{
				throw new ArgumentNullException();
			}
			if (twbRulesString.Length == 0)
			{
				throw new ArgumentException("Invalid argument (empty rules)");
			}
			string[] array = twbRulesString.Split('\f');
			if (array.Length < 2)
			{
				throw new ArgumentException("Invalid TWB Rules (missing rules)");
			}
			List<TwbRule> list = new List<TwbRule>();
			string[] array2 = array[0].Split('\r');
			if (array2.Length >= 2)
			{
				abbreviations = ReadTwbWordList(array2[0]);
				ordinalFollowers = ReadTwbWordList(array2[1]);
			}
			for (int i = 1; i < array.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]))
				{
					continue;
				}
				string[] array3 = array[i].Split('\r');
				if (array3.Length != 7)
				{
					throw new LanguagePlatformException(ErrorCode.SegmentationTWBMalformedRule);
				}
				TwbRule twbRule = default(TwbRule);
				twbRule.Description = array3[0];
				twbRule.StopCharacters = array3[1];
				TwbRule item = twbRule;
				if (!string.IsNullOrEmpty(item.StopCharacters) && item.StopCharacters[0] != '\n' && item.StopCharacters[0] != '\v')
				{
					item.IsFullStopRule = (item.StopCharacters.Length == 1 && item.StopCharacters[0] == '.');
					item.MinSegmentCharacters = int.Parse(array3[2]);
					item.MinSegmentWords = int.Parse(array3[3]);
					item.MinTrailingWhitespace = int.Parse(array3[4]);
					if (item.IsFullStopRule)
					{
						item.MinFullStops = int.Parse(array3[5]);
					}
					string text = array3[6];
					if (text.Length != 9)
					{
						throw new LanguagePlatformException(ErrorCode.SegmentationTWBMalformedRule);
					}
					item.IsException = (text[0] - 48 > 0);
					item.NumberStop = GetCondition(text[2] - 48);
					if (item.IsFullStopRule)
					{
						item.LetterDot = GetCondition(text[1] - 48);
						item.OrdinalDot = Condition.Ignore;
						item.FoundAbbreviation = GetCondition(text[4] - 48);
						item.GuessedAbbreviation = GetCondition(text[5] - 48);
						item.OrdinalFollower = GetCondition(text[8] - 48);
					}
					item.TabFollower = GetCondition(text[6] - 48);
					item.LowerFollower = GetCondition(text[7] - 48);
					list.Add(item);
				}
			}
			SegmentationRules segmentationRules = ConvertRules(list, culture, tmName);
			if (segmentationRules == null || segmentationRules.Count <= 0)
			{
				return null;
			}
			return segmentationRules;
		}

		private static Wordlist ReadTwbWordList(string value)
		{
			Wordlist wordlist = new Wordlist(SearchOption.None);
			if (string.IsNullOrEmpty(value))
			{
				if (wordlist.Count <= 0)
				{
					return null;
				}
				return wordlist;
			}
			string[] array = value.Split('\u0001');
			foreach (string text in array)
			{
				string text2 = text.Trim();
				if (text2.Length > 0)
				{
					wordlist.Add(text2);
				}
			}
			if (wordlist.Count <= 0)
			{
				return null;
			}
			return wordlist;
		}

		private static SegmentationRules ConvertRules(IList<TwbRule> twbRules, CultureInfo culture, string tmName)
		{
			SegmentationRules segmentationRules = new SegmentationRules(culture, "Rules converted from TRADOS TM " + tmName);
			foreach (TwbRule twbRule in twbRules)
			{
				if (!twbRule.IsException)
				{
					SegmentationRule segmentationRule = ConvertSingleRule(twbRule);
					if (segmentationRule != null)
					{
						segmentationRules.AddRule(segmentationRule);
					}
				}
			}
			int count = segmentationRules.Count;
			foreach (TwbRule twbRule2 in twbRules)
			{
				if (twbRule2.IsException)
				{
					for (int i = 0; i < count; i++)
					{
						if (HaveCommonChars(twbRule2.StopCharacters, segmentationRules[i].MatchingContext.TriggerChars))
						{
							SegmentationRule segmentationRule2 = ConvertSingleRule(twbRule2);
							if (segmentationRule2 != null)
							{
								if (segmentationRule2.Exceptions.Count > 0)
								{
									throw new LanguagePlatformException(ErrorCode.SegmentationTWBUnsupportedNestedExceptions);
								}
								segmentationRules[i].Exceptions.Add(segmentationRule2.MatchingContext);
							}
						}
					}
				}
			}
			if (segmentationRules.Count != 0)
			{
				return segmentationRules;
			}
			return null;
		}

		private static SegmentationRule ConvertSingleRule(TwbRule twbRule)
		{
			if (twbRule.IsException && (twbRule.FoundAbbreviation == Condition.Forbid || twbRule.GuessedAbbreviation == Condition.Forbid || twbRule.LetterDot == Condition.Forbid || twbRule.LowerFollower == Condition.Forbid || twbRule.NumberStop == Condition.Forbid || twbRule.OrdinalDot == Condition.Forbid || twbRule.OrdinalFollower == Condition.Forbid || twbRule.TabFollower == Condition.Forbid))
			{
				throw new LanguagePlatformException(ErrorCode.SegmentationTWBUnsupportedExceptionConstraints);
			}
			SegmentationRule segmentationRule = new SegmentationRule();
			segmentationRule.Description.SetText(CultureInfo.InvariantCulture, twbRule.Description);
			segmentationRule.MinimumChars = twbRule.MinSegmentCharacters;
			segmentationRule.MinimumWords = twbRule.MinSegmentWords;
			segmentationRule.Origin = RuleOrigin.Migration;
			segmentationRule.IsEnabled = true;
			switch (twbRule.StopCharacters)
			{
			case ".":
				segmentationRule.Type = RuleType.FullStopRule;
				break;
			case "?!":
				segmentationRule.Type = RuleType.MarksRule;
				break;
			case ";":
				segmentationRule.Type = RuleType.SemicolonRule;
				break;
			case ":":
				segmentationRule.Type = RuleType.ColonRule;
				break;
			default:
				segmentationRule.Type = RuleType.Other;
				break;
			}
			string text = "[" + Regex.Escape(twbRule.StopCharacters) + "]";
			if (twbRule.MinFullStops <= 1)
			{
				text += "+";
			}
			else if (twbRule.MinFullStops > 1)
			{
				text = text + "{" + twbRule.MinFullStops.ToString(CultureInfo.InvariantCulture) + ",}";
			}
			string trailingWhitespaceRx = string.Empty;
			if (twbRule.MinTrailingWhitespace == 1)
			{
				trailingWhitespaceRx = "\\s";
			}
			else if (twbRule.MinTrailingWhitespace > 1)
			{
				trailingWhitespaceRx = "\\\\s{" + twbRule.MinTrailingWhitespace.ToString(CultureInfo.InvariantCulture) + ",}";
			}
			if (twbRule.IsFullStopRule)
			{
				switch (twbRule.FoundAbbreviation)
				{
				case Condition.Forbid:
					segmentationRule.AddException(CreateAbbreviationContext(trailingWhitespaceRx, isException: true));
					break;
				case Condition.Require:
					if (segmentationRule.MatchingContext == null)
					{
						segmentationRule.MatchingContext = CreateAbbreviationContext(trailingWhitespaceRx, isException: false);
						break;
					}
					throw new LanguagePlatformException(ErrorCode.SegmentationTWBUnsupportedMultipleMatchContexts);
				}
				switch (twbRule.LetterDot)
				{
				case Condition.Forbid:
					segmentationRule.AddException(CreateSingleLetterDotContext(trailingWhitespaceRx, isException: true));
					break;
				case Condition.Require:
					if (segmentationRule.MatchingContext == null)
					{
						segmentationRule.MatchingContext = CreateSingleLetterDotContext(trailingWhitespaceRx, isException: false);
						break;
					}
					throw new LanguagePlatformException(ErrorCode.SegmentationTWBUnsupportedMultipleMatchContexts);
				}
				switch (twbRule.OrdinalFollower)
				{
				case Condition.Forbid:
					segmentationRule.AddException(CreateOrdinalFollowerContext(trailingWhitespaceRx, isException: true));
					break;
				case Condition.Require:
					if (segmentationRule.MatchingContext == null)
					{
						segmentationRule.MatchingContext = CreateOrdinalFollowerContext(trailingWhitespaceRx, isException: false);
						break;
					}
					throw new LanguagePlatformException(ErrorCode.SegmentationTWBUnsupportedMultipleMatchContexts);
				}
			}
			switch (twbRule.LowerFollower)
			{
			case Condition.Forbid:
				segmentationRule.AddException(CreateLowerFollowerContext(text, trailingWhitespaceRx, isException: true));
				break;
			}
			switch (twbRule.NumberStop)
			{
			case Condition.Forbid:
				segmentationRule.AddException(CreateNumberStopContext(text, trailingWhitespaceRx, isException: true));
				break;
			case Condition.Require:
				if (segmentationRule.MatchingContext == null)
				{
					segmentationRule.MatchingContext = CreateNumberStopContext(text, trailingWhitespaceRx, isException: false);
					break;
				}
				throw new LanguagePlatformException(ErrorCode.SegmentationTWBUnsupportedMultipleMatchContexts);
			}
			if (segmentationRule.MatchingContext == null)
			{
				if (twbRule.StopCharacters[0] != '\t')
				{
					segmentationRule.MatchingContext = CreateDefaultContext(text + "[\\p{Pe}\\p{Pf}\\p{Po}\"-[\\u002C\\u003A\\u003B\\u055D\\u060C\\u061B\\u0703\\u0704\\u0705\\u0706\\u0707\\u0708\\u0709\\u07F8\\u1363\\u1364\\u1365\\u1366\\u1802\\u1804\\u1808\\u204F\\u205D\\u3001\\uA60D\\uFE10\\uFE11\\uFE13\\uFE14\\uFE50\\uFE51\\uFE54\\uFE55\\uFF0C\\uFF1A\\uFF1B\\uFF64]]*", trailingWhitespaceRx, twbRule.IsException);
				}
				else
				{
					segmentationRule.MatchingContext = CreateDefaultContext(text, trailingWhitespaceRx, twbRule.IsException);
				}
			}
			segmentationRule.MatchingContext.TriggerChars = twbRule.StopCharacters;
			return segmentationRule;
		}

		private static SegmentationContext CreateDefaultContext(string stopRx, string trailingWhitespaceRx, bool isException)
		{
			SegmentationContext segmentationContext = new SegmentationContext
			{
				PrecedingContext = new Context(stopRx, caseInsensitive: false, matchesInputBoundary: false)
			};
			if (!string.IsNullOrEmpty(trailingWhitespaceRx))
			{
				segmentationContext.FollowingContext = new Context(trailingWhitespaceRx, caseInsensitive: false, matchesInputBoundary: true);
			}
			if (isException)
			{
				segmentationContext.Description = new LocalizedString("Standard Exception");
				segmentationContext.ContextType = ContextType.OtherException;
			}
			else
			{
				segmentationContext.Description = new LocalizedString("Standard Break Rule");
				segmentationContext.ContextType = ContextType.MatchContext;
			}
			return segmentationContext;
		}

		private static SegmentationContext CreateAbbreviationContext(string trailingWhitespaceRx, bool isException)
		{
			SegmentationContext segmentationContext = new SegmentationContext
			{
				Requires = "$ABBREVIATIONS",
				PrecedingContext = new Context("(^|[^\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Nd}\\p{Pc}\\p{Lm}\\p{Po}])$ABBREVIATIONS[\\p{Pe}\\p{Pf}\\p{Po}\"-[.]]*", caseInsensitive: false, matchesInputBoundary: false),
				FollowingContext = new Context(trailingWhitespaceRx, caseInsensitive: false, matchesInputBoundary: false)
			};
			if (isException)
			{
				segmentationContext.ContextType = ContextType.AbbreviationException;
				segmentationContext.Description = new LocalizedString("Abbreviation Exception");
			}
			else
			{
				segmentationContext.ContextType = ContextType.Other;
				segmentationContext.Description = new LocalizedString("Abbreviation Rule");
			}
			return segmentationContext;
		}

		private static SegmentationContext CreateOrdinalFollowerContext(string trailingWhitespaceRx, bool isException)
		{
			SegmentationContext segmentationContext = new SegmentationContext
			{
				Requires = "$ORDINALFOLLOWERS",
				PrecedingContext = new Context("(^|[^\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Nd}\\p{Pc}\\p{Lm}\\p{Po}])[0-9]+\\.", caseInsensitive: false, matchesInputBoundary: false),
				FollowingContext = new Context(trailingWhitespaceRx + "$ORDINALFOLLOWERS($|[^\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Nd}\\p{Pc}\\p{Lm}\\p{Po}])", caseInsensitive: false, matchesInputBoundary: false)
			};
			if (isException)
			{
				segmentationContext.ContextType = ContextType.OrdinalFollowerException;
				segmentationContext.Description = new LocalizedString("Ordinal Followers Exception");
			}
			else
			{
				segmentationContext.Description = new LocalizedString("Ordinal Followers Rule");
				segmentationContext.ContextType = ContextType.Other;
			}
			return segmentationContext;
		}

		private static SegmentationContext CreateSingleLetterDotContext(string trailingWhitespaceRx, bool isException)
		{
			SegmentationContext segmentationContext = new SegmentationContext
			{
				PrecedingContext = new Context("(^|\\W)[\\p{Ll}\\p{Lu}]\\.", caseInsensitive: false, matchesInputBoundary: false)
			};
			if (!string.IsNullOrEmpty(trailingWhitespaceRx))
			{
				segmentationContext.FollowingContext = new Context(trailingWhitespaceRx, caseInsensitive: false, matchesInputBoundary: false);
			}
			if (isException)
			{
				segmentationContext.ContextType = ContextType.OtherException;
				segmentationContext.Description = new LocalizedString("Single-Letter Dot Exception");
			}
			else
			{
				segmentationContext.ContextType = ContextType.Other;
				segmentationContext.Description = new LocalizedString("Single-Letter Dot Rule");
			}
			return segmentationContext;
		}

		private static SegmentationContext CreateNumberStopContext(string stopRx, string trailingWhitespaceRx, bool isException)
		{
			SegmentationContext segmentationContext = new SegmentationContext
			{
				PrecedingContext = new Context("(^|\\W)[0-9]+" + stopRx, caseInsensitive: false, matchesInputBoundary: false)
			};
			if (!string.IsNullOrEmpty(trailingWhitespaceRx))
			{
				segmentationContext.FollowingContext = new Context(trailingWhitespaceRx, caseInsensitive: false, matchesInputBoundary: false);
			}
			if (isException)
			{
				segmentationContext.ContextType = ContextType.OtherException;
				segmentationContext.Description = new LocalizedString("Number-Stop Exception");
			}
			else
			{
				segmentationContext.ContextType = ContextType.Other;
				segmentationContext.Description = new LocalizedString("Number-Stop Rule");
			}
			return segmentationContext;
		}

		private static SegmentationContext CreateLowerFollowerContext(string stopRx, string trailingWhitespaceRx, bool isException)
		{
			SegmentationContext segmentationContext = new SegmentationContext
			{
				PrecedingContext = new Context(stopRx + "[\\p{Pe}\\p{Pf}\\p{Po}\"-[\\u002C\\u003A\\u003B\\u055D\\u060C\\u061B\\u0703\\u0704\\u0705\\u0706\\u0707\\u0708\\u0709\\u07F8\\u1363\\u1364\\u1365\\u1366\\u1802\\u1804\\u1808\\u204F\\u205D\\u3001\\uA60D\\uFE10\\uFE11\\uFE13\\uFE14\\uFE50\\uFE51\\uFE54\\uFE55\\uFF0C\\uFF1A\\uFF1B\\uFF64]]*", caseInsensitive: false, matchesInputBoundary: false),
				FollowingContext = new Context(trailingWhitespaceRx + "[\\p{Ll}]", caseInsensitive: false, matchesInputBoundary: false)
			};
			if (isException)
			{
				segmentationContext.ContextType = ContextType.LowercaseFollowerException;
				segmentationContext.Description = new LocalizedString("Lowercase-Follower Exception");
			}
			else
			{
				segmentationContext.ContextType = ContextType.Other;
				segmentationContext.Description = new LocalizedString("Lowercase-Follower Rule");
			}
			return segmentationContext;
		}

		private static bool HaveCommonChars(string a, string b)
		{
			if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
			{
				return false;
			}
			return a.IndexOfAny(b.ToCharArray()) >= 0;
		}

		private static Condition GetCondition(int v)
		{
			switch (v)
			{
			default:
				return Condition.Ignore;
			case 1:
				return Condition.Require;
			case 0:
				return Condition.Forbid;
			}
		}
	}
}
