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
	public class ServerBasedLanguageResourcesTemplate : ILanguageResourcesTemplate, INotifyPropertyChanged, IEquatable<ServerBasedLanguageResourcesTemplate>, IEditableObject, IPermissionCheck
	{
		private LanguageResourceTemplateEntity _entity;

		private LanguageResourceTemplateEntity _backupEntity;

		private LanguageResourceBundleCollection _lazyLanguageResourceBundles;

		private ReadOnlyCollection<ServerBasedTranslationMemory> _lazyTranslationMemories;

		private ScheduledLanguageResourcesApplyOperation _lazyCurrentLangResApplyOperation;

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

		public Guid Id => _entity.UniqueId.Value;

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

		public ReadOnlyCollection<ServerBasedTranslationMemory> TranslationMemories
		{
			get
			{
				if (_lazyTranslationMemories == null)
				{
					TranslationMemoryEntity[] translationMemoriesByLanguageResourceTemplateId = null;// TranslationProviderServer.Service.GetTranslationMemoriesByLanguageResourceTemplateId(_entity.Id, TranslationProviderServer.GetDefaultTranslationMemoryRelationships(), includeLanguageResourceData: false, includeScheduledOperations: false);
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

		public LanguageResourceBundleCollection LanguageResourceBundles
		{
			get
			{
				if (_lazyLanguageResourceBundles == null)
				{
					LanguageResourceEntity[] languageResourcesByTemplateId = null;// TranslationProviderServer.Service.GetLanguageResourcesByTemplateId(_entity.Id);
                    _entity.LanguageResources = new EntityCollection<LanguageResourceEntity>(languageResourcesByTemplateId);
					_lazyLanguageResourceBundles = new LanguageResourceBundleCollection(_entity.LanguageResources);
				}
				return _lazyLanguageResourceBundles;
			}
		}

		internal bool LanguageResourcesLoaded => _lazyLanguageResourceBundles != null;

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

		public ScheduledLanguageResourcesApplyOperation CurrentLangResApplyOperation
		{
			get
			{
				if (Entity.CurrentLangResApplyWorkItemUniqueId.HasValue && _lazyCurrentLangResApplyOperation == null)
				{
					ScheduledOperationEntity scheduledOperation = null;// TranslationProviderServer.Service.GetScheduledOperation(Entity.CurrentLangResApplyWorkItemUniqueId.Value);
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

		public void Delete()
		{
			VerifyNotDeleted("Language Resource Template must have been deleted, contents are null.");
			if (IsNewObject)
			{
				throw new ObjectNotSavedException();
			}
			VerifyPermission("tmlangresource.delete");
			//TranslationProviderServer.Service.DeleteLanguageResourceTemplate(_entity.Id);
			_entity = null;
		}

		public void Save()
		{
			if (LanguageResourcesLoaded)
			{
				LanguageResourceBundles.SaveToEntities();
			}
			if (IsNewObject)
			{
				//Entity = TranslationProviderServer.Service.CreateLanguageResourceTemplate(_entity, ParentResourceGroupPath);
				return;
			}
			VerifyPermission("tmlangresource.edit");
			//Entity = TranslationProviderServer.Service.UpdateLanguageResourceTemplate(_entity);
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

		public bool Equals(ServerBasedLanguageResourcesTemplate other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			ServerBasedLanguageResourcesTemplate serverBasedLanguageResourcesTemplate = obj as ServerBasedLanguageResourcesTemplate;
			if (serverBasedLanguageResourcesTemplate != null)
			{
				return Equals(serverBasedLanguageResourcesTemplate);
			}
			return false;
		}

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
