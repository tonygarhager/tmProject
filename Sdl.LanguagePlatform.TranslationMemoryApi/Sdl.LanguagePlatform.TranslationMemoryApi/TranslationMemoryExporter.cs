using Sdl.Core.TM.ImportExport;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class TranslationMemoryExporter : Exporter
	{
		public const int DefaultTranslationUnitChunkSize = 25;

		public const int MaxTranslationUnitChunkSize = 200;

		private int _chunkSize = 25;

		public int ChunkSize
		{
			get
			{
				return _chunkSize;
			}
			set
			{
				if (value <= 0)
				{
					_chunkSize = 25;
				}
				else if (value > 200)
				{
					_chunkSize = 200;
				}
				else
				{
					_chunkSize = value;
				}
			}
		}

		public ITranslationMemoryLanguageDirection TranslationMemoryLanguageDirection
		{
			get;
			set;
		}

		public new event EventHandler<BatchExportedEventArgs> BatchExported;

		public TranslationMemoryExporter()
		{
			base.BatchExported += Base_BatchExported;
		}

		private void Base_BatchExported(object sender, Sdl.Core.TM.ImportExport.BatchExportedEventArgs e)
		{
			EventHandler<BatchExportedEventArgs> batchExported = this.BatchExported;
			if (batchExported != null)
			{
				BatchExportedEventArgs batchExportedEventArgs = new BatchExportedEventArgs(e.TotalProcessed, e.TotalExported);
				batchExported(this, batchExportedEventArgs);
				if (batchExportedEventArgs.Cancel)
				{
					e.Cancel = true;
				}
			}
		}

		public TranslationMemoryExporter(ITranslationMemoryLanguageDirection languageDirection)
			: this()
		{
			TranslationMemoryLanguageDirection = languageDirection;
		}

		public void Export(string fileName, bool overwriteExisting)
		{
			TranslationMemoryLanguageDirectionImportExport exportOrigin = new TranslationMemoryLanguageDirectionImportExport(TranslationMemoryLanguageDirection);
			Export(exportOrigin, fileName, overwriteExisting);
		}
	}
}
