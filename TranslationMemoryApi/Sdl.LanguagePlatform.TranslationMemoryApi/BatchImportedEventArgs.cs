using Sdl.LanguagePlatform.TranslationMemory;
using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Encapsulates the data published by <see cref="E:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryImporter.BatchImported" /> 
	/// progress events.
	/// </summary>
	public class BatchImportedEventArgs : CancelEventArgs
	{
		private readonly ImportStatistics _statistics;

		/// <summary>
		/// Gets the import statistics collected so far by the import process.
		/// </summary>
		public ImportStatistics Statistics => _statistics;

		/// <summary>
		/// Instantiates a new object using the provided import statistics.
		/// </summary>
		public BatchImportedEventArgs(ImportStatistics statistics)
		{
			_statistics = statistics;
		}
	}
}
