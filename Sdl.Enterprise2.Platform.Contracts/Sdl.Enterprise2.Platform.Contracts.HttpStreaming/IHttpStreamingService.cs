using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.HttpStreaming
{
	[ServiceContract(Name = "HttpStreaming", Namespace = "http://sdl.com/enterprise/platform/2010")]
	public interface IHttpStreamingService
	{
		[OperationContract]
		DownloadSession StartDownload(string downloadHandlerTypeName, string parameter);

		[OperationContract]
		UploadSession StartUpload(string uploadHandlerTypeName, string parameter);

		[OperationContract]
		byte[] ReadData(Guid sessionId, int chunkSize);

		[OperationContract]
		void WriteData(Guid sessionId, byte[] data);

		[OperationContract]
		void CloseSession(Guid sessionId);
	}
}
