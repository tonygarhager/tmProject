using Sdl.Core.Api.DataAccess;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[Entity(Schema = "etm", Table = "FieldGroupTemplate", PrimaryKey = "FieldGroupTemplateId")]
	public class FieldGroupTemplateEntity : ResourceBaseEntity
	{
		private string _name;

		private string _description;

		private bool? _isTmSpecific;

		private EntityCollection<FieldEntity> _fields = new EntityCollection<FieldEntity>();

		private EntityCollection<TranslationMemoryEntity> _translationMemories = new EntityCollection<TranslationMemoryEntity>();

		private Guid? _currentFieldApplyWorkItemUniqueId;

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
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "FieldGroupTemplateId")]
		public EntityCollection<FieldEntity> Fields
		{
			get
			{
				return _fields;
			}
			set
			{
				_fields = value;
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "FieldGroupTemplateId")]
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
		[IgnoreEntityMember]
		public ScheduledOperationEntity CurrentFieldApplyOperation
		{
			get;
			set;
		}

		public FieldGroupTemplateEntity Clone()
		{
			FieldGroupTemplateEntity fieldGroupTemplateEntity = MemberwiseClone() as FieldGroupTemplateEntity;
			fieldGroupTemplateEntity.Fields = new EntityCollection<FieldEntity>();
			foreach (FieldEntity field in Fields)
			{
				fieldGroupTemplateEntity.Fields.Add(field.Clone(fieldGroupTemplateEntity));
			}
			return fieldGroupTemplateEntity;
		}
	}
}
