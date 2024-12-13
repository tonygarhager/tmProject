using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the asynchronous export of translation units from a server-based translation memory, executed on the Execution Server.
	/// Use this class to schedule an asynchronous translation memory export.
	/// </summary>    
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

		/// <summary>
		/// The download CancelEventHandler can be used to abort the file download.
		/// </summary>
		public EventHandler<FileTransferEventArgs> DownloadCancelEvent
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the filter expression which will be used during the export.
		/// </summary>
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

		/// <summary>
		/// Gets a count of the number of translation units which have been processed.
		/// </summary>
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

		/// <summary>
		/// Gets a count of the number of translation units which have been exported.
		/// </summary>
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

		/// <summary>
		/// True when the stream is compressed.
		/// </summary>
		public bool IsStreamCompressed => base.Service.ServerVersion == TMServerVersion.OnPremiseRest;

		private ScheduledServerTranslationMemoryExport()
		{
		}

		/// <summary>
		/// Connect to an existing import.
		/// </summary>
		/// <param name="server">The server.</param>
		/// <param name="entity">The import entity.</param>
		internal ScheduledServerTranslationMemoryExport(TranslationProviderServer server, ExportEntity entity)
			: base(server, entity)
		{
		}

		/// <summary>
		/// Creates a new scheduled translation memory import.
		/// </summary>
		/// <param name="languageDirection">The translation memory language direction in which to import.</param>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="languageDirection" /> is null.</exception>
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

		/// <summary>
		/// Queues the operation for execution on the Execution Server.
		/// </summary>
		public override void Queue()
		{
			if (base.IsScheduled)
			{
				throw new InvalidOperationException("Export already queued.");
			}
			base.Entity = base.Service.QueueTranslationMemoryExport(ExportEntity);
			Refresh();
		}

		/// <summary>
		/// Refreshes the status of the operation.
		/// </summary>
		public override void Refresh()
		{
			if (base.Entity.Id != null)
			{
				base.Entity = base.Service.GetTranslationMemoryExportById(base.Entity.Id);
				base.ScheduledOperationEntity = base.Entity.ScheduledOperation;
			}
		}

		/// <summary>
		/// Downloads the export.
		/// </summary>
		public bool DownloadExport(Stream destinationStream)
		{
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}
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
			}
		}
	}
}
