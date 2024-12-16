namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public interface ILicensingForm
	{
		LicensingFormConfiguration LicensingFormConfiguration
		{
			get;
			set;
		}

		IProductLicense ProductLicense
		{
			get;
		}

		void Close();

		void ActivatePage(LicensePageType pageType);

		void PerformOnlineDeactivation(bool showConfirmationQuestion);
	}
}
