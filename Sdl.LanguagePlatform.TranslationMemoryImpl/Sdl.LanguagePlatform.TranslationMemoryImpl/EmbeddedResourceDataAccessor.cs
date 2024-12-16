using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class EmbeddedResourceDataAccessor : ResourceStorage
	{
		private readonly Dictionary<string, byte[]> _resourceData = new Dictionary<string, byte[]>();

		public EmbeddedResourceDataAccessor(Assembly assembly, string name)
		{
			string name2 = assembly.GetName().Name + "." + name;
			using (Stream stream = assembly.GetManifestResourceStream(name2))
			{
				using (ResourceReader resourceReader = new ResourceReader(stream ?? throw new InvalidOperationException()))
				{
					foreach (DictionaryEntry item in resourceReader)
					{
						_resourceData.Add((string)item.Key, (byte[])item.Value);
					}
				}
			}
		}

		public override List<string> GetAllResourceKeys()
		{
			return _resourceData.Keys.ToList();
		}

		public override byte[] GetResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			string resourceName = GetResourceName(culture, t, fallback);
			if (resourceName != null && _resourceData.TryGetValue(resourceName, out byte[] value))
			{
				return value;
			}
			return null;
		}

		public override Stream ReadResourceData(CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			byte[] resourceData = GetResourceData(culture, t, fallback);
			if (resourceData == null)
			{
				return null;
			}
			return new MemoryStream(resourceData, writable: false);
		}
	}
}
