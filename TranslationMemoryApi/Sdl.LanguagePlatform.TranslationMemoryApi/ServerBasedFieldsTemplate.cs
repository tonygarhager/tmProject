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
	/// <summary>
	/// Represents a fields template stored on the server. A fields template is a named collection of field definitions
	/// which can be applied to one or more translation memories (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedTranslationMemory.FieldsTemplate" />.
	/// Changes made to the fields template will be applied to all translation memories associated with it.
	/// </summary>
	public class ServerBasedFieldsTemplate : IFieldsTemplate, INotifyPropertyChanged, IEquatable<ServerBasedFieldsTemplate>, IEditableObject, IPermissionCheck
	{
		private FieldGroupTemplateEntity _entity;

		private FieldGroupTemplateEntity _backupEntity;

		private bool _isReadOnly;

		private FieldDefinitionCollection _lazyFieldDefinitions;

		private ReadOnlyCollection<ServerBasedTranslationMemory> _lazyTranslationMemories;

		private ScheduledFieldApplyOperation _lazyCurrentFieldApplyOperation;

		/// <summary>
		/// Gets the server.
		/// </summary>
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

		/// <summary>
		/// Gets the unique id of the fields template. This is automatically generated by the system.
		/// </summary>
		public Guid Id
		{
			get
			{
				VerifyNotDeleted();
				return _entity.UniqueId.Value;
			}
		}

		/// <summary>
		/// Gets the collection of translation memories associated with this fields template.
		/// </summary>
		/// <remarks>If the list of translation memories has been pre-loaded, the in-memory collection of translation memories is returned, otherwise
		/// the list of translation memories is retrieved from the server on-demand.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has already been deleted.</exception>
		public ReadOnlyCollection<ServerBasedTranslationMemory> TranslationMemories
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyTranslationMemories == null)
				{
					TranslationMemoryEntity[] translationMemoriesByFieldGroupTemplateId = TranslationProviderServer.Service.GetTranslationMemoriesByFieldGroupTemplateId(_entity.Id, TranslationProviderServer.GetDefaultTranslationMemoryRelationships(), includeLanguageResourceData: false, includeScheduledOperations: false);
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

		/// <summary>
		/// Gets the most recent scheduled field template apply operation; if any.
		/// </summary>
		public ScheduledFieldApplyOperation CurrentFieldApplyOperation
		{
			get
			{
				if (Entity.CurrentFieldApplyWorkItemUniqueId.HasValue && _lazyCurrentFieldApplyOperation == null)
				{
					ScheduledOperationEntity scheduledOperation = TranslationProviderServer.Service.GetScheduledOperation(Entity.CurrentFieldApplyWorkItemUniqueId.Value);
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

		/// <summary>
		/// Gets or sets the name of this fields template.
		/// </summary>
		/// <remarks>Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Save" /> to persists the change after setting this property.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when trying to get or set name of an entity that no longer exists.</exception>
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

		/// <summary>
		/// Gets or sets the description of the fields template.
		/// </summary>
		/// <remarks>Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Save" /> to persists the change after setting this property.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when trying to get/set description of an entity that no longer exists.</exception>
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

		/// <summary>
		/// Gets the fields definitions contained in this fields template.
		/// </summary>
		/// <remarks>If the list of fields definitions has been pre-loaded, the in-memory collection of fields definitions is returned, otherwise
		/// the list of fields definitions is retrieved from the server on-demand.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has already been deleted.</exception>
		public FieldDefinitionCollection FieldDefinitions
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyFieldDefinitions == null)
				{
					FieldEntity[] fieldsByGroupId = TranslationProviderServer.Service.GetFieldsByGroupId(Id);
					_entity.Fields = new EntityCollection<FieldEntity>(fieldsByGroupId);
					_lazyFieldDefinitions = new FieldDefinitionCollection(_entity.Fields, _isReadOnly);
				}
				return _lazyFieldDefinitions;
			}
		}

		/// <summary>
		/// Gets the parent resource group path.
		/// </summary>
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

		/// <summary>
		/// Gets the parent resource group name.
		/// </summary>
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

		/// <summary>
		/// Gets the parent resource group description.
		/// </summary>
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

		/// <summary>
		/// Gets the collection of paths for the linked resource groups.
		/// </summary>
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

		/// <summary>
		/// Returns <code>true</code> if this translation memory has been deleted.
		/// </summary>
		public bool IsDeleted => _entity == null;

		/// <summary>
		/// Returns <code>true</code> if this fields template has not been saved yet.
		/// </summary>
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

		/// <summary>
		/// Returns <code>true</code> if this fields template has unsaved changes.
		/// </summary>
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

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
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

		/// <summary>
		/// Creates a new fields template. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Save" /> to 
		/// persist the fields template, after setting all the required properties.
		/// </summary>
		/// <param name="server">The translation provider server on which the fields template should be created.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="server" /> is null.</exception>
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

		/// <summary>
		/// Constructor for an existing fields template.
		/// </summary>
		/// <param name="server">The translation provider server.</param>
		/// <param name="entity">The entity.</param>
		/// <param name="isReadOnly">Whether this fields template can be edited.</param>
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

		/// <summary>
		/// Deletes this fields template from the system. 
		/// </summary>
		/// <remarks>If any translation memories are currently associated with the fields template, the fields of these
		/// translation memories will not be affected.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has already been deleted.</exception>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectNotSavedException">Thrown when this object has not been initially saved yet.</exception>
		public void Delete()
		{
			VerifyNotDeleted();
			if (IsNewObject)
			{
				throw new ObjectNotSavedException();
			}
			VerifyPermission("tmfields.delete");
			TranslationProviderServer.Service.DeleteFieldGroupTemplate(_entity.Id);
			_entity = null;
		}

		/// <summary>
		/// Saves this fields template, including any changes to its field definitions.
		/// </summary>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has already been deleted.</exception>
		public void Save()
		{
			VerifyNotDeleted();
			if (IsNewObject)
			{
				Entity = TranslationProviderServer.Service.CreateFieldGroupTemplate(_entity, ParentResourceGroupPath);
				return;
			}
			VerifyPermission("tmfields.edit");
			Entity = TranslationProviderServer.Service.UpdateFieldGroupTemplate(_entity);
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

		/// <summary>
		/// Returns true if this fields template has the same <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Id" /> as the specified fields template.
		/// </summary>
		/// <param name="other">A fields template to compare to.</param>
		/// <returns>True if this fields template has the same <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Id" /> as the specified fields template.</returns>
		public bool Equals(ServerBasedFieldsTemplate other)
		{
			return Id.Equals(other.Id);
		}

		/// <summary>
		/// Returns true if this fields template has the same <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Id" /> as the specified fields template.
		/// </summary>
		/// <param name="obj">A fields template to compare to.</param>
		/// <returns>True if this fields template has the same <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Id" /> as the specified fields template.</returns>
		public override bool Equals(object obj)
		{
			ServerBasedFieldsTemplate serverBasedFieldsTemplate = obj as ServerBasedFieldsTemplate;
			if (serverBasedFieldsTemplate != null)
			{
				return Equals(serverBasedFieldsTemplate);
			}
			return false;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
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

		/// <summary>
		/// Determines whether the specified permission has permission.
		/// </summary>
		/// <param name="permission">The permission.</param>
		/// <returns>
		/// 	<c>true</c> if the specified permission has permission; otherwise, <c>false</c>.
		/// </returns>
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