using System.Linq;

namespace Sdl.Common.Licensing.Provider.Core
{
	public static class Helper
	{
		public static string GetStatus(LicenseModeDetails statusCode)
		{
			return GetStatus(statusCode, productAllowed: true);
		}

		public static string GetStatus(LicenseModeDetails statusCode, bool productAllowed)
		{
			switch (statusCode)
			{
			case LicenseModeDetails.NeverAuthorized:
			case LicenseModeDetails.NotAuthorized:
				return StringResources.DesktopLicensing_Status_NotLicensed;
			case LicenseModeDetails.UnlimitedUsage:
				return productAllowed ? StringResources.DesktopLicensing_Status_Licensed : StringResources.DesktopLicensing_Status_NotLicensed;
			case LicenseModeDetails.LeasePeriodLimited:
				return productAllowed ? StringResources.DesktopLicensing_Status_Leased : StringResources.DesktopLicensing_Status_NotLicensed;
			case LicenseModeDetails.AuthorizedUsesLimited:
				return productAllowed ? StringResources.DesktopLicensing_Status_Licensed_LimitedUses : StringResources.DesktopLicensing_Status_NotLicensed;
			case LicenseModeDetails.DemoDaysLimited:
			case LicenseModeDetails.DemoUsesLimited:
			case LicenseModeDetails.DemoUsesDaysLimited:
				return productAllowed ? StringResources.DesktopLicensing_Status_EvaluationMode : StringResources.DesktopLicensing_Status_NotLicensed;
			case LicenseModeDetails.LimitedNetworkLicenses:
				return productAllowed ? StringResources.DesktopLicensing_Status_ServerLicense : StringResources.DesktopLicensing_Status_NotLicensed;
			case LicenseModeDetails.DemoDaysExpired:
			case LicenseModeDetails.DemoUsesExpired:
				return StringResources.DesktopLicensing_Status_EvaluationExpired;
			case LicenseModeDetails.LeasePeriodExpired:
				return StringResources.DesktopLicensing_Status_LeaseExpired;
			case LicenseModeDetails.AuthorizedUsesExpired:
				return StringResources.DesktopLicensing_Status_Licensed_NoUsesLeft;
			case LicenseModeDetails.LeaseUsesLimited:
				return productAllowed ? StringResources.DesktopLicensing_Status_Leased_LimitedUses : StringResources.DesktopLicensing_Status_NotLicensed;
			case LicenseModeDetails.LeaseOrUsesReturnedToServer:
				return StringResources.DesktopLicensing_Status_LicenseWasReturnedToServer;
			case LicenseModeDetails.DemoMinutesLimited:
			case LicenseModeDetails.DemoMinutesInUse:
			case LicenseModeDetails.DemoMinutesExpired:
				return StringResources.DesktopLicensing_Status_EvaluationMinutesMode;
			case LicenseModeDetails.LicenseRemoved:
				return StringResources.DesktopLicensing_Status_LicenseRemoved;
			case LicenseModeDetails.UserChangingDates:
				return StringResources.DesktopLicensing_Status_LicenseMustBeReauthorized;
			default:
				return StringResources.DesktopLicensing_Status_IndeterminateLicenseStatus;
			}
		}

		public static string GetFirstWord(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			string text2 = text.Split(' ', '-', '.').FirstOrDefault();
			if (string.IsNullOrEmpty(text2))
			{
				return string.Empty;
			}
			return text2;
		}
	}
}
