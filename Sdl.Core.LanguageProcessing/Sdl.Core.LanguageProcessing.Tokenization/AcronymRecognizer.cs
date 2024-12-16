using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class AcronymRecognizer : Recognizer
	{
		public AcronymRecognizer(RecognizerSettings settings, int priority, CultureInfo culture)
			: base(settings, TokenType.Acronym, priority, "ACR", "AcronymRecognizer", autoSubstitutable: false, culture)
		{
			SetCultureSpecificBehaviour();
		}

		private void SetCultureSpecificBehaviour()
		{
			string twoLetterISOLanguageName = _Culture.TwoLetterISOLanguageName;
			if (twoLetterISOLanguageName != null && twoLetterISOLanguageName == "ko")
			{
				base.OverrideFallbackRecognizer = true;
			}
		}

		public override string GetSignature(CultureInfo culture)
		{
			return base.GetSignature(culture) + "Acronym0";
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			if (!char.IsUpper(s[from]))
			{
				return null;
			}
			int num = s.Length - 1;
			int i = from + 1;
			int num2 = 1;
			for (; i <= num; i++)
			{
				char c = s[i];
				if (char.IsUpper(c))
				{
					num2 = i - from + 1;
				}
				else if (c != '&')
				{
					break;
				}
			}
			if (num2 < 2)
			{
				return null;
			}
			string text = s.Substring(from, num2);
			SimpleToken simpleToken = new SimpleToken(text, TokenType.Acronym);
			consumedLength = num2;
			simpleToken.Culture = _Culture;
			return simpleToken;
		}
	}
}
