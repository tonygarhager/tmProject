using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdl.Core.Globalization.NumberMetadata
{
	public sealed class NumberMetadataApi
	{
		private static readonly Lazy<NumberMetadataApi> RegistryApi = new Lazy<NumberMetadataApi>(() => new NumberMetadataApi());

		public static NumberMetadataApi Instance => RegistryApi.Value;

		public NumberMetadataRegistry Registry
		{
			get;
		}

		private NumberMetadataApi()
		{
			Registry = new NumberMetadataRegistry();
			Registry.LanguageNumberMetadata = new Dictionary<string, NumberMetadata>();
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string str = executingAssembly.GetName().Name + ".NumberMetadata";
			string prefix = str + ".NumberMetadata";
			List<string> list = (from r in executingAssembly.GetManifestResourceNames()
				where r.StartsWith(prefix) && r.EndsWith(".json")
				select r).ToList();
			list.Sort();
			foreach (string item in list)
			{
				Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(item);
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					string value = streamReader.ReadToEnd();
					NumberMetadata numberMetadata = NumberMetadata.DeserializeObject(value);
					if (numberMetadata.LanguageCode == null)
					{
						throw new NumberMetadataRegistryException("NumberMetadata with null LanguageCode: " + item);
					}
					if (Registry.LanguageNumberMetadata.ContainsKey(numberMetadata.LanguageCode))
					{
						throw new NumberMetadataRegistryException("NumberMetadata with duplicate LanguageCode: " + item);
					}
					Registry.LanguageNumberMetadata.Add(numberMetadata.LanguageCode, numberMetadata);
				}
			}
			Registry.Validate();
		}
	}
}
