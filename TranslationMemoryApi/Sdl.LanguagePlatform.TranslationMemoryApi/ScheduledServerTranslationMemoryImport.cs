using Sdl.Core.Api.DataAccess;
using Sdl.Enterprise2.Studio.Platform.Client.HttpStreaming;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the asynchronous import of a TMX, SDLIFF, ITD or TTX file into a server-based translation memory, executed on the Execution Server.
	/// Use this class to schedule an asynchronous translation memory import.
	/// </summary>    
	public class ScheduledServerTranslationMemoryImport : ImportExportOperationBase
	{
		internal ImportEntity ImportEntity
		{
			get
			{
				return (ImportEntity)base.Entity;
			}
			set
			{
				base.Entity = value;
			}
		}

		/// <summary>
		/// Gets or sets the file to import
		/// </summary>
		/// <value>The source.</value>
		public FileInfo Source
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value specifying whether the fuzzy index should be recomputed afer import.
		/// </summary>
		public bool RecomputeFuzzyIndex
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the import settings.
		/// </summary>
		/// <value>The import settings.</value>
		public ImportSettings ImportSettings
		{
			get
			{
				return ImportEntity.ImportSettings;
			}
			set
			{
				ImportEntity.ImportSettings = value;
			}
		}

		/// <summary>
		/// Gets the name of the import file.
		/// </summary>
		public string FileName => ImportEntity.SourceFile;

		/// <summary>
		/// True, if this instance has been deleted.
		/// </summary>
		public bool Deleted
		{
			get;
			private set;
		}

		/// <summary>
		/// The Upload CancelEventHandler can be used to abort the file upload.
		/// Only applicable to GroupShare 2015 or earlier
		/// </summary>
		public EventHandler<FileTransferEventArgs> UploadCancelEvent
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the current import statistics associated with the import.
		/// </summary>
		/// <value>The statistics.</value>
		public ImportStatistics Statistics => new ImportStatistics
		{
			AddedTranslationUnits = ImportEntity.Added.Value,
			DiscardedTranslationUnits = ImportEntity.Discarded.Value,
			Errors = ImportEntity.Errors.Value,
			MergedTranslationUnits = ImportEntity.Merged.Value,
			OverwrittenTranslationUnits = ImportEntity.Overwritten.Value,
			RawTUs = ImportEntity.RawTU.Value,
			TotalRead = ImportEntity.Read.Value,
			BadTranslationUnits = ImportEntity.Bad.Value
		};

		private ScheduledServerTranslationMemoryImport()
		{
		}

		/// <summary>
		/// Connect to an existing import.
		/// </summary>
		/// <param name="server">The server.</param>
		/// <param name="entity">The import entity.</param>
		internal ScheduledServerTranslationMemoryImport(TranslationProviderServer server, ImportEntity entity)
			: base(server, entity)
		{
		}

		/// <summary>
		/// Creates a new scheduled translation memory import.
		/// </summary>
		/// <param name="languageDirection">The translation memory language direction in which to import.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="languageDirection" /> is null.</exception>
		public ScheduledServerTranslationMemoryImport(ServerBasedTranslationMemoryLanguageDirection languageDirection)
			: base(languageDirection)
		{
			base.Entity = new ImportEntity
			{
				LanguageDirection = new EntityReference<LanguageDirectionEntity>(languageDirection.Entity),
				UniqueId = Guid.NewGuid(),
				ChunkSize = 25,
				ContinueOnError = true
			};
			ImportEntity.ImportSettings = new ImportSettings
			{
				IsDocumentImport = false,
				CheckMatchingSublanguages = false,
				IncrementUsageCount = false,
				NewFields = ImportSettings.NewFieldsOption.Ignore,
				PlainText = false
			};
		}

		/// <summary>
		/// Queues this instance.
		/// </summary>
		public sealed override void Queue()
		{
			if (base.IsScheduled)
			{
				throw new InvalidOperationException("Item previously queued");
			}
			if (ImportSettings == null)
			{
				throw new ArgumentNullException("ImportSettings");
			}
			if (Source == null)
			{
				throw new ArgumentNullException("Source");
			}
			if (base.Service.ServerVersion != TMServerVersion.Cloud && base.Service.ServerVersion != TMServerVersion.OnPremiseRest)
			{
				base.Entity = base.Service.CreateTranslationMemoryImport(ImportEntity);
				Upload();
			}
			base.Entity = base.Service.QueueTranslationMemoryImport(ImportEntity, Source.FullName, RecomputeFuzzyIndex);
			Refresh();
		}

		/// <summary>
		/// Refresh the underlying entity to update the current progress of the import.
		/// </summary>
		public sealed override void Refresh()
		{
			if (base.Entity.Id != null)
			{
				base.Entity = base.Service.GetTranslationMemoryImportById(base.Entity.Id);
				if (base.Entity != null)
				{
					base.ScheduledOperationEntity = base.Entity.ScheduledOperation;
				}
				else
				{
					Deleted = true;
				}
			}
		}

		private bool Upload()
		{
			if (base.IsScheduled)
			{
				throw new InvalidOperationException("Item previously queued");
			}
			if (Source == null)
			{
				throw new ArgumentNullException("Source");
			}
			string data = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", base.Entity.UniqueId.Value, Source.Name);
			HttpStreamingServiceClient httpStreamServiceProxy = new HttpStreamingServiceClient();
			using (FileStream from = Source.OpenRead())
			{
				using (UploadStream to = new UploadStream(httpStreamServiceProxy, "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Services.StreamHandler, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Services", data))
				{
					long sourceLength = Source.Length;
					return ImportExportOperationBase.CopyStream(from, to, delegate(long bytesRead)
					{
						EventHandler<FileTransferEventArgs> uploadCancelEvent = UploadCancelEvent;
						if (uploadCancelEvent != null)
						{
							FileTransferEventArgs fileTransferEventArgs = new FileTransferEventArgs(bytesRead, sourceLength);
							uploadCancelEvent(this, fileTransferEventArgs);
							return !fileTransferEventArgs.Cancel;
						}
						return true;
					});
				}
			}
		}
	}
}
