using System.Collections.Generic;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicensingProvider
	{
		string Id
		{
			get;
		}

		string ProviderName
		{
			get;
		}

		ILicensingProviderConfiguration Configuration
		{
			get;
		}

		IList<IProductLicense> GetProducts();

		IProductLicense Activate(string code);

		void Deactivate(string code);

		IProductLicense OfflineActivate(string certificate);

		string OfflineDeactivate(string code);

		string GetInstallationId();

		string GetDiagnosticInfo(IProductLicense license);

		bool ReturnCommuterLicense(string name, string version);

		bool CheckoutCommuterLicense(string name, string version, int duration);

		bool CheckOutFeature(string name, string version, ref int licenseHandle, bool enableAutoTimer);

		bool CheckInFeature(string name, string version, int licenseHandle);

		string GetFeatureActivationId(string name, string version, int licenseType);
	}
}
