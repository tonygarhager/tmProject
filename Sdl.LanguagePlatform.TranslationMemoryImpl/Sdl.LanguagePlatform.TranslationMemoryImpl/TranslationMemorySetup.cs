using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[DataContract]
	public class TranslationMemorySetup : PersistentObject, ICloneable
	{
		private DateTime _CreationDate;

		private DateTime? _ExpirationDate;

		private DateTime? _LastRecomputeDate;

		private int? _LastRecomputeSize;

		public static readonly int MaximumTranslationMemoryNameLength = 200;

		public static readonly int MaximumFieldNameLength = 400;

		public static readonly int MaximumTextFieldValueLength = 2000;

		public static readonly int MaximumCopyrightFieldLength = 2000;

		public static readonly int MaximumDescriptionFieldLength = 2000;

		public static readonly int MaximumUserNameLength = 255;

		internal bool NormalizeCharWidths => !UsesLegacyHashes;

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string Description
		{
			get;
			set;
		}

		[DataMember]
		public DateTime CreationDate
		{
			get
			{
				return _CreationDate;
			}
			set
			{
				_CreationDate = DateTimeUtilities.Normalize(value);
			}
		}

		[DataMember]
		public bool ExactSearchOnly
		{
			get;
			set;
		}

		[DataMember]
		public bool IsReadOnly
		{
			get;
			set;
		}

		[DataMember]
		public bool CanReverseLanguageDirection
		{
			get;
			set;
		}

		[DataMember]
		public bool EnableFullTextSearch
		{
			get;
			set;
		}

		[DataMember]
		public string CreationUser
		{
			get;
			set;
		}

		[DataMember]
		public string Copyright
		{
			get;
			set;
		}

		[DataMember]
		public LanguagePair LanguageDirection
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? ExpirationDate
		{
			get
			{
				return _ExpirationDate;
			}
			set
			{
				_ExpirationDate = (value.HasValue ? DateTimeUtilities.Normalize(value.Value) : DateTimeUtilities.Normalize(DateTime.MaxValue));
			}
		}

		[DataMember]
		public FuzzyIndexes FuzzyIndexes
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? LastRecomputeDate
		{
			get
			{
				return _LastRecomputeDate;
			}
			set
			{
				_LastRecomputeDate = (value.HasValue ? DateTimeUtilities.Normalize(value.Value) : DateTimeUtilities.Normalize(DateTime.MinValue));
			}
		}

		[DataMember]
		public int? LastRecomputeSize
		{
			get
			{
				return _LastRecomputeSize;
			}
			set
			{
				_LastRecomputeSize = value;
			}
		}

		[DataMember]
		public BuiltinRecognizers Recognizers
		{
			get;
			set;
		}

		[DataMember]
		public TokenizerFlags TokenizerFlags
		{
			get;
			set;
		}

		[DataMember]
		public WordCountFlags WordCountFlags
		{
			get;
			set;
		}

		[DataMember]
		public FGASupport FGASupport
		{
			get;
			set;
		}

		[DataMember]
		public bool UsesLegacyHashes
		{
			get;
			set;
		}

		[DataMember]
		public TextContextMatchType TextContextMatchType
		{
			get;
			set;
		}

		[DataMember]
		public bool CanReportReindexRequired
		{
			get;
			set;
		}

		[DataMember]
		public bool IdContextMatch
		{
			get;
			set;
		}

		[DataMember]
		public FieldDefinitions FieldDeclarations
		{
			get;
			set;
		}

		public TranslationMemorySetup()
		{
			LanguageDirection = new LanguagePair();
			FieldDeclarations = new FieldDefinitions();
			_ExpirationDate = DateTimeUtilities.Normalize(DateTime.MaxValue);
			FuzzyIndexes = (FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetWordBased);
			ExactSearchOnly = false;
			IsReadOnly = false;
			CanReverseLanguageDirection = false;
			TokenizerFlags = TokenizerFlags.DefaultFlags;
			WordCountFlags = WordCountFlags.BreakOnTag;
			TextContextMatchType = TextContextMatchType.PrecedingSourceAndTarget;
		}

		public TranslationMemorySetup(TranslationMemorySetup other)
		{
			if (other.Name != null)
			{
				Name = other.Name;
			}
			if (other.Description != null)
			{
				Description = other.Description;
			}
			_CreationDate = other._CreationDate;
			if (other.CreationUser != null)
			{
				CreationUser = other.CreationUser;
			}
			if (other.Copyright != null)
			{
				Copyright = other.Copyright;
			}
			LanguageDirection = new LanguagePair();
			LanguageDirection.SourceCulture = other.LanguageDirection.SourceCulture;
			LanguageDirection.TargetCulture = other.LanguageDirection.TargetCulture;
			FuzzyIndexes = other.FuzzyIndexes;
			Recognizers = other.Recognizers;
			TokenizerFlags = other.TokenizerFlags;
			WordCountFlags = other.WordCountFlags;
			FGASupport = other.FGASupport;
			CanReportReindexRequired = other.CanReportReindexRequired;
			UsesLegacyHashes = other.UsesLegacyHashes;
			TextContextMatchType = other.TextContextMatchType;
			if (other._ExpirationDate.HasValue)
			{
				_ExpirationDate = other._ExpirationDate;
			}
			if (other._LastRecomputeDate.HasValue)
			{
				_LastRecomputeDate = other._LastRecomputeDate;
			}
			if (other._LastRecomputeSize.HasValue)
			{
				_LastRecomputeSize = other._LastRecomputeSize;
			}
			ExactSearchOnly = other.ExactSearchOnly;
			IsReadOnly = other.IsReadOnly;
			CanReverseLanguageDirection = other.CanReverseLanguageDirection;
			FieldDeclarations = new FieldDefinitions();
			foreach (Field fieldDeclaration in other.FieldDeclarations)
			{
				FieldDeclarations.Add((Field)fieldDeclaration.Clone());
			}
		}

		public object Clone()
		{
			return new TranslationMemorySetup(this);
		}
	}
}
