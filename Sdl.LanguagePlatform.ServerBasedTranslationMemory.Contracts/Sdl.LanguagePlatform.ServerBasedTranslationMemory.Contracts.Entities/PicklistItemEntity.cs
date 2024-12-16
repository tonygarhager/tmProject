using Sdl.Core.Api.DataAccess;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "PicklistItem")]
	[Entity(Schema = "etm", Table = "PickListItem", PrimaryKey = "PickListItemId")]
	public class PicklistItemEntity : Entity, IExtensibleDataObject
	{
		private ExtensionDataObject extensionDataObject;

		private Guid? _uniqueId;

		private string _name;

		private EntityReference<FieldEntity> field = new EntityReference<FieldEntity>();

		[IgnoreEntityMember]
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return extensionDataObject;
			}
			set
			{
				extensionDataObject = value;
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

		[DataMember]
		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "FieldId")]
		public EntityReference<FieldEntity> Field
		{
			get
			{
				return field;
			}
			set
			{
				field = value;
			}
		}

		public PicklistItemEntity Clone(FieldEntity newParent)
		{
			PicklistItemEntity picklistItemEntity = MemberwiseClone() as PicklistItemEntity;
			if (picklistItemEntity == null)
			{
				return null;
			}
			picklistItemEntity.Field = new EntityReference<FieldEntity>(newParent);
			return picklistItemEntity;
		}
	}
}
