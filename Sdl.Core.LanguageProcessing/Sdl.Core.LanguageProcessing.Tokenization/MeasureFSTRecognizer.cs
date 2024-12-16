using Sdl.Core.Globalization.UnitMetadata;
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
	internal class MeasureFSTRecognizer : Recognizer
	{
		private readonly FSTRecognizer _fstRecognizer;

		private readonly NumberFSTEx _numberFstEx;

		private readonly MeasureFSTEx _measureFstEx;

		public Dictionary<string, CustomUnitDefinition> UnitDefinitions => _measureFstEx?.UnitDefinitions;

		public override string GetSignature(CultureInfo culture)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(NumberFSTRecognizer.GetSeparatorsSignatureString(_numberFstEx));
			List<string> list = new List<string>();
			if (_measureFstEx?.UnitDefinitions != null)
			{
				foreach (KeyValuePair<string, CustomUnitDefinition> item in _measureFstEx?.UnitDefinitions)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					string value = string.Empty;
					string value2 = string.Empty;
					if (item.Value != null)
					{
						value = item.Value.Unit.ToString();
						value2 = (string.IsNullOrEmpty(item.Value.CategoryName) ? string.Empty : item.Value.CategoryName);
					}
					stringBuilder2.Append(item.Key);
					stringBuilder2.Append(" ");
					stringBuilder2.Append(value);
					stringBuilder2.Append(" ");
					stringBuilder2.Append(value2);
					list.Add(stringBuilder2.ToString());
				}
			}
			list.Sort((string a, string b) => string.CompareOrdinal(a, b));
			foreach (string item2 in list)
			{
				stringBuilder.Append(item2 + "|");
			}
			if (CultureInfoExtensions.UseFullWidth(culture))
			{
				return base.GetSignature(culture) + "Measure1" + stringBuilder?.ToString();
			}
			return base.GetSignature(culture) + "Measure0" + stringBuilder?.ToString();
		}

		public static bool AppendWordTerminator(CultureInfo culture)
		{
			return NumberFSTRecognizer.AppendWordTerminator(culture);
		}

		public static Recognizer Create(RecognizerSettings settings, IResourceDataAccessor resourceDataAccessor, CultureInfo culture, int priority)
		{
			MeasureFSTRecognizer measureFSTRecognizer = new MeasureFSTRecognizer(settings, culture, priority, resourceDataAccessor);
			SetAdditionalOptions(measureFSTRecognizer);
			return measureFSTRecognizer;
		}

		public static Recognizer Create(RecognizerSettings settings, CultureInfo culture, int priority)
		{
			MeasureFSTRecognizer measureFSTRecognizer = new MeasureFSTRecognizer(settings, culture, priority);
			SetAdditionalOptions(measureFSTRecognizer);
			return measureFSTRecognizer;
		}

		private static void SetAdditionalOptions(Recognizer result)
		{
			result.OnlyIfFollowedByNonwordCharacter = true;
			result.OverrideFallbackRecognizer = true;
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
				return (string s, int p, Token t) => KoreanTextConstraints(this, s, p, t);
			}
			return null;
		}

		internal static bool KoreanTextConstraints(Recognizer r, string s, int p, Token t)
		{
			if (r.DefaultTextConstraints(s, p))
			{
				return true;
			}
			MeasureToken measureToken = t as MeasureToken;
			if (measureToken != null && !string.IsNullOrEmpty(measureToken.UnitString) && !string.IsNullOrEmpty(measureToken.Text))
			{
				if (p >= s.Length || !CharacterProperties.IsKoreanChar(s[p]))
				{
					return false;
				}
				if (measureToken.Unit == Unit.Currency && measureToken.Text[0] == measureToken.UnitString[0])
				{
					return true;
				}
				return !measureToken.UnitString.Any((char x) => CharacterProperties.IsKoreanChar(x));
			}
			return false;
		}

		public MeasureFSTRecognizer(RecognizerSettings settings, CultureInfo culture, int priority)
			: base(settings, TokenType.Measurement, priority, "Measurement", "MeasureFSTRecognizer", autoSubstitutable: false, culture)
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
			_cultureSpecificTextConstraints = GetCultureSpecificTextConstraints(culture);
			FST fst = CreateFST(culture, AppendWordTerminator(culture));
			_fstRecognizer = new FSTRecognizer(fst, culture);
		}

		public MeasureFSTRecognizer(RecognizerSettings settings, CultureInfo culture, int priority, IResourceDataAccessor accessor)
			: base(settings, TokenType.Measurement, priority, "Measurement", "MeasureFSTRecognizer", autoSubstitutable: false, culture)
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
			_cultureSpecificTextConstraints = GetCultureSpecificTextConstraints(culture);
			FST fst;
			if (accessor.GetResourceStatus(culture, LanguageResourceType.MeasurementFST, fallback: true) != ResourceStatus.NotAvailable)
			{
				byte[] resourceData = accessor.GetResourceData(culture, LanguageResourceType.MeasurementFST, fallback: true);
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
				if (accessor.GetResourceStatus(culture, LanguageResourceType.MeasurementFSTEx, fallback: true) != ResourceStatus.NotAvailable)
				{
					resourceData = accessor.GetResourceData(culture, LanguageResourceType.MeasurementFSTEx, fallback: true);
					if (resourceData == null)
					{
						throw new LanguageProcessingException(ErrorMessages.EMSG_ResourceNotAvailable);
					}
					_measureFstEx = MeasureFSTEx.FromBinary(resourceData);
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
			EvaluateAndSortCandidates(list2, _Priority);
			if (allowTokenBundles && list2.Count > 1)
			{
				return new TokenBundle(list2);
			}
			return list2[0].Token;
		}

		internal static FST CreateFST(CultureInfo culture, bool appendWordTerminator)
		{
			NumberFormatData numberFormatData = NumberPatternComputer.GetNumberFormatData(culture, addSeparatorVariants: true, augmentWhitespaceGroupSeparators: true);
			return CreateFST(culture, appendWordTerminator, numberFormatData, null, customUnitsOnly: false, treatFirstSeparatorsAsPrimarySeparators: true);
		}

		internal static FST CreateFST(CultureInfo culture, bool appendWordTerminator, NumberFormatData nfd, HashSet<string> customUnits, bool customUnitsOnly, bool treatFirstSeparatorsAsPrimarySeparators)
		{
			string value = NumberPatternComputer.ComputeFstPattern(nfd, treatFirstSeparatorsAsPrimarySeparators, appendWordTerminator: false);
			StringBuilder stringBuilder = new StringBuilder(value);
			stringBuilder.Append("(");
			bool first = true;
			NumberPatternComputer.AppendDisjunction(stringBuilder, CharacterProperties.Blanks, 'U', ref first);
			stringBuilder.Append(")?(");
			if (customUnitsOnly && (customUnits == null || customUnits.Count == 0))
			{
				throw new Exception("customUnitsOnly specified but none were provided");
			}
			HashSet<string> hashSet = new HashSet<string>();
			if (customUnits != null)
			{
				foreach (string customUnit in customUnits)
				{
					hashSet.Add(customUnit);
				}
			}
			if (!customUnitsOnly)
			{
				List<UnitMetadata> allMetadata = UnitMetadataApi.Instance.Registry.GetAllMetadata(culture.Name);
				foreach (UnitMetadata item in allMetadata)
				{
					foreach (LabelValueSet labelValueSet in item.LabelValueSets)
					{
						foreach (LabelValueCondition labelValueCondition in labelValueSet.LabelValueConditions)
						{
							hashSet.Add(labelValueCondition.Label);
							customUnits?.Add(labelValueCondition.Label);
						}
					}
				}
			}
			first = true;
			foreach (string item2 in hashSet)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					stringBuilder.Append("|");
				}
				stringBuilder.AppendFormat("(<{0}:U>", FST.EscapeSpecial(item2[0]));
				string text = item2.Substring(1);
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.Append(FST.EscapeSpecial(text));
				}
				stringBuilder.Append(")");
			}
			stringBuilder.Append(")");
			if (appendWordTerminator)
			{
				stringBuilder.Append("#>");
			}
			FST fST = FST.Create(stringBuilder.ToString());
			fST.MakeDeterministic();
			return fST;
		}

		internal static void EvaluateAndSortCandidates(List<PrioritizedToken> candidates, int priority)
		{
			if (candidates != null && candidates.Count != 0)
			{
				foreach (PrioritizedToken candidate in candidates)
				{
					MeasureToken measureToken = candidate.Token as MeasureToken;
					int num = 0;
					if (measureToken.DecimalSeparator == NumericSeparator.Alternate || measureToken.GroupSeparator == NumericSeparator.Alternate)
					{
						num++;
					}
					candidate.Priority = priority - num;
				}
				candidates.Sort((PrioritizedToken a, PrioritizedToken b) => b.Priority - a.Priority);
			}
		}

		private MeasureToken Parse(string surface, string output)
		{
			int i = output.IndexOf('U');
			if (i <= 0)
			{
				throw new Exception("Invalid measurement format");
			}
			string surface2 = surface.Substring(0, i);
			string output2 = output.Substring(0, i);
			char c = '\0';
			for (; i < surface.Length && char.IsWhiteSpace(surface[i]); i++)
			{
				if (c == '\0')
				{
					c = surface[i];
				}
			}
			string text = surface.Substring(i);
			NumberToken numericPart = NumberFSTRecognizer.ParseNumber(surface2, output2, _numberFstEx?.SeparatorCombinations, _Culture);
			UnitMetadata unitMetadata = UnitMetadataApi.Instance.Registry.UnitMetadataFromLabel(text, _Culture.Name);
			string customCategory = null;
			if (unitMetadata == null || !Enum.TryParse(unitMetadata.UnitKey, out Unit result))
			{
				result = Unit.NoUnit;
				customCategory = text;
			}
			if (_measureFstEx?.UnitDefinitions != null && _measureFstEx.UnitDefinitions.TryGetValue(text, out CustomUnitDefinition value) && value != null)
			{
				result = value.Unit;
				customCategory = value.CategoryName;
			}
			MeasureToken measureToken = new MeasureToken(surface, numericPart, result, text, c, customCategory);
			measureToken.Culture = _Culture;
			return measureToken;
		}
	}
}
