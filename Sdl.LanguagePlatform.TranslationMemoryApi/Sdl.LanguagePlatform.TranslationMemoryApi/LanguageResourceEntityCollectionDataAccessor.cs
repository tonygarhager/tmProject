using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class LanguageResourceEntityCollectionDataAccessor : IResourceDataAccessor
	{
		private readonly EntityCollection<LanguageResourceEntity> _languageResourceCollection;

		public LanguageResourceEntityCollectionDataAccessor(EntityCollection<LanguageResourceEntity> languageResourceCollection)
		{
			_languageResourceCollection = (languageResourceCollection ?? throw new ArgumentNullException("languageResourceCollection"));
		}

		public ResourceStatus GetResourceStatus(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			LanguageResource languageResource = GetLanguageResource(t, culture, fallback);
			if (languageResource == null)
			{
				return ResourceStatus.NotAvailable;
			}
			return ResourceStatus.Loaded;
		}

		public Stream ReadResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			LanguageResource languageResource = GetLanguageResource(t, culture, fallback);
			if (languageResource != null)
			{
				return new MemoryStream(languageResource.Data);
			}
			return null;
		}

		public byte[] GetResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			return GetLanguageResource(t, culture, fallback)?.Data;
		}

		public List<CultureInfo> GetSupportedCultures(LanguageResourceType t)
		{
			List<CultureInfo> list = new List<CultureInfo>();
			foreach (LanguageResourceEntity item in _languageResourceCollection)
			{
				if (item.Type == t)
				{
					list.Add(new CultureInfo(item.CultureName));
				}
			}
			return list;
		}

		private static LanguageResource ToLanguageResource(LanguageResourceEntity entity)
		{
			if (entity == null)
			{
				return null;
			}
			LanguageResource languageResource = new LanguageResource();
			int id = (entity.Id != null && entity.Id.Value != null && entity.Id.Value is int) ? ((int)entity.Id.Value) : 0;
			Guid guid = entity.UniqueId ?? Guid.Empty;
			languageResource.ResourceId = new PersistentObjectToken(id, guid);
			if (entity.Type.HasValue)
			{
				languageResource.Type = entity.Type.Value;
			}
			languageResource.CultureName = entity.CultureName;
			languageResource.Data = entity.Data;
			return languageResource;
		}

		private LanguageResource GetLanguageResource(LanguageResourceType type, CultureInfo culture, bool fallback)
		{
			LanguageResourceEntity languageResourceEntity = _languageResourceCollection.FirstOrDefault((LanguageResourceEntity resource) => resource.Type == type && (resource.CultureName.Equals(culture.Name, StringComparison.OrdinalIgnoreCase) || resource.Type == LanguageResourceType.Variables));
			if (languageResourceEntity != null)
			{
				return ToLanguageResource(languageResourceEntity);
			}
			if (fallback)
			{
				return ToLanguageResource(_languageResourceCollection.FirstOrDefault((LanguageResourceEntity resource) => resource.Type == type && CultureInfoExtensions.AreCompatible(new CultureInfo(resource.CultureName), culture)));
			}
			return null;
		}
	}
}
