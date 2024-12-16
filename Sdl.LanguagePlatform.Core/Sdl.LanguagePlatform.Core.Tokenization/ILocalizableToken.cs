using System.Globalization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public interface ILocalizableToken
	{
		TokenizationContext TokenizationContext
		{
			get;
			set;
		}

		bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings);

		bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings, ILocalizableToken originalMemoryToken, bool adjustCasing);

		bool SetValue(Token blueprint, bool keepNumericSeparators);
	}
}
