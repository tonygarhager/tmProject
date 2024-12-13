using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Abstract base class for bilingual file-based (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory" />) and in-memory translation memories (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.InMemoryTranslationMemory" />).
	/// Do not inherit from this class.
	/// </summary>
	public abstract class AbstractLocalTranslationMemory : ITranslationMemory, ITranslationProvider, ILocalTranslationMemory
	{
		private TranslationMemorySetup _LazySetup;

		private FieldDefinitionCollection _lazyFieldGroup;

		private LanguageResourceBundleCollection _lazyLanguageResourceBundles;

		private FileBasedTranslationMemoryLanguageDirection _languageDirection;

		private ReadOnlyCollection<LanguagePair> _supportedLanguageDirections;

		private FuzzyIndexTuningSettings _lazyFuzzyIndexTuningSettings;

		private readonly object _lockObject = new object();

		internal ITranslationMemoryDescriptor Descriptor
		{
			get;
		}

		/// <summary>
		/// Gets the container which contains the TM. This may be null if the container is unknown
		/// or has been passed to the service by other means.
		/// </summary>
		internal Container Container => Descriptor.Container;

		internal PersistentObjectToken InternalId => Descriptor.Id;

		/// <summary>
		/// Gets or sets the set of fuzzy indices defined on this TM.
		/// </summary>
		public FuzzyIndexes FuzzyIndexes
		{
			get
			{
				return Setup.FuzzyIndexes;
			}
			set
			{
				Setup.FuzzyIndexes = value;
			}
		}

		/// <summary>
		/// Gets or sets the recognizers which are enabled for this TM.
		/// <remarks>Note that changing recognizers may require reindexing. In addition, in
		/// some cases duplicate TUs may be in the TM if recognizers are enabled which have
		/// been disabled before.</remarks>
		/// </summary>
		public BuiltinRecognizers Recognizers
		{
			get
			{
				return Setup.Recognizers;
			}
			set
			{
				Setup.Recognizers = value;
			}
		}

		/// <summary>
		/// Gets or sets the copyright string for this translation memory.
		/// </summary>
		/// <value></value>
		/// <remarks>You have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.Save" /> in order to perist changes to this property.</remarks>
		public string Copyright
		{
			get
			{
				return Setup.Copyright;
			}
			set
			{
				Setup.Copyright = value;
			}
		}

		/// <summary>
		/// Gets the creation date of this translation memory.
		/// </summary>
		/// <value></value>
		public DateTime CreationDate
		{
			get
			{
				return Setup.CreationDate;
			}
			set
			{
				Setup.CreationDate = value;
			}
		}

		/// <summary>
		/// Gets the creation user of this translation memory.
		/// </summary>
		/// <value></value>
		public string CreationUserName => Setup.CreationUser;

		/// <summary>
		/// Gets or sets a general description of the translation memory.
		/// </summary>
		/// <value></value>
		/// <remarks>You have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.Save" /> in order to perist changes to this property.</remarks>
		public string Description
		{
			get
			{
				return Setup.Description;
			}
			set
			{
				Setup.Description = value;
			}
		}

		/// <summary>
		/// Gets or sets the expiration date for this translation memory.
		/// </summary>
		/// <value></value>
		/// <remarks>You have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.Save" /> in order to perist changes to this property.</remarks>
		public DateTime? ExpirationDate
		{
			get
			{
				return Setup.ExpirationDate;
			}
			set
			{
				Setup.ExpirationDate = value;
			}
		}

		/// <summary>
		/// Gets the custom fields defined for this TM.
		/// </summary>
		/// <value></value>
		/// <remarks>In case this is a server-based translation memory, which is associated with a fields template (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.IFieldsTemplate" />),
		/// this returns a read-only fields collection identical to the template's fields collection. In all other cases,
		/// the field collection returned can be modified. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.Save" /> to persist any changes made to
		/// the fields.</remarks>
		public FieldDefinitionCollection FieldDefinitions
		{
			get
			{
				lock (_lockObject)
				{
					if (_lazyFieldGroup == null)
					{
						_lazyFieldGroup = GetFields();
					}
				}
				return _lazyFieldGroup;
			}
		}

		/// <summary>
		/// Gets the language resources which are associated with this TM.
		/// </summary>
		/// <value></value>
		/// <remarks>In case this is a server-based translation memory, which is associated with a language resources template (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ILanguageResourcesTemplate" />),
		/// this returns a read-only language resources collection identical to the template's language resources collection. In all other cases,
		/// the language resources collection returned can be modified. Note that you have to call <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.Save" /> to persist any changes made to
		/// language resources.</remarks>
		public LanguageResourceBundleCollection LanguageResourceBundles
		{
			get
			{
				lock (_lockObject)
				{
					if (_lazyLanguageResourceBundles == null)
					{
						_lazyLanguageResourceBundles = GetLanguageResourceBundles();
					}
					return _lazyLanguageResourceBundles;
				}
			}
		}

		/// <summary>
		/// Gets a URI which uniquely identifies this translation memory.
		/// </summary>
		public virtual Uri Uri => Descriptor.Uri;

		/// <summary>
		/// Gets the status info for the provider.
		/// </summary>
		/// <value></value>
		public abstract ProviderStatusInfo StatusInfo
		{
			get;
		}

		/// <summary>
		/// The service instance which is currently used by the translation memory.
		/// </summary>
		internal ITranslationMemoryService Service => Descriptor.Service;

		/// <summary>
		/// Gets the name of the translation memory.
		/// </summary>
		public string Name => Descriptor.Name;

		/// <summary>
		/// Gets the setup of the translation memory. This is only valid once the TM has been opened. Note that 
		/// the setup in the database may have changed in the meantime. To ensure that you see the current
		/// status, use <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.Refresh" />.
		/// </summary>
		internal TranslationMemorySetup Setup
		{
			get
			{
				if (_LazySetup == null)
				{
					try
					{
						_LazySetup = Descriptor.GetTranslationMemorySetup();
						if (_LazySetup == null)
						{
							throw new LanguagePlatformException(ErrorCode.TMNotFound);
						}
					}
					catch (FaultException<FaultDescription> ex)
					{
						if (ex.Detail == null)
						{
							throw;
						}
						throw new LanguagePlatformException(ex.Detail);
					}
				}
				return _LazySetup;
			}
		}

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTaggedInput" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsTaggedInput => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsScoring" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsScoring => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSearchForTranslationUnits" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsSearchForTranslationUnits => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsMultipleResults" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsMultipleResults => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsFilters" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsFilters => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsPenalties" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsPenalties => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsStructureContext" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsStructureContext => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsDocumentSearches" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsDocumentSearches => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsUpdate" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsUpdate => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsPlaceables" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsPlaceables => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTranslation" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsTranslation => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsFuzzySearch" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsFuzzySearch => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsConcordanceSearch" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsConcordanceSearch => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsSourceConcordanceSearch" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsSourceConcordanceSearch => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsTargetConcordanceSearch" />. Returns <code>true</code>
		/// if this translation memory has a word-based fuzzy index for the target language (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.FuzzyIndexes" />).
		/// </summary>
		public virtual bool SupportsTargetConcordanceSearch
		{
			get
			{
				if ((Setup.FuzzyIndexes & FuzzyIndexes.TargetCharacterBased) == 0)
				{
					return (Setup.FuzzyIndexes & FuzzyIndexes.TargetWordBased) != 0;
				}
				return true;
			}
		}

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SupportsWordCounts" />. Always true for TMs.
		/// </summary>
		public virtual bool SupportsWordCounts => true;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.IsReadOnly" />. 
		/// </summary>
		public virtual bool IsReadOnly => Setup.IsReadOnly;

		/// <summary>
		/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.TranslationMethod" />. 
		/// Always returns <see cref="F:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMethod.TranslationMemory" />
		/// </summary>
		public TranslationMethod TranslationMethod => TranslationMethod.TranslationMemory;

		/// <summary>
		/// Gets or sets the fuzzy index tuning settings for the TM.
		/// </summary>
		/// <value></value>
		public FuzzyIndexTuningSettings FuzzyIndexTuningSettings
		{
			get
			{
				if (_lazyFuzzyIndexTuningSettings == null)
				{
					try
					{
						_lazyFuzzyIndexTuningSettings = Service.GetFuzzyIndexTuningSettings(Container, Setup.ResourceId);
					}
					catch (FaultException<FaultDescription> ex)
					{
						if (ex.Detail == null)
						{
							throw;
						}
						throw new LanguagePlatformException(ex.Detail);
					}
				}
				return _lazyFuzzyIndexTuningSettings;
			}
			set
			{
				_lazyFuzzyIndexTuningSettings = value;
			}
		}

		/// <summary>
		/// Gets the time of the last fuzzy index statistics recomputation of this TM, if available.
		/// </summary>
		/// <value></value>
		public DateTime? FuzzyIndexStatisticsRecomputedAt
		{
			get
			{
				return Setup.LastRecomputeDate;
			}
			set
			{
				Setup.LastRecomputeDate = value;
			}
		}

		/// <summary>
		/// Gets the size of the TM at the point of the last fuzzy index statistics recomputation,
		/// if available.
		/// </summary>
		/// <value></value>
		public int? FuzzyIndexStatisticsSize
		{
			get
			{
				return Setup.LastRecomputeSize;
			}
			set
			{
				Setup.LastRecomputeSize = value;
			}
		}

		/// <summary>
		/// Gets the one language direction contained in this translation memory.
		/// </summary>
		public ITranslationMemoryLanguageDirection LanguageDirection => _languageDirection;

		/// <summary>
		/// Gets the list of language directions which are supported by this translation memory.
		/// </summary>
		/// <value></value>
		public ReadOnlyCollection<LanguagePair> SupportedLanguageDirections
		{
			get
			{
				if (_supportedLanguageDirections == null)
				{
					List<LanguagePair> list = new List<LanguagePair>(1);
					list.Add(_languageDirection.LanguageDirection);
					_supportedLanguageDirections = new ReadOnlyCollection<LanguagePair>(list);
				}
				return _supportedLanguageDirections;
			}
		}

		internal AbstractLocalTranslationMemory(ITranslationMemoryDescriptor descriptor)
		{
			Descriptor = descriptor;
			_languageDirection = CreateFileBasedTranslationMemoryLanguageDirection();
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected virtual FileBasedTranslationMemoryLanguageDirection CreateFileBasedTranslationMemoryLanguageDirection()
		{
			return new FileBasedTranslationMemoryLanguageDirection(this);
		}

		private FieldDefinitionCollection GetFields()
		{
			FieldDefinitionCollection fieldDefinitionCollection = new FieldDefinitionCollection(new EntityCollection<FieldEntity>(), isReadOnly: false);
			ITranslationMemoryService service = Descriptor.Service;
			foreach (Field field in service.GetFields(Container, InternalId))
			{
				if (!field.IsSystemField)
				{
					fieldDefinitionCollection.Add(new FieldDefinition(field, isReadOnly: false));
				}
			}
			return fieldDefinitionCollection;
		}

		/// <summary>
		/// Gets the total translation unit count for all language directions in this translation memory.
		/// </summary>
		/// <returns>
		/// The total translation unit count for this TM.
		/// </returns>
		public int GetTranslationUnitCount()
		{
			return _languageDirection.GetTranslationUnitCount();
		}

		/// <summary>
		/// Checks whether the current user has the specified permission on this translation memory.
		/// </summary>
		/// <param name="permission">A permission ID. See <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryPermissions" />.</param>
		/// <returns>
		/// True if the user has the speicfied permission for this TM.
		/// </returns>
		public abstract bool HasPermission(string permission);

		/// <summary>
		/// Refreshes the current status information.
		/// </summary>
		public abstract void RefreshStatusInfo();

		/// <summary>
		/// Enforces the currently stored setup information to be refreshed. Note that this
		/// may initiate a full server/database roundtrip.
		/// </summary>
		public void Refresh()
		{
			try
			{
				_LazySetup = Descriptor.GetTranslationMemorySetup();
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
			if (_lazyFieldGroup != null)
			{
				RefreshFields(_lazyFieldGroup);
			}
			if (_lazyLanguageResourceBundles != null)
			{
				_lazyLanguageResourceBundles = GetLanguageResourceBundles();
			}
		}

		/// <summary>
		/// Saves changes made to properties of this translation memory.
		/// </summary>
		public void Save()
		{
			Save(null, CancellationToken.None);
		}

		/// <summary>
		/// Saves changes made to properties of this translation memory.
		/// </summary>
		public void Save(IProgress<TranslationMemoryProgress> progress, CancellationToken cancellationToken)
		{
			try
			{
				Service.ChangeTranslationMemory(Container, Setup, progress, cancellationToken);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
			if (_lazyFieldGroup != null)
			{
				SaveFields(_lazyFieldGroup);
			}
			if (_lazyLanguageResourceBundles != null)
			{
				_lazyLanguageResourceBundles.SaveToEntities();
				SaveLanguageResources(_lazyLanguageResourceBundles.Entities);
			}
			if (_lazyFuzzyIndexTuningSettings != null)
			{
				try
				{
					Service.SetFuzzyIndexTuningSettings(Container, Setup.ResourceId, _lazyFuzzyIndexTuningSettings);
				}
				catch (FaultException<FaultDescription> ex2)
				{
					if (ex2.Detail == null)
					{
						throw;
					}
					throw new LanguagePlatformException(ex2.Detail);
				}
			}
		}

		/// <summary>
		/// Deletes this translation memory.
		/// </summary>
		/// <remarks>In the case of a file-based translation memory, this deletes the translation memory file itself.</remarks>
		public void Delete()
		{
			try
			{
				Service.DeleteTranslationMemory(Container, Setup.ResourceId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Synchronously recomputes the fuzzy index statistics of the TM.
		/// </summary>
		public void RecomputeFuzzyIndexStatistics()
		{
			try
			{
				Service.RecomputeStatistics(Container, Setup.ResourceId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Clear the fuzzy cache of the TM
		/// </summary>
		public void ClearFuzzyCache()
		{
			try
			{
				Service.ClearFuzzyCache(Container, Setup.ResourceId);
			}
			catch (FaultException<FaultDescription> ex)
			{
				if (ex.Detail == null)
				{
					throw;
				}
				throw new LanguagePlatformException(ex.Detail);
			}
		}

		/// <summary>
		/// Gets a flag which indicates whether it is recommended to recompute the fuzzy
		/// index statistics (see <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.RecomputeFuzzyIndexStatistics" />).
		/// </summary>
		/// <returns></returns>
		public bool ShouldRecomputeFuzzyIndexStatistics()
		{
			return RecomputeStatisticsHelper.ShouldRecomputeFuzzyIndexStatistics(Setup.LastRecomputeSize, GetTranslationUnitCount());
		}

		/// <summary>
		/// Checks whether this translation provider supports the specified language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>
		/// True if the specified language direction is supported.
		/// </returns>
		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			return _languageDirection.LanguageDirection.Equals(languageDirection);
		}

		/// <summary>
		/// Gets a specified translation memory language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>
		/// A translation provider for the specified language direction, or null if no language direction matches.
		/// </returns>
		public ITranslationMemoryLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			if (!languageDirection.Equals(_languageDirection.LanguageDirection))
			{
				return null;
			}
			return _languageDirection;
		}

		/// <summary>
		/// Gets a translation provider for the specified language direction.
		/// </summary>
		/// <param name="languageDirection">The language direction.</param>
		/// <returns>
		/// The language direction matching the given source and target language.
		/// </returns>
		ITranslationProviderLanguageDirection ITranslationProvider.GetLanguageDirection(LanguagePair languageDirection)
		{
			return GetLanguageDirection(languageDirection);
		}

		private LanguageResourceBundleCollection GetLanguageResourceBundles()
		{
			EntityCollection<LanguageResourceEntity> entityCollection = new EntityCollection<LanguageResourceEntity>();
			LanguageResource[] languageResources = Descriptor.Service.GetLanguageResources(Container, includeData: true);
			LanguageResource[] array = languageResources;
			foreach (LanguageResource languageResource in array)
			{
				LanguageResourceEntity languageResourceEntity = new LanguageResourceEntity();
				languageResourceEntity.Id = new Identity(languageResource.ResourceId.Id);
				languageResourceEntity.UniqueId = languageResource.ResourceId.Guid;
				languageResourceEntity.Type = languageResource.Type;
				languageResourceEntity.CultureName = languageResource.CultureName;
				languageResourceEntity.Data = languageResource.Data;
				languageResourceEntity.MarkAsClean();
				entityCollection.Add(languageResourceEntity);
			}
			return new LanguageResourceBundleCollection(entityCollection);
		}

		private void SaveLanguageResources(EntityCollection<LanguageResourceEntity> languageResources)
		{
			LanguageResource[] languageResources2 = Descriptor.Service.GetLanguageResources(Container, includeData: false);
			LanguageResource[] array = languageResources2;
			foreach (LanguageResource tmLanguageResource in array)
			{
				if (languageResources.FirstOrDefault((LanguageResourceEntity lr) => lr.Id != null && lr.Id.Value != null && (int)lr.Id.Value == tmLanguageResource.ResourceId.Id) == null)
				{
					Descriptor.Service.UnassignLanguageResourceFromTranslationMemory(Container, tmLanguageResource.ResourceId, InternalId);
					Descriptor.Service.DeleteLanguageResource(Container, tmLanguageResource.ResourceId);
				}
			}
			foreach (LanguageResourceEntity languageResource in languageResources)
			{
				LanguageResource languageResource2 = languageResources2.FirstOrDefault((LanguageResource e) => languageResource.Id != null && languageResource.Id.Value != null && (int)languageResource.Id.Value == e.ResourceId.Id);
				if (languageResource2 == null)
				{
					PersistentObjectToken persistentObjectToken = Descriptor.Service.CreateLanguageResource(Container, ToLanguageResource(languageResource));
					languageResource.Id = new Identity(persistentObjectToken.Id);
					languageResource.UniqueId = persistentObjectToken.Guid;
					Descriptor.Service.AssignLanguageResourceToTranslationMemory(Container, persistentObjectToken, InternalId);
				}
				else if (languageResource.IsDirty)
				{
					Descriptor.Service.UpdateLanguageResource(Container, ToLanguageResource(languageResource));
				}
				languageResource.MarkAsClean();
			}
		}

		private static LanguageResource ToLanguageResource(LanguageResourceEntity entity)
		{
			int id = (entity.Id != null && entity.Id.Value != null) ? ((int)entity.Id.Value) : 0;
			return new LanguageResource
			{
				ResourceId = new PersistentObjectToken(id, entity.UniqueId.Value),
				Type = entity.Type.Value,
				CultureName = entity.CultureName,
				Data = entity.Data
			};
		}

		private void RefreshFields(FieldDefinitionCollection fields)
		{
			while (fields.Entities.Count > 0)
			{
				fields.Entities.RemoveLocal(fields.Entities[0]);
			}
			fields.Clear();
			foreach (FieldDefinition field in GetFields())
			{
				fields.Add(field);
			}
		}

		private void SaveFields(FieldDefinitionCollection fields)
		{
			TMFieldUpdater tMFieldUpdater = new TMFieldUpdater(Service, Container, InternalId, synchronizeIds: true);
			tMFieldUpdater.UpdateTmFields(GetFields().Entities, fields.Entities);
		}

		/// <summary>
		/// Serializes any meaningful state information for this translation provider that can be stored in projects
		/// and sent around the supply chain.
		/// </summary>
		/// <returns>
		/// A string representing the state of this translation provider that can later be passed into
		/// the <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.LoadState(System.String)" /> method to restore the state after creating a new translation provider.
		/// </returns>
		/// <remarks>The format of this string can be decided upon by the translation provider implementation.</remarks>
		public virtual string SerializeState()
		{
			return null;
		}

		/// <summary>
		/// Loads previously serialized state information into this translation provider instance.
		/// </summary>
		/// <param name="translationProviderState">A string representing the state of translation provider that was previously saved
		/// using <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.AbstractLocalTranslationMemory.SerializeState" />.</param>
		/// <remarks>The format of this string can be decided upon by the translation provider implementation.</remarks>
		public virtual void LoadState(string translationProviderState)
		{
		}
	}
}
