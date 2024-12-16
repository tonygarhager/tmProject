using System;
using System.Collections.Generic;

namespace Sdl.Common.Licensing.Provider.Core
{
	internal class ComputedLicenseTypeMapper : ILicenseTypeMapper
	{
		private Func<IProductLicense, int> _licenseTypeDelegate;

		private Func<IList<string>, int> _featureTypeDelegate;

		internal ComputedLicenseTypeMapper(Func<IProductLicense, int> licenseTypeDelegate)
		{
			_licenseTypeDelegate = licenseTypeDelegate;
		}

		internal ComputedLicenseTypeMapper(Func<IList<string>, int> featureTypeDelegate)
		{
			_featureTypeDelegate = featureTypeDelegate;
		}

		public int GetLicenseType(IProductLicense productLicense)
		{
			return _licenseTypeDelegate(productLicense);
		}

		public int GetLicenseType(INativeLicense nativeLicense)
		{
			return _featureTypeDelegate(nativeLicense.GetNativeFeaturesIds());
		}

		public ILicenseFeature GetLicenseTypeFeature(IProductLicense productLicense)
		{
			throw new NotImplementedException();
		}
	}
}
