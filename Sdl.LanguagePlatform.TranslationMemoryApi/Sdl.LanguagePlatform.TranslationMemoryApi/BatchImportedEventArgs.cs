using Sdl.LanguagePlatform.TranslationMemory;
using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class BatchImportedEventArgs : CancelEventArgs
	{
		private readonly ImportStatistics _statistics;

		public ImportStatistics Statistics => _statistics;

		public BatchImportedEventArgs(ImportStatistics statistics)
		{
			_statistics = statistics;
		}
	}
}
