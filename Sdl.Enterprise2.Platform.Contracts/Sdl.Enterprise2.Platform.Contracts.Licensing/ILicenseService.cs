using Sdl.Enterprise2.Platform.Contracts.Communication;
using System;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.Licensing
{
	[ServiceContract(Name = "LicenseService", Namespace = "http://sdl.com/enterprise/platform/2011")]
	public interface ILicenseService
	{
		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool GetLicenseStatus(out bool isTrailLicense, out DateTime? expiryDate);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		LicensedFeature[] GetLicensedFeatures();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		LicensedFeatureInfo[] GetLicensedFeatureInfo();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		string GetLicenseSummary();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		LicenseInfo GetLicenseInfo();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		LicenseInfo GetLicenseInfoExcludeCountedFeatures();

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool QueryFeature(string feature);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool QueryCountedFeature(string feature, out int maxCount, out int allocatedCount);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		License CheckOut(string clientId, string feature, TimeSpan timeout);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool CheckIn(License cal);

		[OperationContract]
		[FaultContract(typeof(ServiceError))]
		bool Refresh(License cal);
	}
}
