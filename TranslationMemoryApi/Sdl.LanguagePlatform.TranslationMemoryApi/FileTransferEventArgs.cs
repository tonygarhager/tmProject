using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Cancelable event args class used by events related to file upload and download.
	/// </summary>
	public class FileTransferEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Gets the number of bytes that have been transferred.
		/// </summary>
		public long BytesTransferred
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the total number of bytes.
		/// </summary>
		public long TotalBytes
		{
			get;
			set;
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public FileTransferEventArgs()
		{
		}

		/// <summary>
		/// Creates instance with bytes downloaded versus total bytes to download.
		/// </summary>
		/// <param name="bytesTransferred">The number of bytes that have been downloaded.</param>
		/// <param name="totalBytes">The total number of bytes that will be downloaded.</param>
		public FileTransferEventArgs(long bytesTransferred, long totalBytes)
		{
			BytesTransferred = bytesTransferred;
			TotalBytes = totalBytes;
		}
	}
}
