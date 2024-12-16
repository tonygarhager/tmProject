using Sdl.Enterprise2.MultiTerm.Client.IdentityModel;
using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.Enterprise2.Platform.Contracts.IO;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.MultiTerm.Client.IO
{
	public class FileOperationServiceClient : IssuedTokenClientBase<IFileOperationService>
	{
		private const string SERVICE_PATH = "/FileOperationService";

		public static FileOperationServiceClient Connect(Uri baseAddress, bool useCompression = false)
		{
			FileOperationServiceClient fileOperationServiceClient = null;
			if (baseAddress != null)
			{
				return new FileOperationServiceClient(baseAddress.AbsoluteUri, null, useCompression);
			}
			return new FileOperationServiceClient();
		}

		public FileOperationServiceClient()
			: base(IdentityInfoCache.Default, "/FileOperationService", (IdentityInfoCache.Default.Count > 0) ? IdentityInfoCache.Default.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public FileOperationServiceClient(string baseAddress)
			: base(IdentityInfoCache.Default, "/FileOperationService", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public FileOperationServiceClient(string baseAddress, UserCredentials credentials)
			: base(IdentityInfoCache.Default, "/FileOperationService", baseAddress, credentials, useCompression: false)
		{
		}

		public FileOperationServiceClient(string baseAddress, UserCredentials credentials, bool useCompression)
			: base(IdentityInfoCache.Default, "/FileOperationService", baseAddress, credentials, useCompression)
		{
		}

		public FileOperationServiceClient(IdentityInfoCache identityCache)
			: base(identityCache, "/FileOperationService", (identityCache.Count > 0) ? identityCache.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public FileOperationServiceClient(IdentityInfoCache identityCache, string baseAddress)
			: base(identityCache, "/FileOperationService", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public FileOperationServiceClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials)
			: base(identityCache, "/FileOperationService", baseAddress, credentials, useCompression: false)
		{
		}

		public FileOperationServiceClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials, bool useCompression)
			: base(identityCache, "/FileOperationService", baseAddress, credentials, useCompression)
		{
		}

		public byte[] ReadData(Guid operationId, int chunkSize)
		{
			IFileOperationService fileOperationService = null;
			bool connectionError = false;
			try
			{
				fileOperationService = GetProxy();
				return fileOperationService.ReadData(operationId, chunkSize);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(fileOperationService as ICommunicationObject, connectionError);
			}
		}

		public void WriteData(Guid operationId, byte[] data)
		{
			IFileOperationService fileOperationService = null;
			bool connectionError = false;
			try
			{
				fileOperationService = GetProxy();
				fileOperationService.WriteData(operationId, data);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(fileOperationService as ICommunicationObject, connectionError);
			}
		}

		public void CancelOperation(Guid operationId)
		{
			IFileOperationService fileOperationService = null;
			bool connectionError = false;
			try
			{
				fileOperationService = GetProxy();
				fileOperationService.CancelOperation(operationId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(fileOperationService as ICommunicationObject, connectionError);
			}
		}

		public Guid? CompleteOperation(Guid operationId)
		{
			IFileOperationService fileOperationService = null;
			bool connectionError = false;
			try
			{
				fileOperationService = GetProxy();
				return fileOperationService.CompleteOperation(operationId);
			}
			catch (FaultException<ServiceError> ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (FaultException ex3)
			{
				Exception ex4 = ProcessServiceException(ex3);
				connectionError = ProcessException(ex4);
				throw ex4;
			}
			catch (Exception ex5)
			{
				connectionError = ProcessException(ex5);
				throw;
			}
			finally
			{
				CleanupChannel(fileOperationService as ICommunicationObject, connectionError);
			}
		}

		public long GetStreamLength(Guid operationId)
		{
			IFileOperationService fileOperationService = null;
			bool connectionError = false;
			try
			{
				fileOperationService = GetProxy();
				return fileOperationService.GetStreamLength(operationId);
			}
			catch (FaultException ex)
			{
				Exception ex2 = ProcessServiceException(ex);
				connectionError = ProcessException(ex2);
				throw ex2;
			}
			catch (Exception ex3)
			{
				connectionError = ProcessException(ex3);
				throw;
			}
			finally
			{
				CleanupChannel(fileOperationService as ICommunicationObject, connectionError);
			}
		}
	}
}
