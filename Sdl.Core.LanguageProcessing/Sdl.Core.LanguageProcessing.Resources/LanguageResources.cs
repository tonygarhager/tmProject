using Sdl.Core.LanguageProcessing.Stemming;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Resources
{
	public class LanguageResources
	{
		private Wordlist _abbreviations;

		private ResourceStatus _abbreviationsStatus;

		private Wordlist _stopwords;

		private ResourceStatus _stopwordsStatus;

		private StemmingRuleSet _stemmingRules;

		private ResourceStatus _stemmingRulesStatus;

		public CultureInfo Culture
		{
			get;
		}

		public IResourceDataAccessor ResourceDataAccessor
		{
			get;
		}

		public string StoplistSignature
		{
			get
			{
				if (_stopwordsStatus == ResourceStatus.NotAvailable || _stopwords == null)
				{
					return "Stopwords0";
				}
				return "Stopwords" + _stopwords.Version.ToString();
			}
		}

		public string AbbreviationsSignature
		{
			get
			{
				EnsureAbbreviationsLoaded();
				if (_abbreviationsStatus == ResourceStatus.NotAvailable || _abbreviations == null)
				{
					return "Abbr0";
				}
				StringBuilder stringBuilder = new StringBuilder();
				List<string> list = new List<string>();
				foreach (string item in _abbreviations.Items)
				{
					list.Add(item.Replace("|", "\\|"));
				}
				list.Sort((string a, string b) => string.CompareOrdinal(a, b));
				foreach (string item2 in list)
				{
					stringBuilder.Append(item2 + "|");
				}
				return "Abbr" + stringBuilder?.ToString();
			}
		}

		public StemmingRuleSet StemmingRules
		{
			get
			{
				if (_stemmingRules != null)
				{
					return _stemmingRules;
				}
				if (_stemmingRulesStatus == ResourceStatus.NotAvailable)
				{
					return null;
				}
				using (Stream s = ResourceDataAccessor.ReadResourceData(Culture, LanguageResourceType.StemmingRules, fallback: true))
				{
					StemmingRuleSetReader stemmingRuleSetReader = new StemmingRuleSetReader(s);
					_stemmingRules = stemmingRuleSetReader.Read(Culture);
					if (_stemmingRules != null)
					{
						_stemmingRulesStatus = ResourceStatus.Loaded;
					}
				}
				return _stemmingRules;
			}
			set
			{
				_stemmingRules = value;
			}
		}

		public LanguageResources(CultureInfo culture)
			: this(culture, null)
		{
		}

		public LanguageResources(CultureInfo culture, IResourceDataAccessor accessor)
		{
			if (accessor == null)
			{
				accessor = Configuration.Load();
			}
			Culture = (culture ?? throw new ArgumentNullException("culture"));
			ResourceDataAccessor = accessor;
			_abbreviationsStatus = ResourceDataAccessor.GetResourceStatus(culture, LanguageResourceType.Abbreviations, fallback: true);
			_stopwordsStatus = ResourceDataAccessor.GetResourceStatus(culture, LanguageResourceType.Stopwords, fallback: true);
			_stemmingRulesStatus = ResourceDataAccessor.GetResourceStatus(culture, LanguageResourceType.StemmingRules, fallback: true);
		}

		private Wordlist LoadWordlist(LanguageResourceType t)
		{
			return ResourceStorage.LoadWordlist(ResourceDataAccessor, Culture, t, fallback: true);
		}

		public bool IsAbbreviation(string s)
		{
			EnsureAbbreviationsLoaded();
			if (_abbreviations != null)
			{
				return _abbreviations.Contains(s);
			}
			return false;
		}

		private void EnsureAbbreviationsLoaded()
		{
			if (_abbreviations == null && _abbreviationsStatus != ResourceStatus.NotAvailable)
			{
				_abbreviations = LoadWordlist(LanguageResourceType.Abbreviations);
				if (_abbreviations != null)
				{
					_abbreviationsStatus = ResourceStatus.Loaded;
				}
			}
		}

		public bool IsStopword(string s)
		{
			if (_stopwords != null)
			{
				return _stopwords.Contains(s);
			}
			if (_stopwordsStatus == ResourceStatus.NotAvailable)
			{
				return false;
			}
			_stopwords = LoadWordlist(LanguageResourceType.Stopwords);
			if (_stopwords != null)
			{
				_stopwordsStatus = ResourceStatus.Loaded;
			}
			if (_stopwords != null)
			{
				return _stopwords.Contains(s);
			}
			return false;
		}
	}
}
