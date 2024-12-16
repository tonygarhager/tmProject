using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Globalization.UnitMetadata
{
	public class UnitMetadataSet
	{
		[JsonProperty]
		internal List<UnitMetadata> UnitMetadataList
		{
			get;
			set;
		}

		[JsonProperty("code")]
		public string LanguageCode
		{
			get;
			set;
		}

		internal string SerializeObject()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		internal static UnitMetadataSet DeserializeObject(string value)
		{
			return JsonConvert.DeserializeObject<UnitMetadataSet>(value);
		}

		internal UnitMetadata GetMetadataByKey(string unitKey)
		{
			return UnitMetadataList.FirstOrDefault((UnitMetadata y) => string.CompareOrdinal(y.UnitKey, unitKey) == 0);
		}
	}
}
