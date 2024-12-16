using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Core
{
	public static class CharacterProperties
	{
		public enum Case
		{
			Upper,
			Lower,
			InitialUpper,
			Other
		}

		private struct BlockRange
		{
			public readonly char First;

			public readonly char Last;

			public readonly UnicodeBlock Block;

			public BlockRange(char first, char last, UnicodeBlock block)
			{
				First = first;
				Last = last;
				Block = block;
			}
		}

		private static readonly BlockRange[] BlockRanges;

		private static readonly Dictionary<UnicodeBlock, BlockRange> BlockRangeIndex;

		public static readonly char[] WhitespaceCharacters;

		public static readonly char[] Blanks;

		private static readonly Dictionary<char, char> BaseChars;

		private static readonly char[] ApostropheCharacters;

		private static readonly char[] ColonCharacters;

		private static readonly char[] CommaCharacters;

		private static readonly char[] DotCharacters;

		private static readonly char[] ExclamationCharacters;

		private static readonly char[] HyphenCharacters;

		private static readonly char[] DashCharacters;

		private static readonly char[] QuestionMarkCharacters;

		private static readonly char[] SemicolonCharacters;

		private static readonly char[] BraceCharacters;

		private static readonly char[] ParagraphSeparatorCharacters;

		private static readonly char[] OpeningBracketCharacters;

		private static readonly char[] ClosingBracketCharacters;

		private static readonly char[] OpeningParenthesisCharacters;

		private static readonly char[] ClosingParenthesisCharacters;

		private static readonly char[] QuoteCharacters;

		private static readonly char[] PercentCharacters;

		private static readonly char[] SingleQuoteCharacters;

		private static readonly char[] OpeningQuoteCharacters;

		private static readonly char[] ClosingQuoteCharacters;

		private static readonly char[] WesternVowelCharacters;

		private static readonly char[] FarEastVowelCharacters;

		private static readonly char[] SpecialSurrounderCharacters;

		private static char[] _uppercaseCharacters;

		public static char[] UppercaseCharacters
		{
			get
			{
				if (_uppercaseCharacters != null)
				{
					return _uppercaseCharacters;
				}
				List<char> list = new List<char>();
				for (char c = ' '; c < '￼'; c = (char)(c + 1))
				{
					if (char.IsUpper(c))
					{
						list.Add(c);
					}
				}
				_uppercaseCharacters = list.ToArray();
				return _uppercaseCharacters;
			}
		}

		static CharacterProperties()
		{
			WhitespaceCharacters = new char[26]
			{
				'\t',
				'\n',
				'\v',
				'\f',
				'\r',
				' ',
				'\u0085',
				'\u00a0',
				'\u1680',
				'\u180e',
				'\u2000',
				'\u2001',
				'\u2002',
				'\u2003',
				'\u2004',
				'\u2005',
				'\u2006',
				'\u2007',
				'\u2008',
				'\u2009',
				'\u200a',
				'\u2028',
				'\u2029',
				'\u202f',
				'\u205f',
				'\u3000'
			};
			Blanks = new char[17]
			{
				' ',
				'\u00a0',
				'\u1680',
				'\u2000',
				'\u2001',
				'\u2002',
				'\u2003',
				'\u2004',
				'\u2005',
				'\u2006',
				'\u2007',
				'\u2008',
				'\u2009',
				'\u200a',
				'\u202f',
				'\u205f',
				'\u3000'
			};
			BaseChars = new Dictionary<char, char>();
			ApostropheCharacters = new char[5]
			{
				'\'',
				'\u02bc',
				'՚',
				'’',
				'＇'
			};
			ColonCharacters = new char[6]
			{
				':',
				'\u02d0',
				'\u02d1',
				'؛',
				'﹕',
				'：'
			};
			CommaCharacters = new char[9]
			{
				',',
				'՝',
				'،',
				'‚',
				'、',
				'﹐',
				'﹑',
				'，',
				'､'
			};
			DotCharacters = new char[8]
			{
				'.',
				'·',
				'։',
				'۔',
				'。',
				'﹒',
				'．',
				'｡'
			};
			ExclamationCharacters = new char[8]
			{
				'!',
				'¡',
				'՜',
				'‼',
				'❢',
				'❣',
				'﹗',
				'！'
			};
			HyphenCharacters = new char[7]
			{
				'-',
				'­',
				'‐',
				'‑',
				'−',
				'﹣',
				'－'
			};
			DashCharacters = new char[4]
			{
				'‒',
				'–',
				'—',
				'―'
			};
			QuestionMarkCharacters = new char[7]
			{
				'?',
				'¿',
				';',
				'՞',
				'؟',
				'﹖',
				'？'
			};
			SemicolonCharacters = new char[4]
			{
				';',
				'؛',
				'﹔',
				'；'
			};
			BraceCharacters = new char[8]
			{
				'{',
				'}',
				'︷',
				'︸',
				'﹛',
				'﹜',
				'｛',
				'｝'
			};
			ParagraphSeparatorCharacters = new char[2]
			{
				'჻',
				'\u2029'
			};
			OpeningBracketCharacters = new char[25]
			{
				'[',
				'{',
				'⁅',
				'〈',
				'〈',
				'《',
				'「',
				'『',
				'【',
				'〔',
				'〖',
				'〘',
				'〚',
				'︷',
				'︹',
				'︻',
				'︽',
				'︿',
				'﹁',
				'﹃',
				'﹛',
				'﹝',
				'［',
				'｛',
				'｢'
			};
			ClosingBracketCharacters = new char[25]
			{
				']',
				'}',
				'⁆',
				'〉',
				'〉',
				'》',
				'」',
				'』',
				'】',
				'〕',
				'〗',
				'〙',
				'〛',
				'︸',
				'︺',
				'︼',
				'︾',
				'﹀',
				'﹂',
				'﹄',
				'﹜',
				'﹞',
				'］',
				'｝',
				'｣'
			};
			OpeningParenthesisCharacters = new char[3]
			{
				'(',
				'﹙',
				'（'
			};
			ClosingParenthesisCharacters = new char[3]
			{
				')',
				'﹚',
				'）'
			};
			QuoteCharacters = new char[21]
			{
				'"',
				'«',
				'»',
				'‘',
				'’',
				'‚',
				'‛',
				'“',
				'”',
				'„',
				'‟',
				'‹',
				'›',
				'❛',
				'❜',
				'❝',
				'❞',
				'〝',
				'〞',
				'〟',
				'＂'
			};
			PercentCharacters = new char[4]
			{
				'%',
				'٪',
				'﹪',
				'％'
			};
			SingleQuoteCharacters = new char[12]
			{
				'\'',
				'‘',
				'’',
				'‚',
				'‛',
				'‹',
				'›',
				'❛',
				'❜',
				'〝',
				'〞',
				'〟'
			};
			OpeningQuoteCharacters = new char[4]
			{
				'«',
				'‘',
				'“',
				'‹'
			};
			ClosingQuoteCharacters = new char[4]
			{
				'»',
				'’',
				'”',
				'›'
			};
			WesternVowelCharacters = new char[10]
			{
				'A',
				'E',
				'I',
				'O',
				'U',
				'a',
				'e',
				'i',
				'o',
				'u'
			};
			FarEastVowelCharacters = new char[20]
			{
				'ぁ',
				'あ',
				'ぃ',
				'い',
				'ぅ',
				'う',
				'ぇ',
				'え',
				'ぉ',
				'お',
				'ァ',
				'ア',
				'ィ',
				'イ',
				'ゥ',
				'ウ',
				'ェ',
				'エ',
				'ォ',
				'オ'
			};
			SpecialSurrounderCharacters = new char[6]
			{
				'<',
				'>',
				'﹤',
				'﹥',
				'＜',
				'＝'
			};
			BlockRanges = new BlockRange[126]
			{
				new BlockRange('\0', '\u007f', UnicodeBlock.BasicLatin),
				new BlockRange('\u0080', 'ÿ', UnicodeBlock.Latin1Supplement),
				new BlockRange('Ā', 'ſ', UnicodeBlock.LatinExtendedA),
				new BlockRange('ƀ', 'ɏ', UnicodeBlock.LatinExtendedB),
				new BlockRange('ɐ', 'ʯ', UnicodeBlock.IPAExtensions),
				new BlockRange('\u02b0', '\u02ff', UnicodeBlock.SpacingModifierLetters),
				new BlockRange('\u0300', '\u036f', UnicodeBlock.CombiningDiacriticalMarks),
				new BlockRange('Ͱ', 'Ͽ', UnicodeBlock.GreekAndCoptic),
				new BlockRange('Ѐ', 'ӿ', UnicodeBlock.Cyrillic),
				new BlockRange('Ԁ', 'ԯ', UnicodeBlock.CyrillicSupplement),
				new BlockRange('\u0530', '֏', UnicodeBlock.Armenian),
				new BlockRange('\u0590', '\u05ff', UnicodeBlock.Hebrew),
				new BlockRange('\u0600', 'ۿ', UnicodeBlock.Arabic),
				new BlockRange('܀', 'ݏ', UnicodeBlock.Syriac),
				new BlockRange('ݐ', 'ݿ', UnicodeBlock.ArabicSupplement),
				new BlockRange('ހ', '\u07bf', UnicodeBlock.Thaana),
				new BlockRange('߀', '\u07ff', UnicodeBlock.NKo),
				new BlockRange('\u0900', 'ॿ', UnicodeBlock.Devanagari),
				new BlockRange('ঀ', '\u09ff', UnicodeBlock.Bengali),
				new BlockRange('\u0a00', '\u0a7f', UnicodeBlock.Gurmukhi),
				new BlockRange('\u0a80', '\u0aff', UnicodeBlock.Gujarati),
				new BlockRange('\u0b00', '\u0b7f', UnicodeBlock.Oriya),
				new BlockRange('\u0b80', '\u0bff', UnicodeBlock.Tamil),
				new BlockRange('\u0c00', '౿', UnicodeBlock.Telugu),
				new BlockRange('\u0c80', '\u0cff', UnicodeBlock.Kannada),
				new BlockRange('\u0d00', 'ൿ', UnicodeBlock.Malayalam),
				new BlockRange('\u0d80', '\u0dff', UnicodeBlock.Sinhala),
				new BlockRange('\u0e00', '\u0e7f', UnicodeBlock.Thai),
				new BlockRange('\u0e80', '\u0eff', UnicodeBlock.Lao),
				new BlockRange('ༀ', '\u0fff', UnicodeBlock.Tibetan),
				new BlockRange('က', '႟', UnicodeBlock.Myanmar),
				new BlockRange('Ⴀ', 'ჿ', UnicodeBlock.Georgian),
				new BlockRange('ᄀ', 'ᇿ', UnicodeBlock.HangulJamo),
				new BlockRange('ሀ', '\u137f', UnicodeBlock.Ethiopic),
				new BlockRange('ᎀ', '\u139f', UnicodeBlock.EthiopicSupplement),
				new BlockRange('Ꭰ', '\u13ff', UnicodeBlock.Cherokee),
				new BlockRange('᐀', 'ᙿ', UnicodeBlock.UnifiedCanadianAboriginalSyllabics),
				new BlockRange('\u1680', '\u169f', UnicodeBlock.Ogham),
				new BlockRange('ᚠ', '\u16ff', UnicodeBlock.Runic),
				new BlockRange('ᜀ', '\u171f', UnicodeBlock.Tagalog),
				new BlockRange('ᜠ', '\u173f', UnicodeBlock.Hanunoo),
				new BlockRange('ᝀ', '\u175f', UnicodeBlock.Buhid),
				new BlockRange('ᝠ', '\u177f', UnicodeBlock.Tagbanwa),
				new BlockRange('ក', '\u17ff', UnicodeBlock.Khmer),
				new BlockRange('᠀', '\u18af', UnicodeBlock.Mongolian),
				new BlockRange('ᤀ', '᥏', UnicodeBlock.Limbu),
				new BlockRange('ᥐ', '\u197f', UnicodeBlock.TaiLe),
				new BlockRange('ᦀ', '᧟', UnicodeBlock.NewTaiLue),
				new BlockRange('᧠', '᧿', UnicodeBlock.KhmerSymbols),
				new BlockRange('ᨀ', '᨟', UnicodeBlock.Buginese),
				new BlockRange('\u1b00', '\u1b7f', UnicodeBlock.Balinese),
				new BlockRange('ᴀ', 'ᵿ', UnicodeBlock.PhoneticExtensions),
				new BlockRange('ᶀ', '\u1dbf', UnicodeBlock.PhoneticExtensionsSupplement),
				new BlockRange('\u1dc0', '\u1dff', UnicodeBlock.CombiningDiacriticalMarksSupplement),
				new BlockRange('Ḁ', 'ỿ', UnicodeBlock.LatinExtendedAdditional),
				new BlockRange('ἀ', '\u1fff', UnicodeBlock.GreekExtended),
				new BlockRange('\u2000', '\u206f', UnicodeBlock.GeneralPunctuation),
				new BlockRange('⁰', '\u209f', UnicodeBlock.SuperscriptsAndSubscripts),
				new BlockRange('₠', '\u20cf', UnicodeBlock.CurrencySymbols),
				new BlockRange('\u20d0', '\u20ff', UnicodeBlock.CombiningDiacriticalMarksForSymbols),
				new BlockRange('℀', '⅏', UnicodeBlock.LetterlikeSymbols),
				new BlockRange('⅐', '\u218f', UnicodeBlock.NumberForms),
				new BlockRange('←', '⇿', UnicodeBlock.Arrows),
				new BlockRange('∀', '⋿', UnicodeBlock.MathematicalOperators),
				new BlockRange('⌀', '\u23ff', UnicodeBlock.MiscellaneousTechnical),
				new BlockRange('␀', '\u243f', UnicodeBlock.ControlPictures),
				new BlockRange('⑀', '\u245f', UnicodeBlock.OpticalCharacterRecognition),
				new BlockRange('①', '⓿', UnicodeBlock.EnclosedAlphanumerics),
				new BlockRange('─', '╿', UnicodeBlock.BoxDrawing),
				new BlockRange('▀', '▟', UnicodeBlock.BlockElements),
				new BlockRange('■', '◿', UnicodeBlock.GeometricShapes),
				new BlockRange('☀', '⛿', UnicodeBlock.MiscellaneousSymbols),
				new BlockRange('✀', '➿', UnicodeBlock.Dingbats),
				new BlockRange('⟀', '⟯', UnicodeBlock.MiscellaneousMathematicalSymbolsA),
				new BlockRange('⟰', '⟿', UnicodeBlock.SupplementalArrowsA),
				new BlockRange('⠀', '⣿', UnicodeBlock.BraillePatterns),
				new BlockRange('⤀', '⥿', UnicodeBlock.SupplementalArrowsB),
				new BlockRange('⦀', '⧿', UnicodeBlock.MiscellaneousMathematicalSymbolsB),
				new BlockRange('⨀', '⫿', UnicodeBlock.SupplementalMathematicalOperators),
				new BlockRange('⬀', '\u2bff', UnicodeBlock.MiscellaneousSymbolsAndArrows),
				new BlockRange('Ⰰ', '\u2c5f', UnicodeBlock.Glagolitic),
				new BlockRange('Ⱡ', 'Ɀ', UnicodeBlock.LatinExtendedC),
				new BlockRange('Ⲁ', '⳿', UnicodeBlock.Coptic),
				new BlockRange('ⴀ', '\u2d2f', UnicodeBlock.GeorgianSupplement),
				new BlockRange('ⴰ', '\u2d7f', UnicodeBlock.Tifinagh),
				new BlockRange('ⶀ', '\u2ddf', UnicodeBlock.EthiopicExtended),
				new BlockRange('⸀', '\u2e7f', UnicodeBlock.SupplementalPunctuation),
				new BlockRange('⺀', '\u2eff', UnicodeBlock.CJKRadicalsSupplement),
				new BlockRange('⼀', '\u2fdf', UnicodeBlock.KangxiRadicals),
				new BlockRange('⿰', '\u2fff', UnicodeBlock.IdeographicDescriptionCharacters),
				new BlockRange('\u3000', '〿', UnicodeBlock.CJKSymbolsAndPunctuation),
				new BlockRange('\u3040', 'ゟ', UnicodeBlock.Hiragana),
				new BlockRange('゠', 'ヿ', UnicodeBlock.Katakana),
				new BlockRange('\u3100', '\u312f', UnicodeBlock.Bopomofo),
				new BlockRange('\u3130', '\u318f', UnicodeBlock.HangulCompatibilityJamo),
				new BlockRange('㆐', '㆟', UnicodeBlock.Kanbun),
				new BlockRange('ㆠ', '\u31bf', UnicodeBlock.BopomofoExtended),
				new BlockRange('㇀', '\u31ef', UnicodeBlock.CJKStrokes),
				new BlockRange('ㇰ', 'ㇿ', UnicodeBlock.KatakanaPhoneticExtensions),
				new BlockRange('㈀', '\u32ff', UnicodeBlock.EnclosedCJKLettersAndMonths),
				new BlockRange('㌀', '㏿', UnicodeBlock.CJKCompatibility),
				new BlockRange('㐀', '\u4dbf', UnicodeBlock.CJKUnifiedIdeographsExtensionA),
				new BlockRange('䷀', '䷿', UnicodeBlock.YijingHexagramSymbols),
				new BlockRange('一', '\u9fff', UnicodeBlock.CJKUnifiedIdeographs),
				new BlockRange('ꀀ', '\ua48f', UnicodeBlock.YiSyllables),
				new BlockRange('꒐', '\ua4cf', UnicodeBlock.YiRadicals),
				new BlockRange('\ua700', '\ua71f', UnicodeBlock.ModifierToneLetters),
				new BlockRange('\ua720', 'ꟿ', UnicodeBlock.LatinExtendedD),
				new BlockRange('ꠀ', '\ua82f', UnicodeBlock.SylotiNagri),
				new BlockRange('ꡀ', '\ua87f', UnicodeBlock.Phagspa),
				new BlockRange('가', '\ud7af', UnicodeBlock.HangulSyllables),
				new BlockRange('\ud800', '\udb7f', UnicodeBlock.HighSurrogates),
				new BlockRange('\udb80', '\udbff', UnicodeBlock.HighPrivateUseSurrogates),
				new BlockRange('\udc00', '\udfff', UnicodeBlock.LowSurrogates),
				new BlockRange('\ue000', '\uf8ff', UnicodeBlock.PrivateUseArea),
				new BlockRange('豈', '\ufaff', UnicodeBlock.CJKCompatibilityIdeographs),
				new BlockRange('ﬀ', 'ﭏ', UnicodeBlock.AlphabeticPresentationForms),
				new BlockRange('ﭐ', '\ufdff', UnicodeBlock.ArabicPresentationFormsA),
				new BlockRange('\ufe00', '\ufe0f', UnicodeBlock.VariationSelectors),
				new BlockRange('︐', '\ufe1f', UnicodeBlock.VerticalForms),
				new BlockRange('\ufe20', '\ufe2f', UnicodeBlock.CombiningHalfMarks),
				new BlockRange('︰', '\ufe4f', UnicodeBlock.CJKCompatibilityForms),
				new BlockRange('﹐', '\ufe6f', UnicodeBlock.SmallFormVariants),
				new BlockRange('ﹰ', '\ufeff', UnicodeBlock.ArabicPresentationFormsB),
				new BlockRange('\uff00', '\uffef', UnicodeBlock.HalfwidthAndFullwidthForms),
				new BlockRange('\ufff0', '\uffff', UnicodeBlock.Specials)
			};
			BlockRangeIndex = new Dictionary<UnicodeBlock, BlockRange>();
			BlockRange[] blockRanges = BlockRanges;
			for (int i = 0; i < blockRanges.Length; i++)
			{
				BlockRange value = blockRanges[i];
				BlockRangeIndex.Add(value.Block, value);
			}
		}

		public static UnicodeBlock GetUnicodeBlock(char c)
		{
			int num = 0;
			int num2 = BlockRanges.Length - 1;
			while (num <= num2)
			{
				int num3 = (num + num2) / 2;
				if (c >= BlockRanges[num3].First && c <= BlockRanges[num3].Last)
				{
					return BlockRanges[num3].Block;
				}
				if (c > BlockRanges[num3].Last)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3 - 1;
				}
			}
			return UnicodeBlock.Unknown;
		}

		public static bool IsInBlock(char c, UnicodeBlock b)
		{
			if (!BlockRangeIndex.TryGetValue(b, out BlockRange value))
			{
				return false;
			}
			if (c >= value.First)
			{
				return c <= value.Last;
			}
			return false;
		}

		public static bool IsAll(string s, Predicate<char> predicate)
		{
			return IsAll(s, 0, predicate);
		}

		public static bool IsAll(string s, int start, Predicate<char> predicate)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			if (start < 0)
			{
				throw new ArgumentOutOfRangeException("start");
			}
			for (int i = start; i < s.Length; i++)
			{
				if (!predicate(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static int GetPrefixLength(string s, Predicate<char> predicate)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (!predicate(s[i]))
				{
					return i;
				}
			}
			return s.Length;
		}

		public static bool IsApostrophe(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(ApostropheCharacters, c) >= 0;
		}

		public static bool IsColon(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(ColonCharacters, c) >= 0;
		}

		public static bool IsComma(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(CommaCharacters, c) >= 0;
		}

		public static bool IsDot(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(DotCharacters, c) >= 0;
		}

		public static bool IsExclamation(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(ExclamationCharacters, c) >= 0;
		}

		public static bool IsHyphen(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(HyphenCharacters, c) >= 0;
		}

		public static bool IsDash(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(DashCharacters, c) >= 0;
		}

		public static bool IsQuestion(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(QuestionMarkCharacters, c) >= 0;
		}

		public static bool IsSemicolon(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(SemicolonCharacters, c) >= 0;
		}

		public static bool IsBrace(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(BraceCharacters, c) >= 0;
		}

		public static bool IsTabulator(char c)
		{
			return c == '\t';
		}

		public static bool IsParagraph(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(ParagraphSeparatorCharacters, c) >= 0;
		}

		public static bool IsOpeningBracket(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(OpeningBracketCharacters, c) >= 0;
		}

		public static bool IsClosingBracket(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(ClosingBracketCharacters, c) >= 0;
		}

		public static bool IsBracket(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			if (!IsOpeningBracket(c))
			{
				return IsClosingBracket(c);
			}
			return true;
		}

		public static bool IsParenthesis(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			if (!IsOpeningParenthesis(c))
			{
				return IsClosingParenthesis(c);
			}
			return true;
		}

		public static bool IsOpeningParenthesis(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(OpeningParenthesisCharacters, c) >= 0;
		}

		public static bool IsClosingParenthesis(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(ClosingParenthesisCharacters, c) >= 0;
		}

		public static bool IsLatinLetter(char c)
		{
			if (c < '\u3000')
			{
				if (char.IsLetter(c))
				{
					return !IsApostrophe(c);
				}
				return false;
			}
			if (c >= 'Ａ' && c <= 'Ｚ')
			{
				return true;
			}
			if (c >= 'ａ')
			{
				return c <= 'ｚ';
			}
			return false;
		}

		public static bool IsQuote(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(QuoteCharacters, c) >= 0;
		}

		public static bool IsPercent(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(PercentCharacters, c) >= 0;
		}

		public static bool IsSingleQuote(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(SingleQuoteCharacters, c) >= 0;
		}

		public static bool IsDoubleQuote(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			if (IsQuote(c))
			{
				return !IsSingleQuote(c);
			}
			return false;
		}

		public static bool IsOpeningQuote(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(OpeningQuoteCharacters, c) >= 0;
		}

		public static bool IsClosingQuote(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			return Array.BinarySearch(ClosingQuoteCharacters, c) >= 0;
		}

		public static bool IsSurrounder(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			if (!IsBracket(c) && !IsParenthesis(c) && !IsQuote(c) && !IsApostrophe(c))
			{
				return Array.BinarySearch(SpecialSurrounderCharacters, c) >= 0;
			}
			return true;
		}

		public static bool IsStop(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			if (!IsColon(c) && !IsDot(c) && !IsExclamation(c) && !IsQuestion(c))
			{
				return IsSemicolon(c);
			}
			return true;
		}

		public static bool IsVowel(char c)
		{
			if (c == '\0')
			{
				return false;
			}
			if (Array.BinarySearch(WesternVowelCharacters, c) < 0)
			{
				return Array.BinarySearch(FarEastVowelCharacters, c) >= 0;
			}
			return true;
		}

		public static bool IsSequenceOf(string s, UnicodeCategory c)
		{
			return !s.Where((char t, int i) => char.GetUnicodeCategory(s, i) != c).Any();
		}

		public static string ToBase(string s)
		{
			return ToBase(s, skipSurrogates: false);
		}

		public static string ToBase(string s, bool skipSurrogates)
		{
			string text = s.Normalize(NormalizationForm.FormD);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				UnicodeCategory unicodeCategory = char.GetUnicodeCategory(text, i);
				if (skipSurrogates && unicodeCategory == UnicodeCategory.Surrogate)
				{
					return s;
				}
				if (unicodeCategory != UnicodeCategory.Control && unicodeCategory != UnicodeCategory.Format && unicodeCategory != UnicodeCategory.ModifierLetter && unicodeCategory != UnicodeCategory.NonSpacingMark && unicodeCategory != UnicodeCategory.OtherNotAssigned && unicodeCategory != UnicodeCategory.ParagraphSeparator && unicodeCategory != UnicodeCategory.PrivateUse && unicodeCategory != UnicodeCategory.SpaceSeparator && unicodeCategory != UnicodeCategory.SpacingCombiningMark)
				{
					stringBuilder.Append(text[i]);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToCase(string s, Case c)
		{
			if (c == Case.Other || string.IsNullOrEmpty(s))
			{
				return s;
			}
			if (c == Case.Upper || (s.Length == 1 && c == Case.InitialUpper))
			{
				return s.ToUpper(CultureInfo.InvariantCulture);
			}
			if (c == Case.Lower)
			{
				return s.ToLower(CultureInfo.InvariantCulture);
			}
			return char.ToUpper(s[0], CultureInfo.InvariantCulture).ToString() + s.Substring(1).ToLower(CultureInfo.InvariantCulture);
		}

		public static char ToCase(char ch, Case c)
		{
			switch (c)
			{
			case Case.Upper:
			case Case.InitialUpper:
				return char.ToUpper(ch, CultureInfo.InvariantCulture);
			case Case.Lower:
				return char.ToLower(ch, CultureInfo.InvariantCulture);
			default:
				return ch;
			}
		}

		public static bool IsWhitespace(char c)
		{
			return Array.BinarySearch(WhitespaceCharacters, c) >= 0;
		}

		public static bool IsUpperOrLower(char c)
		{
			if (!char.IsUpper(c))
			{
				return char.IsLower(c);
			}
			return true;
		}

		public static bool IsBlank(char c)
		{
			return Array.BinarySearch(Blanks, c) >= 0;
		}

		public static Case GetCase(char c)
		{
			switch (char.GetUnicodeCategory(c))
			{
			case UnicodeCategory.LowercaseLetter:
				return Case.Lower;
			case UnicodeCategory.UppercaseLetter:
				return Case.Upper;
			default:
				return Case.Other;
			}
		}

		public static Case GetCase(string s)
		{
			return GetCase(s, 0);
		}

		public static Case GetCase(string s, int length)
		{
			if (string.IsNullOrEmpty(s))
			{
				return Case.Other;
			}
			int num = s.Length;
			if (length > 0 && length < num)
			{
				num = length;
			}
			Case @case = GetCase(s[0]);
			for (int i = 1; i < num; i++)
			{
				if (@case == Case.Other)
				{
					break;
				}
				Case case2 = GetCase(s[i]);
				if (case2 == @case)
				{
					continue;
				}
				if (case2 == Case.Lower)
				{
					if (@case == Case.Upper && i == 1)
					{
						@case = Case.InitialUpper;
					}
					else if (@case != Case.InitialUpper)
					{
						@case = Case.Other;
					}
				}
				else
				{
					@case = Case.Other;
				}
			}
			return @case;
		}

		public static char ToBase(char c)
		{
			if (c >= '\ud800' && c <= '\uf8ff')
			{
				return c;
			}
			lock (BaseChars)
			{
				if (BaseChars.TryGetValue(c, out char value))
				{
					return value;
				}
				value = new string(c, 1).Normalize(NormalizationForm.FormD)[0];
				BaseChars.Add(c, value);
				return value;
			}
		}

		public static bool IsCJKChar(char c)
		{
			if ((c < '\u3040' || c > 'ゟ') && (c < '゠' || c > 'ヺ') && (c < '一' || c > '\u9fff') && (c < '･' || c > '\uff9f') && (c < 'Ａ' || c > 'Ｚ'))
			{
				if (c >= 'ａ')
				{
					return c <= 'ｚ';
				}
				return false;
			}
			return true;
		}

		public static bool IsKoreanChar(char c)
		{
			if (c < 'ᄀ' || c > 'ᇿ')
			{
				if (c >= '가')
				{
					return c <= '힣';
				}
				return false;
			}
			return true;
		}

		public static bool IsJaLongVowelMarker(char c)
		{
			if (c != '\u30fc')
			{
				return c == '\uff70';
			}
			return true;
		}

		public static bool IsCJKPunctuation(char c)
		{
			if ((c <= '\u3000' || c > '〿') && (c < '・' || c > '\u30fe') && (c < '㈀' || c > '\u32ff') && (c < '！' || c > '／') && (c < '：' || c > '＠') && (c < '［' || c > '］'))
			{
				if (c >= '｛')
				{
					return c <= '､';
				}
				return false;
			}
			return true;
		}

		public static UnicodeCategory? GetUnicodeCategoryFromName(string className)
		{
			if (className == null)
			{
				throw new ArgumentNullException();
			}
			switch (className.ToLower(CultureInfo.InvariantCulture))
			{
			case "pe":
				return UnicodeCategory.ClosePunctuation;
			case "pc":
				return UnicodeCategory.ConnectorPunctuation;
			case "cc":
				return UnicodeCategory.Control;
			case "sc":
				return UnicodeCategory.CurrencySymbol;
			case "pd":
				return UnicodeCategory.DashPunctuation;
			case "nd":
				return UnicodeCategory.DecimalDigitNumber;
			case "me":
				return UnicodeCategory.EnclosingMark;
			case "pf":
				return UnicodeCategory.FinalQuotePunctuation;
			case "cf":
				return UnicodeCategory.Format;
			case "pi":
				return UnicodeCategory.InitialQuotePunctuation;
			case "nl":
				return UnicodeCategory.LetterNumber;
			case "zl":
				return UnicodeCategory.LineSeparator;
			case "ll":
				return UnicodeCategory.LowercaseLetter;
			case "sm":
				return UnicodeCategory.MathSymbol;
			case "lm":
				return UnicodeCategory.ModifierLetter;
			case "sk":
				return UnicodeCategory.ModifierSymbol;
			case "mn":
				return UnicodeCategory.NonSpacingMark;
			case "ps":
				return UnicodeCategory.OpenPunctuation;
			case "lo":
				return UnicodeCategory.OtherLetter;
			case "cn":
				return UnicodeCategory.OtherNotAssigned;
			case "no":
				return UnicodeCategory.OtherNumber;
			case "po":
				return UnicodeCategory.OtherPunctuation;
			case "so":
				return UnicodeCategory.OtherSymbol;
			case "zp":
				return UnicodeCategory.ParagraphSeparator;
			case "co":
				return UnicodeCategory.PrivateUse;
			case "zs":
				return UnicodeCategory.SpaceSeparator;
			case "mc":
				return UnicodeCategory.SpacingCombiningMark;
			case "cs":
				return UnicodeCategory.Surrogate;
			case "lt":
				return UnicodeCategory.TitlecaseLetter;
			case "lu":
				return UnicodeCategory.UppercaseLetter;
			default:
				return null;
			}
		}

		public static string GetUnicodeCategoryName(UnicodeCategory cat)
		{
			switch (cat)
			{
			case UnicodeCategory.ClosePunctuation:
				return "pe";
			case UnicodeCategory.ConnectorPunctuation:
				return "pc";
			case UnicodeCategory.Control:
				return "cc";
			case UnicodeCategory.CurrencySymbol:
				return "sc";
			case UnicodeCategory.DashPunctuation:
				return "pd";
			case UnicodeCategory.DecimalDigitNumber:
				return "nd";
			case UnicodeCategory.EnclosingMark:
				return "me";
			case UnicodeCategory.FinalQuotePunctuation:
				return "pf";
			case UnicodeCategory.Format:
				return "cf";
			case UnicodeCategory.InitialQuotePunctuation:
				return "pi";
			case UnicodeCategory.LetterNumber:
				return "nl";
			case UnicodeCategory.LineSeparator:
				return "zl";
			case UnicodeCategory.LowercaseLetter:
				return "ll";
			case UnicodeCategory.MathSymbol:
				return "sm";
			case UnicodeCategory.ModifierLetter:
				return "lm";
			case UnicodeCategory.ModifierSymbol:
				return "sk";
			case UnicodeCategory.NonSpacingMark:
				return "mn";
			case UnicodeCategory.OpenPunctuation:
				return "ps";
			case UnicodeCategory.OtherLetter:
				return "lo";
			case UnicodeCategory.OtherNotAssigned:
				return "cn";
			case UnicodeCategory.OtherNumber:
				return "no";
			case UnicodeCategory.OtherPunctuation:
				return "po";
			case UnicodeCategory.OtherSymbol:
				return "so";
			case UnicodeCategory.ParagraphSeparator:
				return "zp";
			case UnicodeCategory.PrivateUse:
				return "co";
			case UnicodeCategory.SpaceSeparator:
				return "zs";
			case UnicodeCategory.SpacingCombiningMark:
				return "mc";
			case UnicodeCategory.Surrogate:
				return "cs";
			case UnicodeCategory.TitlecaseLetter:
				return "lt";
			case UnicodeCategory.UppercaseLetter:
				return "lu";
			default:
				return null;
			}
		}

		private static bool CheckOrder(IEnumerable<char> c)
		{
			char c2 = '\0';
			foreach (char item in c)
			{
				if (item <= c2)
				{
					return false;
				}
				c2 = item;
			}
			return true;
		}

		private static bool IsAscending(IList<BlockRange> ranges)
		{
			for (int i = 0; i < ranges.Count; i++)
			{
				if (i > 0 && ranges[i].First <= ranges[i - 1].Last)
				{
					return false;
				}
				if (ranges[i].First > ranges[i].Last)
				{
					return false;
				}
			}
			return true;
		}
	}
}
