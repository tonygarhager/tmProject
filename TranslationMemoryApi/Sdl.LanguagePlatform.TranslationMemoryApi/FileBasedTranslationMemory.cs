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
	/// <summary>
	/// Represents a bilingual file-based translation memory.
	/// </summary>
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

		/// <summary>
		/// Sets the user name on the container to override the default windows user ID
		/// </summary>
		public string UserName
		{
			set
			{
				Descriptor.Container.UserNameOverride = value;
			}
		}

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		public string FilePath
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether this instance has an administrator password.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has administrator password; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Gets a value indicating whether this instance has maintenance password.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has maintenance password; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Gets a value indicating whether this instance has read only password.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has read only password; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Gets a value indicating whether this instance has read write password.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has read write password; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Gets a value indicating whether this instance is protected one or more passwords.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is protected; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Determines whether the file based translation memory has been unlocked with sufficient permissions
		/// to be deemed available. When null, no attempt has been made to unlock the file based translation memory 
		/// and obtain sufficient permission. When false, an attempt has been made to unlock the file based translation
		/// memory and obtain sufficient permission that failed. When true, an attempt has been made to unlock the file 
		/// based translation memory and obtain sufficient permission that succeeded. If this is false then StatusInfo.
		/// Available will be false with a message "Not unlocked with sufficient permissions." This property should be
		/// set by the calling code after the user has been challenged.
		/// </summary>
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

		/// <summary>
		/// Gets the status info for the translation memory. A file-based translation memory is considered available if the
		/// translation memory file exists and if the translation memory has been unlocked by calling <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory.Unlock(System.String)" /> (in case
		/// the translation memory) is password protected.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the flags affecting tokenizer behaviour for this TM.
		/// <remarks>Note that changing tokenizer flags may require reindexing.</remarks>
		/// </summary>
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

		/// <summary>
		/// Gets or sets the flags affecting word count behaviour for this TM.
		/// </summary>
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

		/// <summary>
		/// Returns the status of fine-grained-alignment support for the TM
		/// </summary>
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

		/// <summary>
		/// Gets the number of TUs that do not have fine-grained alignment information.
		/// </summary>
		public int UnalignedTranslationUnitCount => (base.Service as IAlignableTranslationMemoryService)?.GetUnalignedTranslationUnitCount(base.Container, base.Setup.ResourceId) ?? 0;

		/// <summary>
		/// Gets the number of TUs that were added after the model used to align them was built, but
		/// before the latest model was built.
		/// </summary>
		public int AlignedPredatedTranslationUnitCount => (base.Service as IAlignableTranslationMemoryService)?.GetAlignedPredatedTranslationUnitCount(base.Container, base.Setup.ResourceId) ?? 0;

		/// <summary>
		/// Gets the number of TUs that have been added subsequent to the date of the translation model
		/// </summary>
		public int TranslationUnitNewerThanModelCount => (base.Service as IAlignableTranslationMemoryService)?.GetPostdatedTranslationUnitCount(base.Container, base.Setup.ResourceId) ?? 0;

		/// <summary>
		/// Returns true if any TUs require reindexing, based on the value of their tokenization_signature_hash column, false otherwise, or null if the TM is a legacy file-based TM that does not have this column.
		/// </summary>
		public bool? ReindexRequired => (base.Service as IReindexableTranslationMemoryService)?.GetReindexRequired(base.Container, base.Setup.ResourceId);

		/// <summary>
		/// Returns the number of TUs that require reindexing, based on the value of their tokenization_signature_hash column, or -1 if the TM is a legacy file-based TM that does not have this column.
		/// </summary>
		public int TuCountForReindex => (base.Service as IReindexableTranslationMemoryService)?.GetTuCountForReindex(base.Container, base.Setup.ResourceId) ?? (-1);

		/// <summary>
		/// Gets the AlignerDefinition that has been set for this TM, or null if there is none
		/// </summary>
		public AlignerDefinition AlignerDefinition => (base.Service as IAlignableTranslationMemoryService)?.GetAlignerDefinition(base.Container, base.Setup.ResourceId);

		/// <summary>
		/// Returns true for file-based TMs capable of reporting whether TUs require reindexing, or false for legacy TMs that do not support this capability.
		/// </summary>
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

		/// <summary>
		/// Returns true if the TM is using legacy segment hashing (and can therefore consume legacy context information in TMX without conversion)
		/// </summary>
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

		/// <summary>
		///  Returns true if the TM was created with support for ID-based context matching
		/// </summary>
		public bool UsesIdContextMatching => base.Setup.IdContextMatch;

		/// <summary>
		/// Returns the type of <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory.TextContextMatchType" /> specified when the TM was created
		/// </summary>
		public TextContextMatchType TextContextMatchType => base.Setup.TextContextMatchType;

		/// <summary>
		/// Indicates whether the TM has enough data for the translation model associated with it to be built
		/// </summary>
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

		/// <summary>
		/// Indicates whether 'quick' alignment (i.e. alignment of any unaligned 
		/// TUs, plus postdated TUs for which a newer model is now available)
		/// is recommended
		/// </summary>
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

		/// <summary>
		/// Provides details of the translation model associated with this file-based TM
		/// </summary>
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

		/// <summary>
		/// Indicates whether a build (or rebuild) of the translation model is recommended
		/// </summary>
		/// <remarks>Recommendation considers a significant amount of new content has been added to the TM since the model was built (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory.TranslationUnitNewerThanModelCount" /> </remarks>
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

		/// <summary>
		/// Reports the progress of building a translation model
		/// </summary>
		public event EventHandler<TranslationModelProgressEventArgs> TranslationModelProgress;

		/// <summary>
		/// Gets the file-based translation memory scheme.
		/// </summary>
		/// <returns>file-based translation memory scheme</returns>
		public static string GetFileBasedTranslationMemoryScheme()
		{
			return "sdltm.file";
		}

		/// <summary>
		/// Determines whether the given Uri represents a file-based translation memory.
		/// </summary>
		/// <param name="uri">The Uri to check.</param>
		/// <returns>Whether <paramref name="uri" /> represents file-based translation memory Uri.</returns>
		public static bool IsFileBasedTranslationMemory(Uri uri)
		{
			if (uri != null)
			{
				return uri.Scheme.Equals(GetFileBasedTranslationMemoryScheme(), StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		/// <summary>
		/// Gets the file-based translation memory Uri from the given file path.
		/// </summary>
		/// <param name="filePath">file path</param>
		/// <returns>file-based translation memory Uri</returns>
		public static Uri GetFileBasedTranslationMemoryUri(string filePath)
		{
			return new FileBasedTranslationMemoryDescriptor(filePath).Uri;
		}

		/// <summary>
		/// Gets the file-based translation memory file path from the given Uri.
		/// </summary>
		/// <param name="uri">A file-based translation memory Uri.</param>
		/// <returns>file-based translation memory file path</returns>
		/// <exception cref="T:System.ArgumentException">Thrown when <paramref name="uri" /> is not a file-based translation memory Uri.</exception>
		public static string GetFileBasedTranslationMemoryFilePath(Uri uri)
		{
			if (!IsFileBasedTranslationMemory(uri))
			{
				throw new ArgumentException("The specified Uri is not a file-based translation memory Uri.", "uri");
			}
			return new FileBasedTranslationMemoryDescriptor(uri).FilePath;
		}

		/// <summary>
		/// Gets the file-based translation memory file name (excluding the path) from the given Uri.
		/// </summary>
		/// <param name="uri">A file-based translation memory Uri.</param>
		/// <returns>The file-based translation memory file name, excluding the path.</returns>
		/// <exception cref="T:System.ArgumentException">Thrown when <paramref name="uri" /> is not a file-based translation memory Uri.</exception>
		public static string GetFileBasedTranslationMemoryName(Uri uri)
		{
			if (!IsFileBasedTranslationMemory(uri))
			{
				throw new ArgumentException("The specified Uri is not a file-based translation memory Uri.", "uri");
			}
			return Path.GetFileName(GetFileBasedTranslationMemoryFilePath(uri));
		}

		/// <summary>
		/// Opens an existing file-based translation memory.
		/// </summary>
		/// <param name="tmFilePath">The absolute path of the translation memory file.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="tmFilePath" /> is null or empty.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">Thrown if <paramref name="tmFilePath" /> does not exist.</exception>
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

		/// <summary>
		/// Opens an existing file-based translation memory given the Uri for a FileBasedTranslationMemory class.
		/// </summary>
		/// <param name="uri">The Uri of a FileBasedTranslationMemory class.</param>
		public FileBasedTranslationMemory(Uri uri)
			: base(new FileBasedTranslationMemoryDescriptor(uri))
		{
			FilePath = Descriptor.FilePath;
			if (!File.Exists(FilePath))
			{
				throw new FileNotFoundException();
			}
		}

		/// <summary>
		/// Opens an existing file-based translation memory and unlocks it with the specified password.
		/// </summary>
		/// <param name="tmFilePath">The absolute path of the translation memory file.</param>
		/// <param name="password">The password.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="tmFilePath" /> or <paramref name="password" /> are null or empty.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">Thrown if <paramref name="tmFilePath" /> does not exist.</exception>
		public FileBasedTranslationMemory(string tmFilePath, string password)
			: this(tmFilePath)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			Unlock(password);
		}

		/// <summary>
		/// Creates a new file-based translation memory.
		/// </summary>
		/// <param name="tmFilePath">The absolute path where the translation memory file should be created.</param>
		/// <param name="description">A description for the translation memory.</param>
		/// <param name="sourceLanguage">A region-qualified culture, representing the source language.</param>
		/// <param name="targetLanguage">A region-qualified culture, representing the target language.</param>
		/// <param name="indexes">The set of fuzzy indexes that should be created in this translation memory.</param>
		/// <param name="recognizers">Recognizer settings.</param>
		/// <param name="tokenizerFlags">Flags affecting tokenizer behaviour for this TM</param>
		/// <param name="wordCountFlags">Flags affecting word count behaviour for this TM</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="tmFilePath" />, <paramref name="description" />, <paramref name="sourceLanguage" /> or <paramref name="targetLanguage" /> is null or empty.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown when <paramref name="sourceLanguage" /> or <paramref name="targetLanguage" /> are not region-qualified cultures.</exception>
		public FileBasedTranslationMemory(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags)
			: this(tmFilePath, description, sourceLanguage, targetLanguage, indexes, recognizers, tokenizerFlags, wordCountFlags, supportsAlignmentData: false)
		{
		}

		/// <summary>
		/// Creates a new file-based translation memory.
		/// </summary>
		/// <param name="tmFilePath">The absolute path where the translation memory file should be created.</param>
		/// <param name="description">A description for the translation memory.</param>
		/// <param name="sourceLanguage">A region-qualified culture, representing the source language.</param>
		/// <param name="targetLanguage">A region-qualified culture, representing the target language.</param>
		/// <param name="indexes">The set of fuzzy indexes that should be created in this translation memory.</param>
		/// <param name="recognizers">Recognizer settings.</param>
		/// <param name="tokenizerFlags">Flags affecting tokenizer behaviour for this TM</param>
		/// <param name="wordCountFlags">Flags affecting word count behaviour for this TM</param>
		/// <param name="textContextMatchType">The type of text context matching the TM should use</param>
		/// <param name="usesIdContextMatch">If true, indicates the TM should support IdContexMatch</param>
		/// <param name="supportsAlignmentData">True if the translation memory should support fine-grained alignment of content, false otherwise</param>
		/// <param name="usesLegacyHashes">True if the translation memory should use legacy segment hashing, for data conversion purposes</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="tmFilePath" />, <paramref name="description" />, <paramref name="sourceLanguage" /> or <paramref name="targetLanguage" /> is null or empty.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown when <paramref name="sourceLanguage" /> or <paramref name="targetLanguage" /> are not region-qualified cultures.</exception>
		public FileBasedTranslationMemory(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, bool supportsAlignmentData, TextContextMatchType textContextMatchType, bool usesIdContextMatch, bool usesLegacyHashes)
			: base(new FileBasedTranslationMemoryDescriptor(tmFilePath, description, sourceLanguage, targetLanguage, indexes, recognizers, tokenizerFlags, wordCountFlags, supportsAlignmentData, textContextMatchType, usesIdContextMatch, usesLegacyHashes))
		{
			FilePath = tmFilePath;
		}

		/// <summary>
		/// Creates a new file-based translation memory.
		/// </summary>
		/// <param name="tmFilePath">The absolute path where the translation memory file should be created.</param>
		/// <param name="description">A description for the translation memory.</param>
		/// <param name="sourceLanguage">A region-qualified culture, representing the source language.</param>
		/// <param name="targetLanguage">A region-qualified culture, representing the target language.</param>
		/// <param name="indexes">The set of fuzzy indexes that should be created in this translation memory.</param>
		/// <param name="recognizers">Recognizer settings.</param>
		/// <param name="tokenizerFlags">Flags affecting tokenizer behaviour for this TM</param>
		/// <param name="wordCountFlags">Flags affecting word count behaviour for this TM</param>
		/// <param name="supportsAlignmentData">True if the translation memory should support fine-grained alignment of content, false otherwise</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown when <paramref name="tmFilePath" />, <paramref name="description" />, <paramref name="sourceLanguage" /> or <paramref name="targetLanguage" /> is null or empty.</exception>
		/// <exception cref="T:System.ArgumentException">Thrown when <paramref name="sourceLanguage" /> or <paramref name="targetLanguage" /> are not region-qualified cultures.</exception>
		public FileBasedTranslationMemory(string tmFilePath, string description, CultureInfo sourceLanguage, CultureInfo targetLanguage, FuzzyIndexes indexes, BuiltinRecognizers recognizers, TokenizerFlags tokenizerFlags, WordCountFlags wordCountFlags, bool supportsAlignmentData)
			: base(new FileBasedTranslationMemoryDescriptor(tmFilePath, description, sourceLanguage, targetLanguage, indexes, recognizers, tokenizerFlags, wordCountFlags, supportsAlignmentData))
		{
			FilePath = tmFilePath;
		}

		/// <summary>
		/// Determines whether a specified password is already in use on the TM.
		/// </summary>
		/// <param name="password">The password to check</param>
		/// <returns>The access mode that the password is set for, or null if the password is not already in use on the TM</returns>
		public TranslationMemoryFileAccessMode? IsPasswordSet(string password)
		{
			Permission permission = Descriptor.ServiceImpl.IsPasswordSet(Descriptor.Container, base.InternalId, password);
			if (permission == Permission.None)
			{
				return null;
			}
			return PermissionConverter.GetFileAccessMode(permission);
		}

		/// <summary>
		/// Sets the administrator password and unlocks the TM so the TM is open in administrator mode.
		/// </summary>
		/// <param name="password">The password; or null to remove the password restriction.
		/// Note that setting this password to null also sets all the other passwords to null (if they are currently set).</param>
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

		/// <summary>
		/// Sets the maintenance password.
		/// </summary>
		/// <remarks>This method just updates the password stored in the TM. It does not unlock the translation memory. Use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory.Unlock(System.String)" /> to do this.</remarks>
		/// <param name="password">The password; or null to remove the password restriction.
		/// Note that setting this password to null also sets all the read-write and read-only passwords to null (if they are currently set).</param>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to set a maintenance password without having set an administrator password.</exception>
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

		/// <summary>
		/// Sets the read write password.
		/// </summary>
		/// <remarks>This method just updates the password stored in the TM. It does not unlock the translation memory. Use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory.Unlock(System.String)" /> to do this.</remarks>
		/// <param name="password">The password; or null to remove the password restriction.
		/// Note that setting this password to null also sets the read-only passwords to null (if it is currently set).</param>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to set a read-write password without having set administrator and maintenance passwords.</exception>
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

		/// <summary>
		/// Sets the read only password.
		/// </summary>
		/// <remarks>This method just updates the password stored in the TM. It does not unlock the translation memory. Use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory.Unlock(System.String)" /> to do this.</remarks>
		/// <param name="password">The password; or null to remove the password restriction.</param>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to set a read-only password without having set administrator, maintenance and read-write passwords.</exception>
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

		/// <summary>
		/// Unlocks the translation memory with the specified password.
		/// </summary>
		public void Unlock(string password)
		{
			_cachedPermissions = PermissionConverter.Convert(Descriptor.Unlock(password));
		}

		/// <summary>
		/// Returns the list of access modes that are valid for the given permission.
		/// So if the password is set for one of the returned access modes, the specified permission will be granted.
		/// For example, if the permission is "batchedittu", the valid access modes would be Maintenance and Administrator. 
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Returns true if permission is granted to access the TM with the specified access mode.
		/// </summary>
		/// <param name="accessMode"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Checks whether the current user has the specified permission on this translation memory.
		/// </summary>
		/// <param name="permission">A permission ID. See <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryPermissions" />.</param>
		/// <returns>
		/// True if the user has the specified permission for this TM.
		/// </returns>
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

		/// <summary>
		/// Refreshes the current status information.
		/// </summary>
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

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">The <paramref name="obj" /> parameter is null.</exception>
		public override bool Equals(object obj)
		{
			FileBasedTranslationMemory fileBasedTranslationMemory = obj as FileBasedTranslationMemory;
			if (fileBasedTranslationMemory == null)
			{
				return false;
			}
			return new FileInfo(fileBasedTranslationMemory.FilePath).Equals(new FileInfo(FilePath));
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return FilePath.GetHashCode();
		}

		/// <summary>
		/// Measures how well the model 'fits' the TM content, by counting out-of-vocabulary words
		/// </summary>
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

		/// <summary>
		/// Deletes any fine-grained alignment data from the TM
		/// </summary>
		public void ClearAlignmentData()
		{
			(base.Service as IAlignableTranslationMemoryService)?.ClearAlignmentData(base.Container, base.Setup.ResourceId);
		}

		/// <summary>
		/// Provides similar functionality to ReindexTranslationUnits, except that only TUs that require reindexing are reindexed, based on the value of their tokenization_signature_hash column, or no TUs if the TM is a legacy file-based TM that does not have this column.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="progress"></param>
		public void SelectiveReindexTranslationUnits(CancellationToken token, IProgress<int> progress)
		{
			(base.Service as IReindexableTranslationMemoryService)?.SelectiveReindexTranslationUnits(base.Container, base.Setup.ResourceId, token, progress);
		}

		/// <summary>
		/// Performs fine-grained alignment on translation units
		/// </summary>
		/// <param name="unalignedOnly">If true, will only operate on translation units that do not already have any alignment information</param>
		/// <param name="unalignedOrPostdatedOnly">If true, will only operate on translation units that do not already have any alignment information or are postdated (see remarks). In this case, <paramref name="unalignedOnly" /> must be false.</param>
		/// <param name="iter"></param>
		/// <returns>True if there are more translation units to process, false otherwise</returns>
		/// <remarks>An aligned, postdated TU is one that has been aligned, but was added to the TM after the translation model used for alignment was built.</remarks>
		public bool AlignTranslationUnits(bool unalignedOnly, bool unalignedOrPostdatedOnly, ref RegularIterator iter)
		{
			IAlignableTranslationMemoryService alignableTranslationMemoryService = base.Service as IAlignableTranslationMemoryService;
			if (alignableTranslationMemoryService == null)
			{
				throw new Exception("No IAlignableTranslationMemoryService!");
			}
			return alignableTranslationMemoryService.AlignTranslationUnits(base.Container, base.Setup.ResourceId, unalignedOnly, unalignedOrPostdatedOnly, ref iter);
		}

		/// <summary>
		/// Performs bulk fine-grained alignment on translation units in a TM
		/// </summary>
		/// <param name="unalignedOnly">If true, will only operate on translation units that do not already have any alignment information</param>
		/// <param name="unalignedOrPostdatedOnly">If true, will only operate on translation units that do not already have any alignment information or are postdated (see remarks). In this case, <paramref name="unalignedOnly" /> must be false.</param>
		/// <param name="token">A CancellationToken whose CancellationTokenSource can be used to cancel the alignment</param>
		/// <param name="progress"></param>
		/// <remarks>An aligned, postdated TU is one that has been aligned, but was added to the TM after the translation model used for alignment was built.</remarks>
		public void AlignTranslationUnits(bool unalignedOnly, bool unalignedOrPostdatedOnly, CancellationToken token, IProgress<int> progress)
		{
			IAlignableTranslationMemoryService alignableTranslationMemoryService = base.Service as IAlignableTranslationMemoryService;
			if (alignableTranslationMemoryService == null)
			{
				throw new Exception("No IAlignableTranslationMemoryService!");
			}
			alignableTranslationMemoryService.AlignTranslationUnits(base.Container, base.Setup.ResourceId, unalignedOnly, unalignedOrPostdatedOnly, token, progress);
		}

		/// <summary>
		/// Builds the translation model associated with this file-based TM
		/// </summary>
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

		/// <summary>
		///
		/// </summary>
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

		/// <summary>
		///
		/// </summary>
		/// <param name="progressEventArgs"></param>
		protected void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="progressEventArgs"></param>
		protected void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			if (this.TranslationModelProgress != null)
			{
				this.TranslationModelProgress(this, progressEventArgs);
			}
		}
	}
}
