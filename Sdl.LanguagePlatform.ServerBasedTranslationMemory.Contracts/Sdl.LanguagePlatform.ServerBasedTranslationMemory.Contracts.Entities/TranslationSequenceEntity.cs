using Sdl.Core.Api.DataAccess;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "TranslationSequence")]
	[Entity(Schema = "etm", Table = "TranslationSequence", PrimaryKey = "TranslationSequenceId")]
	public class TranslationSequenceEntity : Entity, IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private Guid? _uniqueId;

		private string _name;

		private string _description;

		private EntityCollection<TranslationSequenceItemEntity> _translationSequenceItems = new EntityCollection<TranslationSequenceItemEntity>();

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
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "TranslationSequenceId")]
		public EntityCollection<TranslationSequenceItemEntity> TranslationSequenceItems
		{
			get
			{
				return _translationSequenceItems;
			}
			set
			{
				_translationSequenceItems = value;
			}
		}
	}
}
