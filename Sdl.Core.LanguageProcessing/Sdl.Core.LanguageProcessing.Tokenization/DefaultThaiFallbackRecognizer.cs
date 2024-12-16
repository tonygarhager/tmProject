using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class DefaultThaiFallbackRecognizer : DefaultFallbackRecognizer
	{
		public DefaultThaiFallbackRecognizer(RecognizerSettings settings, TokenType t, int priority, CultureInfo culture, IResourceDataAccessor dataAccessor)
			: base(settings, t, priority, culture, dataAccessor)
		{
			_IsFallbackRecognizer = true;
		}

		public override string GetSignature(CultureInfo culture)
		{
			return base.GetSignature(culture) + "DefaultThai0";
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			if (string.IsNullOrEmpty(s) || from >= s.Length)
			{
				return null;
			}
			consumedLength = 0;
			int startIndex = from;
			if (char.IsPunctuation(s[from]))
			{
				while (from < s.Length && char.IsPunctuation(s[from]))
				{
					consumedLength++;
					from++;
				}
				return new SimpleToken(s.Substring(startIndex, consumedLength), TokenType.GeneralPunctuation)
				{
					Culture = _Culture
				};
			}
			if (!CharacterProperties.IsInBlock(s[from], UnicodeBlock.Thai))
			{
				return base.Recognize(s, from, allowTokenBundles, ref consumedLength);
			}
			while (from < s.Length && CharacterProperties.IsInBlock(s[from], UnicodeBlock.Thai))
			{
				consumedLength++;
				from++;
			}
			SimpleToken simpleToken = new SimpleToken(s.Substring(startIndex, consumedLength), TokenType.CharSequence);
			simpleToken.Culture = _Culture;
			return simpleToken;
		}
	}
}
