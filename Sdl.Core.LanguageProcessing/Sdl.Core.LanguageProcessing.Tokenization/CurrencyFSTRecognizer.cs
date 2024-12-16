using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class CurrencyFSTRecognizer : Recognizer
	{
		private readonly FSTRecognizer _fstRecognizer;

		private readonly NumberFSTEx _numberFstEx;

		private readonly CurrencyFSTEx _currencyFstEx;

		public List<CurrencyFormat> CurrencyFormats => _currencyFstEx?.CurrencyFormats;

		public override string GetSignature(CultureInfo culture)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(NumberFSTRecognizer.GetSeparatorsSignatureString(_numberFstEx));
			if (_currencyFstEx?.CurrencyFormats == null)
			{
				return $"{base.GetSignature(culture)}CurrencyFST0{stringBuilder}";
			}
			stringBuilder.Append(" ");
			List<string> list = new List<string>();
			foreach (CurrencyFormat currencyFormat in _currencyFstEx.CurrencyFormats)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(currencyFormat.Symbol);
				if (currencyFormat.Separators != null)
				{
					foreach (char separator in currencyFormat.Separators)
					{
						int num = separator;
						stringBuilder2.Append(num.ToString("X4") + ";");
					}
				}
				if (currencyFormat.CurrencySymbolPositions != null)
				{
					foreach (CurrencySymbolPosition currencySymbolPosition in currencyFormat.CurrencySymbolPositions)
					{
						stringBuilder2.Append(currencySymbolPosition.ToString());
					}
				}
				list.Add(stringBuilder2.ToString());
			}
			list.Sort((string a, string b) => string.CompareOrdinal(a, b));
			foreach (string item in list)
			{
				stringBuilder.Append(item + "|");
			}
			return base.GetSignature(culture) + "CurrencyFST0" + stringBuilder?.ToString();
		}

		public static bool AppendWordTerminator(CultureInfo culture)
		{
			return NumberFSTRecognizer.AppendWordTerminator(culture);
		}

		internal static Recognizer Create(RecognizerSettings settings, CultureInfo culture, int priority, IResourceDataAccessor accessor, CurrencyFSTEx fstEx)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (culture.IsNeutralCulture)
			{
				throw new ArgumentException("Cannot compute measurement patterns for neutral cultures");
			}
			if (culture.NumberFormat == null)
			{
				throw new ArgumentException("No number format info available for the specified culture");
			}
			if (accessor == null)
			{
				accessor = new ResourceFileResourceAccessor();
			}
			NumberFSTEx numberFstEx = null;
			CurrencyFSTEx fstEx2 = null;
			FST fst;
			if (accessor.GetResourceStatus(culture, LanguageResourceType.CurrencyFST, fallback: true) != ResourceStatus.NotAvailable)
			{
				byte[] resourceData = accessor.GetResourceData(culture, LanguageResourceType.CurrencyFST, fallback: true);
				if (resourceData == null)
				{
					throw new LanguageProcessingException(ErrorMessages.EMSG_ResourceNotAvailable);
				}
				fst = FST.Create(resourceData);
				if (accessor.GetResourceStatus(culture, LanguageResourceType.NumberFSTEx, fallback: true) != ResourceStatus.NotAvailable)
				{
					resourceData = accessor.GetResourceData(culture, LanguageResourceType.NumberFSTEx, fallback: true);
					if (resourceData == null)
					{
						throw new LanguageProcessingException(ErrorMessages.EMSG_ResourceNotAvailable);
					}
					numberFstEx = NumberFSTEx.FromBinary(resourceData);
				}
				if (accessor.GetResourceStatus(culture, LanguageResourceType.CurrencyFSTEx, fallback: true) != ResourceStatus.NotAvailable)
				{
					resourceData = accessor.GetResourceData(culture, LanguageResourceType.CurrencyFSTEx, fallback: true);
					if (resourceData == null)
					{
						throw new LanguageProcessingException(ErrorMessages.EMSG_ResourceNotAvailable);
					}
					fstEx2 = CurrencyFSTEx.FromBinary(resourceData);
				}
			}
			else
			{
				if (fstEx == null)
				{
					return null;
				}
				fst = CreateFST(culture, CultureInfoExtensions.UseBlankAsWordSeparator(culture), fstEx);
			}
			FSTRecognizer fstRecog = new FSTRecognizer(fst, culture);
			CurrencyFSTRecognizer currencyFSTRecognizer = new CurrencyFSTRecognizer(settings, priority, fstRecog, fstEx2, numberFstEx, culture);
			SetAdditionalOptions(currencyFSTRecognizer);
			return currencyFSTRecognizer;
		}

		internal CurrencyFSTRecognizer(RecognizerSettings settings, int priority, FSTRecognizer fstRecog, CurrencyFSTEx fstEx, NumberFSTEx numberFstEx, CultureInfo culture)
			: base(settings, TokenType.Measurement, priority, "Currency", "CurrencyFSTRecognizer", autoSubstitutable: false, culture)
		{
			_fstRecognizer = fstRecog;
			_currencyFstEx = fstEx;
			_numberFstEx = numberFstEx;
			_cultureSpecificTextConstraints = GetCultureSpecificTextConstraints(culture);
		}

		private Func<string, int, Token, bool> GetCultureSpecificTextConstraints(CultureInfo culture)
		{
			if (AppendWordTerminator(culture))
			{
				return null;
			}
			string twoLetterISOLanguageName = culture.TwoLetterISOLanguageName;
			if (twoLetterISOLanguageName != null && twoLetterISOLanguageName == "ko")
			{
				return (string s, int p, Token t) => MeasureFSTRecognizer.KoreanTextConstraints(this, s, p, t);
			}
			return null;
		}

		private static void SetAdditionalOptions(Recognizer result)
		{
			result.OnlyIfFollowedByNonwordCharacter = true;
			result.OverrideFallbackRecognizer = true;
		}

		private static Dictionary<string, List<CurrencyFormat>> GroupCurrencyFormatsBySeparators(IEnumerable<CurrencyFormat> cfs)
		{
			Dictionary<string, List<CurrencyFormat>> dictionary = new Dictionary<string, List<CurrencyFormat>>();
			List<char> list = new List<char>();
			list.AddRange(CharacterProperties.Blanks);
			list.Add('Z');
			foreach (CurrencyFormat cf in cfs)
			{
				IEnumerable<char> values = list;
				if (cf.Separators != null && cf.Separators.Count > 0)
				{
					HashSet<char> separators = cf.Separators;
					if (separators.Contains('\0'))
					{
						separators.Remove('\0');
						separators.Add('Z');
					}
					values = separators;
				}
				string key = string.Join(string.Empty, values);
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, new List<CurrencyFormat>());
				}
				dictionary[key].Add(cf);
			}
			return dictionary;
		}

		private static StringBuilder PatternFromGroupedCurrencyFormats(Dictionary<string, List<CurrencyFormat>> groupedCurrencies, bool symbolsBefore)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			bool flag = true;
			foreach (KeyValuePair<string, List<CurrencyFormat>> groupedCurrency in groupedCurrencies)
			{
				if (!flag)
				{
					stringBuilder.Append("|");
				}
				flag = false;
				stringBuilder.Append("(");
				List<char> list = groupedCurrency.Key.ToList();
				bool flag2 = list.Contains('Z');
				if (flag2)
				{
					list.Remove('Z');
				}
				StringBuilder stringBuilder2 = new StringBuilder();
				if (list.Count > 0)
				{
					stringBuilder2.Append("(");
					bool first = true;
					NumberPatternComputer.AppendDisjunction(stringBuilder2, list, 'U', ref first);
					stringBuilder2.Append(")");
					if (flag2)
					{
						stringBuilder2.Append("?");
					}
				}
				StringBuilder stringBuilder3 = new StringBuilder();
				stringBuilder3.Append("(");
				bool flag3 = true;
				foreach (CurrencyFormat item in groupedCurrency.Value)
				{
					if (flag3)
					{
						flag3 = false;
					}
					else
					{
						stringBuilder3.Append("|");
					}
					stringBuilder3.Append("(");
					string symbol = item.Symbol;
					foreach (char c in symbol)
					{
						stringBuilder3.AppendFormat("<{0}:C>", FST.EscapeSpecial(c));
					}
					stringBuilder3.Append(")");
				}
				stringBuilder3.Append(")");
				if (symbolsBefore)
				{
					stringBuilder.Append(stringBuilder3);
					stringBuilder.Append(stringBuilder2);
				}
				else
				{
					stringBuilder.Append(stringBuilder2);
					stringBuilder.Append(stringBuilder3);
				}
				stringBuilder.Append(")");
			}
			stringBuilder.Append(")");
			return stringBuilder;
		}

		internal static FST CreateFST(CultureInfo culture, bool appendWordTerminator, CurrencyFSTEx fstEx)
		{
			NumberFormatData numberFormatData = NumberPatternComputer.GetNumberFormatData(culture, addSeparatorVariants: true, augmentWhitespaceGroupSeparators: true);
			return CreateFST(culture, appendWordTerminator, numberFormatData, fstEx, treatFirstSeparatorsAsPrimarySeparators: true);
		}

		internal static FST CreateFST(CultureInfo culture, bool appendWordTerminator, NumberFormatData nfd, CurrencyFSTEx fstEx, bool treatFirstSeparatorsAsPrimarySeparators)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<CurrencyFormat> list = new List<CurrencyFormat>();
			List<CurrencyFormat> list2 = new List<CurrencyFormat>();
			int currencyPositivePattern = culture.NumberFormat.CurrencyPositivePattern;
			bool flag = currencyPositivePattern % 2 == 0;
			foreach (CurrencyFormat currencyFormat in fstEx.CurrencyFormats)
			{
				List<CurrencySymbolPosition> list3 = new List<CurrencySymbolPosition>();
				if (currencyFormat.CurrencySymbolPositions != null && currencyFormat.CurrencySymbolPositions.Count > 0)
				{
					list3 = currencyFormat.CurrencySymbolPositions;
				}
				else
				{
					list3.Add(flag ? CurrencySymbolPosition.beforeAmount : CurrencySymbolPosition.afterAmount);
				}
				if (list3.Contains(CurrencySymbolPosition.beforeAmount))
				{
					list.Add(currencyFormat);
				}
				if (list3.Contains(CurrencySymbolPosition.afterAmount))
				{
					list2.Add(currencyFormat);
				}
			}
			Dictionary<string, List<CurrencyFormat>> groupedCurrencies = GroupCurrencyFormatsBySeparators(list);
			Dictionary<string, List<CurrencyFormat>> groupedCurrencies2 = GroupCurrencyFormatsBySeparators(list2);
			string value = NumberPatternComputer.ComputeFstPattern(nfd, treatFirstSeparatorsAsPrimarySeparators, appendWordTerminator: false);
			bool flag2 = list.Count > 0 && list2.Count > 0;
			if (flag2)
			{
				stringBuilder.Append("(");
			}
			if (list.Count > 0)
			{
				stringBuilder.Append(PatternFromGroupedCurrencyFormats(groupedCurrencies, symbolsBefore: true));
				stringBuilder.Append(value);
			}
			if (flag2)
			{
				stringBuilder.Append(")|(");
			}
			if (list2.Count > 0)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(PatternFromGroupedCurrencyFormats(groupedCurrencies2, symbolsBefore: false));
			}
			if (flag2)
			{
				stringBuilder.Append(")");
			}
			if (appendWordTerminator)
			{
				stringBuilder.Append("#>");
			}
			FST fST = FST.Create(stringBuilder.ToString());
			fST.MakeDeterministic();
			return fST;
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			consumedLength = 0;
			List<FSTMatch> list = _fstRecognizer.ComputeMatches(s, from, ignoreCase: false, 0, keepLongestMatchesOnly: true);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			List<PrioritizedToken> list2 = null;
			foreach (FSTMatch item in list)
			{
				if (_cultureSpecificTextConstraints != null || VerifyContextConstraints(s, item.Index + item.Length))
				{
					if (item.Output == null || item.Output.Length != item.Length || item.Length == 0)
					{
						throw new Exception("Internal error: invalid measurement FST");
					}
					if (consumedLength == 0)
					{
						consumedLength = item.Length;
					}
					MeasureToken measureToken = Parse(s.Substring(item.Index, item.Length), item.Output);
					if (measureToken != null && (_cultureSpecificTextConstraints == null || VerifyContextConstraints(s, item.Index + item.Length, measureToken)))
					{
						if (list2 == null)
						{
							list2 = new List<PrioritizedToken>();
						}
						list2.Add(new PrioritizedToken(measureToken, 0));
					}
				}
			}
			if (list2 == null || list2.Count == 0)
			{
				return null;
			}
			MeasureFSTRecognizer.EvaluateAndSortCandidates(list2, _Priority);
			if (allowTokenBundles && list2.Count > 1)
			{
				return new TokenBundle(list2);
			}
			return list2[0].Token;
		}

		private MeasureToken Parse(string surface, string output)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			char unitSeparator = '\0';
			int num = output.IndexOf('U');
			if (num == 0)
			{
				throw new Exception("Invalid measurement format");
			}
			if (num > 0)
			{
				unitSeparator = surface[num];
			}
			int num2 = output.IndexOf('C');
			int num3 = output.LastIndexOf('C');
			if (num2 < 0)
			{
				throw new Exception("Invalid measurement format");
			}
			string surface2;
			string output2;
			if (num2 > 0)
			{
				int length = num2;
				if (num > 0)
				{
					length = num;
				}
				surface2 = surface.Substring(0, length);
				output2 = output.Substring(0, length);
			}
			else
			{
				int startIndex = num3 + 1;
				if (num > 0)
				{
					startIndex = num + 1;
				}
				surface2 = surface.Substring(startIndex);
				output2 = output.Substring(startIndex);
			}
			string unitPart = surface.Substring(num2, num3 - num2 + 1);
			NumberToken numericPart = NumberFSTRecognizer.ParseNumber(surface2, output2, _numberFstEx?.SeparatorCombinations, _Culture);
			MeasureToken measureToken = new MeasureToken(surface, numericPart, Unit.Currency, unitPart, unitSeparator);
			CurrencyFormat currencyFormat = _currencyFstEx?.CurrencyFormats?.FirstOrDefault((CurrencyFormat x) => string.CompareOrdinal(x.Symbol, unitPart) == 0);
			if (currencyFormat != null)
			{
				measureToken.CustomCategory = currencyFormat.Category;
			}
			measureToken.Culture = _Culture;
			return measureToken;
		}
	}
}
