using System.Collections.Generic;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public class MultiProviderLicensingForm : ILicensingForm
	{
		private LicenseManagerView _licenseManagerView;

		private readonly LicenseManager _licenseManager;

		public LicensingFormConfiguration LicensingFormConfiguration
		{
			get;
			set;
		}

		public IProductLicense ProductLicense => _licenseManager.ProductLicense;

		public bool HasBeenActivated => ProductLicense != null && ProductLicense.Status == LicenseStatus.Authorized && (_licenseManager.IsProductAllowed == null || (_licenseManager.IsProductAllowed(ProductLicense, null) && ProductLicense.ModeDetail != LicenseModeDetails.LicenseServerNotRunning));

		public MultiProviderLicensingForm(IList<ILicensingProviderConfiguration> lpcs, bool enableServerLicensing, ILicensingProvider licProvider, IProductLicense prodLicense = null)
		{
			_licenseManager = new LicenseManager(licProvider, prodLicense, this, lpcs, enableServerLicensing);
		}

		private void InitializeLicenseManagerForm(IList<CustomStatusPage> customStatusPages)
		{
			_licenseManagerView = new LicenseManagerView();
			_licenseManagerView.Initialize(_licenseManager, customStatusPages);
			_licenseManagerView.CloseCommand = _licenseManager.Configuration.CloseCommand;
		}

		public ILicensingDialog GetForm(IList<CustomStatusPage> customStatusPages = null)
		{
			InitializeLicenseManagerForm(customStatusPages);
			return _licenseManagerView;
		}

		public void ActivatePage(LicensePageType pageType)
		{
			if (_licenseManagerView != null)
			{
				_licenseManagerView.ActivatePage(pageType);
			}
		}

		public void PerformOnlineDeactivation(bool showConfirmationQuestion)
		{
			if (_licenseManagerView != null)
			{
				_licenseManagerView.PerformOnlineDeactivation(showConfirmationQuestion);
			}
		}

		public void Close()
		{
			_licenseManagerView.Close();
		}
	}
}
