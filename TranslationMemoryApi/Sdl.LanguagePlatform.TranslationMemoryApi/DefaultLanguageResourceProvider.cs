using Sdl.Core.Globalization.UnitMetadata;
using Sdl.Core.LanguageProcessing;
using Sdl.Core.LanguageProcessing.Resources;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Provides access to the default data for language resources.
	/// </summary>
	public class DefaultLanguageResourceProvider
	{
		private ResourceFileResourceAccessor _resourceFileAccessor;

		private RecognizerFactory _recognizerFactory;

		/// <summary> 
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.DefaultLanguageResourceProvider" /> class.
		/// </summary>
		public DefaultLanguageResourceProvider()
		{
			_recognizerFactory = new RecognizerFactory();
			_resourceFileAccessor = new ResourceFileResourceAccessor();
		}

		/// <summary>
		/// Gets the default language resource data for the specified type and language.
		/// </summary>
		/// <param name="type">The language resource type requested.</param>
		/// <param name="language">The language for for which to get the default language resource data.</param>
		/// <returns>The requested language resource data.</returns>
		private byte[] GetDefaultLanguageResource(LanguageResourceType type, CultureInfo language)
		{
			return _resourceFileAccessor.GetResourceData(language, type, fallback: true);
		}

		/// <summary>
		/// Gets default language resources for the specified language.
		/// </summary>
		/// <param name="language">The language for which to get the default language resources.</param>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.LanguageResourceBundle" /> instance, populated with the default language resources. Note that
		/// some default language resources might be null.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="language" /> is null.</exception>
		public LanguageResourceBundle GetDefaultLanguageResources(CultureInfo language)
		{
			if (language == null)
			{
				throw new ArgumentNullException("language");
			}
			LanguageResourceBundle languageResourceBundle = new LanguageResourceBundle(language);
			languageResourceBundle.Abbreviations = GetDefaultWordlist(LanguageResourceType.Abbreviations, language);
			languageResourceBundle.OrdinalFollowers = GetDefaultWordlist(LanguageResourceType.OrdinalFollowers, language);
			languageResourceBundle.Variables = GetDefaultWordlist(LanguageResourceType.Variables, language);
			byte[] defaultLanguageResource = GetDefaultLanguageResource(LanguageResourceType.SegmentationRules, language);
			languageResourceBundle.SegmentationRules = _recognizerFactory.GetSegmentationRules(defaultLanguageResource, language.Name);
			languageResourceBundle.NumbersSeparators = GetDefaultNumberSeparators(language);
			languageResourceBundle.CurrencyFormats = GetDefaultCurrencyFormats(language);
			languageResourceBundle.MeasurementUnits = GetDefaultMeasurementUnits(language);
			FillDefaultDateTimeFormats(language, languageResourceBundle);
			return languageResourceBundle;
		}

		private Dictionary<string, CustomUnitDefinition> GetDefaultMeasurementUnits(CultureInfo language)
		{
			UnitMetadataRegistry registry = UnitMetadataApi.Instance.Registry;
			List<UnitMetadata> allMetadata = registry.GetAllMetadata(language.Name);
			Dictionary<string, CustomUnitDefinition> dictionary = new Dictionary<string, CustomUnitDefinition>();
			foreach (UnitMetadata item in allMetadata)
			{
				CustomUnitDefinition customUnitDefinition = new CustomUnitDefinition
				{
					Unit = Unit.NoUnit
				};
				if (Enum.TryParse(item.UnitKey, out Unit result))
				{
					customUnitDefinition.Unit = result;
				}
				else
				{
					customUnitDefinition.CategoryName = item.UnitKey;
				}
				foreach (LabelValueSet labelValueSet in item.LabelValueSets)
				{
					foreach (LabelValueCondition labelValueCondition in labelValueSet.LabelValueConditions)
					{
						if (!dictionary.ContainsKey(labelValueCondition.Label))
						{
							dictionary.Add(labelValueCondition.Label, customUnitDefinition);
						}
					}
				}
			}
			return dictionary;
		}

		public Wordlist GetDefaultWordlist(LanguageResourceType type, CultureInfo language)
		{
			byte[] defaultLanguageResource = GetDefaultLanguageResource(type, language);
			return _recognizerFactory.CreateWordList(defaultLanguageResource, ignoreComments: true);
		}

		public List<SeparatorCombination> GetDefaultNumberSeparators(CultureInfo language)
		{
			(FST, NumberFSTEx, FST, MeasureFSTEx, FST, CurrencyFSTEx) measureNumberAndCurrencyLanguageResources = 
				NumberPatternComputer.GetMeasureNumberAndCurrencyLanguageResources(language, LanguageMetadata.GetOrCreateMetadata(language.Name), null, null, null, customSeparatorsOnly: false, customUnitsOnly: false);
			return measureNumberAndCurrencyLanguageResources.Item2.SeparatorCombinations;
		}

		private List<CurrencyFormat> GetDefaultCurrencyFormats(CultureInfo language)
		{
			return CurrencyFSTEx.GetDefaults(language, LanguageMetadata.GetOrCreateMetadata(language.Name), null).CurrencyFormats;
		}

		private void FillDefaultDateTimeFormats(CultureInfo language, LanguageResourceBundle bundle)
		{
			List<(LanguageResourceType, FST, LanguageResourceType, DateTimeFSTEx)> dateTimeLanguageResources = DateTimePatternComputer.GetDateTimeLanguageResources(language, LanguageMetadata.GetMetadata(language.Name), null, customOnly: false);
			foreach (var item3 in dateTimeLanguageResources)
			{
				(LanguageResourceType, FST, LanguageResourceType, DateTimeFSTEx) valueTuple = item3;
				LanguageResourceType item = valueTuple.Item3;
				DateTimeFSTEx item2 = valueTuple.Item4;
				switch (item)
				{
				case LanguageResourceType.ShortTimeFSTEx:
					bundle.ShortTimeFormats = item2.Patterns;
					break;
				case LanguageResourceType.LongTimeFSTEx:
					bundle.LongTimeFormats = item2.Patterns;
					break;
				case LanguageResourceType.ShortDateFSTEx:
					bundle.ShortDateFormats = item2.Patterns;
					break;
				case LanguageResourceType.LongDateFSTEx:
					bundle.LongDateFormats = item2.Patterns;
					break;
				}
			}
		}
	}
}
