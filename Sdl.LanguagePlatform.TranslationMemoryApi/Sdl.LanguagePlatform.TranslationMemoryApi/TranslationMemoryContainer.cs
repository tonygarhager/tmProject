using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class TranslationMemoryContainer : IEditableObject, INotifyPropertyChanged, IEquatable<TranslationMemoryContainer>, IPermissionCheck
	{
		private ContainerEntity _entity;

		private ContainerEntity _backupEntity;

		internal DatabaseServer _lazyDatabaseServer;

		internal ReadOnlyCollection<ServerBasedTranslationMemory> _lazyTranslationMemories;

		internal IList<string> _lazyTranslationMemoryNames;

		private string _userName;

		private string _password;

		internal ContainerEntity Entity
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
		[RegularExpression("[^\\\\/\"<>\\|\\*\\?%]+", ErrorMessage = "The following characters ^ \\ / \\\"  < > | * ? %  are not allowed")]
		[StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
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

		[Required(ErrorMessage = "Required Field")]
		[RegularExpression("^[\\p{L}_][\\w_@#\\$]*$", ErrorMessage = "Only alpha-numeric and underscore characters allowed.")]
		[StringLength(123, ErrorMessage = "Database Name cannot exceed 123 characters.")]
		public string DatabaseName
		{
			get
			{
				VerifyNotDeleted();
				return _entity.DatabaseName;
			}
			set
			{
				VerifyNotDeleted();
				if (DatabaseServer == null || DatabaseServer.ServerType == DatabaseServerType.SqlServer)
				{
					PropertyValueValidator.Validate(this, value);
				}
				_entity.DatabaseName = value;
				OnPropertyChanged("DatabaseName");
			}
		}

		[Required(ErrorMessage = "Required Field")]
		[StringLength(30, ErrorMessage = "User Name too long!")]
		public string UserName
		{
			get
			{
				VerifyNotDeleted();
				if (!string.IsNullOrEmpty(_entity.DatabaseName))
				{
					string[] array = _entity.DatabaseName.Split('#');
					_userName = ((array.Length == 1) ? _entity.DatabaseName : array[0]);
				}
				return _userName;
			}
			set
			{
				VerifyNotDeleted();
				if (_userName != null && value != null && DatabaseServer.ServerType == DatabaseServerType.Oracle)
				{
					PropertyValueValidator.Validate(this, value);
				}
				_userName = value;
				if (!string.IsNullOrEmpty(_userName) && !string.IsNullOrEmpty(_password))
				{
					_entity.DatabaseName = $"{_userName}#{_password}";
					OnPropertyChanged("DatabaseName");
				}
				OnPropertyChanged("UserName");
			}
		}

		[Required(ErrorMessage = "Required Field")]
		[StringLength(30, ErrorMessage = "Password too long!")]
		public string Password
		{
			get
			{
				VerifyNotDeleted();
				if (!string.IsNullOrEmpty(_entity.DatabaseName))
				{
					string[] array = _entity.DatabaseName.Split('#');
					if (array.Length != 1)
					{
						return array[1];
					}
					return _entity.DatabaseName;
				}
				return _password;
			}
			set
			{
				VerifyNotDeleted();
				if (_password != null && value != null && DatabaseServer.ServerType == DatabaseServerType.Oracle)
				{
					PropertyValueValidator.Validate(this, value);
				}
				_password = value;
				if (DatabaseServer.ServerType == DatabaseServerType.Oracle)
				{
					_entity.DatabaseName = $"{UserName}#{_password}";
					OnPropertyChanged("DatabaseName");
				}
				OnPropertyChanged("Password");
			}
		}

		public string DisplayText => $"{Name} ({((DatabaseServer.ServerType == DatabaseServerType.Oracle) ? UserName : DatabaseName)})";

		[Required(ErrorMessage = "You must select a database server")]
		public DatabaseServer DatabaseServer
		{
			get
			{
				if (_lazyDatabaseServer == null && _entity.DatabaseServer.ForeignKey != null)
				{
					DatabaseServerEntity databaseServerById = null;// TranslationProviderServer.Service.GetDatabaseServerById(_entity.DatabaseServer.ForeignKey, new string[0]);
                    _lazyDatabaseServer = new DatabaseServer(TranslationProviderServer, databaseServerById);
				}
				return _lazyDatabaseServer;
			}
			set
			{
				PropertyValueValidator.Validate(this, value);
				_lazyDatabaseServer = value;
				_entity.DatabaseServer.ForeignKey = _lazyDatabaseServer.Entity.Id;
				OnPropertyChanged("DatabaseServer");
			}
		}

		public ReadOnlyCollection<ServerBasedTranslationMemory> TranslationMemories
		{
			get
			{
				if (_lazyTranslationMemories == null)
				{
					TranslationMemoryEntity[] translationMemoryByContainerId = null;// TranslationProviderServer.Service.GetTranslationMemoryByContainerId(Entity.Id, TranslationProviderServer.GetDefaultTranslationMemoryRelationships(), includeLanguageResourceData: false, includeScheduledOperations: false);
                    ClientObjectBuilder clientObjectBuilder = new ClientObjectBuilder(TranslationProviderServer);
					clientObjectBuilder[clientObjectBuilder.CreateKey(Entity)] = this;
					List<ServerBasedTranslationMemory> list = new List<ServerBasedTranslationMemory>();
					TranslationMemoryEntity[] array = translationMemoryByContainerId;
					foreach (TranslationMemoryEntity entity in array)
					{
						list.Add(ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(clientObjectBuilder, entity));
					}
					_lazyTranslationMemories = list.AsReadOnly();
				}
				return _lazyTranslationMemories;
			}
		}

		public IList<string> TranslationMemoryNames
		{
			get
			{
				if (_lazyTranslationMemoryNames == null)
				{
					LoadTranslationMemoryNames();
				}
				return _lazyTranslationMemoryNames;
			}
		}

		[Required(ErrorMessage = "Required Field")]
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

		internal static TranslationMemoryContainer BuildTranslationMemoryContainer(ClientObjectBuilder builder, ContainerEntity entity)
		{
			ClientObjectKey key = builder.CreateKey(entity);
			TranslationMemoryContainer translationMemoryContainer = builder[key] as TranslationMemoryContainer;
			if (translationMemoryContainer != null)
			{
				return translationMemoryContainer;
			}
			translationMemoryContainer = (TranslationMemoryContainer)(builder[key] = new TranslationMemoryContainer(builder.Server, entity));
			ClientObjectKey key2 = builder.CreateKey<DatabaseServerEntity>(entity.DatabaseServer.ForeignKey);
			DatabaseServer databaseServer = builder[key2] as DatabaseServer;
			if (databaseServer == null && entity.DatabaseServer.Entity != null)
			{
				databaseServer = DatabaseServer.BuildDatabaseServer(builder, entity.DatabaseServer.Entity);
			}
			entity.DatabaseServer = new EntityReference<DatabaseServerEntity>(entity.DatabaseServer.ForeignKey);
			translationMemoryContainer._lazyDatabaseServer = databaseServer;
			if (entity.TranslationMemories.IsLoaded)
			{
				List<ServerBasedTranslationMemory> list = new List<ServerBasedTranslationMemory>();
				foreach (TranslationMemoryEntity translationMemory in entity.TranslationMemories)
				{
					ServerBasedTranslationMemory item = ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, translationMemory);
					list.Add(item);
				}
				translationMemoryContainer._lazyTranslationMemories = list.AsReadOnly();
				entity.TranslationMemories = new EntityCollection<TranslationMemoryEntity>();
			}
			return translationMemoryContainer;
		}

		public TranslationMemoryContainer(TranslationProviderServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			TranslationProviderServer = server;
			_entity = new ContainerEntity
			{
				UniqueId = Guid.NewGuid()
			};
		}

		internal TranslationMemoryContainer(TranslationProviderServer server, ContainerEntity entity)
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
				throw new ArgumentException(StringResources.TranslationMemoryContainer_EntryHasNoID, "entity");
			}
			TranslationProviderServer = server;
			_entity = entity;
		}

		internal TranslationMemoryContainer(DatabaseServer databaseServer, ContainerEntity entity)
		{
			if (databaseServer == null)
			{
				throw new ArgumentNullException("databaseServer");
			}
			if (databaseServer.TranslationProviderServer == null)
			{
				throw new ArgumentNullException("databaseServer.TranslationProviderServer");
			}
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.Id == null || entity.Id.Value == null)
			{
				throw new ArgumentException(StringResources.TranslationMemoryContainer_EntryHasNoID, "entity");
			}
			TranslationProviderServer = databaseServer.TranslationProviderServer;
			_entity = entity;
			_lazyDatabaseServer = databaseServer;
		}

		public void LoadTranslationMemoryNames()
		{
			_lazyTranslationMemoryNames = null;// TranslationProviderServer.Service.GetTranslationMemoryNamesByContainerId(Entity.Id).ToList();
        }

		public void Delete(bool deleteContainerDatabase)
		{
			VerifyNotDeleted();
			if (_entity.Id == null)
			{
				throw new ObjectNotSavedException("Database Server has to be saved before it can be deleted.");
			}
			VerifyPermission("tmcontainer.delete");
			//TranslationProviderServer.Service.DeleteContainer(_entity.Id, deleteContainerDatabase);
			DatabaseServer.DeleteContainer(this);
			_entity = null;
		}

		public void Save()
		{
			VerifyNotDeleted();
			if (string.IsNullOrEmpty(_entity.DatabaseName))
			{
				throw new NullPropertyException("Container has no DatabaseName.");
			}
			if (string.IsNullOrEmpty(_entity.Name))
			{
				throw new NullPropertyException("Container has no name.");
			}
			if (_entity.DatabaseServer.ForeignKey == null)
			{
				throw new NullPropertyException("DatabaseServer does not exist for this Container.");
			}
			if (IsNewObject)
			{
				//_entity = TranslationProviderServer.Service.CreateContainer(_entity, ParentResourceGroupPath);
				DatabaseServer.AddContainer(this);
			}
			else
			{
				VerifyPermission("tmcontainer.edit");
				//_entity = TranslationProviderServer.Service.UpdateContainer(_entity);
			}
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

		private void VerifyNotDeleted()
		{
			VerifyNotDeleted("The container has been deleted.");
		}

		private void VerifyNotDeleted(string message)
		{
			if (IsDeleted)
			{
				throw new ObjectDeletedException(message);
			}
		}

		private void VerifyPermission(string permission)
		{
			if (!HasPermission(permission))
			{
				throw new SecurityException($"Current user does not have {permission} permission on this container.");
			}
		}

		void IEditableObject.BeginEdit()
		{
			if (_entity != null && _backupEntity == null)
			{
				_backupEntity = new ContainerEntity();
				_backupEntity.Id = _entity.Id;
				_backupEntity.Name = _entity.Name;
				_backupEntity.Description = _entity.Description;
				_backupEntity.UniqueId = _entity.UniqueId;
				_backupEntity.DatabaseName = _entity.DatabaseName;
				_backupEntity.DatabaseServer = new EntityReference<DatabaseServerEntity>(_entity.DatabaseServer.ForeignKey);
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

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public bool Equals(TranslationMemoryContainer other)
		{
			return Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			TranslationMemoryContainer translationMemoryContainer = (TranslationMemoryContainer)obj;
			if (translationMemoryContainer != null)
			{
				return Equals(translationMemoryContainer);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
