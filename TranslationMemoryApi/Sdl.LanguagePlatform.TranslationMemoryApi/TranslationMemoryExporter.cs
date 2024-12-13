using Sdl.Core.TM.ImportExport;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// This class implements exporting translation units into TMX files.
	/// </summary>
	public class TranslationMemoryExporter : Exporter
	{
		/// <summary>
		/// Defines the default size for a chunk of translation units to be processed in one roundtrip through the import/export process.
		/// <value>The default chunk size is 50.</value>
		/// </summary>
		public const int DefaultTranslationUnitChunkSize = 25;

		/// <summary>
		/// Defines the maximum allowed chunk size.
		/// <value>The maximum allowed chunk size is 200.</value>
		/// </summary>
		public const int MaxTranslationUnitChunkSize = 200;

		private int _chunkSize = 25;

		/// <summary>
		/// Gets or sets the size of the translation unit chunk size
		/// </summary>
		/// <value>The size of the chunk.</value>
		/// <remarks>The exporter implementation has always ignored this value. It is left in this class following refactoring, purely to retain binary compatibility</remarks>
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

		/// <summary>
		/// Gets or sets the translation memory language direction.
		/// </summary>
		/// <value>The translation memory language direction.</value>
		public ITranslationMemoryLanguageDirection TranslationMemoryLanguageDirection
		{
			get;
			set;
		}

		/// <summary>
		/// Occurs when ever a batch is imported, set Cancel = True to stop the export.
		/// </summary>
		/// <remarks>Hides the base class event (which our handler re-fires through this one) so that existing client code will see no namespace change after refactoring</remarks>
		public new event EventHandler<BatchExportedEventArgs> BatchExported;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryExporter" /> class.
		/// </summary>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryExporter" /> class with the specified languageDirection.
		/// </summary>
		/// <param name="languageDirection">The translation memory language direction to export from.</param>
		public TranslationMemoryExporter(ITranslationMemoryLanguageDirection languageDirection)
			: this()
		{
			TranslationMemoryLanguageDirection = languageDirection;
		}

		/// <summary>
		/// Starts the export process with the configured settings into a file with the specified name.
		/// <remarks>This method supports GZip compressed files, such as: export.tmx.gz</remarks>
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="overwriteExisting">if set to <c>true</c> [overwrite existing].</param>
		public void Export(string fileName, bool overwriteExisting)
		{
			TranslationMemoryLanguageDirectionImportExport exportOrigin = new TranslationMemoryLanguageDirectionImportExport(TranslationMemoryLanguageDirection);
			Export(exportOrigin, fileName, overwriteExisting);
		}
	}
}
