using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class DefaultKoreanFallbackRecognizer : DefaultFallbackRecognizer
	{
		public DefaultKoreanFallbackRecognizer(RecognizerSettings settings, TokenType t, int priority, CultureInfo culture, IResourceDataAccessor dataAccessor)
			: base(settings, t, priority, culture, dataAccessor)
		{
			_IsFallbackRecognizer = true;
		}

		public override string GetSignature(CultureInfo culture)
		{
			return base.GetSignature(culture) + "DefaultKorean0";
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			consumedLength = 0;
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			int length = s.Length;
			int num = from;
			char c = s[num];
			if (IsHardTokenTerminator(s, num, _Settings))
			{
				consumedLength = 1;
				Token token = new SimpleToken(s.Substring(from, consumedLength), TokenType.GeneralPunctuation);
				token.Culture = _Culture;
				return token;
			}
			bool flag = CharacterProperties.IsCJKChar(c);
			while (num < length && !char.IsWhiteSpace(c) && !char.IsSeparator(c) && !char.IsDigit(c) && !IsHardTokenTerminator(s, num, _Settings))
			{
				bool flag2 = CharacterProperties.IsCJKChar(c);
				if (flag2 != flag)
				{
					break;
				}
				num++;
				if (num < length)
				{
					c = s[num];
					flag = flag2;
				}
			}
			int num2 = num;
			for (num = from; num < num2 && (IsSeparablePunct(s, num) || s[num] == '.'); num++)
			{
			}
			if (num > from)
			{
				consumedLength = num - from;
				Token token2 = new SimpleToken(s.Substring(from, consumedLength), TokenType.GeneralPunctuation);
				token2.Culture = _Culture;
				return token2;
			}
			bool flag3 = false;
			bool flag4;
			do
			{
				flag4 = false;
				while (num2 - 1 > num && IsSeparablePunct(s, num2 - 1))
				{
					num2--;
					flag4 = true;
				}
				int i;
				for (i = 0; num2 - 1 - i > num && s[num2 - 1 - i] == '.'; i++)
				{
				}
				if (i > 1)
				{
					num2 -= i;
					flag4 = true;
				}
				else if (i == 1)
				{
					if (Resources == null || !Resources.IsAbbreviation(s.Substring(from, num2 - from)))
					{
						num2--;
						flag4 = true;
					}
					else
					{
						flag3 = true;
					}
				}
			}
			while (flag4);
			consumedLength = num2 - from;
			if (consumedLength == 0)
			{
				return null;
			}
			SimpleToken simpleToken = new SimpleToken(s.Substring(from, consumedLength), (!flag3) ? TokenType.Word : TokenType.Abbreviation);
			simpleToken.IsStopword = (Resources?.IsStopword(simpleToken.Text.ToLowerInvariant()) ?? false);
			simpleToken.Culture = _Culture;
			return simpleToken;
		}

		private static bool IsSeparablePunct(string s, int pos)
		{
			int result;
			switch (char.GetUnicodeCategory(s, pos))
			{
			case UnicodeCategory.OtherPunctuation:
				result = ((s[pos] != '.') ? 1 : 0);
				break;
			default:
				result = 0;
				break;
			case UnicodeCategory.OpenPunctuation:
			case UnicodeCategory.ClosePunctuation:
			case UnicodeCategory.InitialQuotePunctuation:
			case UnicodeCategory.FinalQuotePunctuation:
			case UnicodeCategory.MathSymbol:
				result = 1;
				break;
			}
			return (byte)result != 0;
		}

		private static bool IsHardTokenTerminator(string s, int p, RecognizerSettings settings)
		{
			UnicodeCategory unicodeCategory = char.GetUnicodeCategory(s, p);
			if (unicodeCategory == UnicodeCategory.SpaceSeparator || unicodeCategory == UnicodeCategory.ParagraphSeparator || unicodeCategory == UnicodeCategory.LineSeparator || unicodeCategory == UnicodeCategory.OpenPunctuation || unicodeCategory == UnicodeCategory.ClosePunctuation || unicodeCategory == UnicodeCategory.FinalQuotePunctuation || unicodeCategory == UnicodeCategory.InitialQuotePunctuation || (unicodeCategory == UnicodeCategory.MathSymbol && (settings.BreakOnHyphen || !CharacterProperties.IsHyphen(s[p]))))
			{
				return true;
			}
			char c = s[p];
			if (c != '/' && c != '\\' && !CharacterProperties.IsColon(c) && !CharacterProperties.IsSemicolon(c) && (!CharacterProperties.IsHyphen(c) || !settings.BreakOnHyphen) && (!CharacterProperties.IsDash(c) || !settings.BreakOnDash))
			{
				return CharacterProperties.IsApostrophe(c);
			}
			return true;
		}
	}
}
