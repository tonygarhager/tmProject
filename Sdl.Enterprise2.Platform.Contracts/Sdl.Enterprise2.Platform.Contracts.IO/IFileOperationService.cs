using Sdl.Enterprise2.Platform.Contracts.Communication;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.IO
{
	[ServiceContract(Name = "FileOperationService", Namespace = "http://sdl.com/enterprise/platform/2010")]
	public interface IFileOperationService
	{
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		byte[] ReadData(Guid operationId, int chunkSize);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void WriteData(Guid operationId, byte[] data);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		void CancelOperation(Guid operationId);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		Guid? CompleteOperation(Guid operationId);

		[OperationContract]
		long GetStreamLength(Guid operationId);
	}
}
