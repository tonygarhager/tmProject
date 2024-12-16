using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdl.Core.Globalization.UnitMetadata
{
	public sealed class UnitMetadataApi
	{
		private static readonly Lazy<UnitMetadataApi> RegistryApi = new Lazy<UnitMetadataApi>(() => new UnitMetadataApi());

		public static UnitMetadataApi Instance => RegistryApi.Value;

		public UnitMetadataRegistry Registry
		{
			get;
		}

		private UnitMetadataApi()
		{
			Registry = new UnitMetadataRegistry();
			Initialize();
		}

		internal void Initialize()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("Sdl.Core.Globalization.UnitMetadata.UnitDefinitions.json");
			if (manifestResourceStream != null)
			{
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					string value = streamReader.ReadToEnd();
					Registry.UnitDefinitionSet = UnitDefinitionSet.DeserializeObject(value);
				}
			}
			string str = executingAssembly.GetName().Name + ".UnitMetadata";
			string prefix = str + ".UnitMetadataSet";
			List<string> list = (from r in executingAssembly.GetManifestResourceNames()
				where r.StartsWith(prefix) && r.EndsWith(".json")
				select r).ToList();
			list.Sort();
			foreach (string item in list)
			{
				manifestResourceStream = executingAssembly.GetManifestResourceStream(item);
				using (StreamReader streamReader2 = new StreamReader(manifestResourceStream))
				{
					string value2 = streamReader2.ReadToEnd();
					UnitMetadataSet unitMetadataSet = UnitMetadataSet.DeserializeObject(value2);
					if (unitMetadataSet.LanguageCode == null)
					{
						throw new UnitMetadataRegistryException("UnitMetadataSet with null LanguageCode: " + item);
					}
					if (Registry.UnitMetadataSets.ContainsKey(unitMetadataSet.LanguageCode))
					{
						throw new UnitMetadataRegistryException("UnitMetadataSet with duplicate LanguageCode: " + item);
					}
					Registry.UnitMetadataSets.Add(unitMetadataSet.LanguageCode, unitMetadataSet);
				}
			}
			Registry.FixupNumberMetadata();
			Registry.Validate();
		}
	}
}
