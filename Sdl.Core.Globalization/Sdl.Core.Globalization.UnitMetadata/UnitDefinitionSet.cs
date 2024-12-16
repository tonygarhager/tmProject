using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Globalization.UnitMetadata
{
	public class UnitDefinitionSet
	{
		public List<UnitDefinition> UnitDefinitions
		{
			get;
			set;
		}

		public UnitDefinition GetDefinitionByKey(string unitKey)
		{
			return UnitDefinitions.FirstOrDefault((UnitDefinition x) => string.CompareOrdinal(x.UnitKey, unitKey) == 0);
		}

		internal string SerializeObject()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		internal static UnitDefinitionSet DeserializeObject(string value)
		{
			return JsonConvert.DeserializeObject<UnitDefinitionSet>(value);
		}
	}
}
