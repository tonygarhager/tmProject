using Sdl.Enterprise2.Platform.Contracts.HttpStreaming;
using System;
using System.IO;

namespace Sdl.Enterprise2.MultiTerm.Client.HttpStreaming
{
	public class DownloadStream : Stream
	{
		private readonly HttpStreamingServiceClient _httpStreamServiceClient;

		private readonly DownloadSession _downloadSession;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length
		{
			get
			{
				if (!_downloadSession.Length.HasValue)
				{
					throw new NotSupportedException();
				}
				return _downloadSession.Length.Value;
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public DownloadStream(HttpStreamingServiceClient httpStreamServiceProxy, string downloadHandlerTypeName, string data)
		{
			_httpStreamServiceClient = httpStreamServiceProxy;
			_downloadSession = _httpStreamServiceClient.StartDownload(downloadHandlerTypeName, data);
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			byte[] array = _httpStreamServiceClient.ReadData(_downloadSession.SessionId, count);
			int num = array.Length;
			if (num == 0)
			{
				return 0;
			}
			Array.Copy(array, 0, buffer, offset, array.Length);
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			_httpStreamServiceClient.CloseSession(_downloadSession.SessionId);
			base.Close();
		}
	}
}
