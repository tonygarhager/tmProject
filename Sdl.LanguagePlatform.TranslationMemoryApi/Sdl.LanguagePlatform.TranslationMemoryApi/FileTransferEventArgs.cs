using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class FileTransferEventArgs : CancelEventArgs
	{
		public long BytesTransferred
		{
			get;
			set;
		}

		public long TotalBytes
		{
			get;
			set;
		}

		public FileTransferEventArgs()
		{
		}

		public FileTransferEventArgs(long bytesTransferred, long totalBytes)
		{
			BytesTransferred = bytesTransferred;
			TotalBytes = totalBytes;
		}
	}
}
