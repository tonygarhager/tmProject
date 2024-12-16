using Sdl.Core.Globalization.UnitMetadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[DataContract]
	public class MeasureToken : NumberToken
	{
		[DataMember]
		public string CustomCategory
		{
			get;
			set;
		}

		[DataMember]
		public Unit Unit
		{
			get;
			set;
		}

		[DataMember]
		public char UnitSeparator
		{
			get;
			set;
		}

		[DataMember]
		public string UnitString
		{
			get;
			set;
		}

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => true;

		public bool IsValid
		{
			get
			{
				if (Unit == Unit.NoUnit)
				{
					return UnitString != null;
				}
				return true;
			}
		}

		public MeasureToken(string text, string sign, string decimalPart, string fractionalPart, Unit unit, string unitString, char unitSeparator, NumberFormatInfo format)
			: base(text, sign, decimalPart, fractionalPart, format)
		{
			Unit = unit;
			UnitString = unitString;
			UnitSeparator = unitSeparator;
		}

		public MeasureToken(string text, NumberToken numericPart, Unit unit, string unitString, char unitSeparator)
			: this(text, numericPart, unit, unitString, unitSeparator, null)
		{
		}

		public MeasureToken(string text, NumberToken numericPart, Unit unit, string unitString, char unitSeparator, string customCategory)
			: base(text, numericPart.GroupSeparator, numericPart.DecimalSeparator, numericPart.AlternateGroupSeparator, numericPart.AlternateDecimalSeparator, numericPart.Sign, numericPart.RawSign, numericPart.RawDecimalDigits, numericPart.RawFractionalDigits)
		{
			Unit = unit;
			UnitString = unitString;
			UnitSeparator = unitSeparator;
			CustomCategory = customCategory;
		}

		public MeasureToken(MeasureToken other)
			: base(other)
		{
			Unit = other.Unit;
			UnitSeparator = other.UnitSeparator;
			UnitString = other.UnitString;
			CustomCategory = other.CustomCategory;
		}

		private bool CategoriesMatch(MeasureToken other)
		{
			if (Unit != Unit.NoUnit && Unit != Unit.Currency)
			{
				return true;
			}
			if (CustomCategory == null != (other.CustomCategory == null))
			{
				return false;
			}
			if (CustomCategory == null)
			{
				return string.CompareOrdinal(UnitString, other.UnitString) == 0;
			}
			return string.CompareOrdinal(other.CustomCategory, CustomCategory) == 0;
		}

		private bool CurrencyMismatch(MeasureToken other)
		{
			if (Unit != Unit.Currency)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(CustomCategory))
			{
				return false;
			}
			return !string.Equals(UnitString, other.UnitString, StringComparison.Ordinal);
		}

		public override bool SetValue(Token blueprint, bool keepNumericSeparators)
		{
			bool result = base.SetValue(blueprint, keepNumericSeparators);
			MeasureToken measureToken = blueprint as MeasureToken;
			if (measureToken == null)
			{
				Unit = Unit.NoUnit;
				UnitSeparator = '\0';
				UnitString = null;
			}
			else
			{
				if (!KeepUnitString(measureToken))
				{
					Unit = measureToken.Unit;
					UnitSeparator = measureToken.UnitSeparator;
					UnitString = measureToken.UnitString;
					CustomCategory = measureToken.CustomCategory;
					result = true;
				}
				if (UnitSeparator == measureToken.UnitSeparator)
				{
					return result;
				}
				UnitSeparator = measureToken.UnitSeparator;
				result = true;
			}
			return result;
		}

		public override SegmentElement Duplicate()
		{
			return new MeasureToken(this);
		}

		public override Similarity GetSimilarity(SegmentElement other, bool allowCompatibility)
		{
			Similarity bundleSimilarity = GetBundleSimilarity(other);
			if (other == null || other.GetType() != GetType())
			{
				return bundleSimilarity;
			}
			MeasureToken measureToken = other as MeasureToken;
			if (measureToken == null)
			{
				return bundleSimilarity;
			}
			bool flag = base.GetSimilarity((SegmentElement)measureToken) == Similarity.IdenticalValueAndType;
			if (Unit == Unit.NoUnit && measureToken.Unit == Unit.NoUnit)
			{
				bool flag2 = false;
				if (allowCompatibility)
				{
					flag2 = PhysicalUnit.AreUnitsSameClass(base.Culture, UnitString, measureToken.UnitString);
				}
				if (!flag2)
				{
					if (CustomCategory != null || measureToken.CustomCategory != null)
					{
						flag2 = (CustomCategory != null && measureToken.CustomCategory != null && string.CompareOrdinal(CustomCategory, measureToken.CustomCategory) == 0);
					}
					else if (string.CompareOrdinal(UnitString, measureToken.UnitString) == 0)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					if (!flag)
					{
						return Similarity.IdenticalType;
					}
					return Similarity.IdenticalValueAndType;
				}
				return Similarity.None;
			}
			if (Unit == measureToken.Unit || (allowCompatibility && PhysicalUnit.AreUnitsSameClass(base.Culture, Unit, measureToken.Unit)))
			{
				if (!flag)
				{
					return Similarity.IdenticalType;
				}
				return Similarity.IdenticalValueAndType;
			}
			return Similarity.None;
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			return GetSimilarity(other, allowCompatibility: false);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			if (!base.Equals(obj))
			{
				return false;
			}
			MeasureToken measureToken = (MeasureToken)obj;
			if (Unit == measureToken.Unit)
			{
				if (Unit == Unit.NoUnit)
				{
					return string.Equals(UnitString, measureToken.UnitString, StringComparison.Ordinal);
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 3) | (int)Unit;
		}

		protected override TokenType GetTokenType()
		{
			return TokenType.Measurement;
		}

		public override bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings)
		{
			return Localize(culture, autoLocalizationSettings, null, adjustCasing: false);
		}

		private bool KeepUnitString(MeasureToken other)
		{
			if (Unit != other.Unit || !CategoriesMatch(other) || CurrencyMismatch(other))
			{
				return false;
			}
			return true;
		}

		public override bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings, ILocalizableToken originalMemoryToken, bool adjustCasing)
		{
			string text = Text;
			LanguageMetadata orCreateMetadata = LanguageMetadata.GetOrCreateMetadata(culture.Name);
			UnitMetadataRegistry registry = UnitMetadataApi.Instance.Registry;
			Localize(culture, autoLocalizationSettings, originalMemoryToken, adjustCasing, Unit == Unit.Currency);
			StringBuilder stringBuilder = new StringBuilder(Text);
			UnitSeparationMode unitSeparationMode = autoLocalizationSettings?.UnitSeparationMode ?? UnitSeparationMode.Auto;
			LocalizationParametersSource localizationParametersSource = (autoLocalizationSettings != null && originalMemoryToken is MeasureToken) ? autoLocalizationSettings.LocalizationParametersSource : LocalizationParametersSource.FromDocument;
			MeasureToken measureToken = originalMemoryToken as MeasureToken;
			bool flag = measureToken != null && KeepUnitString(measureToken);
			string unitString = UnitString;
			CurrencyFormat currencyFormat = null;
			CurrencySymbolPosition currencySymbolPosition = (orCreateMetadata.NumberFormat.CurrencyPositivePattern % 2 == 0) ? CurrencySymbolPosition.beforeAmount : CurrencySymbolPosition.afterAmount;
			bool flag2 = originalMemoryToken != null && localizationParametersSource != LocalizationParametersSource.FromDocument;
			if (Unit == Unit.Currency)
			{
				if (autoLocalizationSettings != null)
				{
					flag2 &= !autoLocalizationSettings.FormatCurrencyPositionUniformly;
				}
				if (base.TokenizationContext?.CurrencyFormats != null)
				{
					if (!string.IsNullOrEmpty(CustomCategory))
					{
						currencyFormat = base.TokenizationContext?.CurrencyFormats.FirstOrDefault((CurrencyFormat x) => string.CompareOrdinal(x.Category, CustomCategory) == 0);
						if (currencyFormat == null)
						{
							CustomCategory = null;
						}
					}
					if (currencyFormat == null)
					{
						currencyFormat = base.TokenizationContext?.CurrencyFormats.FirstOrDefault((CurrencyFormat x) => string.CompareOrdinal(x.Symbol, unitString) == 0);
					}
					if (currencyFormat != null)
					{
						if (!flag)
						{
							unitString = currencyFormat.Symbol;
						}
						if (currencyFormat.CurrencySymbolPositions != null && currencyFormat.CurrencySymbolPositions.Count > 0)
						{
							currencySymbolPosition = currencyFormat.CurrencySymbolPositions[0];
						}
					}
				}
				else
				{
					CustomCategory = null;
				}
				if (flag2)
				{
					MeasureToken measureToken2 = originalMemoryToken as MeasureToken;
					if (measureToken2 != null && measureToken2.Text != null && measureToken2.Text.Length > 0 && measureToken2.Text.Contains(unitString))
					{
						currencySymbolPosition = ((!char.IsDigit(measureToken2.Text[0])) ? CurrencySymbolPosition.beforeAmount : CurrencySymbolPosition.afterAmount);
					}
				}
				if (!flag2 && autoLocalizationSettings != null && autoLocalizationSettings.CurrencySymbolPosition.HasValue)
				{
					currencySymbolPosition = autoLocalizationSettings.CurrencySymbolPosition.Value;
				}
			}
			else
			{
				string text2 = null;
				if (!flag && base.TokenizationContext?.UnitDefinitions != null)
				{
					if (Unit != Unit.NoUnit)
					{
						foreach (KeyValuePair<string, CustomUnitDefinition> unitDefinition in base.TokenizationContext.UnitDefinitions)
						{
							if (unitDefinition.Value != null && unitDefinition.Value.Unit == Unit)
							{
								text2 = unitDefinition.Key;
								break;
							}
						}
					}
					else if (!string.IsNullOrEmpty(CustomCategory))
					{
						foreach (KeyValuePair<string, CustomUnitDefinition> unitDefinition2 in base.TokenizationContext.UnitDefinitions)
						{
							if (unitDefinition2.Value != null && unitDefinition2.Value.Unit == Unit.NoUnit && unitDefinition2.Value.CategoryName != null && string.CompareOrdinal(CustomCategory, unitDefinition2.Value.CategoryName) == 0)
							{
								text2 = unitDefinition2.Key;
								break;
							}
						}
					}
				}
				if (text2 == null && (Unit != Unit.NoUnit || !string.IsNullOrEmpty(CustomCategory)) && Unit != Unit.Other)
				{
					string unitKey = (Unit == Unit.NoUnit) ? CustomCategory : Unit.ToString();
					LabelValueSet labelValueSet = registry.GetPreferredLabelValueSet(unitKey, culture.Name);
					if (flag)
					{
						labelValueSet = registry.UnitMetadataFromLabel(UnitString, culture.Name)?.LabelValueSets.FirstOrDefault((LabelValueSet x) => x.LabelValueConditions.Any((LabelValueCondition y) => string.Compare(y.Label, UnitString) == 0));
					}
					if (labelValueSet != null)
					{
						string languageCodeFound;
						UnitMetadata unitMetadata = registry.UnitMetadataFromKey(unitKey, "", out languageCodeFound);
						bool flag3 = unitMetadata.LabelValueSets.Contains(labelValueSet);
						UnitMetadata unitMetadata2 = registry.UnitMetadataFromLabel(unitString, culture.Name);
						if (unitMetadata2 != null && flag3)
						{
							labelValueSet = unitMetadata2.LabelValueSetFromLabel(unitString);
						}
						text2 = ((!string.IsNullOrEmpty(base.RawFractionalDigits) || !int.TryParse(base.RawDecimalDigits, out int result)) ? labelValueSet.GetLabel(base.Value) : labelValueSet.GetLabel(result));
					}
				}
				if (text2 != null)
				{
					unitString = text2;
				}
			}
			string value = string.Empty;
			switch (unitSeparationMode)
			{
			case UnitSeparationMode.InsertSpace:
				value = " ";
				break;
			case UnitSeparationMode.InsertNonbreakingSpace:
				value = "\u00a0";
				break;
			case UnitSeparationMode.InsertSpecifiedSeparator:
				value = ((autoLocalizationSettings == null || autoLocalizationSettings.UnitSeparator == '\0') ? " " : autoLocalizationSettings.UnitSeparator.ToString());
				break;
			default:
				if (localizationParametersSource == LocalizationParametersSource.FromMemory)
				{
					if (originalMemoryToken == null)
					{
						if (Unit == Unit.Currency)
						{
							if (orCreateMetadata.NumberFormat.CurrencyPositivePattern >= 2)
							{
								value = "\u00a0";
							}
						}
						else
						{
							value = "\u00a0";
						}
					}
					else
					{
						MeasureToken measureToken3 = originalMemoryToken as MeasureToken;
						if (measureToken3 != null && measureToken3.UnitSeparator != 0)
						{
							value = measureToken3.UnitSeparator.ToString();
						}
					}
				}
				else if (UnitSeparator != 0)
				{
					value = UnitSeparator.ToString();
				}
				break;
			case UnitSeparationMode.DeleteWhitespace:
				break;
			}
			if (Unit == Unit.Currency && currencySymbolPosition == CurrencySymbolPosition.beforeAmount)
			{
				stringBuilder.Insert(0, value);
				stringBuilder.Insert(0, unitString);
			}
			else
			{
				stringBuilder.Append(value);
				if (Unit == Unit.Mpercent)
				{
					unitString = PhysicalUnit.GetPreferredAbbreviation(Unit, culture);
					if (unitString == null)
					{
						unitString = PhysicalUnit.GetPreferredAbbreviation(Unit, null);
					}
				}
				string value2 = (unitString == null) ? PhysicalUnit.GetPreferredAbbreviation(Unit, null) : unitString;
				stringBuilder.Append(value2);
			}
			string text3 = stringBuilder.ToString();
			base.Culture = culture;
			if (!string.Equals(text3, Text, StringComparison.Ordinal))
			{
				Text = text3;
				return true;
			}
			Text = text;
			return false;
		}

		public override void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitMeasureToken(this);
		}
	}
}
