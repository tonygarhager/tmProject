using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Core.Resources
{
	public interface IResourceDataAccessor
	{
		ResourceStatus GetResourceStatus(CultureInfo culture, LanguageResourceType t, bool fallback);

		Stream ReadResourceData(CultureInfo culture, LanguageResourceType t, bool fallback);

		byte[] GetResourceData(CultureInfo culture, LanguageResourceType t, bool fallback);

		List<CultureInfo> GetSupportedCultures(LanguageResourceType t);
	}
}
