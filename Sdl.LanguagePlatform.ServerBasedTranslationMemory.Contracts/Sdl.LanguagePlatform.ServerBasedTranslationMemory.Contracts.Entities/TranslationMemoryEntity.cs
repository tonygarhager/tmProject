using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "TranslationMemory")]
	[Entity(Schema = "etm", Table = "TranslationMemory", PrimaryKey = "TranslationMemoryId")]
	public class TranslationMemoryEntity : ResourceBaseEntity
	{
		private string _name;

		private string _description;

		private DateTime? _creationDate;

		private string _creationUser;

		private string _copyright;

		private DateTime? _expirationDate;

		private BuiltinRecognizers? _recognizers;

		private FuzzyIndexes? _fuzzyIndexes;

		private TokenizerFlags? _tokenizerFlags;

		private WordCountFlags? _wordCountFlags;

		private EntityReference<FieldGroupTemplateEntity> _fieldGroupTemplate = new EntityReference<FieldGroupTemplateEntity>();

		private EntityReference<LanguageResourceTemplateEntity> _languageResourceTemplate = new EntityReference<LanguageResourceTemplateEntity>();

		private Guid? _currentRecomputeStatisticsWorkItemUniqueId;

		private Guid? _currentReindexWorkItemUniqueId;

		private Guid? _currentFieldApplyWorkItemUniqueId;

		private bool? _fieldUpdatePending;

		private Guid? _currentLanguageResourceApplyWorkItemUniqueId;

		private bool? _languageResourceUpdatePending;

		private DateTime? _lastRecomputeDate;

		private int? _lastRecomputeSize;

		private int? _minScoreIncrease;

		private int? _minSearchVectorLengthSourceWordIndex;

		private int? _minSearchVectorLengthTargetWordIndex;

		private int? _minSearchVectorLengthSourceCharIndex;

		private int? _minSearchVectorLengthTargetCharIndex;

		private bool? _isProjectTranslationMemory;

		private EntityReference<ContainerEntity> _container = new EntityReference<ContainerEntity>();

		private EntityCollection<LanguageDirectionEntity> _languageDirections = new EntityCollection<LanguageDirectionEntity>();

		private bool? _usesLegacyHashes;

		private bool? _usesIdContextMatching;

		private TextContextMatchType? _textContextMatch;

		[EntityColumn]
		[DataMember]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (!(_name == value))
				{
					_name = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				if (!(_description == value))
				{
					_description = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public DateTime? CreationDate
		{
			get
			{
				return _creationDate;
			}
			set
			{
				if (!(_creationDate == value))
				{
					if (value.HasValue)
					{
						_creationDate = DateTimeUtilities.Normalize(value.Value);
					}
					else
					{
						_creationDate = null;
					}
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string CreationUser
		{
			get
			{
				return _creationUser;
			}
			set
			{
				if (!(_creationUser == value))
				{
					_creationUser = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string Copyright
		{
			get
			{
				return _copyright;
			}
			set
			{
				if (!(_copyright == value))
				{
					_copyright = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public DateTime? ExpirationDate
		{
			get
			{
				return _expirationDate;
			}
			set
			{
				if (!(_expirationDate == value))
				{
					_expirationDate = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public BuiltinRecognizers? Recognizers
		{
			get
			{
				return _recognizers;
			}
			set
			{
				if (_recognizers != value)
				{
					_recognizers = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public FuzzyIndexes? FuzzyIndexes
		{
			get
			{
				return _fuzzyIndexes;
			}
			set
			{
				if (_fuzzyIndexes != value)
				{
					_fuzzyIndexes = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public TokenizerFlags? TokenizerFlags
		{
			get
			{
				return _tokenizerFlags;
			}
			set
			{
				if (_tokenizerFlags != value)
				{
					_tokenizerFlags = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public WordCountFlags? WordCountFlags
		{
			get
			{
				return _wordCountFlags;
			}
			set
			{
				if (_wordCountFlags != value)
				{
					_wordCountFlags = value;
					MarkAsDirty();
				}
			}
		}

		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "FieldGroupTemplateId")]
		[DataMember]
		public EntityReference<FieldGroupTemplateEntity> FieldGroupTemplate
		{
			get
			{
				return _fieldGroupTemplate;
			}
			set
			{
				if (_fieldGroupTemplate != value)
				{
					_fieldGroupTemplate = value;
					MarkAsDirty();
				}
			}
		}

		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "LanguageResourceTemplateId")]
		[DataMember]
		public EntityReference<LanguageResourceTemplateEntity> LanguageResourceTemplate
		{
			get
			{
				return _languageResourceTemplate;
			}
			set
			{
				if (_languageResourceTemplate != value)
				{
					_languageResourceTemplate = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public Guid? CurrentRecomputeStatisticsWorkItemUniqueId
		{
			get
			{
				return _currentRecomputeStatisticsWorkItemUniqueId;
			}
			set
			{
				if (!(_currentRecomputeStatisticsWorkItemUniqueId == value))
				{
					_currentRecomputeStatisticsWorkItemUniqueId = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[IgnoreEntityMember]
		public ScheduledOperationEntity CurrentRecomputeStatisticsOperation
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn]
		public Guid? CurrentReindexWorkItemUniqueId
		{
			get
			{
				return _currentReindexWorkItemUniqueId;
			}
			set
			{
				if (!(_currentReindexWorkItemUniqueId == value))
				{
					_currentReindexWorkItemUniqueId = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[IgnoreEntityMember]
		public ScheduledOperationEntity CurrentReindexOperation
		{
			get;
			set;
		}

		[DataMember]
		[EntityColumn]
		public Guid? CurrentFieldApplyWorkItemUniqueId
		{
			get
			{
				return _currentFieldApplyWorkItemUniqueId;
			}
			set
			{
				if (!(_currentFieldApplyWorkItemUniqueId == value))
				{
					_currentFieldApplyWorkItemUniqueId = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public bool? FieldUpdatePending
		{
			get
			{
				return _fieldUpdatePending;
			}
			set
			{
				if (_fieldUpdatePending != value)
				{
					_fieldUpdatePending = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn(ColumnName = "CurrentLangResApplyWorkItemUniqueId")]
		public Guid? CurrentLanguageResourceApplyWorkItemUniqueId
		{
			get
			{
				return _currentLanguageResourceApplyWorkItemUniqueId;
			}
			set
			{
				if (!(_currentLanguageResourceApplyWorkItemUniqueId == value))
				{
					_currentLanguageResourceApplyWorkItemUniqueId = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn(ColumnName = "LangResUpdatePending")]
		public bool? LanguageResourceUpdatePending
		{
			get
			{
				return _languageResourceUpdatePending;
			}
			set
			{
				if (_languageResourceUpdatePending != value)
				{
					_languageResourceUpdatePending = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public DateTime? LastRecomputeDate
		{
			get
			{
				return _lastRecomputeDate;
			}
			set
			{
				if (!(_lastRecomputeDate == value))
				{
					_lastRecomputeDate = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public int? LastRecomputeSize
		{
			get
			{
				return _lastRecomputeSize;
			}
			set
			{
				if (_lastRecomputeSize != value)
				{
					_lastRecomputeSize = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public int? MinScoreIncrease
		{
			get
			{
				return _minScoreIncrease;
			}
			set
			{
				if (_minScoreIncrease != value)
				{
					_minScoreIncrease = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public int? MinSearchVectorLengthSourceWordIndex
		{
			get
			{
				return _minSearchVectorLengthSourceWordIndex;
			}
			set
			{
				if (_minSearchVectorLengthSourceWordIndex != value)
				{
					_minSearchVectorLengthSourceWordIndex = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public int? MinSearchVectorLengthTargetWordIndex
		{
			get
			{
				return _minSearchVectorLengthTargetWordIndex;
			}
			set
			{
				if (_minSearchVectorLengthTargetWordIndex != value)
				{
					_minSearchVectorLengthTargetWordIndex = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public int? MinSearchVectorLengthSourceCharIndex
		{
			get
			{
				return _minSearchVectorLengthSourceCharIndex;
			}
			set
			{
				if (_minSearchVectorLengthSourceCharIndex != value)
				{
					_minSearchVectorLengthSourceCharIndex = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public int? MinSearchVectorLengthTargetCharIndex
		{
			get
			{
				return _minSearchVectorLengthTargetCharIndex;
			}
			set
			{
				if (_minSearchVectorLengthTargetCharIndex != value)
				{
					_minSearchVectorLengthTargetCharIndex = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public bool? IsProjectTranslationMemory
		{
			get
			{
				return _isProjectTranslationMemory;
			}
			set
			{
				if (_isProjectTranslationMemory != value)
				{
					_isProjectTranslationMemory = value;
					MarkAsDirty();
				}
			}
		}

		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "ContainerId")]
		[DataMember]
		public EntityReference<ContainerEntity> Container
		{
			get
			{
				return _container;
			}
			set
			{
				_container = value;
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "TranslationMemoryId")]
		public EntityCollection<LanguageDirectionEntity> LanguageDirections
		{
			get
			{
				return _languageDirections;
			}
			set
			{
				_languageDirections = value;
			}
		}

		[DataMember]
		[EntityColumn]
		public bool? UsesLegacyHashes
		{
			get
			{
				return _usesLegacyHashes;
			}
			set
			{
				if (_usesLegacyHashes != value)
				{
					_usesLegacyHashes = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public bool? UsesIdContextMatching
		{
			get
			{
				return _usesIdContextMatching;
			}
			set
			{
				if (_usesIdContextMatching != value)
				{
					_usesIdContextMatching = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[EntityColumn]
		public TextContextMatchType? TextContextMatchType
		{
			get
			{
				return _textContextMatch;
			}
			set
			{
				if (_textContextMatch != value)
				{
					_textContextMatch = value;
					MarkAsDirty();
				}
			}
		}

		public TranslationMemoryEntity Clone()
		{
			TranslationMemoryEntity translationMemoryEntity = MemberwiseClone() as TranslationMemoryEntity;
			if (translationMemoryEntity == null)
			{
				return translationMemoryEntity;
			}
			translationMemoryEntity.FieldGroupTemplate = (FieldGroupTemplate.IsLoaded ? new EntityReference<FieldGroupTemplateEntity>(FieldGroupTemplate.Entity.Clone()) : new EntityReference<FieldGroupTemplateEntity>(FieldGroupTemplate.ForeignKey));
			translationMemoryEntity.LanguageResourceTemplate = (LanguageResourceTemplate.IsLoaded ? new EntityReference<LanguageResourceTemplateEntity>(LanguageResourceTemplate.Entity.Clone()) : new EntityReference<LanguageResourceTemplateEntity>(LanguageResourceTemplate.ForeignKey));
			translationMemoryEntity.LanguageDirections = new EntityCollection<LanguageDirectionEntity>();
			foreach (LanguageDirectionEntity languageDirection in LanguageDirections)
			{
				translationMemoryEntity.LanguageDirections.Add(languageDirection.Clone(translationMemoryEntity));
			}
			return translationMemoryEntity;
		}

		public FuzzyIndexTuningSettings CreateFuzzyIndexTuningSettings()
		{
			return new FuzzyIndexTuningSettings
			{
				MinScoreIncrease = MinScoreIncrease.Value,
				MinSearchVectorLengthSourceCharIndex = MinSearchVectorLengthSourceCharIndex.Value,
				MinSearchVectorLengthSourceWordIndex = MinSearchVectorLengthSourceWordIndex.Value,
				MinSearchVectorLengthTargetCharIndex = MinSearchVectorLengthTargetCharIndex.Value,
				MinSearchVectorLengthTargetWordIndex = MinSearchVectorLengthTargetWordIndex.Value
			};
		}
	}
}
