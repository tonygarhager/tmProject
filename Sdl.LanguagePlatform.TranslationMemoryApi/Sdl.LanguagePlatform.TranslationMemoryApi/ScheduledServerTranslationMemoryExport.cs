using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class ScheduledServerTranslationMemoryExport : ImportExportOperationBase
	{
		internal ExportEntity ExportEntity
		{
			get
			{
				return (ExportEntity)base.Entity;
			}
			set
			{
				base.Entity = value;
			}
		}

		public EventHandler<FileTransferEventArgs> DownloadCancelEvent
		{
			get;
			set;
		}

		public FilterExpression FilterExpression
		{
			get
			{
				return ExportEntity.FilterExpression;
			}
			set
			{
				ExportEntity.FilterExpression = value;
			}
		}

		public int TranslationUnitsProcessed
		{
			get
			{
				if (!ExportEntity.Processed.HasValue)
				{
					return 0;
				}
				return ExportEntity.Processed.Value;
			}
		}

		public int TranslationUnitsExported
		{
			get
			{
				if (!ExportEntity.Exported.HasValue)
				{
					return 0;
				}
				return ExportEntity.Exported.Value;
			}
		}

		public bool IsStreamCompressed => false;//base.Service.ServerVersion == TMServerVersion.OnPremiseRest;

		private ScheduledServerTranslationMemoryExport()
		{
		}

		internal ScheduledServerTranslationMemoryExport(TranslationProviderServer server, ExportEntity entity)
			: base(server, entity)
		{
		}

		public ScheduledServerTranslationMemoryExport(ServerBasedTranslationMemoryLanguageDirection languageDirection)
			: base(languageDirection)
		{
			base.Entity = new ExportEntity
			{
				LanguageDirection = new EntityReference<LanguageDirectionEntity>(languageDirection.Entity),
				UniqueId = Guid.NewGuid(),
				ChunkSize = 25,
				ContinueOnError = true
			};
		}

		public override void Queue()
		{
			if (base.IsScheduled)
			{
				throw new InvalidOperationException("Export already queued.");
			}
			base.Entity = null;// base.Service.QueueTranslationMemoryExport(ExportEntity);
            Refresh();
		}

		public override void Refresh()
		{
			if (base.Entity.Id != null)
			{
				base.Entity = null;// base.Service.GetTranslationMemoryExportById(base.Entity.Id);
                base.ScheduledOperationEntity = base.Entity.ScheduledOperation;
			}
		}

		public bool DownloadExport(Stream destinationStream)
		{
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}/*
			using (Stream stream = base.Service.GetTranslationMemoryExportDownloadStream(ExportEntity))
			{
				long downloadLength = stream.Length;
				return ImportExportOperationBase.CopyStream(stream, destinationStream, delegate(long bytesRead)
				{
					EventHandler<FileTransferEventArgs> downloadCancelEvent = DownloadCancelEvent;
					if (downloadCancelEvent != null)
					{
						FileTransferEventArgs fileTransferEventArgs = new FileTransferEventArgs(bytesRead, downloadLength);
						downloadCancelEvent(this, fileTransferEventArgs);
						return !fileTransferEventArgs.Cancel;
					}
					return true;
				});
			}*/
			return true;
		}
	}
}
