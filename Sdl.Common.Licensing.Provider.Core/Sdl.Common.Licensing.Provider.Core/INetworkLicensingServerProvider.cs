using Sdl.Common.Licensing.Provider.Core.Models;
using System.Collections.Generic;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface INetworkLicensingServerProvider
	{
		List<ActivatedFeature> ActivateLicenseOnline(string code, int quantity);

		List<ActivatedFeature> DeactivateLicenseOnline(string code);

		List<ActivatedFeature> GetActivatedFeatures();

		List<ActivatedLicense> GetActivatedLicenses();

		void RemoveFeature(string featureName, string featureVersion, string aid);
	}
}
