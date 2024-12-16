using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing
{
	internal class TokenizerHelper
	{
		internal static Func<IWordBoundaryFinder> GetWordBoundaryFinder = () => new StandardWordBoundaryFinder();

		private static IEnumerable<Token> GetIcuTokens(string sequence, SegmentRange first, SegmentRange last, CultureInfo culture)
		{
			List<Token> list = new List<Token>();
			SimpleToken item;
			if (sequence.Length == 1)
			{
				item = new SimpleToken(sequence, TokenType.Word)
				{
					Span = new SegmentRange(first.From.Index, first.From.Position, last.From.Position),
					Culture = culture
				};
				list.Add(item);
				return list;
			}
			bool flag = false;
			List<int> list2;
			if (culture.ToString() == "ja-JP")
			{
				flag = true;
				IWordBoundaryFinder wordBoundaryFinder = GetWordBoundaryFinder();
				list2 = wordBoundaryFinder.FindJaBoundaries(sequence);
			}
			else
			{
				IWordBoundaryFinder wordBoundaryFinder2 = GetWordBoundaryFinder();
				list2 = wordBoundaryFinder2.FindZhBoundaries(sequence, culture.ToString());
			}
			if (list2.Count > 1)
			{
				list2.RemoveAt(list2.Count - 1);
			}
			SortedSet<int> sortedSet = new SortedSet<int>(list2);
			List<int> list3 = new List<int>();
			foreach (int item2 in sortedSet)
			{
				if (item2 < 0 || item2 > sequence.Length - 1)
				{
					list3.Add(item2);
				}
				else if (flag && CharacterProperties.IsJaLongVowelMarker(sequence[item2]))
				{
					list3.Add(item2);
				}
			}
			foreach (int item3 in list3)
			{
				sortedSet.Remove(item3);
			}
			if (sortedSet.Count < 1)
			{
				item = new SimpleToken(sequence, TokenType.Word)
				{
					Span = new SegmentRange(first.From.Index, first.From.Position, last.From.Position),
					Culture = culture
				};
				list.Add(item);
				return list;
			}
			int num = 0;
			int num2 = 0;
			string text;
			foreach (int item4 in sortedSet)
			{
				text = sequence.Substring(num, item4 - num);
				item = new SimpleToken(text, TokenType.Word)
				{
					Span = new SegmentRange(first.From.Index, first.From.Position + num2, first.From.Position + item4 - 1),
					Culture = culture
				};
				list.Add(item);
				num2 += item.Span.Length;
				num = item4;
			}
			text = sequence.Substring(num);
			item = new SimpleToken(text, TokenType.Word)
			{
				Span = new SegmentRange(last.From.Index, list[list.Count - 1].Span.Into.Position + 1, last.From.Position),
				Culture = culture
			};
			list.Add(item);
			return list;
		}

		public List<Token> TokenizeIcu(List<Token> tokens, CultureInfo culture, Wordlist stopWordlist)
		{
			if (!IsCjCulture(culture))
			{
				return tokens;
			}
			bool flag = string.CompareOrdinal(culture.Name.Substring(0, 2), "ja") == 0;
			List<Token> list = new List<Token>();
			string text = string.Empty;
			SegmentRange first = null;
			SegmentRange last = null;
			foreach (Token token in tokens)
			{
				if (token.Type == TokenType.CharSequence || (flag && token.Text != null && token.Text.Length == 1 && CharacterProperties.IsJaLongVowelMarker(token.Text[0])))
				{
					if (text == string.Empty)
					{
						first = token.Span;
					}
					last = token.Span;
					text += token.Text;
				}
				else
				{
					if (text != string.Empty)
					{
						list.AddRange(GetIcuTokens(text, first, last, culture));
						text = string.Empty;
					}
					list.Add(token);
				}
			}
			if (text != string.Empty)
			{
				list.AddRange(GetIcuTokens(text, first, last, culture));
			}
			if (stopWordlist == null)
			{
				return list;
			}
			foreach (Token item in list)
			{
				SimpleToken simpleToken = item as SimpleToken;
				if (simpleToken != null && stopWordlist.Contains(simpleToken.Text))
				{
					simpleToken.IsStopword = true;
				}
			}
			return list;
		}

		public static bool IsCjCulture(CultureInfo culture)
		{
			return "zh-CHT, zh-TW, zh-CN, zh-HK, zh-SG, zh-MO, ja-JP".Contains(culture.ToString());
		}

		internal static bool UsesAdvancedTokenization(CultureInfo culture)
		{
			return IsCjCulture(culture);
		}
	}
}
