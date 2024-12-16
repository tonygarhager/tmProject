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

		public FileInfo Source
		{
			get;
			set;
		}

		public bool RecomputeFuzzyIndex
		{
			get;
			set;
		}

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

		public string FileName => ImportEntity.SourceFile;

		public bool Deleted
		{
			get;
			private set;
		}

		public EventHandler<FileTransferEventArgs> UploadCancelEvent
		{
			get;
			set;
		}

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

		internal ScheduledServerTranslationMemoryImport(TranslationProviderServer server, ImportEntity entity)
			: base(server, entity)
		{
		}

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
			//if (base.Service.ServerVersion != TMServerVersion.Cloud && base.Service.ServerVersion != TMServerVersion.OnPremiseRest)
			{
				//base.Entity = base.Service.CreateTranslationMemoryImport(ImportEntity);
				Upload();
			}
			//base.Entity = base.Service.QueueTranslationMemoryImport(ImportEntity, Source.FullName, RecomputeFuzzyIndex);
			Refresh();
		}

		public sealed override void Refresh()
		{
			if (base.Entity.Id != null)
			{
				//base.Entity = base.Service.GetTranslationMemoryImportById(base.Entity.Id);
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
