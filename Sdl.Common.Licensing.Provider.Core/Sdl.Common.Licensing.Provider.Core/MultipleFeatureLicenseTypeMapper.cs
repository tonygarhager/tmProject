using Sdl.Common.Licensing.Provider.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Common.Licensing.Provider.Core
{
	internal class MultipleFeatureLicenseTypeMapper : ILicenseTypeMapper
	{
		private LicenseTypeToFeaturesMap _licenseTypeToFeatures;

		public MultipleFeatureLicenseTypeMapper(LicenseTypeToFeaturesMap licenseTypeToFeatures)
		{
			_licenseTypeToFeatures = licenseTypeToFeatures;
		}

		public int GetLicenseType(IProductLicense productLicense)
		{
			foreach (KeyValuePair<int, List<int>> licenseTypeToFeature in _licenseTypeToFeatures)
			{
				if (licenseTypeToFeature.Value.All((int featureId) => productLicense.HasFeature(featureId.ToString())))
				{
					return licenseTypeToFeature.Key;
				}
			}
			throw new LicenseTypeNotFoundException(StringResources.LicenseTypeNotFound);
		}

		public int GetLicenseType(INativeLicense nativeLicense)
		{
			IList<string> nativeFeatureIds = nativeLicense.GetNativeFeaturesIds();
			foreach (KeyValuePair<int, List<int>> licenseTypeToFeature in _licenseTypeToFeatures)
			{
				if (licenseTypeToFeature.Value.All((int fid) => nativeFeatureIds.Any((string nid) => nid == fid.ToString())))
				{
					return licenseTypeToFeature.Key;
				}
			}
			throw new LicenseTypeNotFoundException(StringResources.LicenseTypeNotFound);
		}

		public ILicenseFeature GetLicenseTypeFeature(IProductLicense productLicense)
		{
			throw new NotImplementedException();
		}
	}
}
