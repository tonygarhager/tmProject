using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal class TranslationMemorySetupOptions : ITranslationMemorySetupOptions
	{
		private IList<LanguagePair> _languageDirections = new List<LanguagePair>();

		private IList<FieldDefinition> _fields = new List<FieldDefinition>();

		private IFieldIdentifierMappingsCollection _inputFieldIdentifierMappings = new InputFieldIdentifierMappingsCollection();

		private IDictionary<CultureInfo, ILegacyLanguageResources> _languageResources = new Dictionary<CultureInfo, ILegacyLanguageResources>();

		private static bool? _useLegacyHashingByDefault;

		private static object _locker = new object();

		private const string _CreateStrictHashingFileBasedTMsKey = "CreateStrictHashingFileBasedTMs";

		internal static bool UseLegacyHashingByDefault
		{
			get
			{
				lock (_locker)
				{
					if (_useLegacyHashingByDefault.HasValue)
					{
						return _useLegacyHashingByDefault.Value;
					}
					_useLegacyHashingByDefault = false;
					string text = ConfigurationManager.AppSettings["CreateStrictHashingFileBasedTMs"];
					if (text != null && bool.TryParse(text, out bool result))
					{
						_useLegacyHashingByDefault = !result;
					}
					return _useLegacyHashingByDefault.Value;
				}
			}
		}

		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Copyright
		{
			get;
			set;
		}

		public DateTime? ExpirationDate
		{
			get;
			set;
		}

		public ICollection<LanguagePair> LanguageDirections => _languageDirections;

		public FuzzyIndexes FuzzyIndexes
		{
			get;
			set;
		}

		public BuiltinRecognizers Recognizers
		{
			get;
			set;
		}

		public TokenizerFlags TokenizerFlags
		{
			get;
			set;
		}

		public WordCountFlags WordCountFlags
		{
			get;
			set;
		}

		public FGASupport FGASupport
		{
			get;
			set;
		}

		public IList<FieldDefinition> Fields
		{
			get
			{
				return _fields;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_fields = value;
			}
		}

		public IFieldIdentifierMappingsCollection InputFieldIdentifierMappings
		{
			get
			{
				return _inputFieldIdentifierMappings;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_inputFieldIdentifierMappings = value;
			}
		}

		public IDictionary<CultureInfo, ILegacyLanguageResources> LanguageResources
		{
			get
			{
				return _languageResources;
			}
			set
			{
				_languageResources = value;
			}
		}

		public TextContextMatchType TextContextMatchType
		{
			get;
			set;
		}

		public bool UsesIdContextMatch
		{
			get;
			set;
		}

		public bool UsesLegacyHashes
		{
			get;
			set;
		}

		public TranslationMemorySetupOptions()
		{
			TextContextMatchType = TextContextMatchType.PrecedingSourceAndTarget;
			UsesLegacyHashes = UseLegacyHashingByDefault;
		}
	}
}
