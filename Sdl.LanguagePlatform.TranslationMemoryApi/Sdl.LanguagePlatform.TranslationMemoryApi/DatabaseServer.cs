using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class DatabaseServer : IEditableObject, INotifyPropertyChanged, IEquatable<DatabaseServer>, IPermissionCheck
	{
		private DatabaseServerEntity _entity;

		private DatabaseServerEntity _backupEntity;

		internal ReadOnlyCollection<TranslationMemoryContainer> _lazyContainers;

		internal DatabaseServerEntity Entity
		{
			get
			{
				return _entity;
			}
			private set
			{
				_entity = value;
				OnPropertyChanged(null);
			}
		}

		public TranslationProviderServer TranslationProviderServer
		{
			get;
			private set;
		}

		public Guid Id
		{
			get
			{
				VerifyNotDeleted();
				return _entity.UniqueId.Value;
			}
		}

		[Required(ErrorMessage = "Required Field")]
		[RegularExpression("^[\\w\\s-']*", ErrorMessage = "Characters are not allowed.")]
		[StringLength(50, ErrorMessage = "Name too long!")]
		public string Name
		{
			get
			{
				VerifyNotDeleted();
				return _entity.Name;
			}
			set
			{
				VerifyNotDeleted();
				PropertyValueValidator.Validate(this, value);
				_entity.Name = value;
				OnPropertyChanged("Name");
			}
		}

		[StringLength(255, ErrorMessage = "Description too long!")]
		public string Description
		{
			get
			{
				VerifyNotDeleted();
				return _entity.Description;
			}
			set
			{
				VerifyNotDeleted();
				PropertyValueValidator.Validate(this, value);
				_entity.Description = value;
				OnPropertyChanged("Description");
			}
		}

		[Required]
		[StringLength(255, ErrorMessage = "Server name too long!")]
		public string ServerName
		{
			get
			{
				VerifyNotDeleted();
				return _entity.ServerName;
			}
			set
			{
				VerifyNotDeleted();
				PropertyValueValidator.Validate(this, value);
				if (string.IsNullOrEmpty(value))
				{
					throw new NullPropertyException("Can't set to null or an empty string.");
				}
				if (IsNewObject)
				{
					_entity.ServerName = value;
					OnPropertyChanged("ServerName");
					return;
				}
				throw new InvalidOperationException("ServerName value cannot be re-assigned once it has been initialised.");
			}
		}

		[Required(ErrorMessage = "Required Field")]
		public DatabaseServerType ServerType
		{
			get
			{
				VerifyNotDeleted();
				return (DatabaseServerType)_entity.Type.Value;
			}
			set
			{
				VerifyNotDeleted();
				if (!IsNewObject)
				{
					throw new InvalidOperationException("ServerType cannot be re-assigned once it has been initialised");
				}
				_entity.Type = (Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities.DatabaseServerType)value;
				OnPropertyChanged("ServerType");
			}
		}

		public DatabaseServerAuthenticationType AuthenticationType
		{
			get
			{
				VerifyNotDeleted();
				return (DatabaseServerAuthenticationType)_entity.AuthenticationType.Value;
			}
			set
			{
				if (_entity == null)
				{
					throw new ObjectDeletedException("Database Server must have been deleted, contents are null.");
				}
				_entity.AuthenticationType = (Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities.DatabaseServerAuthenticationType)value;
				OnPropertyChanged("AuthenticationType");
			}
		}

		[Required(ErrorMessage = "Required Field")]
		[StringLength(50, ErrorMessage = "User Name too long!")]
		public string UserName
		{
			get
			{
				VerifyNotDeleted();
				return _entity.UserName;
			}
			set
			{
				VerifyNotDeleted();
				if (_entity.UserName != null && value != null && AuthenticationType == DatabaseServerAuthenticationType.Database)
				{
					PropertyValueValidator.Validate(this, value);
				}
				_entity.UserName = value;
				OnPropertyChanged("UserName");
			}
		}

		[Required(ErrorMessage = "Required Field")]
		[StringLength(50, ErrorMessage = "Password too long!")]
		public string Password
		{
			get
			{
				VerifyNotDeleted();
				return _entity.Password;
			}
			set
			{
				VerifyNotDeleted();
				if (_entity.Password != null && value != null && AuthenticationType == DatabaseServerAuthenticationType.Database)
				{
					PropertyValueValidator.Validate(this, value);
				}
				_entity.Password = value;
				OnPropertyChanged("Password");
			}
		}

		public ReadOnlyCollection<TranslationMemoryContainer> Containers
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyContainers == null)
				{
					ContainerEntity[] containersByDatabaseServerId = null;// TranslationProviderServer.Service.GetContainersByDatabaseServerId(_entity.Id, new string[0]);
                    List<TranslationMemoryContainer> list = new List<TranslationMemoryContainer>();
					ClientObjectBuilder clientObjectBuilder = new ClientObjectBuilder(TranslationProviderServer);
					clientObjectBuilder[clientObjectBuilder.CreateKey(Entity)] = this;
					ContainerEntity[] array = containersByDatabaseServerId;
					foreach (ContainerEntity entity in array)
					{
						list.Add(TranslationMemoryContainer.BuildTranslationMemoryContainer(clientObjectBuilder, entity));
					}
					_lazyContainers = list.AsReadOnly();
				}
				return _lazyContainers;
			}
		}

		public string ParentResourceGroupPath
		{
			get
			{
				return _entity.ParentResourceGroupPath;
			}
			set
			{
				_entity.ParentResourceGroupPath = value;
			}
		}

		public string ParentResourceGroupName
		{
			get
			{
				return _entity.ParentResourceGroupName;
			}
			set
			{
				_entity.ParentResourceGroupName = value;
			}
		}

		public string ParentResourceGroupDescription
		{
			get
			{
				return _entity.ParentResourceGroupDescription;
			}
			set
			{
				_entity.ParentResourceGroupDescription = value;
			}
		}

		public string[] LinkedResourceGroupPaths
		{
			get
			{
				return _entity.LinkedResourceGroupPaths;
			}
			set
			{
				_entity.LinkedResourceGroupPaths = value;
			}
		}

		public bool IsDirty
		{
			get
			{
				if (_entity == null)
				{
					return false;
				}
				return _entity.IsDirty;
			}
		}

		public bool IsDeleted => _entity == null;

		public bool IsNewObject
		{
			get
			{
				if (_entity == null)
				{
					return false;
				}
				if (_entity.Id != null)
				{
					return _entity.Id.Value == null;
				}
				return true;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		internal static DatabaseServer BuildDatabaseServer(ClientObjectBuilder builder, DatabaseServerEntity entity)
		{
			ClientObjectKey key = builder.CreateKey(entity);
			DatabaseServer databaseServer = builder[key] as DatabaseServer;
			if (databaseServer != null)
			{
				return databaseServer;
			}
			databaseServer = (DatabaseServer)(builder[key] = new DatabaseServer(builder.Server, entity));
			if (entity.Containers.IsLoaded)
			{
				List<TranslationMemoryContainer> list = new List<TranslationMemoryContainer>();
				foreach (ContainerEntity container in entity.Containers)
				{
					TranslationMemoryContainer item = TranslationMemoryContainer.BuildTranslationMemoryContainer(builder, container);
					list.Add(item);
				}
				databaseServer._lazyContainers = list.AsReadOnly();
				entity.Containers = new EntityCollection<ContainerEntity>();
			}
			return databaseServer;
		}

		public DatabaseServer(TranslationProviderServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			TranslationProviderServer = server;
			_entity = new DatabaseServerEntity
			{
				Type = Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities.DatabaseServerType.SqlServer,
				AuthenticationType = Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities.DatabaseServerAuthenticationType.Windows,
				UniqueId = Guid.NewGuid()
			};
			_lazyContainers = new ReadOnlyCollection<TranslationMemoryContainer>(new List<TranslationMemoryContainer>());
		}

		internal DatabaseServer(TranslationProviderServer server, DatabaseServerEntity entity)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.Id == null || entity.Id.Value == null)
			{
				throw new ArgumentException(StringResources.DatabaseServer_EntityHasNoId, "entity");
			}
			TranslationProviderServer = server;
			_entity = entity;
		}

		public void Delete(bool deleteContainerDatabases)
		{
			VerifyNotDeleted();
			VerifyExistingObject("Database Server has to be saved before it can be deleted.");
			VerifyPermission("tmdbserver.delete");
			//TranslationProviderServer.Service.DeleteDatabaseServer(_entity.Id, deleteContainerDatabases);
			_entity = null;
		}

		internal void AddContainer(TranslationMemoryContainer container)
		{
			List<TranslationMemoryContainer> list = new List<TranslationMemoryContainer>(Containers)
			{
				container
			};
			_lazyContainers = list.AsReadOnly();
		}

		internal bool DeleteContainer(TranslationMemoryContainer container)
		{
			List<TranslationMemoryContainer> list = new List<TranslationMemoryContainer>(Containers);
			bool result = list.Remove(container);
			_lazyContainers = list.AsReadOnly();
			return result;
		}

		public void Save()
		{
			VerifyNotDeleted();
			if (IsNewObject)
			{
				//_entity = TranslationProviderServer.Service.CreateDatabaseServer(_entity, ParentResourceGroupPath);
				return;
			}
			VerifyPermission("tmdbserver.edit");
			//_entity = TranslationProviderServer.Service.UpdateDatabaseServer(_entity);
		}

		public bool HasPermission(string permission)
		{
			VerifyNotDeleted();
			if (_entity.Permissions != null)
			{
				return _entity.Permissions.HasPermission(permission);
			}
			return true;
		}

		private void VerifyExistingObject(string message)
		{
			if (IsNewObject)
			{
				throw new ObjectDeletedException(message);
			}
		}

		private void VerifyNotDeleted(string message)
		{
			if (IsDeleted)
			{
				throw new ObjectDeletedException(message);
			}
		}

		private void VerifyNotDeleted()
		{
			VerifyNotDeleted("The database server has been deleted.");
		}

		private void VerifyPermission(string permission)
		{
			if (!HasPermission(permission))
			{
				throw new SecurityException($"Current user does not have {permission} permission on this database server.");
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		void IEditableObject.BeginEdit()
		{
			if (_entity != null && _backupEntity == null)
			{
				_backupEntity = new DatabaseServerEntity();
				_backupEntity.Id = _entity.Id;
				_backupEntity.Name = _entity.Name;
				_backupEntity.Description = _entity.Description;
				_backupEntity.Password = _entity.Password;
				_backupEntity.ServerName = _entity.ServerName;
				_backupEntity.Type = _entity.Type;
				_backupEntity.UniqueId = _entity.UniqueId;
				_backupEntity.UserName = _entity.UserName;
				_backupEntity.AuthenticationType = _entity.AuthenticationType;
				_backupEntity.MarkAsClean();
			}
		}

		void IEditableObject.CancelEdit()
		{
			if (_entity != null && _backupEntity != null)
			{
				Entity = _backupEntity;
				_backupEntity = null;
			}
		}

		void IEditableObject.EndEdit()
		{
			_backupEntity = null;
		}

		public bool Equals(DatabaseServer other)
		{
			return Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			DatabaseServer databaseServer = (DatabaseServer)obj;
			if (databaseServer != null)
			{
				return Equals(databaseServer);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
