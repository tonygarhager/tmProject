using Sdl.Core.LanguageProcessing.Tokenization;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	public class TokenizationContext
	{
		internal Dictionary<DateTimePatternType, List<string>> KeyedDateTimeFormats;

		public CultureInfo Culture
		{
			get;
		}

		public Dictionary<DateTimePatternType, List<string>> DateTimeFormats
		{
			get;
		}

		public List<SeparatorCombination> SeparatorCombinations
		{
			get;
		}

		public Dictionary<string, CustomUnitDefinition> UnitDefinitions
		{
			get;
		}

		public List<CurrencyFormat> CurrencyFormats
		{
			get;
		}

		public TokenizationContext(CultureInfo culture, Dictionary<DateTimePatternType, List<string>> dateTimeFormats, List<SeparatorCombination> separatorCombinations, Dictionary<string, CustomUnitDefinition> unitDefinitions, List<CurrencyFormat> currencyFormats)
		{
			Culture = culture;
			DateTimeFormats = dateTimeFormats;
			SeparatorCombinations = separatorCombinations;
			UnitDefinitions = unitDefinitions;
			CurrencyFormats = currencyFormats;
		}
	}
}
