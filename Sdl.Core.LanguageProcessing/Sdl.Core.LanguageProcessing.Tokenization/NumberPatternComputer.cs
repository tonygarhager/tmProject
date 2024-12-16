using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class NumberPatternComputer
	{
		public static readonly string DefaultDigits = "0123456789";

		public static readonly string FullWidthDigits = "０１２３４５６７８９";

		internal static readonly bool AllowTrailingSign = true;

		internal static readonly bool SupportNonstandardGrouping = false;

		internal static char[] NumericPositiveSymbols = new char[2]
		{
			'+',
			'＋'
		};

		internal static char[] NumericNegativeSymbols = new char[4]
		{
			'-',
			'–',
			'−',
			'－'
		};

		public static bool DoAddEnusSeparators(CultureInfo culture)
		{
			string twoLetterISOLanguageName = culture.TwoLetterISOLanguageName;
			return twoLetterISOLanguageName.Equals("fr", StringComparison.OrdinalIgnoreCase);
		}

		public static NumberFormatData GetNumberFormatData(CultureInfo culture, bool addSeparatorVariants, bool augmentWhitespaceGroupSeparators)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (culture.IsNeutralCulture)
			{
				throw new ArgumentException("Cannot compute number format information for neutral cultures");
			}
			return GetNumberFormatData(culture, LanguageMetadata.GetOrCreateMetadata(culture.Name), addSeparatorVariants, augmentWhitespaceGroupSeparators);
		}

		private static NumberFormatData GetNumberFormatData(CultureInfo culture, LanguageMetadata lm, bool addSeparatorVariants, bool augmentWhitespaceGroupSeparators)
		{
			string text = string.Join("", lm.NativeDigits);
			if (text.Length != 10)
			{
				return null;
			}
			NumberFormatData numberFormatData = new NumberFormatData();
			numberFormatData.Digits.Add(text);
			if (!string.Equals(text, DefaultDigits))
			{
				numberFormatData.Digits.Add(DefaultDigits);
			}
			if (CultureInfoExtensions.UseFullWidth(culture) && !numberFormatData.Digits.Contains(FullWidthDigits))
			{
				numberFormatData.Digits.Add(FullWidthDigits);
			}
			numberFormatData.NegativeSigns = lm.NegativeSign;
			numberFormatData.PositiveSigns = lm.PositiveSign;
			numberFormatData.NumberGroupSizes = lm.NumberGroupSizes;
			numberFormatData.NumberNegativePattern = lm.NumberNegativePattern;
			numberFormatData.AddSeparatorCombination(lm.NumberGroupSeparator, lm.NumberDecimalSeparator, augmentWhitespaceGroupSeparators);
			numberFormatData.AddSeparatorCombination(lm.CurrencyGroupSeparator, lm.CurrencyDecimalSeparator, augmentWhitespaceGroupSeparators);
			if (!addSeparatorVariants)
			{
				return numberFormatData;
			}
			switch (culture.TwoLetterISOLanguageName.ToLowerInvariant())
			{
			case "fr":
				numberFormatData.AddSeparatorCombination(",", ".", augmentGroupSeparators: false);
				break;
			case "pl":
				numberFormatData.AddSeparatorCombination(".", ",", augmentGroupSeparators: false);
				break;
			}
			if (CultureInfoExtensions.UseFullWidth(culture))
			{
				numberFormatData.AddSeparatorCombination(StringUtilities.HalfWidthToFullWidth2(lm.NumberGroupSeparator), StringUtilities.HalfWidthToFullWidth2(lm.NumberDecimalSeparator), augmentWhitespaceGroupSeparators);
				numberFormatData.AddSeparatorCombination(StringUtilities.HalfWidthToFullWidth2(lm.CurrencyGroupSeparator), StringUtilities.HalfWidthToFullWidth2(lm.CurrencyDecimalSeparator), augmentWhitespaceGroupSeparators);
			}
			for (int num = numberFormatData.SeparatorCombinations.Count - 1; num >= 0; num--)
			{
				SeparatorCombination separatorCombination = numberFormatData.SeparatorCombinations[num];
				if (separatorCombination.IsSwappable())
				{
					numberFormatData.AddSeparatorCombination(separatorCombination.DecimalSeparators, separatorCombination.GroupSeparators, augmentGroupSeparators: false);
				}
			}
			return numberFormatData;
		}

		public static (FST NumberFST, NumberFSTEx NumberFSTEx, FST MeasureFST, MeasureFSTEx MeasureFSTEx, FST CurrencyFST, CurrencyFSTEx CurrencyFSTEx) GetMeasureNumberAndCurrencyLanguageResources(CultureInfo ci, LanguageMetadata lm, List<SeparatorCombination> customSeparatorCombinations, Dictionary<string, CustomUnitDefinition> customUnits, List<CurrencyFormat> currencyFormats, bool customSeparatorsOnly, bool customUnitsOnly)
		{
			NumberFormatData numberFormatData = GetNumberFormatData(ci, lm, addSeparatorVariants: true, augmentWhitespaceGroupSeparators: false);
			NumberFormatData numberFormatData2 = GetNumberFormatData(ci, lm, addSeparatorVariants: true, augmentWhitespaceGroupSeparators: true);
			if (customSeparatorsOnly && (customSeparatorCombinations == null || customSeparatorCombinations.Count == 0))
			{
				throw new ArgumentException("customSeparatorsOnly specified but not separator combinations were provided");
			}
			if (customSeparatorsOnly)
			{
				numberFormatData.SeparatorCombinations.Clear();
				numberFormatData2.SeparatorCombinations.Clear();
			}
			if (customSeparatorCombinations != null)
			{
				foreach (SeparatorCombination customSeparatorCombination in customSeparatorCombinations)
				{
					if (customSeparatorCombination.GroupSeparators == null || customSeparatorCombination.GroupSeparators.Length > 1 || customSeparatorCombination.DecimalSeparators == null || customSeparatorCombination.DecimalSeparators.Length > 1)
					{
						throw new SeparatorException();
					}
					numberFormatData.AddSeparatorCombination(customSeparatorCombination.GroupSeparators, customSeparatorCombination.DecimalSeparators, augmentGroupSeparators: false);
					numberFormatData2.AddSeparatorCombination(customSeparatorCombination.GroupSeparators, customSeparatorCombination.DecimalSeparators, augmentGroupSeparators: true);
				}
			}
			FST item = FSTGenerator.GenerateNumberFST(ci, numberFormatData2, treatFirstSeparatorsAsPrimarySeparators: true);
			NumberFSTEx item2 = new NumberFSTEx
			{
				SeparatorCombinations = (customSeparatorsOnly ? customSeparatorCombinations : numberFormatData.SeparatorCombinations)
			};
			HashSet<string> hashSet = new HashSet<string>();
			if (customUnits == null)
			{
				customUnits = new Dictionary<string, CustomUnitDefinition>();
			}
			foreach (KeyValuePair<string, CustomUnitDefinition> customUnit in customUnits)
			{
				if (!hashSet.Add(customUnit.Key))
				{
					throw new ArgumentException("Duplicate unit string found: " + customUnit.Key);
				}
			}
			FST item3 = FSTGenerator.GenerateMeasurementFST(ci, numberFormatData2, hashSet, customUnitsOnly, treatFirstSeparatorsAsPrimarySeparators: true);
			MeasureFSTEx measureFSTEx = new MeasureFSTEx
			{
				UnitDefinitions = new Dictionary<string, CustomUnitDefinition>()
			};
			foreach (KeyValuePair<string, CustomUnitDefinition> customUnit2 in customUnits)
			{
				measureFSTEx.UnitDefinitions.Add(customUnit2.Key, customUnit2.Value);
			}
			if (!customUnitsOnly)
			{
				foreach (string item6 in hashSet)
				{
					if (!customUnits.ContainsKey(item6))
					{
						measureFSTEx.UnitDefinitions.Add(item6, null);
					}
				}
			}
			if (currencyFormats == null || currencyFormats.Count == 0)
			{
				currencyFormats = CurrencyFSTEx.GetDefaults(ci, lm, null).CurrencyFormats;
			}
			CurrencyFSTEx item4 = new CurrencyFSTEx
			{
				CurrencyFormats = currencyFormats
			};
			FST item5 = FSTGenerator.GenerateCurrencyFST(ci, numberFormatData2, currencyFormats, treatFirstSeparatorsAsPrimarySeparators: true);
			return (item, item2, item3, measureFSTEx, item5, item4);
		}

		internal static void AppendDisjunction(StringBuilder sb, char symbol, char output, ref bool first)
		{
			if (!first)
			{
				sb.Append("|");
			}
			sb.AppendFormat("<{0}:{1}>", FST.EscapeSpecial(symbol), output);
			first = false;
		}

		private static bool IsBalanced(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return true;
			}
			int num = 0;
			bool flag = false;
			foreach (char c in s)
			{
				if (flag)
				{
					flag = false;
					continue;
				}
				switch (c)
				{
				case '\\':
					flag = true;
					break;
				case '(':
					num++;
					break;
				case ')':
					num--;
					if (num < 0)
					{
						return false;
					}
					break;
				}
			}
			return num == 0;
		}

		internal static void AppendDisjunction(StringBuilder sb, string symbols, char output, ref bool first)
		{
			if (!string.IsNullOrEmpty(symbols))
			{
				foreach (char symbol in symbols)
				{
					AppendDisjunction(sb, symbol, output, ref first);
				}
			}
		}

		internal static void AppendDisjunction(StringBuilder sb, IList<char> symbols, char output, ref bool first)
		{
			if (symbols != null && symbols.Count != 0)
			{
				foreach (char symbol in symbols)
				{
					AppendDisjunction(sb, symbol, output, ref first);
				}
			}
		}

		private static string ComputeSign(string positiveSigns, string negativeSigns, bool optional)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			bool first = true;
			AppendDisjunction(stringBuilder, positiveSigns, '+', ref first);
			AppendDisjunction(stringBuilder, negativeSigns, '-', ref first);
			stringBuilder.Append(optional ? ")?" : ")");
			return stringBuilder.ToString();
		}

		private static string ComputeSingleDigit(IEnumerable<string> digitSet)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool first = true;
			stringBuilder.Append("(");
			foreach (string item in digitSet)
			{
				for (int i = 0; i < 10; i++)
				{
					AppendDisjunction(stringBuilder, item[i], DefaultDigits[i], ref first);
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static string ComputeFstPattern(NumberFormatData data, bool treatFirstSeparatorsAsPrimarySeparators, bool appendWordTerminator)
		{
			string text = ComputeSingleDigit(data.Digits);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(NumericPositiveSymbols);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(NumericNegativeSymbols);
			string positiveSigns = stringBuilder.ToString();
			string negativeSigns = stringBuilder2.ToString();
			string text2 = ComputeSign(positiveSigns, negativeSigns, optional: true);
			string text3 = ComputeSign(positiveSigns, negativeSigns, optional: false);
			StringBuilder stringBuilder3 = new StringBuilder();
			string combinedDecimalSeparators = data.GetCombinedDecimalSeparators();
			stringBuilder3.AppendFormat("({0}+((", text);
			bool first = true;
			for (int i = 0; i < combinedDecimalSeparators.Length; i++)
			{
				if (i == 0 && treatFirstSeparatorsAsPrimarySeparators)
				{
					AppendDisjunction(stringBuilder3, combinedDecimalSeparators[i], 'D', ref first);
				}
				else
				{
					AppendDisjunction(stringBuilder3, combinedDecimalSeparators[i], 'd', ref first);
				}
			}
			stringBuilder3.AppendFormat("){0}+)?)", text);
			StringBuilder stringBuilder4 = new StringBuilder();
			stringBuilder4.AppendFormat("{0}({1}{2}?)?(", text, text, text);
			first = true;
			foreach (SeparatorCombination separatorCombination in data.SeparatorCombinations)
			{
				StringBuilder stringBuilder5 = new StringBuilder();
				stringBuilder5.Append("(");
				for (int j = 0; j < separatorCombination.GroupSeparators.Length; j++)
				{
					char c = 'g';
					if (first && treatFirstSeparatorsAsPrimarySeparators)
					{
						c = 'G';
					}
					if (j > 0)
					{
						stringBuilder5.Append("|");
					}
					stringBuilder5.AppendFormat("((<{0}:{1}>{2}{3}{4})+)", FST.EscapeSpecial(separatorCombination.GroupSeparators[j]), c, text, text, text);
				}
				stringBuilder5.Append(")");
				stringBuilder5.Append("((");
				for (int k = 0; k < separatorCombination.DecimalSeparators.Length; k++)
				{
					char c2 = 'd';
					if (first && treatFirstSeparatorsAsPrimarySeparators)
					{
						c2 = 'D';
					}
					if (k > 0)
					{
						stringBuilder5.Append("|");
					}
					stringBuilder5.AppendFormat("<{0}:{1}>", FST.EscapeSpecial(separatorCombination.DecimalSeparators[k]), c2);
				}
				stringBuilder5.AppendFormat("){0}+)?", text);
				if (!first)
				{
					stringBuilder4.AppendFormat("|");
				}
				stringBuilder4.AppendFormat("({0})", stringBuilder5);
				if (first)
				{
					first = false;
				}
			}
			stringBuilder4.Append(")");
			StringBuilder stringBuilder6 = new StringBuilder();
			if (!AllowTrailingSign || data.NumberNegativePattern < 3)
			{
				stringBuilder6.AppendFormat("({0}({1}|{2}))", text2, stringBuilder4, stringBuilder3);
			}
			else
			{
				stringBuilder6.AppendFormat("(({0}({1}|{2}))|(({1}|{2}){3}))", text3, stringBuilder4, stringBuilder3, text2);
			}
			if (appendWordTerminator)
			{
				stringBuilder6.Append("#>");
			}
			return stringBuilder6.ToString();
		}
	}
}
