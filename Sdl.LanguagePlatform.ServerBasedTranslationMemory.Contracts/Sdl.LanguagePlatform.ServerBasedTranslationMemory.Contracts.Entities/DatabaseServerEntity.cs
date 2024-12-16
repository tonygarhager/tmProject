using Sdl.Core.Api.DataAccess;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "DatabaseServer")]
	[Entity(Schema = "etm", Table = "DatabaseServer", PrimaryKey = "DatabaseServerId")]
	public class DatabaseServerEntity : ResourceBaseEntity
	{
		private string _name;

		private string _description;

		private DatabaseServerType? _type;

		private string _serverName;

		private DatabaseServerAuthenticationType? _authenticationType;

		private string _userName;

		private string _password;

		private EntityCollection<ContainerEntity> _containers = new EntityCollection<ContainerEntity>();

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

		[EntityColumn(ColumnName = "DatabaseServerTypeId")]
		[DataMember]
		public DatabaseServerType? Type
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
		public string ServerName
		{
			get
			{
				return _serverName;
			}
			set
			{
				if (!(_serverName == value))
				{
					_serverName = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn(ColumnName = "AuthenticationTypeId")]
		[DataMember]
		public DatabaseServerAuthenticationType? AuthenticationType
		{
			get
			{
				return _authenticationType;
			}
			set
			{
				if (_authenticationType != value)
				{
					_authenticationType = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string UserName
		{
			get
			{
				return _userName;
			}
			set
			{
				if (!(_userName == value))
				{
					_userName = value;
					MarkAsDirty();
				}
			}
		}

		[EntityColumn]
		[DataMember]
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				if (!(_password == value))
				{
					_password = value;
					MarkAsDirty();
				}
			}
		}

		[Relationship(Type = RelationshipType.OneToMany, RelationshipKey = "DatabaseServerId")]
		[DataMember]
		public EntityCollection<ContainerEntity> Containers
		{
			get
			{
				return _containers;
			}
			set
			{
				if (_containers != value)
				{
					_containers = value;
					MarkAsDirty();
				}
			}
		}

		public DatabaseServerEntity ShallowCopy()
		{
			return MemberwiseClone() as DatabaseServerEntity;
		}
	}
}
