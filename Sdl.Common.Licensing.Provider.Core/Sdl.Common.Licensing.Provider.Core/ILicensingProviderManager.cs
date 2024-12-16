using System.Collections.Generic;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicensingProviderManager
	{
		IList<ILicensingProviderFactory> LicensingProviderFactories
		{
			get;
		}

		ILicensingProvider CreateProvider(ILicensingProviderConfiguration config, string preferredProviderId);

		ILicensingControlsProvider CreateControlsProvider(ILicensingProviderConfiguration config);
	}
}
