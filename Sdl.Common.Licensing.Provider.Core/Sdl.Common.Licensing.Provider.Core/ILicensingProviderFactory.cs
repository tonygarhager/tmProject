namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicensingProviderFactory
	{
		string ProviderId
		{
			get;
		}

		bool IsActivationCode(string activationCode);

		bool IsValidServerName(string serverName);

		ILicensingProvider CreateLicensingProvider(ILicensingProviderConfiguration config);

		ILicensingControlsProvider CreateLicensingControlsProvider(ILicensingProviderConfiguration config);

		INetworkLicensingServerProvider CreateNetworkLicensingProvider(ILicensingProviderConfiguration config);
	}
}
