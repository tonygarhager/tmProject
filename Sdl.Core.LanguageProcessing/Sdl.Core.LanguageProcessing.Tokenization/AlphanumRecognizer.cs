using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class AlphanumRecognizer : Recognizer
	{
		private enum CharType
		{
			Letter,
			Number,
			Dot,
			Dash,
			Underscore
		}

		public AlphanumRecognizer(RecognizerSettings settings, int priority, CultureInfo culture)
			: base(settings, TokenType.AlphaNumeric, priority, "AlphaNumeric", "AlphaNumericRecognizer", autoSubstitutable: false, culture)
		{
			_OverrideFallbackRecognizer = true;
		}

		public override string GetSignature(CultureInfo culture)
		{
			return base.GetSignature(culture) + "Alphanum0";
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			consumedLength = 0;
			int num = s.Length - 1;
			bool flag = CharacterProperties.IsLatinLetter(s[from]);
			if (!char.IsDigit(s[from]) && !flag)
			{
				return null;
			}
			bool flag2 = flag;
			bool flag3 = !flag2;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = flag2 && char.IsLower(s[from]);
			CharType charType = (!flag2) ? CharType.Number : CharType.Letter;
			int i = from + 1;
			int num2 = from;
			for (; i <= num; i++)
			{
				char c = s[i];
				if (CharacterProperties.IsLatinLetter(c))
				{
					bool flag8 = char.IsUpper(c);
					if (!flag8 && flag4)
					{
						return null;
					}
					flag2 = true;
					if (!flag8)
					{
						flag7 = true;
					}
					charType = CharType.Letter;
					num2 = i;
				}
				else if (char.IsDigit(c))
				{
					flag3 = true;
					charType = CharType.Number;
					num2 = i;
				}
				else if (CharacterProperties.IsHyphen(c) || CharacterProperties.IsDash(c))
				{
					if (flag7)
					{
						if (!flag2 || !flag3)
						{
							return null;
						}
						num2 = i - 1;
						break;
					}
					if (flag5 | flag6)
					{
						num2 = i - 1;
						break;
					}
					flag4 = true;
					num2 = i;
					charType = CharType.Dash;
				}
				else if (CharacterProperties.IsDot(c))
				{
					if (flag4 | flag6)
					{
						num2 = i - 1;
						break;
					}
					flag5 = true;
					num2 = i;
					charType = CharType.Dot;
				}
				else
				{
					if (c != '_' && c != '\uff3f')
					{
						break;
					}
					if (flag4 | flag5)
					{
						return null;
					}
					flag6 = true;
					num2 = i;
					charType = CharType.Underscore;
				}
			}
			switch (charType)
			{
			case CharType.Dot:
			case CharType.Dash:
				num2--;
				break;
			case CharType.Underscore:
				return null;
			}
			if (!flag2 || !flag3)
			{
				return null;
			}
			string text = s.Substring(from, num2 - from + 1);
			SimpleToken simpleToken = new SimpleToken(text, TokenType.AlphaNumeric);
			consumedLength = text.Length;
			simpleToken.Culture = _Culture;
			return simpleToken;
		}
	}
}
