using System;
using System.Globalization;
using System.IO;

namespace Sdl.Enterprise2.Studio.Platform.Client.IO
{
	public sealed class FileOperationStream : Stream
	{
		private readonly Guid _FileOperationId;

		private readonly FileAccess _FileAccess;

		private readonly Uri _ServerUri;

		private long? _Length;

		private readonly bool _useCompression;

		public override bool CanRead => _FileAccess != FileAccess.Write;

		public bool UseCompression => _useCompression;

		public override bool CanSeek => false;

		public override bool CanWrite => _FileAccess == FileAccess.Write;

		public override long Length
		{
			get
			{
				if (!_Length.HasValue)
				{
					using (FileOperationServiceClient fileOperationServiceClient = FileOperationServiceClient.Connect(_ServerUri))
					{
						_Length = fileOperationServiceClient.GetStreamLength(_FileOperationId);
					}
				}
				return _Length.Value;
			}
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private FileOperationStream()
		{
			_useCompression = false;
		}

		public FileOperationStream(Guid operationId)
			: this(null, operationId, FileAccess.Read)
		{
		}

		public FileOperationStream(Uri serverUri, Guid operationId, FileAccess fileAccess)
			: this()
		{
			_ServerUri = serverUri;
			_FileAccess = fileAccess;
			_FileOperationId = operationId;
			if (fileAccess == FileAccess.Write)
			{
				_Length = 0L;
			}
		}

		public FileOperationStream(Uri serverUri, Guid operationId, FileAccess fileAccess, bool useCompression)
			: this(serverUri, operationId, fileAccess)
		{
			_useCompression = useCompression;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Error, count {0} is <= 0.", count));
			}
			byte[] array = null;
			using (FileOperationServiceClient fileOperationServiceClient = FileOperationServiceClient.Connect(_ServerUri))
			{
				array = fileOperationServiceClient.ReadData(_FileOperationId, count);
			}
			int num = array.Length;
			if (num == 0)
			{
				return 0;
			}
			Array.Copy(array, 0, buffer, offset, array.Length);
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			byte[] array = buffer;
			if (count != buffer.Length || offset != 0)
			{
				array = new byte[count];
				Array.Copy(buffer, offset, array, 0, count);
			}
			_Length += array.Length;
			using (FileOperationServiceClient fileOperationServiceClient = FileOperationServiceClient.Connect(_ServerUri, UseCompression))
			{
				fileOperationServiceClient.WriteData(_FileOperationId, array);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}
	}
}
