using Sdl.LanguagePlatform.Core;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing
{
	public class AdvancedTokenization
	{
		public static bool TokenizesToWords(CultureInfo culture)
		{
			if (!CultureInfoExtensions.UseBlankAsWordSeparator(culture))
			{
				return TokenizerHelper.UsesAdvancedTokenization(culture);
			}
			return true;
		}
	}
}
