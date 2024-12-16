namespace Sdl.Common.Licensing.Provider.Core
{
	public abstract class LicenseRegistryAccess : AbstractRegistryAccess, ILicenseRegistryAccess
	{
		protected const string RegistryKeyName_DeactivationCode = "DeactivationCode";

		protected const string RegistryKeyName_LicenseCode = "LicenseCode";

		protected const string RegistryKeyName_ServerName = "ServerName";

		protected const string RegistryKeyName_UseLicenseServer = "UseLicenseServer";

		protected const string RegistryKeyName_UseBorrowedLicense = "UseBorrowedLicense";

		protected const string RegistryKeyName_CurrentLicensingProviderID = "CurrentLPID";

		protected const string RegistryKeyName_CheckedOutFeatures = "CheckedOutFeatures";

		protected const string RegistryKeyName_CheckedOutEdition = "CheckedOutEdition";

		public string DeactivationCode
		{
			get
			{
				return GetKeyValue("DeactivationCode") as string;
			}
			set
			{
				SetKeyValue("DeactivationCode", value);
			}
		}

		public string LicenseCode
		{
			get
			{
				return GetKeyValue("LicenseCode") as string;
			}
			set
			{
				SetKeyValue("LicenseCode", value);
			}
		}

		public string LicenseServer
		{
			get
			{
				return GetKeyValue("ServerName") as string;
			}
			set
			{
				SetKeyValue("ServerName", value);
			}
		}

		public bool UseLicenseServer
		{
			get
			{
				string value = GetKeyValue("UseLicenseServer") as string;
				return !string.IsNullOrEmpty(value) && bool.Parse(value);
			}
			set
			{
				SetKeyValue("UseLicenseServer", value);
			}
		}

		public bool UseBorrowedLicense
		{
			get
			{
				string value = GetKeyValue("UseBorrowedLicense") as string;
				return !string.IsNullOrEmpty(value) && bool.Parse(value);
			}
			set
			{
				SetKeyValue("UseBorrowedLicense", value);
			}
		}

		public string CurrentLicensingProvider
		{
			get
			{
				return GetKeyValue("CurrentLPID") as string;
			}
			set
			{
				SetKeyValue("CurrentLPID", value);
			}
		}

		public string CheckedOutFeatures
		{
			get
			{
				return GetKeyValue("CheckedOutFeatures") as string;
			}
			set
			{
				SetKeyValue("CheckedOutFeatures", value);
			}
		}

		public string CheckedOutEdition
		{
			get
			{
				return GetKeyValue("CheckedOutEdition") as string;
			}
			set
			{
				SetKeyValue("CheckedOutEdition", value);
			}
		}

		protected LicenseRegistryAccess(string registryPath)
			: base(registryPath)
		{
		}
	}
}
