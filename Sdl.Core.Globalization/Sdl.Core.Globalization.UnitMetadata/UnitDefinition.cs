using Newtonsoft.Json;

namespace Sdl.Core.Globalization.UnitMetadata
{
	public class UnitDefinition
	{
		[JsonProperty("key")]
		public string UnitKey
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public string UnitType
		{
			get;
			set;
		}
	}
}
