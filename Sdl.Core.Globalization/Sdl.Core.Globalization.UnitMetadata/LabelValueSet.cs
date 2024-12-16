using Newtonsoft.Json;
using Sdl.Core.Globalization.NumberMetadata;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Globalization.UnitMetadata
{
	public class LabelValueSet
	{
		internal Sdl.Core.Globalization.NumberMetadata.NumberMetadata NumberMetadata
		{
			get;
			set;
		}

		[JsonProperty("pref", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public bool Preferred
		{
			get;
			set;
		}

		[JsonProperty("labels")]
		public List<LabelValueCondition> LabelValueConditions
		{
			get;
			set;
		}

		public bool ContainsLabel(string label)
		{
			return LabelValueConditions.Any((LabelValueCondition x) => string.CompareOrdinal(label, x.Label) == 0);
		}

		public string GetLabel(int value)
		{
			if (LabelValueConditions.Count == 1)
			{
				return LabelValueConditions[0].Label;
			}
			string id = NumberMetadata.GetRangeID(value);
			LabelValueCondition labelValueCondition = LabelValueConditions.FirstOrDefault((LabelValueCondition x) => x.HasID(id));
			if (labelValueCondition != null)
			{
				return labelValueCondition.Label;
			}
			return LabelValueConditions[0].Label;
		}

		public string GetLabel(double value)
		{
			if (LabelValueConditions.Count == 1)
			{
				return LabelValueConditions[0].Label;
			}
			string id = NumberMetadata.GetRangeID(value);
			LabelValueCondition labelValueCondition = LabelValueConditions.FirstOrDefault((LabelValueCondition x) => x.HasID(id));
			if (labelValueCondition != null)
			{
				return labelValueCondition.Label;
			}
			return LabelValueConditions[0].Label;
		}

		internal string Validate(Sdl.Core.Globalization.NumberMetadata.NumberMetadata numberMetadata)
		{
			if (LabelValueConditions.Count == 0)
			{
				return "Must have at least one LabelValueCondition";
			}
			if (LabelValueConditions.Any((LabelValueCondition x) => string.IsNullOrWhiteSpace(x.Label)))
			{
				return "Found LabelValueCondition with empty label";
			}
			if (LabelValueConditions.Count == 1)
			{
				if (LabelValueConditions[0].NumberGrammarRangeSetID != null)
				{
					return "Only 1 condition but NumberGrammarRangeSetID specified";
				}
				return null;
			}
			if (numberMetadata?.NumberGrammarRangeSets == null || numberMetadata.NumberGrammarRangeSets.Count == 0)
			{
				return ">1 form specified but language has no plural forms";
			}
			if (numberMetadata.NumberGrammarRangeSets.Count != LabelValueConditions.Count)
			{
				return "Number of label forms does not matching number of language plural types";
			}
			foreach (NumberGrammarRangeSet i in numberMetadata.NumberGrammarRangeSets)
			{
				if (!LabelValueConditions.Any((LabelValueCondition x) => string.CompareOrdinal(x.NumberGrammarRangeSetID, i.ID) == 0))
				{
					return "LabelValueCondition not matching any NumberGrammarRangeSetID";
				}
			}
			return null;
		}
	}
}
