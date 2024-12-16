namespace Sdl.Common.Licensing.Provider.Core
{
	public interface ILicenseTypeMapper
	{
		int GetLicenseType(IProductLicense productLicense);

		int GetLicenseType(INativeLicense nativeLicense);

		ILicenseFeature GetLicenseTypeFeature(IProductLicense productLicense);
	}
}
