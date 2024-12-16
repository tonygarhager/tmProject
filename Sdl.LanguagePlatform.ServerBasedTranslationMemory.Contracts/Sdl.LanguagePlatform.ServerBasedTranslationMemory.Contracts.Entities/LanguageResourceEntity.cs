using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[Entity(Schema = "etm", Table = "LanguageResource", PrimaryKey = "LanguageResourceId")]
	public class LanguageResourceEntity : Entity
	{
		private Guid? _uniqueId;

		private LanguageResourceType? _type;

		private string _cultureName;

		private byte[] _data;

		private EntityReference<LanguageResourceTemplateEntity> _languageResourceTemplate = new EntityReference<LanguageResourceTemplateEntity>();

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

		[EntityColumn(ColumnName = "LanguageResourceTypeId")]
		[DataMember]
		public LanguageResourceType? Type
		{
			get
			{
				return _type;
			}
			set
			{
				if (_type != value)
				{
					_type = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string CultureName
		{
			get
			{
				return _cultureName;
			}
			set
			{
				if (!(_cultureName == value))
				{
					_cultureName = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public byte[] Data
		{
			get
			{
				return _data;
			}
			set
			{
				if (_data != value)
				{
					_data = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "LanguageResourceTemplateId")]
		public EntityReference<LanguageResourceTemplateEntity> LanguageResourceTemplate
		{
			get
			{
				return _languageResourceTemplate;
			}
			set
			{
				_languageResourceTemplate = value;
			}
		}

		public LanguageResourceEntity Clone()
		{
			return MemberwiseClone() as LanguageResourceEntity;
		}
	}
}
