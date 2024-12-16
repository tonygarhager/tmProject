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
	public class ServerBasedFieldsTemplate : IFieldsTemplate, INotifyPropertyChanged, IEquatable<ServerBasedFieldsTemplate>, IEditableObject, IPermissionCheck
	{
		private FieldGroupTemplateEntity _entity;

		private FieldGroupTemplateEntity _backupEntity;

		private bool _isReadOnly;

		private FieldDefinitionCollection _lazyFieldDefinitions;

		private ReadOnlyCollection<ServerBasedTranslationMemory> _lazyTranslationMemories;

		private ScheduledFieldApplyOperation _lazyCurrentFieldApplyOperation;

		public TranslationProviderServer TranslationProviderServer
		{
			get;
			private set;
		}

		internal FieldGroupTemplateEntity Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
				if (_lazyFieldDefinitions != null)
				{
					_lazyFieldDefinitions.UpdateEntities(_entity.Fields);
				}
			}
		}

		public Guid Id
		{
			get
			{
				VerifyNotDeleted();
				return _entity.UniqueId.Value;
			}
		}

		public ReadOnlyCollection<ServerBasedTranslationMemory> TranslationMemories
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyTranslationMemories == null)
				{
					TranslationMemoryEntity[] translationMemoriesByFieldGroupTemplateId = null;// TranslationProviderServer.Service.GetTranslationMemoriesByFieldGroupTemplateId(_entity.Id, TranslationProviderServer.GetDefaultTranslationMemoryRelationships(), includeLanguageResourceData: false, includeScheduledOperations: false);
                    List<ServerBasedTranslationMemory> list = new List<ServerBasedTranslationMemory>();
					TranslationMemoryEntity[] array = translationMemoriesByFieldGroupTemplateId;
					foreach (TranslationMemoryEntity entity in array)
					{
						list.Add(new ServerBasedTranslationMemory(TranslationProviderServer, entity));
					}
					_lazyTranslationMemories = list.AsReadOnly();
				}
				return _lazyTranslationMemories;
			}
		}

		public ScheduledFieldApplyOperation CurrentFieldApplyOperation
		{
			get
			{
				if (Entity.CurrentFieldApplyWorkItemUniqueId.HasValue && _lazyCurrentFieldApplyOperation == null)
				{
					ScheduledOperationEntity scheduledOperation = null;// TranslationProviderServer.Service.GetScheduledOperation(Entity.CurrentFieldApplyWorkItemUniqueId.Value);
                    if (scheduledOperation != null)
					{
						_lazyCurrentFieldApplyOperation = new ScheduledFieldApplyOperation(scheduledOperation);
						_lazyCurrentFieldApplyOperation.FieldsTemplate = this;
					}
					else
					{
						Entity.CurrentFieldApplyWorkItemUniqueId = null;
					}
				}
				return _lazyCurrentFieldApplyOperation;
			}
		}

		[Required(ErrorMessage = "Required Field")]
		[RegularExpression("[^\\\\/\"<>\\|\\*\\?%]+", ErrorMessage = "The following characters ^ \\ / \\\"  < > | * ? %  are not allowed")]
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

		public FieldDefinitionCollection FieldDefinitions
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyFieldDefinitions == null)
				{
					FieldEntity[] fieldsByGroupId = null;// TranslationProviderServer.Service.GetFieldsByGroupId(Id);
                    _entity.Fields = new EntityCollection<FieldEntity>(fieldsByGroupId);
					_lazyFieldDefinitions = new FieldDefinitionCollection(_entity.Fields, _isReadOnly);
				}
				return _lazyFieldDefinitions;
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
				VerifyNotDeleted();
				PropertyValueValidator.Validate(this, value);
				_entity.ParentResourceGroupPath = value;
				OnPropertyChanged("ParentResourceGroupPath");
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

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				PrivatePropertyChanged += value;
			}
			remove
			{
				PrivatePropertyChanged -= value;
			}
		}

		private event PropertyChangedEventHandler PrivatePropertyChanged;

		internal static ServerBasedFieldsTemplate BuildServerBasedFieldsTemplate(ClientObjectBuilder builder, FieldGroupTemplateEntity entity)
		{
			ClientObjectKey key = builder.CreateKey(entity);
			ServerBasedFieldsTemplate serverBasedFieldsTemplate = builder[key] as ServerBasedFieldsTemplate;
			if (serverBasedFieldsTemplate != null)
			{
				return serverBasedFieldsTemplate;
			}
			serverBasedFieldsTemplate = (ServerBasedFieldsTemplate)(builder[key] = new ServerBasedFieldsTemplate(builder.Server, entity, isReadOnly: false));
			if (entity.TranslationMemories.IsLoaded)
			{
				List<ServerBasedTranslationMemory> list = new List<ServerBasedTranslationMemory>();
				foreach (TranslationMemoryEntity translationMemory in entity.TranslationMemories)
				{
					ServerBasedTranslationMemory item = ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, translationMemory);
					list.Add(item);
				}
				serverBasedFieldsTemplate._lazyTranslationMemories = list.AsReadOnly();
				entity.TranslationMemories = new EntityCollection<TranslationMemoryEntity>();
			}
			if (entity.CurrentFieldApplyOperation != null)
			{
				serverBasedFieldsTemplate._lazyCurrentFieldApplyOperation = new ScheduledFieldApplyOperation(entity.CurrentFieldApplyOperation);
				serverBasedFieldsTemplate._lazyCurrentFieldApplyOperation.FieldsTemplate = serverBasedFieldsTemplate;
			}
			return serverBasedFieldsTemplate;
		}

		public ServerBasedFieldsTemplate(TranslationProviderServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			TranslationProviderServer = server;
			_entity = new FieldGroupTemplateEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.IsTmSpecific = false;
			_lazyFieldDefinitions = new FieldDefinitionCollection(_entity.Fields, isReadOnly: false);
		}

		internal ServerBasedFieldsTemplate(TranslationProviderServer server, FieldGroupTemplateEntity entity, bool isReadOnly)
		{
			TranslationProviderServer = server;
			_entity = entity;
			_isReadOnly = isReadOnly;
			if (entity.Fields.IsLoaded)
			{
				_lazyFieldDefinitions = new FieldDefinitionCollection(entity.Fields, isReadOnly);
			}
		}

		internal ServerBasedFieldsTemplate(ServerBasedTranslationMemory tm, bool isReadOnly, bool isNewTMSpecific)
		{
			TranslationProviderServer = tm.TranslationProviderServer;
			_entity = tm.Entity.FieldGroupTemplate.Entity;
			_isReadOnly = isReadOnly;
			if ((_entity.Fields.IsLoaded || tm.IsNewObject) | isNewTMSpecific)
			{
				_lazyFieldDefinitions = new FieldDefinitionCollection(_entity.Fields, isReadOnly);
			}
		}

		public void Delete()
		{
			VerifyNotDeleted();
			if (IsNewObject)
			{
				throw new ObjectNotSavedException();
			}
			VerifyPermission("tmfields.delete");
			//TranslationProviderServer.Service.DeleteFieldGroupTemplate(_entity.Id);
			_entity = null;
		}

		public void Save()
		{
			VerifyNotDeleted();
			if (IsNewObject)
			{
				//Entity = TranslationProviderServer.Service.CreateFieldGroupTemplate(_entity, ParentResourceGroupPath);
				return;
			}
			VerifyPermission("tmfields.edit");
			//Entity = TranslationProviderServer.Service.UpdateFieldGroupTemplate(_entity);
		}

		private void VerifyNotDeleted()
		{
			VerifyNotDeleted("The field template has been deleted.");
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
				throw new SecurityException($"Current user does not have {permission} permission on this field template.");
			}
		}

		public bool Equals(ServerBasedFieldsTemplate other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			ServerBasedFieldsTemplate serverBasedFieldsTemplate = obj as ServerBasedFieldsTemplate;
			if (serverBasedFieldsTemplate != null)
			{
				return Equals(serverBasedFieldsTemplate);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PrivatePropertyChanged != null)
			{
				this.PrivatePropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		void IEditableObject.BeginEdit()
		{
			if (_entity != null && _backupEntity == null)
			{
				_backupEntity = _entity.Clone();
				_backupEntity.MarkAsClean();
			}
		}

		void IEditableObject.CancelEdit()
		{
			if (_entity != null && _backupEntity != null)
			{
				_entity = _backupEntity;
				_backupEntity = null;
			}
		}

		void IEditableObject.EndEdit()
		{
			_backupEntity = null;
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
	}
}
