using Sdl.LanguagePlatform.Core.Resources;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class CachedResourceDataAccessor : IResourceDataAccessor
	{
		private readonly IList<LanguageResource> _resources;

		public CachedResourceDataAccessor(IList<LanguageResource> resources)
		{
			_resources = resources;
		}

		private LanguageResource FindResource(CultureInfo culture, LanguageResourceType t)
		{
			if (_resources == null || _resources.Count == 0)
			{
				return null;
			}
			foreach (LanguageResource resource in _resources)
			{
				if (resource != null && resource.Type == t)
				{
					if (resource.Culture == null || object.Equals(resource.Culture, CultureInfo.InvariantCulture) || t == LanguageResourceType.Variables)
					{
						return resource;
					}
					if (object.Equals(resource.Culture, culture) || culture.TwoLetterISOLanguageName.Equals(resource.Culture.TwoLetterISOLanguageName))
					{
						return resource;
					}
				}
			}
			return null;
		}

		public ResourceStatus GetResourceStatus(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			LanguageResource languageResource = FindResource(culture, t);
			if (languageResource?.Data != null && languageResource.Data.Length != 0)
			{
				return ResourceStatus.Loaded;
			}
			return ResourceStatus.NotAvailable;
		}

		public Stream ReadResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			byte[] resourceData = GetResourceData(culture, t, fallback);
			if (resourceData != null)
			{
				return new MemoryStream(resourceData, writable: false);
			}
			return null;
		}

		public byte[] GetResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			LanguageResource languageResource = FindResource(culture, t);
			if (languageResource?.Data == null || languageResource.Data.Length == 0)
			{
				return null;
			}
			return languageResource.Data;
		}

		public List<CultureInfo> GetSupportedCultures(LanguageResourceType t)
		{
			return null;
		}
	}
}
