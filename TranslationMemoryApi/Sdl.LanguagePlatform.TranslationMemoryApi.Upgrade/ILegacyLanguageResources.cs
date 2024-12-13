using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a set of migrated legacy language resources originating from a legacy translation memory.
	/// </summary>
	public interface ILegacyLanguageResources
	{
		/// <summary>
		/// Gets the display name of this language resource set. Can be null.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets the list of variables. Can be null.
		/// </summary>
		Wordlist Variables
		{
			get;
		}

		/// <summary>
		/// Gets the list of ordinal followers. Can be null.
		/// </summary>
		Wordlist OrdinalFollowers
		{
			get;
		}

		/// <summary>
		/// Gets the list of abbreviations. Can be null.
		/// </summary>
		Wordlist Abbreviations
		{
			get;
		}

		/// <summary>
		/// Gets the (automatically migrated) segmentation rules. Can be null.
		/// </summary>
		SegmentationRules SegmentationRules
		{
			get;
		}

		/// <summary>
		/// Gets the number separators combinations list. Can be null.
		/// </summary>
		List<SeparatorCombination> NumbersSeparators
		{
			get;
		}

		/// <summary>
		/// Gets the list of currency formats. Can be null.
		/// </summary>
		List<CurrencyFormat> CurrencyFormats
		{
			get;
		}

		/// <summary>
		/// Gets the list of measurement units. Can be null.
		/// </summary>
		Dictionary<string, CustomUnitDefinition> MeasurementUnits
		{
			get;
		}

		/// <summary>
		/// Gets the list of date time formats. Can be null.
		/// </summary>
		ILookup<LanguageResourceType, string> DateTimeFormats
		{
			get;
		}
	}
}
