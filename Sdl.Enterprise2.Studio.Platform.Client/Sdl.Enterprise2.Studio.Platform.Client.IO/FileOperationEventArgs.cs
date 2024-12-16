using System.ComponentModel;

namespace Sdl.Enterprise2.Studio.Platform.Client.IO
{
	public class FileOperationEventArgs : CancelEventArgs
	{
		public long BytesTransferred
		{
			get;
			private set;
		}

		public long TotalBytes
		{
			get;
			private set;
		}

		public FileOperationEventArgs(long bytesTransferred, long totalBytes)
		{
			BytesTransferred = bytesTransferred;
			TotalBytes = totalBytes;
		}

		public FileOperationEventArgs(long bytesTransferred, long totalBytes, bool cancel)
			: base(cancel)
		{
			BytesTransferred = bytesTransferred;
			TotalBytes = totalBytes;
		}
	}
}
