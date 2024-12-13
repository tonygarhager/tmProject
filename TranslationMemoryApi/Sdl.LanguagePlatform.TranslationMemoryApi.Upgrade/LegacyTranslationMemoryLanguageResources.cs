using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class LegacyTranslationMemoryLanguageResources
	{
		private readonly ILegacyLanguageResources _languageResources;

		private readonly ITranslationMemorySetupOptions _setup;

		public LegacyTranslationMemoryLanguageResources(ITranslationMemorySetupOptions setup, ILegacyLanguageResources languageResources)
		{
			_setup = setup;
			_languageResources = languageResources;
		}

		public LanguageResourceBundle AsResourceBundle(CultureInfo sourceLanguage)
		{
			LanguageResourceBundle languageResourceBundle = new LanguageResourceBundle(sourceLanguage);
			CopySegmentationRules(languageResourceBundle);
			CopyVariables(languageResourceBundle);
			CopyAbbreviations(sourceLanguage, languageResourceBundle);
			CopyOrdinalFollowers(sourceLanguage, languageResourceBundle);
			CopyNumberSeparators(languageResourceBundle);
			CopyCurrencyFormats(languageResourceBundle);
			CopyMeasurementUnits(languageResourceBundle);
			CopyDateTimeFormats(languageResourceBundle);
			return languageResourceBundle;
		}

		private void CopyAbbreviations(CultureInfo sourceLanguage, LanguageResourceBundle bundle)
		{
			if (_setup.LanguageResources != null && _languageResources.Abbreviations != null)
			{
				bundle.Abbreviations = MergeDefaultAbbreviations(_languageResources.Abbreviations, sourceLanguage);
			}
		}

		private void CopyCurrencyFormats(LanguageResourceBundle bundle)
		{
			if (_languageResources.CurrencyFormats != null)
			{
				bundle.CurrencyFormats = _languageResources.CurrencyFormats;
			}
		}

		private void CopyDateTimeFormats(LanguageResourceBundle bundle)
		{
			ILookup<LanguageResourceType, string> formats;
			if (_languageResources.DateTimeFormats != null)
			{
				formats = _languageResources.DateTimeFormats;
				bundle.ShortTimeFormats = Formats(LanguageResourceType.ShortTimeFSTEx);
				bundle.LongTimeFormats = Formats(LanguageResourceType.LongTimeFSTEx);
				bundle.ShortDateFormats = Formats(LanguageResourceType.ShortDateFSTEx);
				bundle.LongDateFormats = Formats(LanguageResourceType.LongDateFSTEx);
			}
			List<string> Formats(LanguageResourceType type)
			{
				if (!formats.Contains(type))
				{
					return null;
				}
				return formats[type].ToList();
			}
		}

		private void CopyMeasurementUnits(LanguageResourceBundle bundle)
		{
			if (_languageResources.MeasurementUnits != null)
			{
				bundle.MeasurementUnits = _languageResources.MeasurementUnits;
			}
		}

		private void CopyNumberSeparators(LanguageResourceBundle bundle)
		{
			if (_languageResources.NumbersSeparators != null)
			{
				bundle.NumbersSeparators = _languageResources.NumbersSeparators;
			}
		}

		private void CopyOrdinalFollowers(CultureInfo sourceLanguage, LanguageResourceBundle bundle)
		{
			if (_setup.LanguageResources != null && _languageResources.OrdinalFollowers != null)
			{
				bundle.OrdinalFollowers = MergeDefaultOrdinalFollowers(_languageResources.OrdinalFollowers, sourceLanguage);
			}
		}

		private void CopySegmentationRules(LanguageResourceBundle bundle)
		{
			if (_languageResources.SegmentationRules != null)
			{
				bundle.SegmentationRules = _languageResources.SegmentationRules;
			}
		}

		private void CopyVariables(LanguageResourceBundle bundle)
		{
			if (_languageResources.Variables != null)
			{
				bundle.Variables = _languageResources.Variables;
			}
		}

		private Wordlist GetDefaultWordlist(CultureInfo sourceCulture, LanguageResourceType resourceType)
		{
			DefaultLanguageResourceProvider defaultLanguageResourceProvider = new DefaultLanguageResourceProvider();
			return defaultLanguageResourceProvider.GetDefaultWordlist(resourceType, sourceCulture);
		}

		private Wordlist MergeDefaultAbbreviations(Wordlist userAbbreviations, CultureInfo sourceCulture)
		{
			return MergeDefaultWordlist(userAbbreviations, sourceCulture, LanguageResourceType.Abbreviations);
		}

		private Wordlist MergeDefaultOrdinalFollowers(Wordlist userOrdinalFollowers, CultureInfo sourceCulture)
		{
			return MergeDefaultWordlist(userOrdinalFollowers, sourceCulture, LanguageResourceType.OrdinalFollowers);
		}

		private Wordlist MergeDefaultWordlist(Wordlist userWordlist, CultureInfo sourceCulture, LanguageResourceType resourceType)
		{
			Wordlist defaultWordlist = GetDefaultWordlist(sourceCulture, resourceType);
			Wordlist wordlist = new Wordlist(userWordlist);
			if (defaultWordlist != null && defaultWordlist.Count > 0)
			{
				wordlist.Merge(defaultWordlist);
			}
			return wordlist;
		}
	}
}
