namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public delegate bool IsLicenseValidForMachineDelegate(IProductLicense productLicense, out string shortStatus, out string longerWarningMessage);
}
