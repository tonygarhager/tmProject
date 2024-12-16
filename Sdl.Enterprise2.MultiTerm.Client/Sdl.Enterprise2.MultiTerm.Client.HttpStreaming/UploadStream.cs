using Sdl.Enterprise2.Platform.Contracts.HttpStreaming;
using System;
using System.IO;

namespace Sdl.Enterprise2.MultiTerm.Client.HttpStreaming
{
	public class UploadStream : Stream
	{
		private readonly HttpStreamingServiceClient httpStreamServiceClient;

		private readonly UploadSession uploadSession;

		public override bool CanRead => false;

		public override bool CanSeek => false;

		public override bool CanWrite => true;

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
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

		public UploadStream(HttpStreamingServiceClient httpStreamServiceProxy, string uploadHandlerTypeName, string data)
		{
			httpStreamServiceClient = httpStreamServiceProxy;
			uploadSession = httpStreamServiceClient.StartUpload(uploadHandlerTypeName, data);
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
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
			byte[] array = buffer;
			if (count != buffer.Length || offset != 0)
			{
				array = new byte[count];
				Array.Copy(buffer, offset, array, 0, count);
			}
			httpStreamServiceClient.WriteData(uploadSession.SessionId, array);
		}

		public override void Close()
		{
			httpStreamServiceClient.CloseSession(uploadSession.SessionId);
			base.Close();
		}
	}
}
