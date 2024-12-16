using Sdl.Enterprise2.MultiTerm.Client.IdentityModel;
using Sdl.Enterprise2.Platform.Contracts.HttpStreaming;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.MultiTerm.Client.HttpStreaming
{
	public class HttpStreamingServiceClient : IssuedTokenClientBase<IHttpStreamingService>
	{
		private const string SERVICE_PATH = "/HttpStreamingService";

		public HttpStreamingServiceClient()
			: base(IdentityInfoCache.Default, "/HttpStreamingService", (IdentityInfoCache.Default.Count > 0) ? IdentityInfoCache.Default.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public HttpStreamingServiceClient(string baseAddress)
			: base(IdentityInfoCache.Default, "/HttpStreamingService", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public HttpStreamingServiceClient(string baseAddress, UserCredentials credentials)
			: base(IdentityInfoCache.Default, "/HttpStreamingService", baseAddress, credentials, useCompression: false)
		{
		}

		public HttpStreamingServiceClient(string baseAddress, UserCredentials credentials, bool useCompression)
			: base(IdentityInfoCache.Default, "/HttpStreamingService", baseAddress, credentials, useCompression)
		{
		}

		public HttpStreamingServiceClient(IdentityInfoCache identityCache)
			: base(identityCache, "/HttpStreamingService", (identityCache.Count > 0) ? identityCache.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public HttpStreamingServiceClient(IdentityInfoCache identityCache, string baseAddress)
			: base(identityCache, "/HttpStreamingService", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public HttpStreamingServiceClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials)
			: base(identityCache, "/HttpStreamingService", baseAddress, credentials, useCompression: false)
		{
		}

		public HttpStreamingServiceClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials, bool useCompression)
			: base(identityCache, "/HttpStreamingService", baseAddress, credentials, useCompression)
		{
		}

		public DownloadSession StartDownload(string downloadHandlerTypeName, string parameter)
		{
			IHttpStreamingService httpStreamingService = null;
			bool connectionError = false;
			try
			{
				httpStreamingService = GetProxy();
				return httpStreamingService.StartDownload(downloadHandlerTypeName, parameter);
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
				CleanupChannel(httpStreamingService as ICommunicationObject, connectionError);
			}
		}

		public UploadSession StartUpload(string uploadHandlerTypeName, string parameter)
		{
			IHttpStreamingService httpStreamingService = null;
			bool connectionError = false;
			try
			{
				httpStreamingService = GetProxy();
				return httpStreamingService.StartUpload(uploadHandlerTypeName, parameter);
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
				CleanupChannel(httpStreamingService as ICommunicationObject, connectionError);
			}
		}

		public byte[] ReadData(Guid sessionId, int chunkSize)
		{
			IHttpStreamingService httpStreamingService = null;
			bool connectionError = false;
			try
			{
				httpStreamingService = GetProxy();
				return httpStreamingService.ReadData(sessionId, chunkSize);
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
				CleanupChannel(httpStreamingService as ICommunicationObject, connectionError);
			}
		}

		public void WriteData(Guid sessionId, byte[] data)
		{
			IHttpStreamingService httpStreamingService = null;
			bool connectionError = false;
			try
			{
				httpStreamingService = GetProxy();
				httpStreamingService.WriteData(sessionId, data);
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
				CleanupChannel(httpStreamingService as ICommunicationObject, connectionError);
			}
		}

		public void CloseSession(Guid sessionId)
		{
			IHttpStreamingService httpStreamingService = null;
			bool connectionError = false;
			try
			{
				httpStreamingService = GetProxy();
				httpStreamingService.CloseSession(sessionId);
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
				CleanupChannel(httpStreamingService as ICommunicationObject, connectionError);
			}
		}
	}
}
