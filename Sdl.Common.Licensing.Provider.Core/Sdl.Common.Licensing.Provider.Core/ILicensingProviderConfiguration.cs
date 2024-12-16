using System.Collections.Generic;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicensingProviderConfiguration
	{
		string Name
		{
			get;
		}

		string ProviderId
		{
			get;
		}

		string ProductName
		{
			get;
		}

		bool IsDefault
		{
			get;
		}

		Proxy Proxy
		{
			get;
			set;
		}

		ILicenseRegistryAccess Registry
		{
			get;
		}

		List<string> AvailableEditions
		{
			get;
			set;
		}

		LicenseDescriptionDelegate LicenseDescription
		{
			get;
			set;
		}

		bool IsLocal();
	}
}
