using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.FGA;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class CallContext : IDisposable
	{
		private class OverrideIdentity : IIdentity
		{
			public string AuthenticationType => "Override";

			public bool IsAuthenticated => true;

			public string Name
			{
				get;
			}

			public OverrideIdentity(string overrideName)
			{
				if (overrideName == null)
				{
					throw new LanguagePlatformException(ErrorCode.AuthInvalidUser);
				}
				overrideName = overrideName.Trim();
				if (overrideName.Length == 0 || overrideName.Length > TranslationMemorySetup.MaximumUserNameLength)
				{
					throw new LanguagePlatformException(ErrorCode.AuthInvalidUser);
				}
				Name = overrideName;
			}
		}

		private IStorage _storage;

		private IAlignableStorage _alignableStorage;

		private Dictionary<string, string> _storageOptions;

		private ResourceManager _resourceManager;

		private IIdentity _user;

		private readonly AnnotatedTmManager _atmManager;

		private bool _wcfHosted;

		private IAlignableCorpusManager _alignableCorpusManager;

		private readonly List<Task> _tasksOnComplete = new List<Task>();

		public int TuLimit
		{
			get;
			set;
		}

		public Container Container
		{
			get;
		}

		public string UserName => _user.Name;

		public CultureInfo ErrorMessageCulture
		{
			get;
			private set;
		}

		public Guid ResourceId
		{
			get;
			set;
		}

		public Permission Permissions
		{
			get;
			set;
		}

		public IAlignableStorage AlignableStorage
		{
			get
			{
				if (_alignableStorage == null)
				{
					_alignableStorage = (Storage as IAlignableStorage);
				}
				if (_alignableStorage == null)
				{
					throw new Exception("IStorage implementation does not implement IAlignableStorage: " + _storage.GetType().Name);
				}
				return _alignableStorage;
			}
		}

		public IStorage Storage => _storage ?? (_storage = ((Container != null) ? StorageFactory.Create(Container) : StorageFactory.Create(_storageOptions)));

		public IAlignableCorpusManager AlignableCorpusManager => _alignableCorpusManager ?? (_alignableCorpusManager = DefaultAlignableCorpusManagerFactory.GetAlignableCorpusManager(this));

		public bool IsFilebasedTm => Storage is SqliteStorage;

		public bool IsInMemoryTm => Storage is InMemoryStorage;

		public ResourceManager ResourceManager => _resourceManager ?? (_resourceManager = new ResourceManager(this));

		public CallContext(Container container, Dictionary<string, string> apiOptions, AnnotatedTmManager atmManager)
		{
			if (Container != null && _storageOptions != null)
			{
				throw new ArgumentException("You can't pass storage options as well as a container object");
			}
			_atmManager = (atmManager ?? throw new ArgumentNullException("atmManager"));
			_storageOptions = apiOptions;
			Container = container;
			GetUserInfo();
		}

		public void Validate(Permission requiredPermission, PersistentObjectToken resourceId)
		{
			if (_user == null)
			{
				throw new LanguagePlatformException(ErrorCode.AuthUnknownOrNonauthenticatedUser);
			}
			ResourceId = ((resourceId != null) ? GetResourceIdFromObjectToken(resourceId) : Guid.Empty);
			Permissions = ((requiredPermission == Permission.CreateSchema) ? Permission.Administrator : GetPermissions(resourceId));
			Require(requiredPermission);
			if (resourceId == null || (requiredPermission & Permission.ReadTU) == 0 || !IsFilebasedTm || TuLimit <= 0 || ResourceManager.GetTuCount(resourceId) <= TuLimit)
			{
				return;
			}
			throw new LanguagePlatformException(ErrorCode.TMTULimitExceeded);
		}

		public Permission GetPermissions(PersistentObjectToken resourceId)
		{
			if (!IsFilebasedTm)
			{
				return Permission.Administrator;
			}
			SqliteStorage obj = (_storage as SqliteStorage) ?? throw new Exception("Unexpected");
			string value;
			if (Container != null)
			{
				value = (((FileContainer)Container).Password ?? string.Empty);
			}
			else if (_storageOptions == null || !_storageOptions.TryGetValue("pwd", out value))
			{
				value = string.Empty;
			}
			return obj.GetPasswordPermissionLevel(resourceId, value);
		}

		public void UpdatePassword(string pwd)
		{
			FileContainer fileContainer = Container as FileContainer;
			if (fileContainer != null)
			{
				fileContainer.Password = pwd;
				return;
			}
			if (_storageOptions == null)
			{
				if (pwd == null)
				{
					return;
				}
				_storageOptions = new Dictionary<string, string>();
			}
			_storageOptions["pwd"] = pwd;
		}

		private void GetUserInfo()
		{
			OperationContext current = OperationContext.Current;
			_wcfHosted = (current != null);
			if (_wcfHosted && current?.ServiceSecurityContext?.PrimaryIdentity != null)
			{
				_user = current.ServiceSecurityContext.PrimaryIdentity;
			}
			if (Container != null && !string.IsNullOrEmpty(Container.UserNameOverride))
			{
				_user = new OverrideIdentity(Container.UserNameOverride);
			}
			if (_user == null)
			{
				_user = Thread.CurrentPrincipal.Identity;
			}
			if (_user == null || !_user.IsAuthenticated)
			{
				WindowsIdentity windowsIdentity = (WindowsIdentity)(_user = WindowsIdentity.GetCurrent());
			}
			if (_user == null)
			{
				throw new LanguagePlatformException(ErrorCode.AuthInvalidUser);
			}
			ErrorMessageCulture = CultureInfo.GetCultureInfo("en-US");
		}

		private void Require(Permission p)
		{
			if (p != 0 && (Permissions & p) == 0)
			{
				throw new LanguagePlatformException(ErrorCode.AuthInsufficientPermissions);
			}
		}

		public ServerInfo GetServerInfo()
		{
			return new ServerInfo("1.0.0.0", "The server is up and running.", ServerInfo.ServerStatus.Running, UserName);
		}

		private static Guid GetResourceIdFromObjectToken(PersistentObjectToken rt)
		{
			return rt?.Guid ?? Guid.Empty;
		}

		public AlignableCorpusId GetAlignableCorpusId(PersistentObjectToken tmId)
		{
			return new StorageBasedAlignableCorpusId(tmId);
		}

		public Importer GetImporter(PersistentObjectToken tmId)
		{
			return new Importer(this, tmId);
		}

		public AnnotatedTranslationMemory GetAnnotatedTranslationMemory(PersistentObjectToken tmId)
		{
			return _atmManager.GetAnnotatedTranslationMemory(this, tmId);
		}

		public void Complete()
		{
			if (_storage != null)
			{
				_storage.Flush();
				_storage.CommitTransaction();
				List<Task> list = new List<Task>();
				list.AddRange(_tasksOnComplete);
				_tasksOnComplete.Clear();
				foreach (Task item in list)
				{
					item.RunSynchronously();
				}
				_storage.Flush();
				_storage.CommitTransaction();
			}
		}

		public void AddOnCompleteTask(Task task)
		{
			_tasksOnComplete.Add(task);
		}

		public void Dispose()
		{
			if (_storage != null)
			{
				_storage.AbortTransaction();
				_storage.Dispose();
				_storage = null;
			}
		}
	}
}
