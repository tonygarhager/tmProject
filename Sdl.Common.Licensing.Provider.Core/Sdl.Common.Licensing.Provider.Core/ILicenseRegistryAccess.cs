namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicenseRegistryAccess
	{
		string DeactivationCode
		{
			get;
			set;
		}

		string LicenseCode
		{
			get;
			set;
		}

		string LicenseServer
		{
			get;
			set;
		}

		bool UseLicenseServer
		{
			get;
			set;
		}

		bool UseBorrowedLicense
		{
			get;
			set;
		}

		string CurrentLicensingProvider
		{
			get;
			set;
		}

		string CheckedOutFeatures
		{
			get;
			set;
		}

		string CheckedOutEdition
		{
			get;
			set;
		}

		bool CanUpdateLicenseServerName();
	}
}
