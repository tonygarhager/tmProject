using Sdl.Core.Api.DataAccess;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[Entity(Schema = "etm", Table = "LanguageResourceTemplate", PrimaryKey = "LanguageResourceTemplateId")]
	public class LanguageResourceTemplateEntity : ResourceBaseEntity
	{
		private string _name;

		private string _description;

		private bool? _isTmSpecific;

		private EntityCollection<LanguageResourceEntity> _languageResources = new EntityCollection<LanguageResourceEntity>();

		private string _recognizers;

		private string _wordCountFlags;

		private string _tokenizerFlags;

		private EntityCollection<TranslationMemoryEntity> _translationMemories = new EntityCollection<TranslationMemoryEntity>();

		private Guid? _currentLangResApplyWorkItemUniqueId;

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

		[EntityColumn]
		[DataMember]
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

		[DataMember]
		[EntityColumn]
		public bool? IsTmSpecific
		{
			get
			{
				return _isTmSpecific;
			}
			set
			{
				if (_isTmSpecific != value)
				{
					_isTmSpecific = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "LanguageResourceTemplateId")]
		public EntityCollection<LanguageResourceEntity> LanguageResources
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

		[EntityColumn]
		[DataMember]
		public string Recognizers
		{
			get
			{
				return _recognizers;
			}
			set
			{
				if (!(_recognizers == value))
				{
					_recognizers = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string WordCountFlags
		{
			get
			{
				return _wordCountFlags;
			}
			set
			{
				if (!(_wordCountFlags == value))
				{
					_wordCountFlags = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string TokenizerFlags
		{
			get
			{
				return _tokenizerFlags;
			}
			set
			{
				if (!(_tokenizerFlags == value))
				{
					_tokenizerFlags = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "LanguageResourceTemplateId")]
		public EntityCollection<TranslationMemoryEntity> TranslationMemories
		{
			get
			{
				return _translationMemories;
			}
			set
			{
				_translationMemories = value;
			}
		}

		[DataMember]
		[EntityColumn]
		public Guid? CurrentLangResApplyWorkItemUniqueId
		{
			get
			{
				return _currentLangResApplyWorkItemUniqueId;
			}
			set
			{
				if (!(_currentLangResApplyWorkItemUniqueId == value))
				{
					_currentLangResApplyWorkItemUniqueId = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[IgnoreEntityMember]
		public ScheduledOperationEntity CurrentLangResApplyOperation
		{
			get;
			set;
		}

		public LanguageResourceTemplateEntity Clone()
		{
			LanguageResourceTemplateEntity languageResourceTemplateEntity = MemberwiseClone() as LanguageResourceTemplateEntity;
			languageResourceTemplateEntity.LanguageResources = new EntityCollection<LanguageResourceEntity>();
			foreach (LanguageResourceEntity languageResource in LanguageResources)
			{
				LanguageResourceEntity languageResourceEntity = languageResource.Clone();
				languageResourceEntity.LanguageResourceTemplate = new EntityReference<LanguageResourceTemplateEntity>(this);
				languageResourceTemplateEntity.LanguageResources.Add(languageResourceEntity);
			}
			return languageResourceTemplateEntity;
		}
	}
}
