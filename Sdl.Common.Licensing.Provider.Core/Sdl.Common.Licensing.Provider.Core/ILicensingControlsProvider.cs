using Sdl.Common.Licensing.Provider.Core.UI;

namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicensingControlsProvider
	{
		IUIControl GetBorrowLicenseControl(LicenseManagerView parent);

		IUIControl GetOfflineDeactivationControl(LicenseManagerView parent);

		IUIControl GetLicenseServerEditionsControl(LicenseManagerView parent);
	}
}
