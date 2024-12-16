using Sdl.LanguagePlatform.TranslationMemory;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class AnnotatedTmManager
	{
		private Dictionary<int, AnnotatedTranslationMemory> _cache;

		private readonly object _cacheLock = new object();

		public bool IsCacheEnabled => _cache != null;

		public void EnableCache()
		{
			_cache = new Dictionary<int, AnnotatedTranslationMemory>();
		}

		public void DisableCache()
		{
			lock (_cacheLock)
			{
				_cache = null;
			}
		}

		public void Clear()
		{
			if (_cache != null)
			{
				lock (_cacheLock)
				{
					_cache.Clear();
				}
			}
		}

		public AnnotatedTranslationMemory GetAnnotatedTranslationMemory(CallContext context, PersistentObjectToken tmId)
		{
			if (_cache == null)
			{
				return new AnnotatedTranslationMemory(context.ResourceManager.GetLanguageResources(tmId, includeData: true), 0, context.ResourceManager.GetTranslationMemory(tmId));
			}
			int resourcesWriteCount = context.Storage.GetResourcesWriteCount();
			lock (_cacheLock)
			{
				if (_cache.TryGetValue(tmId.Id, out AnnotatedTranslationMemory value))
				{
					if (value.ResourcesWriteCount >= resourcesWriteCount)
					{
						_ = value.ResourcesWriteCount;
						return value;
					}
					value.UpdateResources(context.ResourceManager.GetLanguageResources(tmId, includeData: true), resourcesWriteCount);
					return value;
				}
				value = new AnnotatedTranslationMemory(context.ResourceManager.GetLanguageResources(tmId, includeData: true), resourcesWriteCount, context.ResourceManager.GetTranslationMemory(tmId));
				_cache.Add(tmId.Id, value);
				return value;
			}
		}
	}
}
