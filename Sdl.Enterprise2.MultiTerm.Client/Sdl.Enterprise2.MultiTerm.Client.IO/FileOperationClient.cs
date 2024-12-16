using System;
using System.IO;

namespace Sdl.Enterprise2.MultiTerm.Client.IO
{
	public sealed class FileOperationClient : IFileOperation
	{
		private readonly Guid _FileOperationId;

		private readonly FileOperationStream _Stream;

		private readonly Uri _ServerUri;

		private readonly bool _UseCompression;

		public Guid Id => _FileOperationId;

		public Stream Stream => _Stream;

		public bool UseCompression => _UseCompression;

		public FileOperationClient(Guid operationId, FileAccess fileAccess)
			: this(null, operationId, fileAccess)
		{
		}

		public FileOperationClient(Uri serverUri, Guid operationId, FileAccess fileAccess)
		{
			_ServerUri = serverUri;
			_FileOperationId = operationId;
			_UseCompression = false;
			_Stream = new FileOperationStream(_ServerUri, Id, fileAccess);
		}

		public FileOperationClient(Uri serverUri, Guid operationId, FileAccess fileAccess, bool useCompression)
		{
			_ServerUri = serverUri;
			_FileOperationId = operationId;
			_UseCompression = useCompression;
			_Stream = new FileOperationStream(_ServerUri, Id, fileAccess, UseCompression);
		}

		public void Upload(string sourcePath, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			using (FileStream sourceStream = File.OpenRead(sourcePath))
			{
				Upload(sourceStream, progressEventHandler);
			}
		}

		public void Upload(Stream sourceStream, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			if (!_Stream.CanWrite)
			{
				throw new InvalidOperationException("Error, cannot read from current Stream.");
			}
			FileStreamUtil.CopyStream(sourceStream, _Stream, progressEventHandler);
		}

		public void Download(string destinationPath, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			using (FileStream destinationStream = File.Create(destinationPath))
			{
				Download(destinationStream, progressEventHandler);
			}
		}

		public void Download(Stream destinationStream, EventHandler<FileOperationEventArgs> progressEventHandler)
		{
			if (!_Stream.CanRead)
			{
				throw new InvalidOperationException("Error, cannot read from current Stream.");
			}
			FileStreamUtil.CopyStream(_Stream, destinationStream, progressEventHandler);
		}

		public void Cancel()
		{
			using (FileOperationServiceClient fileOperationServiceClient = FileOperationServiceClient.Connect(_ServerUri))
			{
				fileOperationServiceClient.CancelOperation(_FileOperationId);
			}
		}

		public Guid? Complete()
		{
			using (FileOperationServiceClient fileOperationServiceClient = FileOperationServiceClient.Connect(_ServerUri))
			{
				return fileOperationServiceClient.CompleteOperation(_FileOperationId);
			}
		}
	}
}
