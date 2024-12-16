using Sdl.Common.Licensing.Provider.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Common.Licensing.Provider.Core
{
	internal class SingleFeatureLicenseTypeMapper<T> : ILicenseTypeMapper
	{
		private readonly LicenseTypeToFeatureMap<T> _licenseTypeToFeature;

		public LicenseTypeToFeatureMap<T> LicenseTypeToFeatureMap => _licenseTypeToFeature;

		public SingleFeatureLicenseTypeMapper(LicenseTypeToFeatureMap<T> licenseTypeToFeature)
		{
			_licenseTypeToFeature = licenseTypeToFeature;
		}

		public int GetLicenseType(IProductLicense productLicense)
		{
			foreach (KeyValuePair<int, T> item in _licenseTypeToFeature)
			{
				if (productLicense.HasFeature(item.Value.ToString()))
				{
					return item.Key;
				}
			}
			throw new LicenseTypeNotFoundException(StringResources.LicenseTypeNotFound);
		}

		public int GetLicenseType(INativeLicense nativeLicense)
		{
			IList<string> nativeFeaturesIds = nativeLicense.GetNativeFeaturesIds();
			foreach (KeyValuePair<int, T> idt in _licenseTypeToFeature)
			{
				if (nativeFeaturesIds.FirstOrDefault((string id) => id == idt.Value.ToString()) != null)
				{
					return idt.Key;
				}
			}
			throw new LicenseTypeNotFoundException(StringResources.LicenseTypeNotFound);
		}

		public ILicenseFeature GetLicenseTypeFeature(IProductLicense productLicense)
		{
			foreach (KeyValuePair<int, T> item in _licenseTypeToFeature)
			{
				string id = item.Value.ToString();
				if (productLicense.HasFeature(id))
				{
					return productLicense.GetFeature(id);
				}
			}
			return null;
		}
	}
}
