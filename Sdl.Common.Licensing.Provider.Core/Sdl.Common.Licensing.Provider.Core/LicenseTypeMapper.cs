using System;

namespace Sdl.Common.Licensing.Provider.Core
{
	public static class LicenseTypeMapper
	{
		public static ILicenseTypeMapper LicenseTypeByFeaturePropertyTypeMapper(string featurePropertyName)
		{
			return new ProductTypeByFeaturePropertyTypeMapper(featurePropertyName);
		}

		public static ILicenseTypeMapper SingleFeatureLicenseTypeMapper(LicenseTypeToFeatureMap<int> licenseTypeToFeature)
		{
			return new SingleFeatureLicenseTypeMapper<int>(licenseTypeToFeature);
		}

		public static ILicenseTypeMapper SingleFeatureLicenseTypeMapper(LicenseTypeToFeatureMap<string> licenseTypeToFeature)
		{
			return new SingleFeatureLicenseTypeMapper<string>(licenseTypeToFeature);
		}

		public static ILicenseTypeMapper MultipleFeatureLicenseTypeMapper(LicenseTypeToFeaturesMap licenseTypeToFeature)
		{
			return new MultipleFeatureLicenseTypeMapper(licenseTypeToFeature);
		}

		public static ILicenseTypeMapper MultipleFeatureLicenseTypeMapper(Func<IProductLicense, int> licenseTypeDelegate)
		{
			return new ComputedLicenseTypeMapper(licenseTypeDelegate);
		}
	}
}
