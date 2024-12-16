using Sdl.Core.Api.DataAccess;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "Container")]
	[Entity(Schema = "etm", Table = "Container", PrimaryKey = "ContainerId")]
	public class ContainerEntity : ResourceBaseEntity
	{
		private string _name;

		private string _description;

		private string _databaseName;

		private EntityReference<DatabaseServerEntity> _databaseServer = new EntityReference<DatabaseServerEntity>();

		private EntityCollection<TranslationMemoryEntity> _translationMemories = new EntityCollection<TranslationMemoryEntity>();

		[DataMember]
		[EntityColumn]
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
		public string DatabaseName
		{
			get
			{
				return _databaseName;
			}
			set
			{
				if (!(_databaseName == value))
				{
					_databaseName = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.ManyToOne, RelationshipKey = "DatabaseServerId")]
		public EntityReference<DatabaseServerEntity> DatabaseServer
		{
			get
			{
				return _databaseServer;
			}
			set
			{
				if (_databaseServer != value)
				{
					_databaseServer = value;
					MarkAsDirty();
				}
			}
		}

		[DataMember]
		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "ContainerId")]
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
	}
}
