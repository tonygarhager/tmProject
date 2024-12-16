using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class BatchExportedEventArgs : CancelEventArgs
	{
		private readonly int _totalProcessed;

		private readonly int _totalExported;

		public int TotalExported => _totalExported;

		public int TotalProcessed => _totalProcessed;

		public BatchExportedEventArgs(int totalProcessed, int totalExported)
		{
			_totalProcessed = totalProcessed;
			_totalExported = totalExported;
		}
	}
}
