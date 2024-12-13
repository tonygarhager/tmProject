using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.ObjectModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Re
	/// </summary>
	public class CloudBasedTranslationMemory : RemoteTranslationMemory, ITranslationMemory2015, ITranslationMemory, ITranslationProvider
	{
		internal CloudBasedTranslationMemoryLanguageDirectionCollection _lazyLanguageDirections;

		/// <summary>
		///
		/// </summary>
		public CloudTranslationProvider Provider
		{
			get;
			private set;
		}

		string ITranslationMemory.CreationUserName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		DateTime? ITranslationMemory.ExpirationDate
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		FieldDefinitionCollection ITranslationMemory.FieldDefinitions
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		LanguageResourceBundleCollection ITranslationMemory.LanguageResourceBundles
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		FuzzyIndexTuningSettings ITranslationMemory.FuzzyIndexTuningSettings
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		DateTime? ITranslationMemory.FuzzyIndexStatisticsRecomputedAt
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		int? ITranslationMemory.FuzzyIndexStatisticsSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets this list of language directions for this translation memory.
		/// </summary>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public CloudBasedTranslationMemoryLanguageDirectionCollection LanguageDirections
		{
			get
			{
				VerifyNotDeleted();
				if (_lazyLanguageDirections == null)
				{
					LanguageDirectionEntity[] languageDirectionsByTranslationMemoryId = Provider.Service.GetLanguageDirectionsByTranslationMemoryId(base.Entity.Id, new string[0]);
					base.Entity.LanguageDirections = new EntityCollection<LanguageDirectionEntity>(languageDirectionsByTranslationMemoryId);
					_lazyLanguageDirections = new CloudBasedTranslationMemoryLanguageDirectionCollection(this, base.Entity.LanguageDirections);
				}
				return _lazyLanguageDirections;
			}
		}

		ReadOnlyCollection<LanguagePair> ITranslationMemory.SupportedLanguageDirections
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ProviderStatusInfo ITranslationProvider.StatusInfo
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		Uri ITranslationProvider.Uri
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsTaggedInput
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsScoring
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsSearchForTranslationUnits
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsMultipleResults
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsFilters
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsPenalties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsStructureContext
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsDocumentSearches
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsUpdate
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsPlaceables
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsTranslation
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsFuzzySearch
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsConcordanceSearch
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsSourceConcordanceSearch
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsTargetConcordanceSearch
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.SupportsWordCounts
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		TranslationMethod ITranslationProvider.TranslationMethod
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ITranslationProvider.IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		TokenizerFlags ITranslationMemory2015.TokenizerFlags
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		WordCountFlags ITranslationMemory2015.WordCountFlags
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Creates a new instance of a Cloud based translation memo
		/// </summary>
		/// <param name="provider"></param>
		public CloudBasedTranslationMemory(CloudTranslationProvider provider)
			: base(new TranslationMemoryEntity())
		{
			Provider = provider;
		}

		internal CloudBasedTranslationMemory(CloudTranslationProvider provider, TranslationMemoryEntity entity)
			: base(entity)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.Id == null || entity.Id.Value == null)
			{
				throw new ArgumentException(StringResources.DatabaseServer_EntityHasNoId, "entity");
			}
			Provider = provider;
		}

		internal static CloudBasedTranslationMemory BuildCloudBasedTranslationMemory(ClientObjectBuilder<CloudTranslationProvider> builder, TranslationMemoryEntity entity)
		{
			ClientObjectKey key = builder.CreateKey(entity);
			CloudBasedTranslationMemory cloudBasedTranslationMemory = builder[key] as CloudBasedTranslationMemory;
			if (cloudBasedTranslationMemory != null)
			{
				return cloudBasedTranslationMemory;
			}
			cloudBasedTranslationMemory = (CloudBasedTranslationMemory)(builder[key] = new CloudBasedTranslationMemory(builder.Server, entity));
			if (entity.LanguageDirections.IsLoaded)
			{
				cloudBasedTranslationMemory._lazyLanguageDirections = new CloudBasedTranslationMemoryLanguageDirectionCollection(cloudBasedTranslationMemory, entity.LanguageDirections);
			}
			return cloudBasedTranslationMemory;
		}

		/// <summary>
		/// Saves the changes to this translation memory. This includes
		/// changes to language directions, fields and language resources.
		/// </summary>
		/// <exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ObjectDeletedException">Thrown when this object has been deleted.</exception>
		public void Save()
		{
			if (base.IsNewObject)
			{
				TranslationMemoryEntity translationMemoryEntity = Provider.Service.CreateTranslationMemory(base.Entity, "/");
				base.Entity.Id = translationMemoryEntity.Id;
				base.Entity.UniqueId = translationMemoryEntity.UniqueId;
				base.Entity.CreationDate = translationMemoryEntity.CreationDate;
				base.Entity.CreationUser = translationMemoryEntity.CreationUser;
			}
			else
			{
				TranslationMemoryEntity translationMemoryEntity = Provider.Service.UpdateTranslationMemory(base.Entity);
			}
		}

		/// <summary>
		/// Deletes this translation memory.
		/// </summary>
		/// <remarks>In the case of a file-based translation memory, this deletes the translation memory file itself. 
		/// Server-based translation memories are deleted from the server, including all their content.</remarks>
		public void Delete()
		{
			VerifyNotDeleted();
			Provider.Service.DeleteTranslationMemory(base.Entity.Id);
			base.Entity = null;
		}

		bool ITranslationMemory.HasPermission(string permission)
		{
			throw new NotImplementedException();
		}

		int ITranslationMemory.GetTranslationUnitCount()
		{
			throw new NotImplementedException();
		}

		bool ITranslationMemory.ShouldRecomputeFuzzyIndexStatistics()
		{
			throw new NotImplementedException();
		}

		void ITranslationMemory.RecomputeFuzzyIndexStatistics()
		{
			throw new NotImplementedException();
		}

		bool ITranslationProvider.SupportsLanguageDirection(LanguagePair languageDirection)
		{
			throw new NotImplementedException();
		}

		ITranslationMemoryLanguageDirection ITranslationMemory.GetLanguageDirection(LanguagePair languageDirection)
		{
			throw new NotImplementedException();
		}

		ITranslationProviderLanguageDirection ITranslationProvider.GetLanguageDirection(LanguagePair languageDirection)
		{
			throw new NotImplementedException();
		}

		void ITranslationProvider.RefreshStatusInfo()
		{
			throw new NotImplementedException();
		}

		string ITranslationProvider.SerializeState()
		{
			throw new NotImplementedException();
		}

		void ITranslationProvider.LoadState(string translationProviderState)
		{
			throw new NotImplementedException();
		}
	}
}
