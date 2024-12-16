using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class DefaultJAZHFallbackRecognizer : DefaultFallbackRecognizer
	{
		public DefaultJAZHFallbackRecognizer(RecognizerSettings settings, TokenType t, int priority, CultureInfo culture, IResourceDataAccessor dataAccessor)
			: base(settings, t, priority, culture, dataAccessor)
		{
			_IsFallbackRecognizer = true;
		}

		public override string GetSignature(CultureInfo culture)
		{
			return base.GetSignature(culture) + "DefaultJAZH0";
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			if (string.IsNullOrEmpty(s) || from >= s.Length)
			{
				return null;
			}
			consumedLength = 0;
			if (CharacterProperties.IsCJKPunctuation(s[from]))
			{
				if (from < s.Length && CharacterProperties.IsCJKPunctuation(s[from]))
				{
					consumedLength++;
				}
				return new SimpleToken(s.Substring(from, consumedLength), TokenType.GeneralPunctuation)
				{
					Culture = _Culture
				};
			}
			if (!CharacterProperties.IsCJKChar(s[from]))
			{
				return base.Recognize(s, from, allowTokenBundles, ref consumedLength);
			}
			if (from < s.Length && CharacterProperties.IsCJKChar(s[from]))
			{
				consumedLength++;
			}
			SimpleToken simpleToken = new SimpleToken(s.Substring(from, consumedLength), TokenType.CharSequence);
			simpleToken.Culture = _Culture;
			return simpleToken;
		}
	}
}
