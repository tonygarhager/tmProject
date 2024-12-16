using Sdl.Enterprise2.MultiTerm.Client.IdentityModel;
using Sdl.Enterprise2.Platform.Contracts.Communication;
using Sdl.Enterprise2.Platform.Contracts.Licensing;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.MultiTerm.Client.Licensing
{
	public class LicenseAdminServiceClient : IssuedTokenClientBase<ILicenseAdminService>, ILicenseAdminService
	{
		private const string SERVICE_PATH = "/LicenseAdminService";

		public LicenseAdminServiceClient()
			: base(IdentityInfoCache.Default, "/LicenseAdminService", (IdentityInfoCache.Default.Count > 0) ? IdentityInfoCache.Default.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public LicenseAdminServiceClient(string baseAddress)
			: base(IdentityInfoCache.Default, "/LicenseAdminService", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public LicenseAdminServiceClient(string baseAddress, UserCredentials credentials)
			: base(IdentityInfoCache.Default, "/LicenseAdminService", baseAddress, credentials, useCompression: false)
		{
		}

		public LicenseAdminServiceClient(string baseAddress, UserCredentials credentials, bool useCompression)
			: base(IdentityInfoCache.Default, "/LicenseAdminService", baseAddress, credentials, useCompression)
		{
		}

		public LicenseAdminServiceClient(IdentityInfoCache identityCache)
			: base(identityCache, "/LicenseAdminService", (identityCache.Count > 0) ? identityCache.DefaultKey : null, (UserCredentials)null, useCompression: false)
		{
		}

		public LicenseAdminServiceClient(IdentityInfoCache identityCache, string baseAddress)
			: base(identityCache, "/LicenseAdminService", baseAddress, (UserCredentials)null, useCompression: false)
		{
		}

		public LicenseAdminServiceClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials)
			: base(identityCache, "/LicenseAdminService", baseAddress, credentials, useCompression: false)
		{
		}

		public LicenseAdminServiceClient(IdentityInfoCache identityCache, string baseAddress, UserCredentials credentials, bool useCompression)
			: base(identityCache, "/LicenseAdminService", baseAddress, credentials, useCompression)
		{
		}

		public LicensedFeature[] GetLicensedFeatures()
		{
			ILicenseAdminService licenseAdminService = null;
			bool connectionError = false;
			try
			{
				licenseAdminService = GetProxy();
				return licenseAdminService.GetLicensedFeatures();
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
				CleanupChannel(licenseAdminService as ICommunicationObject, connectionError);
			}
		}

		public LicensedFeatureInfo[] GetLicensedFeatureInfo()
		{
			ILicenseAdminService licenseAdminService = null;
			bool connectionError = false;
			try
			{
				licenseAdminService = GetProxy();
				return licenseAdminService.GetLicensedFeatureInfo();
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
				CleanupChannel(licenseAdminService as ICommunicationObject, connectionError);
			}
		}

		public string GetLicenseSummary()
		{
			ILicenseAdminService licenseAdminService = null;
			bool connectionError = false;
			try
			{
				licenseAdminService = GetProxy();
				return licenseAdminService.GetLicenseSummary();
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
				CleanupChannel(licenseAdminService as ICommunicationObject, connectionError);
			}
		}

		public LicenseInfo GetLicenseInfo()
		{
			ILicenseAdminService licenseAdminService = null;
			bool connectionError = false;
			try
			{
				licenseAdminService = GetProxy();
				return licenseAdminService.GetLicenseInfo();
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
				CleanupChannel(licenseAdminService as ICommunicationObject, connectionError);
			}
		}
	}
}
