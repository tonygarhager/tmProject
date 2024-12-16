using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface ILegacyLanguageResources
	{
		string Name
		{
			get;
		}

		Wordlist Variables
		{
			get;
		}

		Wordlist OrdinalFollowers
		{
			get;
		}

		Wordlist Abbreviations
		{
			get;
		}

		SegmentationRules SegmentationRules
		{
			get;
		}

		List<SeparatorCombination> NumbersSeparators
		{
			get;
		}

		List<CurrencyFormat> CurrencyFormats
		{
			get;
		}

		Dictionary<string, CustomUnitDefinition> MeasurementUnits
		{
			get;
		}

		ILookup<LanguageResourceType, string> DateTimeFormats
		{
			get;
		}
	}
}
