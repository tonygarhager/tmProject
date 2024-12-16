using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class FSTGenerator
	{
		public static FST GenerateNumberFST(CultureInfo ci)
		{
			return NumberFSTRecognizer.CreateFST(ci, NumberFSTRecognizer.AppendWordTerminator(ci));
		}

		public static FST GenerateNumberFST(CultureInfo ci, NumberFormatData nfd, bool treatFirstSeparatorsAsPrimarySeparators)
		{
			return NumberFSTRecognizer.CreateFST(ci, NumberFSTRecognizer.AppendWordTerminator(ci), nfd, treatFirstSeparatorsAsPrimarySeparators);
		}

		public static FST GenerateMeasurementFST(CultureInfo ci)
		{
			return MeasureFSTRecognizer.CreateFST(ci, CultureInfoExtensions.UseBlankAsWordSeparator(ci));
		}

		public static FST GenerateMeasurementFST(CultureInfo ci, NumberFormatData nfd, HashSet<string> customUnits, bool customUnitsOnly, bool treatFirstSeparatorsAsPrimarySeparators)
		{
			return MeasureFSTRecognizer.CreateFST(ci, MeasureFSTRecognizer.AppendWordTerminator(ci), nfd, customUnits, customUnitsOnly, treatFirstSeparatorsAsPrimarySeparators);
		}

		public static FST GenerateCurrencyFST(CultureInfo ci, NumberFormatData nfd, List<CurrencyFormat> currencyFormats, bool treatFirstSeparatorsAsPrimarySeparators)
		{
			CurrencyFSTEx fstEx = new CurrencyFSTEx
			{
				CurrencyFormats = currencyFormats
			};
			return CurrencyFSTRecognizer.CreateFST(ci, CurrencyFSTRecognizer.AppendWordTerminator(ci), nfd, fstEx, treatFirstSeparatorsAsPrimarySeparators);
		}
	}
}
