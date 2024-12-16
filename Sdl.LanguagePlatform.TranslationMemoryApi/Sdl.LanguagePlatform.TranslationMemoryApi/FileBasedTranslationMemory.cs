using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class FileBasedTranslationMemory : AbstractLocalTranslationMemory, IFileBasedTranslationMemory, IAlignableTranslationMemory, ITranslationMemory2015, ITranslationMemory, ITranslationProvider, IReindexableTranslationMemory, ILocalTranslationMemory, IAdvancedContextTranslationMemory
	{
		internal class FileBasedTranslationMemoryDescriptor : ITranslationMemoryDescriptor
		{
			private const string _fileBasedTmType = "sdltm";

			private const string _fileBasedTmProtocol = "file";

			private TranslationProviderUriBuilder _fileTmUriBuilder;

			private API _service;

			private FileContainer _container;

			private static bool? _useLegacyHashingByDefault;

			private static object _locker = new object();

			private const string _CreateStrictHashingFileBasedTMsKey = "CreateStrictHashingFileBasedTMs";

			internal static bool UseLegacyHashingByDefault
			{
				get
				{
					lock (_locker)
					{
						if (_useLegacyHashingByDefault.HasValue)
						{
							return _useLegacyHashingByDefault.Value;
						}
						_useLegacyHashingByDefault = false;
						string text = ConfigurationManager.AppSettings["CreateStrictHashingFileBasedTMs"];
						if (text != null && bool.TryParse(text, out bool result))
						{
							_useLegacyHashingByDefault = !result;
						}
						return _useLegacyHashingByDefault.Value;
					}
				}
			}

			public string FilePath => _fileTmUriBuilder.Resource;

			public PersistentObjectToken Id
			{
				get
				{
					TranslationMemorySetup translationMemorySetup = GetTranslationMemorySetup(checkPermissions: false);
					return translationMemorySetup.ResourceId;
				}
			}

			public Uri Uri => _fileTmUriBuilder.Uri;

			public Container Container => _container;

			public string Name => Path.GetFileNameWithoutExtension(_fileTmUriBuilder.Resource);

			public ITranslationMemoryService Service => _service;

			public API ServiceImpl => _service;

			public FileBasedTranslationMemoryDescriptor(string tmFilePath)
			{
				_fileTmUriBuilder = new TranslationProviderUriBuilder("sdltm", "file");
				_fileTmUriBuilder.Resource = tmFilePath;
				_container = new FileContainer(tmFilePath, createIfNotExists: false);
				_service = new API();
			}

			public FileBasedTranslationMemoryDescriptor(Uri uri)
			{
				_fileTmUriBuilder = new TranslationProviderUriBuilder(uri);
				_container = new FileContainer(_fileTmUriBuilder.Resource, createIfNotExists: false);
				_service = new API();
			}

			public FileBasedTranslationMemoryDescriptor(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags)
				: this(tmFilePath, description, sourceLanguage, targetLanguage, indexes, recognizers, tokenizerFlags, wordCountFlags, supportsAlignmentData: false)
			{
			}

			public FileBasedTranslationMemoryDescriptor(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, bool supportsAlignmentData)
				: this(tmFilePath, description, sourceLanguage, targetLanguage, indexes, recognizers, tokenizerFlags, wordCountFlags, supportsAlignmentData, TextContextMatchType.PrecedingSourceAndTarget, usesIdContextMatch: false, UseLegacyHashingByDefault)
			{
			}

			public FileBasedTranslationMemoryDescriptor(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, bool supportsAlignmentData, TextContextMatchType textContextMatchType, bool usesIdContextMatch, bool usesLegacyHashes)
			{
				_fileTmUriBuilder = new TranslationProviderUriBuilder("sdltm", "file");
				if (string.IsNullOrEmpty(tmFilePath))
				{
					throw new ArgumentNullException("tmFilePath");
				}
				if (description == null)
				{
					throw new ArgumentNullException("description");
				}
				if (sourceLanguage == null)
				{
					throw new ArgumentNullException("sourceLanguage");
				}
				if (targetLanguage == null)
				{
					throw new ArgumentNullException("targetLanguage");
				}
				if (sourceLanguage.IsNeutralCulture)
				{
					throw new ArgumentException("Neutral cultures are not supported as the source language of a translation memory.");
				}
				if (targetLanguage.IsNeutralCulture)
				{
					throw new ArgumentException("Neutral cultures are not supported as the target language of a translation memory.");
				}
				_fileTmUriBuilder.Resource = tmFilePath;
				Directory.CreateDirectory(Path.GetDirectoryName(tmFilePath));
				_container = new FileContainer(tmFilePath, createIfNotExists: true);
				_service = new API();
				_service.CreateSchema(_container);
				TranslationMemorySetup translationMemorySetup = new TranslationMemorySetup
				{
					Description = description,
					LanguageDirection = new LanguagePair(sourceLanguage, targetLanguage),
					Name = Path.GetFileNameWithoutExtension(tmFilePath),
					FuzzyIndexes = indexes,
					Recognizers = recognizers,
					TokenizerFlags = tokenizerFlags,
					WordCountFlags = wordCountFlags,
					FGASupport = ((!supportsAlignmentData) ? FGASupport.Off : FGASupport.Automatic),
					TextContextMatchType = textContextMatchType,
					IdContextMatch = usesIdContextMatch,
					UsesLegacyHashes = usesLegacyHashes
				};
				translationMemorySetup.ResourceId = _service.CreateTranslationMemory(_container, translationMemorySetup);
			}

			private TranslationMemorySetup GetTranslationMemorySetup(bool checkPermissions)
			{
				TranslationMemorySetup[] translationMemories = _service.GetTranslationMemories(_container, checkPermissions);
				if (translationMemories == null || translationMemories.Length == 0)
				{
					throw new LanguagePlatformException(ErrorCode.TMNotFound);
				}
				if (translationMemories.Length > 1)
				{
					throw new InvalidOperationException("Storing multiple translation memories in a file-based container is not supported.");
				}
				return translationMemories[0];
			}

			public TranslationMemorySetup GetTranslationMemorySetup()
			{
				return GetTranslationMemorySetup(checkPermissions: true);
			}

			public Permission Unlock(string password)
			{
				_container.Password = null;
				if (!string.IsNullOrEmpty(password))
				{
					_container.Password = password;
				}
				Permission permissions = _service.GetPermissions(_container, Id);
				if (permissions == Permission.None)
				{
					throw new TranslationProviderAuthenticationException("Invalid password.");
				}
				return permissions;
			}
		}

		private ProviderStatusInfo _StatusInfo;

		private bool? _isUnlockedWithSufficientPermissions;

		private string[] _cachedPermissions;

		private bool? _hasAdministratorPassword;

		private bool? _hasMaintenancePassword;

		private bool? _hasReadOnlyPassword;

		private bool? _hasReadWritePassword;

		internal new FileBasedTranslationMemoryDescriptor Descriptor => (FileBasedTranslationMemoryDescriptor)base.Descriptor;

		public string UserName
		{
			set
			{
				Descriptor.Container.UserNameOverride = value;
			}
		}

		public string FilePath
		{
			get;
		}

		public bool HasAdministratorPassword
		{
			get
			{
				if (!_hasAdministratorPassword.HasValue)
				{
					_hasAdministratorPassword = Descriptor.ServiceImpl.HasAdministratorPassword(Descriptor.Container, base.InternalId);
				}
				return _hasAdministratorPassword.Value;
			}
		}

		public bool HasMaintenancePassword
		{
			get
			{
				if (!_hasMaintenancePassword.HasValue)
				{
					_hasMaintenancePassword = Descriptor.ServiceImpl.HasMaintenancePassword(Descriptor.Container, base.InternalId);
				}
				return _hasMaintenancePassword.Value;
			}
		}

		public bool HasReadOnlyPassword
		{
			get
			{
				if (!_hasReadOnlyPassword.HasValue)
				{
					_hasReadOnlyPassword = Descriptor.ServiceImpl.HasReadOnlyPassword(Descriptor.Container, base.InternalId);
				}
				return _hasReadOnlyPassword.Value;
			}
		}

		public bool HasReadWritePassword
		{
			get
			{
				if (!_hasReadWritePassword.HasValue)
				{
					_hasReadWritePassword = Descriptor.ServiceImpl.HasReadWritePassword(Descriptor.Container, base.InternalId);
				}
				return _hasReadWritePassword.Value;
			}
		}

		public bool IsProtected
		{
			get
			{
				if (!HasAdministratorPassword && !HasMaintenancePassword && !HasReadWritePassword)
				{
					return HasReadOnlyPassword;
				}
				return true;
			}
		}

		public bool? IsUnlockedWithSufficientPermissions
		{
			get
			{
				return _isUnlockedWithSufficientPermissions;
			}
			set
			{
				_isUnlockedWithSufficientPermissions = value;
			}
		}

		public override ProviderStatusInfo StatusInfo
		{
			get
			{
				if (_isUnlockedWithSufficientPermissions.HasValue && !_isUnlockedWithSufficientPermissions.Value)
				{
					return new ProviderStatusInfo(available: false, StringResources.NotUnlockedWithSufficientPermissions);
				}
				if (_StatusInfo == null)
				{
					RefreshStatusInfo();
				}
				return _StatusInfo;
			}
		}

		public TokenizerFlags TokenizerFlags
		{
			get
			{
				return base.Setup.TokenizerFlags;
			}
			set
			{
				base.Setup.TokenizerFlags = value;
			}
		}

		public WordCountFlags WordCountFlags
		{
			get
			{
				return base.Setup.WordCountFlags;
			}
			set
			{
				base.Setup.WordCountFlags = value;
			}
		}

		public FGASupport FGASupport
		{
			get
			{
				return base.Setup.FGASupport;
			}
			set
			{
				base.Setup.FGASupport = value;
			}
		}

		public int UnalignedTranslationUnitCount => (base.Service as IAlignableTranslationMemoryService)?.GetUnalignedTranslationUnitCount(base.Container, base.Setup.ResourceId) ?? 0;

		public int AlignedPredatedTranslationUnitCount => (base.Service as IAlignableTranslationMemoryService)?.GetAlignedPredatedTranslationUnitCount(base.Container, base.Setup.ResourceId) ?? 0;

		public int TranslationUnitNewerThanModelCount => (base.Service as IAlignableTranslationMemoryService)?.GetPostdatedTranslationUnitCount(base.Container, base.Setup.ResourceId) ?? 0;

		public bool? ReindexRequired => (base.Service as IReindexableTranslationMemoryService)?.GetReindexRequired(base.Container, base.Setup.ResourceId);

		public int TuCountForReindex => (base.Service as IReindexableTranslationMemoryService)?.GetTuCountForReindex(base.Container, base.Setup.ResourceId) ?? (-1);

		public AlignerDefinition AlignerDefinition => (base.Service as IAlignableTranslationMemoryService)?.GetAlignerDefinition(base.Container, base.Setup.ResourceId);

		public bool CanReportReindexRequired
		{
			get
			{
				return base.Setup.CanReportReindexRequired;
			}
			set
			{
				base.Setup.CanReportReindexRequired = value;
			}
		}

		public bool UsesLegacyHashes
		{
			get
			{
				return base.Setup.UsesLegacyHashes;
			}
			set
			{
				base.Setup.UsesLegacyHashes = value;
			}
		}

		public bool UsesIdContextMatching => base.Setup.IdContextMatch;

		public TextContextMatchType TextContextMatchType => base.Setup.TextContextMatchType;

		public bool CanBuildModel
		{
			get
			{
				ITranslationModelService translationModelService = base.Service as ITranslationModelService;
				if (translationModelService == null)
				{
					return false;
				}
				ModelBasedAlignerDefinition modelBasedAlignerDefinition = GetModelBasedAlignerDefinition(translationModelService);
				if (modelBasedAlignerDefinition == null)
				{
					return false;
				}
				return translationModelService.CanBuildModel(base.Container, modelBasedAlignerDefinition.ModelId);
			}
		}

		public bool ShouldAlign
		{
			get
			{
				IAlignableTranslationMemoryService alignableTranslationMemoryService = base.Service as IAlignableTranslationMemoryService;
				if (alignableTranslationMemoryService == null)
				{
					return false;
				}
				int alignedPredatedTranslationUnitCount = alignableTranslationMemoryService.GetAlignedPredatedTranslationUnitCount(base.Container, base.Setup.ResourceId);
				int unalignedTranslationUnitCount = alignableTranslationMemoryService.GetUnalignedTranslationUnitCount(base.Container, base.Setup.ResourceId);
				int num = alignedPredatedTranslationUnitCount + unalignedTranslationUnitCount;
				if (num > 1000)
				{
					return true;
				}
				return (double)num * 1.0 / (double)base.Service.GetTuCount(base.Container, base.Setup.ResourceId) > 0.1;
			}
		}

		public TranslationModelDetails ModelDetails
		{
			get
			{
				ITranslationModelService translationModelService = base.Service as ITranslationModelService;
				if (translationModelService == null)
				{
					return null;
				}
				ModelBasedAlignerDefinition modelBasedAlignerDefinition = GetModelBasedAlignerDefinition(translationModelService);
				if (modelBasedAlignerDefinition == null)
				{
					return null;
				}
				return translationModelService.GetModelDetails(base.Container, modelBasedAlignerDefinition.ModelId);
			}
		}

		public bool ShouldBuildModel
		{
			get
			{
				ITranslationModelService translationModelService = base.Service as ITranslationModelService;
				if (translationModelService == null)
				{
					return false;
				}
				ModelBasedAlignerDefinition modelBasedAlignerDefinition = GetModelBasedAlignerDefinition(translationModelService);
				if (modelBasedAlignerDefinition == null)
				{
					return false;
				}
				return translationModelService.ShouldBuildModel(base.Container, modelBasedAlignerDefinition.ModelId);
			}
		}

		public event EventHandler<TranslationModelProgressEventArgs> TranslationModelProgress;

		public static string GetFileBasedTranslationMemoryScheme()
		{
			return "sdltm.file";
		}

		public static bool IsFileBasedTranslationMemory(Uri uri)
		{
			if (uri != null)
			{
				return uri.Scheme.Equals(GetFileBasedTranslationMemoryScheme(), StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public static Uri GetFileBasedTranslationMemoryUri(string filePath)
		{
			return new FileBasedTranslationMemoryDescriptor(filePath).Uri;
		}

		public static string GetFileBasedTranslationMemoryFilePath(Uri uri)
		{
			if (!IsFileBasedTranslationMemory(uri))
			{
				throw new ArgumentException("The specified Uri is not a file-based translation memory Uri.", "uri");
			}
			return new FileBasedTranslationMemoryDescriptor(uri).FilePath;
		}

		public static string GetFileBasedTranslationMemoryName(Uri uri)
		{
			if (!IsFileBasedTranslationMemory(uri))
			{
				throw new ArgumentException("The specified Uri is not a file-based translation memory Uri.", "uri");
			}
			return Path.GetFileName(GetFileBasedTranslationMemoryFilePath(uri));
		}

		public FileBasedTranslationMemory(string tmFilePath)
			: base(new FileBasedTranslationMemoryDescriptor(tmFilePath))
		{
			if (string.IsNullOrEmpty(tmFilePath))
			{
				throw new ArgumentNullException("tmFilePath");
			}
			if (!File.Exists(tmFilePath))
			{
				throw new FileNotFoundException();
			}
			FilePath = tmFilePath;
		}

		public FileBasedTranslationMemory(Uri uri)
			: base(new FileBasedTranslationMemoryDescriptor(uri))
		{
			FilePath = Descriptor.FilePath;
			if (!File.Exists(FilePath))
			{
				throw new FileNotFoundException();
			}
		}

		public FileBasedTranslationMemory(string tmFilePath, string password)
			: this(tmFilePath)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			Unlock(password);
		}

		public FileBasedTranslationMemory(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags)
			: this(tmFilePath, description, sourceLanguage, targetLanguage, indexes, recognizers, tokenizerFlags, wordCountFlags, supportsAlignmentData: false)
		{
		}

		public FileBasedTranslationMemory(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, bool supportsAlignmentData, TextContextMatchType textContextMatchType, bool usesIdContextMatch, bool usesLegacyHashes)
			: base(new FileBasedTranslationMemoryDescriptor(tmFilePath, description, sourceLanguage, targetLanguage, indexes, recognizers, tokenizerFlags, wordCountFlags, supportsAlignmentData, textContextMatchType, usesIdContextMatch, usesLegacyHashes))
		{
			FilePath = tmFilePath;
		}

		public FileBasedTranslationMemory(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, bool supportsAlignmentData)
			: base(new FileBasedTranslationMemoryDescriptor(tmFilePath, description, sourceLanguage, targetLanguage, indexes, recognizers, tokenizerFlags, wordCountFlags, supportsAlignmentData))
		{
			FilePath = tmFilePath;
		}

		public TranslationMemoryFileAccessMode? IsPasswordSet(string password)
		{
			Permission permission = Descriptor.ServiceImpl.IsPasswordSet(Descriptor.Container, base.InternalId, password);
			if (permission == Permission.None)
			{
				return null;
			}
			return PermissionConverter.GetFileAccessMode(permission);
		}

		public void SetAdministratorPassword(string password)
		{
			if (password == null)
			{
				SetMaintenancePassword(null);
				SetReadWritePassword(null);
				SetReadOnlyPassword(null);
			}
			_cachedPermissions = null;
			Descriptor.ServiceImpl.SetAdministratorPassword(Descriptor.Container, base.InternalId, password);
			Descriptor.Unlock(password);
			_hasAdministratorPassword = null;
		}

		public void SetMaintenancePassword(string password)
		{
			if (password != null)
			{
				if (!HasAdministratorPassword)
				{
					throw new InvalidOperationException("You have to set an administrator password before setting a maintenance password.");
				}
			}
			else
			{
				SetReadWritePassword(null);
				SetReadOnlyPassword(null);
			}
			_cachedPermissions = null;
			Descriptor.ServiceImpl.SetMaintenancePassword(Descriptor.Container, base.InternalId, password);
			_hasMaintenancePassword = null;
		}

		public void SetReadWritePassword(string password)
		{
			if (password != null)
			{
				if (!HasAdministratorPassword)
				{
					throw new InvalidOperationException("You have to set an administrator password before setting a read-write password.");
				}
				if (!HasMaintenancePassword)
				{
					throw new InvalidOperationException("You have to set an maintenance password before setting a read-write password.");
				}
			}
			else
			{
				SetReadOnlyPassword(null);
			}
			_cachedPermissions = null;
			Descriptor.ServiceImpl.SetReadWritePassword(Descriptor.Container, base.InternalId, password);
			_hasReadWritePassword = null;
		}

		public void SetReadOnlyPassword(string password)
		{
			if (password != null)
			{
				if (!HasAdministratorPassword)
				{
					throw new InvalidOperationException("You have to set an administrator password before setting a read-only password.");
				}
				if (!HasMaintenancePassword)
				{
					throw new InvalidOperationException("You have to set an maintenance password before setting a read-only password.");
				}
				if (!HasReadWritePassword)
				{
					throw new InvalidOperationException("You have to set an read-write password before setting a read-only password.");
				}
			}
			_cachedPermissions = null;
			Descriptor.ServiceImpl.SetReadOnlyPassword(Descriptor.Container, base.InternalId, password);
			_hasReadOnlyPassword = null;
		}

		public void Unlock(string password)
		{
			_cachedPermissions = PermissionConverter.Convert(Descriptor.Unlock(password));
		}

		public IList<TranslationMemoryFileAccessMode> GetValidAccessModes(string permission)
		{
			Permission permission2 = PermissionConverter.Convert(permission);
			List<TranslationMemoryFileAccessMode> list = new List<TranslationMemoryFileAccessMode>();
			if ((permission2 & Permission.ReadOnly) != 0)
			{
				list.Add(TranslationMemoryFileAccessMode.ReadOnly);
			}
			if ((permission2 & Permission.ReadWrite) != 0)
			{
				list.Add(TranslationMemoryFileAccessMode.ReadWrite);
			}
			if ((permission2 & Permission.Maintenance) != 0)
			{
				list.Add(TranslationMemoryFileAccessMode.Maintenance);
			}
			if ((permission2 & Permission.Administrator) != 0)
			{
				list.Add(TranslationMemoryFileAccessMode.Administrator);
			}
			return list;
		}

		public bool HasAccessModePermission(TranslationMemoryFileAccessMode accessMode)
		{
			Permission explicitPermissionsInAccessMode = PermissionConverter.GetExplicitPermissionsInAccessMode(accessMode);
			string[] array = PermissionConverter.Convert(explicitPermissionsInAccessMode);
			if (array == null || array.Length == 0)
			{
				return false;
			}
			return HasPermission(array[0]);
		}

		public override bool HasPermission(string permission)
		{
			string[] permissions = GetPermissions();
			return Array.IndexOf(permissions, permission) != -1;
		}

		private Permission GetPermissionsInternal()
		{
			return Descriptor.ServiceImpl.GetPermissions(Descriptor.Container, base.InternalId);
		}

		private string[] GetPermissions()
		{
			if (_cachedPermissions == null)
			{
				Permission permissionsInternal = GetPermissionsInternal();
				_cachedPermissions = PermissionConverter.Convert(permissionsInternal);
			}
			return _cachedPermissions;
		}

		public override void RefreshStatusInfo()
		{
			_StatusInfo = GetStatusInfo();
		}

		private ProviderStatusInfo GetStatusInfo()
		{
			try
			{
				ServerInfo serverInfo = base.Service.QueryServerInfo(base.Container);
				return new ProviderStatusInfo(serverInfo.Status == ServerInfo.ServerStatus.Running, serverInfo.StatusMessage);
			}
			catch (Exception ex)
			{
				return new ProviderStatusInfo(available: false, ex.Message);
			}
		}

		public override bool Equals(object obj)
		{
			FileBasedTranslationMemory fileBasedTranslationMemory = obj as FileBasedTranslationMemory;
			if (fileBasedTranslationMemory == null)
			{
				return false;
			}
			return new FileInfo(fileBasedTranslationMemory.FilePath).Equals(new FileInfo(FilePath));
		}

		public override int GetHashCode()
		{
			return FilePath.GetHashCode();
		}

		public TranslationModelFitness MeasureModelFitness(ref RegularIterator iterator, bool postdatedOrUnalignedOnly)
		{
			IAlignableTranslationMemoryService alignableTranslationMemoryService = base.Service as IAlignableTranslationMemoryService;
			AlignerDefinition alignerDefinition = alignableTranslationMemoryService?.GetAlignerDefinition(base.Container, base.Setup.ResourceId);
			if (alignerDefinition == null)
			{
				return null;
			}
			if (alignerDefinition.IsModelFree)
			{
				return null;
			}
			ModelBasedAlignerDefinition modelBasedAlignerDefinition = alignerDefinition as ModelBasedAlignerDefinition;
			return alignableTranslationMemoryService.MeasureModelFitness(base.Container, base.Setup.ResourceId, ref iterator, modelBasedAlignerDefinition.ModelId, postdatedOrUnalignedOnly);
		}

		public void ClearAlignmentData()
		{
			(base.Service as IAlignableTranslationMemoryService)?.ClearAlignmentData(base.Container, base.Setup.ResourceId);
		}

		public void SelectiveReindexTranslationUnits(CancellationToken token, IProgress<int> progress)
		{
			(base.Service as IReindexableTranslationMemoryService)?.SelectiveReindexTranslationUnits(base.Container, base.Setup.ResourceId, token, progress);
		}

		public bool AlignTranslationUnits(bool unalignedOnly, bool unalignedOrPostdatedOnly, ref RegularIterator iter)
		{
			IAlignableTranslationMemoryService alignableTranslationMemoryService = base.Service as IAlignableTranslationMemoryService;
			if (alignableTranslationMemoryService == null)
			{
				throw new Exception("No IAlignableTranslationMemoryService!");
			}
			return alignableTranslationMemoryService.AlignTranslationUnits(base.Container, base.Setup.ResourceId, unalignedOnly, unalignedOrPostdatedOnly, ref iter);
		}

		public void AlignTranslationUnits(bool unalignedOnly, bool unalignedOrPostdatedOnly, CancellationToken token, IProgress<int> progress)
		{
			IAlignableTranslationMemoryService alignableTranslationMemoryService = base.Service as IAlignableTranslationMemoryService;
			if (alignableTranslationMemoryService == null)
			{
				throw new Exception("No IAlignableTranslationMemoryService!");
			}
			alignableTranslationMemoryService.AlignTranslationUnits(base.Container, base.Setup.ResourceId, unalignedOnly, unalignedOrPostdatedOnly, token, progress);
		}

		public void BuildModel()
		{
			if (!CanBuildModel)
			{
				throw new Exception("Model cannot be built!");
			}
			ITranslationModelService translationModelService = base.Service as ITranslationModelService;
			if (translationModelService != null)
			{
				ModelBasedAlignerDefinition modelBasedAlignerDefinition = GetModelBasedAlignerDefinition(translationModelService);
				if (modelBasedAlignerDefinition != null)
				{
					API aPI = base.Service as API;
					try
					{
						aPI.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
						{
							OnProgress(args);
							_ = args.ProgressNumber % 1000;
						};
						translationModelService.BuildModel(base.Container, modelBasedAlignerDefinition.ModelId);
					}
					catch (FaultException)
					{
					}
				}
			}
		}

		public void ClearModel()
		{
			ITranslationModelService translationModelService = base.Service as ITranslationModelService;
			if (translationModelService != null)
			{
				ModelBasedAlignerDefinition modelBasedAlignerDefinition = GetModelBasedAlignerDefinition(translationModelService);
				if (modelBasedAlignerDefinition != null)
				{
					translationModelService.ClearModel(base.Container, modelBasedAlignerDefinition.ModelId);
				}
			}
		}

		private ModelBasedAlignerDefinition GetModelBasedAlignerDefinition(ITranslationModelService modelService)
		{
			AlignerDefinition alignerDefinition = (base.Service as IAlignableTranslationMemoryService)?.GetAlignerDefinition(base.Container, base.Setup.ResourceId);
			if (alignerDefinition == null)
			{
				return null;
			}
			if (alignerDefinition.IsModelFree)
			{
				return null;
			}
			return alignerDefinition as ModelBasedAlignerDefinition;
		}

		protected void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		protected void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			if (this.TranslationModelProgress != null)
			{
				this.TranslationModelProgress(this, progressEventArgs);
			}
		}
	}
}
