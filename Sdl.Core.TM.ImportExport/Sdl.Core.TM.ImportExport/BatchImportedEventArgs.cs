using Sdl.LanguagePlatform.TranslationMemory;
using System.ComponentModel;

namespace Sdl.Core.TM.ImportExport
{
	public class BatchImportedEventArgs : CancelEventArgs
	{
		public ImportStatistics Statistics
		{
			get;
		}

		public ImportResults BatchResults
		{
			get;
			set;
		}

		public BatchImportedEventArgs(ImportStatistics statistics)
		{
			Statistics = statistics;
		}
	}
}
