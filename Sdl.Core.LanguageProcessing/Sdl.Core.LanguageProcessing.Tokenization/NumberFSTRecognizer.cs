using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal class NumberFSTRecognizer : Recognizer
	{
		private readonly FSTRecognizer _fstRecognizer;

		private readonly NumberFSTEx _numberFstEx;

		public List<SeparatorCombination> SeparatorCombinationsComputed => _numberFstEx?.SeparatorCombinations;

		internal static string GetSeparatorsSignatureString(NumberFSTEx numberFstEx)
		{
			if (numberFstEx?.SeparatorCombinations == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			List<string> list = new List<string>();
			foreach (SeparatorCombination separatorCombination in numberFstEx.SeparatorCombinations)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(num.ToString() + " ");
				stringBuilder2.Append(separatorCombination.GroupSeparators);
				stringBuilder2.Append("\t");
				stringBuilder2.Append(separatorCombination.DecimalSeparators);
				stringBuilder2.Append("\t");
				list.Add(stringBuilder2.ToString());
				num++;
			}
			list.Sort((string a, string b) => string.CompareOrdinal(a, b));
			foreach (string item in list)
			{
				stringBuilder.Append(item);
			}
			return stringBuilder.ToString();
		}

		public override string GetSignature(CultureInfo culture)
		{
			string separatorsSignatureString = GetSeparatorsSignatureString(_numberFstEx);
			if (CultureInfoExtensions.UseFullWidth(culture))
			{
				return base.GetSignature(culture) + "Number1" + separatorsSignatureString;
			}
			return base.GetSignature(culture) + "Number0" + separatorsSignatureString;
		}

		public static Recognizer Create(RecognizerSettings settings, IResourceDataAccessor resourceDataAccessor, CultureInfo culture, int priority)
		{
			NumberFSTRecognizer result = new NumberFSTRecognizer(settings, culture, priority, resourceDataAccessor);
			SetAdditionalOptions(result, culture);
			return result;
		}

		public static Recognizer Create(RecognizerSettings settings, CultureInfo culture, int priority)
		{
			NumberFSTRecognizer result = new NumberFSTRecognizer(settings, culture, priority);
			SetAdditionalOptions(result, culture);
			return result;
		}

		private static void SetAdditionalOptions(NumberFSTRecognizer result, CultureInfo culture)
		{
			result.OnlyIfFollowedByNonwordCharacter = CultureInfoExtensions.UseBlankAsWordSeparator(culture);
			if (result.AdditionalTerminators == null)
			{
				result.AdditionalTerminators = new CharacterSet();
			}
			result.AdditionalTerminators.Add('-');
			result.OverrideFallbackRecognizer = true;
		}

		public NumberFSTRecognizer(RecognizerSettings settings, CultureInfo culture, int priority)
			: base(settings, TokenType.Number, priority, "Number", "NumberFSTRecognizer", autoSubstitutable: false, culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (culture.IsNeutralCulture)
			{
				throw new ArgumentException("Cannot compute number patterns for neutral cultures");
			}
			if (culture.NumberFormat == null)
			{
				throw new ArgumentException("No number format info available for the specified culture");
			}
			_cultureSpecificTextConstraints = GetCultureSpecificTextConstraints(culture);
			FST fst = CreateFST(culture, AppendWordTerminator(culture));
			_fstRecognizer = new FSTRecognizer(fst, culture);
		}

		public static bool AppendWordTerminator(CultureInfo culture)
		{
			if (!CultureInfoExtensions.UseBlankAsWordSeparator(culture))
			{
				return false;
			}
			return string.CompareOrdinal(culture.TwoLetterISOLanguageName, "ko") != 0;
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
				return KoreanTextConstraints;
			}
			return null;
		}

		private bool KoreanTextConstraints(string s, int p, Token t)
		{
			if (DefaultTextConstraints(s, p))
			{
				return true;
			}
			if (p < s.Length)
			{
				return CharacterProperties.IsKoreanChar(s[p]);
			}
			return false;
		}

		public NumberFSTRecognizer(RecognizerSettings settings, CultureInfo culture, int priority, IResourceDataAccessor accessor)
			: base(settings, TokenType.Number, priority, "Number", "NumberFSTRecognizer", autoSubstitutable: false, culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (culture.IsNeutralCulture)
			{
				throw new ArgumentException("Cannot compute number patterns for neutral cultures");
			}
			if (culture.NumberFormat == null)
			{
				throw new ArgumentException("No number format info available for the specified culture");
			}
			if (accessor == null)
			{
				accessor = new ResourceFileResourceAccessor();
			}
			_cultureSpecificTextConstraints = GetCultureSpecificTextConstraints(culture);
			FST fst;
			if (accessor.GetResourceStatus(culture, LanguageResourceType.NumberFST, fallback: true) != ResourceStatus.NotAvailable)
			{
				byte[] resourceData = accessor.GetResourceData(culture, LanguageResourceType.NumberFST, fallback: true);
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
					_numberFstEx = NumberFSTEx.FromBinary(resourceData);
				}
			}
			else
			{
				fst = CreateFST(culture, AppendWordTerminator(culture));
			}
			_fstRecognizer = new FSTRecognizer(fst, culture);
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
				if (VerifyContextConstraints(s, item.Index + item.Length))
				{
					if (item.Output == null || item.Output.Length != item.Length || item.Length == 0)
					{
						throw new Exception("Internal error: invalid number FST");
					}
					if (consumedLength == 0)
					{
						consumedLength = item.Length;
					}
					NumberToken numberToken = ParseNumber(s.Substring(item.Index, item.Length), item.Output, _numberFstEx?.SeparatorCombinations, _Culture);
					if (numberToken != null)
					{
						if (list2 == null)
						{
							list2 = new List<PrioritizedToken>();
						}
						list2.Add(new PrioritizedToken(numberToken, 0));
					}
				}
			}
			if (list2 == null || list2.Count == 0)
			{
				return null;
			}
			EvaluateAndSortCandidates(list2);
			if (allowTokenBundles && list2.Count > 1)
			{
				return new TokenBundle(list2);
			}
			return list2[0].Token;
		}

		internal static FST CreateFST(CultureInfo culture, bool appendWordTerminator)
		{
			NumberFormatData numberFormatData = NumberPatternComputer.GetNumberFormatData(culture, addSeparatorVariants: true, augmentWhitespaceGroupSeparators: true);
			return CreateFST(culture, appendWordTerminator, numberFormatData);
		}

		internal static FST CreateFST(CultureInfo culture, bool appendWordTerminator, NumberFormatData nfd)
		{
			return CreateFST(culture, appendWordTerminator, nfd, treatFirstSeparatorsAsPrimarySeparators: true);
		}

		internal static FST CreateFST(CultureInfo culture, bool appendWordTerminator, NumberFormatData nfd, bool treatFirstSeparatorsAsPrimarySeparators)
		{
			string expression = NumberPatternComputer.ComputeFstPattern(nfd, treatFirstSeparatorsAsPrimarySeparators, appendWordTerminator);
			FST fST = FST.Create(expression);
			fST.MakeDeterministic();
			return fST;
		}

		private void EvaluateAndSortCandidates(List<PrioritizedToken> candidates)
		{
			if (candidates != null && candidates.Count != 0)
			{
				foreach (PrioritizedToken candidate in candidates)
				{
					NumberToken numberToken = candidate.Token as NumberToken;
					int num = 0;
					if (numberToken.DecimalSeparator == NumericSeparator.Alternate || numberToken.GroupSeparator == NumericSeparator.Alternate)
					{
						num++;
					}
					candidate.Priority = _Priority - num;
				}
				candidates.Sort((PrioritizedToken a, PrioritizedToken b) => b.Priority - a.Priority);
			}
		}

		internal static NumberToken ParseNumber(string surface, string output, List<SeparatorCombination> separatorCombinationsComputed, CultureInfo culture)
		{
			if (surface == null)
			{
				throw new ArgumentNullException("surface");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			char alternateGroupSeparator = '\0';
			char alternateDecimalSeparator = '\0';
			Sign sign = Sign.None;
			NumericSeparator numericSeparator = NumericSeparator.None;
			NumericSeparator numericSeparator2 = NumericSeparator.None;
			int num = 0;
			int num2 = surface.Length;
			if (NumberPatternComputer.AllowTrailingSign && num2 > 0)
			{
				char c = output[num2 - 1];
				char value = surface[num2 - 1];
				if (c == '-' || c == '+')
				{
					sign = ((c != '-') ? Sign.Plus : Sign.Minus);
					stringBuilder.Append(value);
					num2--;
				}
			}
			for (int i = 0; i < num2; i++)
			{
				char c2 = surface[i];
				char c = output[i];
				switch (num)
				{
				case 0:
					switch (c)
					{
					case '+':
					case '-':
						sign = ((c != '-') ? Sign.Plus : Sign.Minus);
						stringBuilder.Append(c2);
						num = 1;
						break;
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						stringBuilder2.Append(c);
						num = 1;
						break;
					default:
						throw new Exception($"Unexpected input in {surface}/{output} at position {i}");
					}
					break;
				case 1:
					if (c >= '0' && c <= '9')
					{
						stringBuilder2.Append(c);
						num = 1;
						break;
					}
					switch (c)
					{
					case 'G':
					case 'g':
						if (numericSeparator == NumericSeparator.None)
						{
							if (separatorCombinationsComputed != null && separatorCombinationsComputed.Count > 0)
							{
								alternateGroupSeparator = c2;
							}
							if (c == 'g')
							{
								numericSeparator = NumericSeparator.Alternate;
								alternateGroupSeparator = c2;
							}
							else
							{
								numericSeparator = NumericSeparator.Primary;
							}
						}
						break;
					case 'D':
					case 'd':
						if (numericSeparator2 == NumericSeparator.None)
						{
							if (separatorCombinationsComputed != null && separatorCombinationsComputed.Count > 0)
							{
								alternateDecimalSeparator = c2;
							}
							if (c == 'd')
							{
								numericSeparator2 = NumericSeparator.Alternate;
								alternateDecimalSeparator = c2;
							}
							else
							{
								numericSeparator2 = NumericSeparator.Primary;
							}
						}
						num = 2;
						break;
					default:
						throw new Exception($"Unexpected input in {surface}/{output} at position {i}");
					}
					break;
				case 2:
					if (c >= '0' && c <= '9')
					{
						stringBuilder3.Append(c);
						num = 2;
						break;
					}
					throw new Exception($"Unexpected input in {surface}/{output} at position {i}");
				default:
					throw new Exception("Internal error");
				}
			}
			return new NumberToken(surface, numericSeparator, numericSeparator2, alternateGroupSeparator, alternateDecimalSeparator, sign, (stringBuilder.Length > 0) ? stringBuilder.ToString() : null, (stringBuilder2.Length > 0) ? stringBuilder2.ToString() : null, (stringBuilder3.Length > 0) ? stringBuilder3.ToString() : null)
			{
				Culture = culture
			};
		}
	}
}
