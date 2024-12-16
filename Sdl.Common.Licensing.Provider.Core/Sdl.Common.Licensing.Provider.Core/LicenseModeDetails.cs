namespace Sdl.Common.Licensing.Provider.Core
{
	public enum LicenseModeDetails
	{
		NeverAuthorized = 0,
		NotAuthorized = 1,
		UnlimitedUsage = 2,
		LeasePeriodLimited = 3,
		AuthorizedUsesLimited = 4,
		DemoDaysLimited = 5,
		DemoUsesLimited = 6,
		DemoUsesDaysLimited = 7,
		LimitedNetworkLicenses = 8,
		DemoDaysExpired = 9,
		DemoUsesExpired = 10,
		LeasePeriodExpired = 11,
		AuthorizedUsesExpired = 12,
		LeaseUsesLimited = 13,
		LeaseOrUsesReturnedToServer = 14,
		DemoMinutesLimited = 0xF,
		DemoMinutesInUse = 0x10,
		DemoMinutesExpired = 17,
		LicenseRemoved = 20,
		UserChangingDates = 21,
		UserLoggedOn = 0xF,
		ExportedLicense = 0x10,
		ExportedLicenseExpired = 17,
		LicenseServerNotRunning = 18,
		LicenseServerHasNoFeatures = 19
	}
}
