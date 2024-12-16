using Sdl.Enterprise2.Platform.Contracts.Communication;
using System.ServiceModel;

namespace Sdl.Enterprise2.Platform.Contracts.Licensing
{
	[ServiceContract(Name = "ILicenseAdminService", Namespace = "http://sdl.com/enterprise/platform/2011")]
	public interface ILicenseAdminService
	{
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
	}
}
