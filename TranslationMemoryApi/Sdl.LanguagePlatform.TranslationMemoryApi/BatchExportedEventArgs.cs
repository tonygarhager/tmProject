using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	///  Encapsulates data sent with events published by <see cref="E:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryExporter.BatchExported" />.
	/// </summary>
	public class BatchExportedEventArgs : CancelEventArgs
	{
		private readonly int _totalProcessed;

		private readonly int _totalExported;

		/// <summary>
		/// The total number of exported translation units. This is usually 
		/// equivalent to <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.BatchExportedEventArgs.TotalProcessed" />, but may be lower in case
		/// the export process uses filters.
		/// </summary>
		public int TotalExported => _totalExported;

		/// <summary>
		/// The total number of processed translation units. This is usually 
		/// equivalent to <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.BatchExportedEventArgs.TotalExported" />, but if the export process
		/// uses filters, the number of processed translation units may be lower.
		/// </summary>
		public int TotalProcessed => _totalProcessed;

		/// <summary>
		/// Instantiates a new event data object.
		/// </summary>
		/// <param name="totalProcessed">The total number of processed translation units</param>
		/// <param name="totalExported">The total number of exported translation units</param>
		public BatchExportedEventArgs(int totalProcessed, int totalExported)
		{
			_totalProcessed = totalProcessed;
			_totalExported = totalExported;
		}
	}
}
