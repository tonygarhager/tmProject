using Newtonsoft.Json;

namespace Sdl.Core.Globalization.UnitMetadata
{
	public class LabelValueCondition
	{
		[JsonProperty("rangeid", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string NumberGrammarRangeSetID
		{
			get;
			set;
		}

		[JsonProperty("label")]
		public string Label
		{
			get;
			set;
		}

		internal bool HasID(string id)
		{
			return string.CompareOrdinal(id, NumberGrammarRangeSetID) == 0;
		}
	}
}
