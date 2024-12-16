using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Globalization.NumberMetadata
{
	public class NumberMetadata
	{
		[JsonProperty("code")]
		public string LanguageCode
		{
			get;
			set;
		}

		[JsonProperty("sets")]
		public List<NumberGrammarRangeSet> NumberGrammarRangeSets
		{
			get;
			set;
		}

		internal string SerializeObject()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		internal static NumberMetadata DeserializeObject(string value)
		{
			return JsonConvert.DeserializeObject<NumberMetadata>(value);
		}

		internal string GetRangeID(int i)
		{
			NumberGrammarRangeSet numberGrammarRangeSet = NumberGrammarRangeSets.FirstOrDefault((NumberGrammarRangeSet x) => x.IntNumberGrammarRanges != null && (x.IntNumberGrammarRanges.Count == 0 || x.IntNumberGrammarRanges.Any((IntNumberGrammarRange y) => y.IsInRange(i))));
			if (numberGrammarRangeSet != null)
			{
				return numberGrammarRangeSet.ID;
			}
			return NumberGrammarRangeSets.First().ID;
		}

		internal string GetRangeID(double d)
		{
			NumberGrammarRangeSet numberGrammarRangeSet = NumberGrammarRangeSets.FirstOrDefault((NumberGrammarRangeSet x) => x.DoubleNumberGrammarRanges != null && (x.DoubleNumberGrammarRanges.Count == 0 || x.DoubleNumberGrammarRanges.Any((DoubleNumberGrammarRange y) => y.IsInRange(d))));
			if (numberGrammarRangeSet != null)
			{
				return numberGrammarRangeSet.ID;
			}
			return NumberGrammarRangeSets.First().ID;
		}

		internal string Validate()
		{
			List<IntNumberGrammarRange> list = new List<IntNumberGrammarRange>();
			foreach (NumberGrammarRangeSet numberGrammarRangeSet in NumberGrammarRangeSets)
			{
				if (numberGrammarRangeSet.IntNumberGrammarRanges.Any((IntNumberGrammarRange x) => x.RangeTransforms != null && x.RangeTransforms.Count > 0))
				{
					return null;
				}
				list.AddRange(numberGrammarRangeSet.IntNumberGrammarRanges);
			}
			IntNumberGrammarRange intNumberGrammarRange = list.FirstOrDefault((IntNumberGrammarRange x) => !x.Validate());
			if (intNumberGrammarRange != null)
			{
				return "Invalid range: " + intNumberGrammarRange.ToString();
			}
			return ValidateIntegerRanges(list);
		}

		private string ValidateIntegerRanges(List<IntNumberGrammarRange> ranges)
		{
			if (ranges.Count((IntNumberGrammarRange x) => x.LowerBound == 0) != 1)
			{
				return "No single range covering 'down to zero'";
			}
			if (ranges.Count((IntNumberGrammarRange x) => !x.UpperBound.HasValue) != 1)
			{
				return "No single range covering 'all above X'";
			}
			IntNumberGrammarRange intNumberGrammarRange = ranges.First((IntNumberGrammarRange x) => !x.UpperBound.HasValue);
			List<IntNumberGrammarRange> list = ranges.FindAll((IntNumberGrammarRange x) => x.UpperBound.HasValue);
			list.Sort((IntNumberGrammarRange a, IntNumberGrammarRange b) => a.LowerBound.CompareTo(b.UpperBound.Value));
			for (int i = 0; i < list.Count - 1; i++)
			{
				if (list[i].LowerBound == list[i + 1].LowerBound || list[i].UpperBound != list[i + 1].LowerBound - 1)
				{
					return "Ranges are not contiguous";
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			IntNumberGrammarRange intNumberGrammarRange2 = list.Last();
			if (intNumberGrammarRange.LowerBound == intNumberGrammarRange2.UpperBound + 1)
			{
				return null;
			}
			return "Ranges are not contiguous";
		}
	}
}
