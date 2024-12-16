using System;
using System.IO;

namespace Sdl.Enterprise2.MultiTerm.Client.IO
{
	public interface IFileOperation
	{
		Guid Id
		{
			get;
		}

		Stream Stream
		{
			get;
		}

		bool UseCompression
		{
			get;
		}

		void Upload(string sourcePath, EventHandler<FileOperationEventArgs> progressEventHandler);

		void Upload(Stream sourceStream, EventHandler<FileOperationEventArgs> progressEventHandler);

		void Download(string destinationPath, EventHandler<FileOperationEventArgs> progressEventHandler);

		void Download(Stream destinationStream, EventHandler<FileOperationEventArgs> progressEventHandler);

		void Cancel();

		Guid? Complete();
	}
}
