using Sdl.Enterprise2.MultiTerm.Client.IdentityModel;
using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.Enterprise2.Platform.Contracts.Licensing;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.MultiTerm.Client.Licensing
{
	public class LicenseServiceClient : IssuedTokenClientBase<ILicenseService>, ILicenseService
	{
		private const string SERVICE_PATH = "/LicenseServer";

		public LicenseServiceClient()
			: base(IdentityInfoCache.Default, "/LicenseServer", (IdentityInfoCache.Default.Count > 0) ? IdentityInfoCache.Default.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public LicenseServiceClient(string baseAddress)
			: base(IdentityInfoCache.Default, "/LicenseServer", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public LicenseServiceClient(string baseAddress, UserCredentials credentials)
			: base(IdentityInfoCache.Default, "/LicenseServer", baseAddress, credentials, useCompression: false)
		{
		}

		public LicenseServiceClient(string baseAddress, UserCredentials credentials, bool useCompression)
			: base(IdentityInfoCache.Default, "/LicenseServer", baseAddress, credentials, useCompression)
		{
		}

		public LicenseServiceClient(IdentityInfoCache identityCache)
			: base(identityCache, "/LicenseServer", (identityCache.Count > 0) ? identityCache.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public LicenseServiceClient(IdentityInfoCache identityCache, string baseAddress)
			: base(identityCache, "/LicenseServer", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public LicenseServiceClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials)
			: base(identityCache, "/LicenseServer", baseAddress, credentials, useCompression: false)
		{
		}

		public LicenseServiceClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials, bool useCompression)
			: base(identityCache, "/LicenseServer", baseAddress, credentials, useCompression)
		{
		}

		public bool GetLicenseStatus(out bool isTrailLicense, out DateTime? expiryDate)
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.GetLicenseStatus(out isTrailLicense, out expiryDate);
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public LicensedFeature[] GetLicensedFeatures()
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.GetLicensedFeatures();
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public LicensedFeatureInfo[] GetLicensedFeatureInfo()
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.GetLicensedFeatureInfo();
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public string GetLicenseSummary()
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.GetLicenseSummary();
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public LicenseInfo GetLicenseInfo()
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.GetLicenseInfo();
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public LicenseInfo GetLicenseInfoExcludeCountedFeatures()
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.GetLicenseInfoExcludeCountedFeatures();
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public bool QueryFeature(string feature)
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.QueryFeature(feature);
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public bool QueryCountedFeature(string feature, out int maxCount, out int allocatedCount)
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.QueryCountedFeature(feature, out maxCount, out allocatedCount);
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public License CheckOut(string clientId, string feature, TimeSpan timeout)
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.CheckOut(clientId, feature, timeout);
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public bool CheckIn(License cal)
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.CheckIn(cal);
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}

		public bool Refresh(License cal)
		{
			ILicenseService licenseService = null;
			bool connectionError = false;
			try
			{
				licenseService = GetProxy();
				return licenseService.Refresh(cal);
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
				CleanupChannel(licenseService as ICommunicationObject, connectionError);
			}
		}
	}
}
