using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class TranslationMemory : DbObject
	{
		private string _name;

		private LanguagePair _languageDirection;

		private DateTime _creationDate;

		private DateTime? _expirationDate;

		private DateTime? _lastRecomputeDate;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public LanguagePair LanguageDirection
		{
			get
			{
				return _languageDirection ?? (_languageDirection = new LanguagePair());
			}
			set
			{
				_languageDirection = value;
			}
		}

		public string Copyright
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string CreationUser
		{
			get;
			set;
		}

		public DateTime CreationDate
		{
			get
			{
				return _creationDate;
			}
			set
			{
				_creationDate = DateTimeUtilities.Normalize(value);
			}
		}

		public DateTime? ExpirationDate
		{
			get
			{
				return _expirationDate;
			}
			set
			{
				_expirationDate = (value.HasValue ? DateTimeUtilities.Normalize(value.Value) : DateTimeUtilities.Normalize(DateTime.MaxValue));
			}
		}

		public FuzzyIndexes FuzzyIndexes
		{
			get;
			set;
		}

		public DateTime? LastRecomputeDate
		{
			get
			{
				return _lastRecomputeDate;
			}
			set
			{
				_lastRecomputeDate = (value.HasValue ? DateTimeUtilities.Normalize(value.Value) : DateTimeUtilities.Normalize(DateTime.MaxValue));
			}
		}

		public int? LastRecomputeSize
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

		public int DataVersion
		{
			get;
			set;
		}

		public TextContextMatchType TextContextMatchType
		{
			get;
			set;
		}

		public bool CanReportReindexRequired
		{
			get;
			set;
		}

		public bool IdContextMatch
		{
			get;
			set;
		}

		public bool ExactSearchOnly => FuzzyIndexes == (FuzzyIndexes)0;

		public bool EnableFullTextSearch => false;

		public TranslationMemory(Guid guid, string name, string creationUser, DateTime creationDate)
			: base(0, guid)
		{
			Name = name;
			CreationUser = creationUser;
			_creationDate = creationDate;
			_expirationDate = null;
			FuzzyIndexes = (FuzzyIndexes)0;
		}

		public TranslationMemory(TranslationMemorySetup setup)
			: base(setup.ResourceId.Id, Guid.NewGuid())
		{
			Name = setup.Name;
			LanguageDirection = setup.LanguageDirection;
			CreationUser = setup.CreationUser;
			_creationDate = setup.CreationDate;
			Description = setup.Description;
			Copyright = setup.Copyright;
			_expirationDate = setup.ExpirationDate;
			FuzzyIndexes = setup.FuzzyIndexes;
			_lastRecomputeDate = setup.LastRecomputeDate;
			LastRecomputeSize = setup.LastRecomputeSize;
			Recognizers = setup.Recognizers;
			TokenizerFlags = setup.TokenizerFlags;
			WordCountFlags = setup.WordCountFlags;
			FGASupport = setup.FGASupport;
			DataVersion = ((!setup.UsesLegacyHashes) ? 1 : 0);
			TextContextMatchType = setup.TextContextMatchType;
			CanReportReindexRequired = setup.CanReportReindexRequired;
			IdContextMatch = setup.IdContextMatch;
		}

		private bool DatesInUtc()
		{
			if (_creationDate.Kind == DateTimeKind.Utc && (!_expirationDate.HasValue || _expirationDate.Value.Kind == DateTimeKind.Utc))
			{
				if (_lastRecomputeDate.HasValue)
				{
					return _lastRecomputeDate.Value.Kind == DateTimeKind.Utc;
				}
				return true;
			}
			return false;
		}

		internal TranslationMemory(int id, Guid guid, string name, string srcLang, string trgLang, BuiltinRecognizers recognizers, string creationUser, DateTime creationDate, string copyright, string description, DateTime? expirationDate, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags)
			: this(guid, name, creationUser, creationDate)
		{
			base.Id = id;
			LanguageDirection.SourceCultureName = srcLang;
			LanguageDirection.TargetCultureName = trgLang;
			Copyright = copyright;
			Description = description;
			_expirationDate = expirationDate;
			Recognizers = recognizers;
			TokenizerFlags = tokenizerFlags;
			WordCountFlags = wordCountFlags;
		}
	}
}
