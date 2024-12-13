using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ILanguageResourcesTemplate" /> which is stored on a server.
	/// </summary>
	public class ServerBasedLanguageResourcesTemplate : ILanguageResourcesTemplate, INotifyPropertyChanged, IEquatable<ServerBasedLanguageResourcesTemplate>, IEditableObject, IPermissionCheck
	{
		private LanguageResourceTemplateEntity _entity;

		private LanguageResourceTemplateEntity _backupEntity;

		private LanguageResourceBundleCollection _lazyLanguageResourceBundles;

		private ReadOnlyCollection<ServerBasedTranslationMemory> _lazyTranslationMemories;

		private ScheduledLanguageResourcesApplyOperation _lazyCurrentLangResApplyOperation;

		/// <summary>
		/// Gets the server.
		/// </summary>
		public TranslationProviderServer TranslationProviderServer
		{
			get;
			private set;
		}

		internal LanguageResourceTemplateEntity Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
				if (_lazyLanguageResourceBundles != null)
				{
					_lazyLanguageResourceBundles.UpdateEntities(_entity.LanguageResources);
				}
			}
		}

		/// <summary>
		/// Gets the unique ID of this language resources template.
		/// </summary>
		public Guid Id => _entity.UniqueId.Value;

		/// <summary>
		/// Gets or sets the name of this language resources template.
		/// </summary>
		/// <remarks>Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.Save" /> to persists the change after setting this property.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when trying to get or set the name of an entity that no longer exists.</exception>
		[Required(ErrorMessage = "Required Field")]
		[RegularExpression("[^\\\\/\"<>\\|\\*\\?%]+", ErrorMessage = "The following characters ^ \\ / \\\"  < > | * ? %  are not allowed")]
		[StringLength(50, ErrorMessage = "Name too long!")]
		public string Name
		{
			get
			{
				return _entity.Name;
			}
			set
			{
				PropertyValueValidator.Validate(this, value);
				_entity.Name = value;
				OnPropertyChanged("Name");
			}
		}

		/// <summary>
		/// Gets or sets the description of this language resources template.
		/// </summary>
		/// <remarks>Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.Save" /> to persists the change after setting this property.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when trying to get or set the description of an entity that no longer exists.</exception>
		[StringLength(255, ErrorMessage = "Description too long!")]
		public string Description
		{
			get
			{
				return _entity.Description;
			}
			set
			{
				PropertyValueValidator.Validate(this, value);
				_entity.Description = value;
				OnPropertyChanged("Description");
			}
		}

		/// <summary>
		/// Gets the collection of translation memories associated with this language resources template.
		/// </summary>
		/// <remarks>If the list of translation memories has been pre-loaded, the in-memory collection of translation memories is returned, otherwise
		/// the list of translation memories is retrieved from the server on-demand.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has already been deleted.</exception>
		public ReadOnlyCollection<ServerBasedTranslationMemory> TranslationMemories
		{
			get
			{
				if (_lazyTranslationMemories == null)
				{
					TranslationMemoryEntity[] translationMemoriesByLanguageResourceTemplateId = TranslationProviderServer.Service.GetTranslationMemoriesByLanguageResourceTemplateId(_entity.Id, TranslationProviderServer.GetDefaultTranslationMemoryRelationships(), includeLanguageResourceData: false, includeScheduledOperations: false);
					List<ServerBasedTranslationMemory> list = new List<ServerBasedTranslationMemory>();
					TranslationMemoryEntity[] array = translationMemoriesByLanguageResourceTemplateId;
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
		/// Gets the language resources bundles contained in this template.
		/// </summary>
		/// <remarks>Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.Save" /> to persist any changes made to
		/// language resources.</remarks>
		/// <remarks>If the list of language resources bundles has been pre-loaded, the in-memory collection of language resources bundles is returned, otherwise
		/// the list of language resources bundles is retrieved from the server on-demand.</remarks>
		public LanguageResourceBundleCollection LanguageResourceBundles
		{
			get
			{
				if (_lazyLanguageResourceBundles == null)
				{
					LanguageResourceEntity[] languageResourcesByTemplateId = TranslationProviderServer.Service.GetLanguageResourcesByTemplateId(_entity.Id);
					_entity.LanguageResources = new EntityCollection<LanguageResourceEntity>(languageResourcesByTemplateId);
					_lazyLanguageResourceBundles = new LanguageResourceBundleCollection(_entity.LanguageResources);
				}
				return _lazyLanguageResourceBundles;
			}
		}

		internal bool LanguageResourcesLoaded => _lazyLanguageResourceBundles != null;

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
		/// Gets the most recent scheduled field template apply operation; if any.
		/// </summary>
		public ScheduledLanguageResourcesApplyOperation CurrentLangResApplyOperation
		{
			get
			{
				if (Entity.CurrentLangResApplyWorkItemUniqueId.HasValue && _lazyCurrentLangResApplyOperation == null)
				{
					ScheduledOperationEntity scheduledOperation = TranslationProviderServer.Service.GetScheduledOperation(Entity.CurrentLangResApplyWorkItemUniqueId.Value);
					if (scheduledOperation != null)
					{
						_lazyCurrentLangResApplyOperation = new ScheduledLanguageResourcesApplyOperation(scheduledOperation);
						_lazyCurrentLangResApplyOperation.LanguageResourcesTemplate = this;
					}
					else
					{
						Entity.CurrentLangResApplyWorkItemUniqueId = null;
					}
				}
				return _lazyCurrentLangResApplyOperation;
			}
		}

		/// <summary>
		/// Returns <code>true</code> if this translation memory has been deleted.
		/// </summary>
		public bool IsDeleted => _entity == null;

		public BuiltinRecognizers? Recognizers
		{
			get
			{
				if (_entity.Recognizers == null)
				{
					return null;
				}
				return (BuiltinRecognizers?)Enum.Parse(typeof(BuiltinRecognizers), _entity.Recognizers);
			}
			set
			{
				_entity.Recognizers = value.ToString();
			}
		}

		public WordCountFlags? WordCountFlags
		{
			get
			{
				if (_entity.WordCountFlags == null)
				{
					return null;
				}
				return (WordCountFlags?)Enum.Parse(typeof(WordCountFlags), _entity.WordCountFlags);
			}
			set
			{
				_entity.WordCountFlags = value.ToString();
			}
		}

		public TokenizerFlags? TokenizerFlags
		{
			get
			{
				if (_entity.TokenizerFlags == null)
				{
					return null;
				}
				return (TokenizerFlags?)Enum.Parse(typeof(TokenizerFlags), _entity.TokenizerFlags);
			}
			set
			{
				_entity.TokenizerFlags = value.ToString();
			}
		}

		/// <summary>
		/// Returns <code>true</code> if this language resources template has not been saved yet.
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
		/// Returns <code>true</code> if this language resources template has unsaved changes.
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

		internal static ServerBasedLanguageResourcesTemplate BuildServerBasedLanguageResourcesTemplate(ClientObjectBuilder builder, LanguageResourceTemplateEntity entity)
		{
			ClientObjectKey key = builder.CreateKey(entity);
			ServerBasedLanguageResourcesTemplate serverBasedLanguageResourcesTemplate = builder[key] as ServerBasedLanguageResourcesTemplate;
			if (serverBasedLanguageResourcesTemplate != null)
			{
				return serverBasedLanguageResourcesTemplate;
			}
			serverBasedLanguageResourcesTemplate = (ServerBasedLanguageResourcesTemplate)(builder[key] = new ServerBasedLanguageResourcesTemplate(builder.Server, entity));
			if (entity.TranslationMemories.IsLoaded)
			{
				List<ServerBasedTranslationMemory> list = new List<ServerBasedTranslationMemory>();
				foreach (TranslationMemoryEntity translationMemory in entity.TranslationMemories)
				{
					ServerBasedTranslationMemory item = ServerBasedTranslationMemory.BuildServerBasedTranslationMemory(builder, translationMemory);
					list.Add(item);
				}
				serverBasedLanguageResourcesTemplate._lazyTranslationMemories = list.AsReadOnly();
				entity.TranslationMemories = new EntityCollection<TranslationMemoryEntity>();
			}
			if (entity.CurrentLangResApplyOperation != null)
			{
				serverBasedLanguageResourcesTemplate._lazyCurrentLangResApplyOperation = new ScheduledLanguageResourcesApplyOperation(entity.CurrentLangResApplyOperation);
				serverBasedLanguageResourcesTemplate._lazyCurrentLangResApplyOperation.LanguageResourcesTemplate = serverBasedLanguageResourcesTemplate;
			}
			return serverBasedLanguageResourcesTemplate;
		}

		/// <summary>
		/// Creates a new, empty language resources template.
		/// </summary>
		/// <param name="server">The server on which to create the language resources template.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="server" /> is null.</exception>
		public ServerBasedLanguageResourcesTemplate(TranslationProviderServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			TranslationProviderServer = server;
			_entity = new LanguageResourceTemplateEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.IsTmSpecific = false;
			_lazyLanguageResourceBundles = new LanguageResourceBundleCollection(_entity.LanguageResources);
		}

		internal ServerBasedLanguageResourcesTemplate(TranslationProviderServer server, LanguageResourceTemplateEntity entity)
		{
			TranslationProviderServer = server;
			_entity = entity;
			if (entity.LanguageResources.IsLoaded)
			{
				_lazyLanguageResourceBundles = new LanguageResourceBundleCollection(entity.LanguageResources);
			}
		}

		internal ServerBasedLanguageResourcesTemplate(ServerBasedTranslationMemory tm, bool isReadOnly, bool isNewTMSpecific)
		{
			TranslationProviderServer = tm.TranslationProviderServer;
			_entity = tm.Entity.LanguageResourceTemplate.Entity;
			if ((_entity.LanguageResources.IsLoaded || tm.IsNewObject) | isNewTMSpecific)
			{
				_lazyLanguageResourceBundles = new LanguageResourceBundleCollection(_entity.LanguageResources);
			}
		}

		/// <summary>
		/// Deletes this language resources template.
		/// </summary>
		/// <remarks>If any translation memories are currently associated with the language resources template, the language resources of these
		/// translation memories will not be affected.</remarks>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has already been deleted.</exception>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectNotSavedException">Thrown when this object has not been initially saved yet.</exception>
		public void Delete()
		{
			VerifyNotDeleted("Language Resource Template must have been deleted, contents are null.");
			if (IsNewObject)
			{
				throw new ObjectNotSavedException();
			}
			VerifyPermission("tmlangresource.delete");
			TranslationProviderServer.Service.DeleteLanguageResourceTemplate(_entity.Id);
			_entity = null;
		}

		/// <summary>
		/// Saves this language resources template, including any changes to its language resource bundles.
		/// </summary>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has already been deleted.</exception>
		public void Save()
		{
			if (LanguageResourcesLoaded)
			{
				LanguageResourceBundles.SaveToEntities();
			}
			if (IsNewObject)
			{
				Entity = TranslationProviderServer.Service.CreateLanguageResourceTemplate(_entity, ParentResourceGroupPath);
				return;
			}
			VerifyPermission("tmlangresource.edit");
			Entity = TranslationProviderServer.Service.UpdateLanguageResourceTemplate(_entity);
		}

		private void VerifyPermission(string permission)
		{
			if (!HasPermission(permission))
			{
				throw new SecurityException($"Current user does not have {permission} permission on this language resource template.");
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
			VerifyNotDeleted("The language resource template has been deleted.");
		}

		/// <summary>
		/// Gets whether this object has the permission with the specified name.
		/// </summary>
		/// <param name="permission">The permission name.</param>
		/// <returns>
		/// 	<code>true</code> is the object has the specified permission.
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

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PrivatePropertyChanged != null)
			{
				this.PrivatePropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Returns true if this template has the same <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.Id" /> as the specified template.
		/// </summary>
		/// <param name="other">A template to compare to.</param>
		/// <returns>True if this template has the same <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.Id" /> as the specified template.</returns>
		public bool Equals(ServerBasedLanguageResourcesTemplate other)
		{
			return Id.Equals(other.Id);
		}

		/// <summary>
		/// Returns true if this language resources template has the same <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.Id" /> as the specified language resources template.
		/// </summary>
		/// <param name="obj">A language resources template to compare to.</param>
		/// <returns>True if this language resources template has the same <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.Id" /> as the specified language resources template.</returns>
		public override bool Equals(object obj)
		{
			ServerBasedLanguageResourcesTemplate serverBasedLanguageResourcesTemplate = obj as ServerBasedLanguageResourcesTemplate;
			if (serverBasedLanguageResourcesTemplate != null)
			{
				return Equals(serverBasedLanguageResourcesTemplate);
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
	}
}
