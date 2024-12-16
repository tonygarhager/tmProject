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

		internal Container Container => Descriptor.Container;

		internal PersistentObjectToken InternalId => Descriptor.Id;

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

		public string CreationUserName => Setup.CreationUser;

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

		public virtual Uri Uri => Descriptor.Uri;

		public abstract ProviderStatusInfo StatusInfo
		{
			get;
		}

		internal ITranslationMemoryService Service => Descriptor.Service;

		public string Name => Descriptor.Name;

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

		public virtual bool SupportsTaggedInput => true;

		public virtual bool SupportsScoring => true;

		public virtual bool SupportsSearchForTranslationUnits => true;

		public virtual bool SupportsMultipleResults => true;

		public virtual bool SupportsFilters => true;

		public virtual bool SupportsPenalties => true;

		public virtual bool SupportsStructureContext => true;

		public virtual bool SupportsDocumentSearches => true;

		public virtual bool SupportsUpdate => true;

		public virtual bool SupportsPlaceables => true;

		public virtual bool SupportsTranslation => true;

		public virtual bool SupportsFuzzySearch => true;

		public virtual bool SupportsConcordanceSearch => true;

		public virtual bool SupportsSourceConcordanceSearch => true;

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

		public virtual bool SupportsWordCounts => true;

		public virtual bool IsReadOnly => Setup.IsReadOnly;

		public TranslationMethod TranslationMethod => TranslationMethod.TranslationMemory;

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

		public ITranslationMemoryLanguageDirection LanguageDirection => _languageDirection;

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

		public int GetTranslationUnitCount()
		{
			return _languageDirection.GetTranslationUnitCount();
		}

		public abstract bool HasPermission(string permission);

		public abstract void RefreshStatusInfo();

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

		public void Save()
		{
			Save(null, CancellationToken.None);
		}

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

		public bool ShouldRecomputeFuzzyIndexStatistics()
		{
			return RecomputeStatisticsHelper.ShouldRecomputeFuzzyIndexStatistics(Setup.LastRecomputeSize, GetTranslationUnitCount());
		}

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			return _languageDirection.LanguageDirection.Equals(languageDirection);
		}

		public ITranslationMemoryLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			if (!languageDirection.Equals(_languageDirection.LanguageDirection))
			{
				return null;
			}
			return _languageDirection;
		}

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

		public virtual string SerializeState()
		{
			return null;
		}

		public virtual void LoadState(string translationProviderState)
		{
		}
	}
}
