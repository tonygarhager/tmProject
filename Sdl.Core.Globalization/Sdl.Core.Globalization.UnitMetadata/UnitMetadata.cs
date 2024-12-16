using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Globalization.UnitMetadata
{
	public class UnitMetadata
	{
		[JsonProperty("key")]
		public string UnitKey
		{
			get;
			set;
		}

		[JsonProperty("noinherit", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public bool DoNotInherit
		{
			get;
			set;
		}

		[JsonProperty("sets")]
		public List<LabelValueSet> LabelValueSets
		{
			get;
			set;
		}

		public LabelValueSet LabelValueSetFromLabel(string label)
		{
			return LabelValueSets.FirstOrDefault((LabelValueSet x) => x.ContainsLabel(label));
		}
	}
}
