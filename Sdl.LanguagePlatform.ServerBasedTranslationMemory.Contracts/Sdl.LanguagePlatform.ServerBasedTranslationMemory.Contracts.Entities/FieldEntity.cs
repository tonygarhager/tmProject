using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "Field")]
	[Entity(Schema = "etm", Table = "Field", PrimaryKey = "FieldId")]
	public class FieldEntity : Entity, IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private Guid? _uniqueId;

		private string _name;

		private FieldValueType? _valueType;

		private EntityReference<FieldGroupTemplateEntity> _fieldGroupTemplate = new EntityReference<FieldGroupTemplateEntity>();

		private EntityCollection<PicklistItemEntity> _pickListItems = new EntityCollection<PicklistItemEntity>();

		[IgnoreEntityMember]
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return _extensionDataObject;
			}
			set
			{
				_extensionDataObject = value;
			}
		}

		[EntityColumn]
		[DataMember]
		public Guid? UniqueId
		{
			get
			{
				return _uniqueId;
			}
			set
			{
				if (!(_uniqueId == value))
				{
					_uniqueId = value;
					MarkAsDirty();
				}
			}
		}

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

		[EntityColumn(ColumnName = "FieldValueTypeId")]
		[DataMember]
		public FieldValueType? ValueType
		{
			get
			{
				return _valueType;
			}
			set
			{
				if (_valueType != value)
				{
					_valueType = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "FieldGroupTemplateId")]
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

		[DataMember]
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "FieldId")]
		public EntityCollection<PicklistItemEntity> PickListItems
		{
			get
			{
				return _pickListItems;
			}
			set
			{
				_pickListItems = value;
			}
		}

		public FieldEntity Clone(FieldGroupTemplateEntity newParent)
		{
			FieldEntity fieldEntity = MemberwiseClone() as FieldEntity;
			fieldEntity.PickListItems = new EntityCollection<PicklistItemEntity>();
			foreach (PicklistItemEntity pickListItem in PickListItems)
			{
				fieldEntity.PickListItems.Add(pickListItem.Clone(fieldEntity));
			}
			fieldEntity.FieldGroupTemplate = new EntityReference<FieldGroupTemplateEntity>(newParent);
			return fieldEntity;
		}
	}
}
