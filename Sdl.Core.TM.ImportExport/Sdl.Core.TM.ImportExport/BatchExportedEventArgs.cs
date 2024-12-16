using System.ComponentModel;

namespace Sdl.Core.TM.ImportExport
{
	public class BatchExportedEventArgs : CancelEventArgs
	{
		public int TotalExported
		{
			get;
		}

		public int TotalProcessed
		{
			get;
		}

		public BatchExportedEventArgs(int totalProcessed, int totalExported)
		{
			TotalProcessed = totalProcessed;
			TotalExported = totalExported;
		}
	}
}
