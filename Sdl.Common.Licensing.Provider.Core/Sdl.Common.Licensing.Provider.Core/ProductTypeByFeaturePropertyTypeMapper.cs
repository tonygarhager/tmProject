using Sdl.Common.Licensing.Provider.Core.Exceptions;
using System;

namespace Sdl.Common.Licensing.Provider.Core
{
	internal class ProductTypeByFeaturePropertyTypeMapper : ILicenseTypeMapper
	{
		public string FeatureId
		{
			get;
			private set;
		}

		public string FeaturePropertyName
		{
			get;
			private set;
		}

		internal ProductTypeByFeaturePropertyTypeMapper(string featurePropertyName)
		{
			FeaturePropertyName = featurePropertyName;
		}

		internal ProductTypeByFeaturePropertyTypeMapper(string featureId, string featurePropertyName)
		{
			FeatureId = featureId;
			FeaturePropertyName = featurePropertyName;
		}

		public int GetLicenseType(IProductLicense productLicense)
		{
			if (!string.IsNullOrEmpty(FeatureId))
			{
				throw new NotImplementedException(StringResources.PerFeatureNotImplemented);
			}
			dynamic property = productLicense.GetProperty(FeaturePropertyName);
			if (property == null)
			{
				throw new LicensingProviderException(string.Format(StringResources.PropertyValueNotDefined, FeaturePropertyName));
			}
			return int.Parse(property);
		}

		public int GetLicenseType(INativeLicense nativeLicense)
		{
			throw new NotSupportedException();
		}

		public ILicenseFeature GetLicenseTypeFeature(IProductLicense productLicense)
		{
			throw new NotImplementedException();
		}
	}
}
