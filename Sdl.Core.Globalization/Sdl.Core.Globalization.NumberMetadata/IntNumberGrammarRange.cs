using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Globalization.NumberMetadata
{
	public class IntNumberGrammarRange
	{
		[JsonProperty("transforms", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public List<RangeTransform> RangeTransforms
		{
			get;
			set;
		}

		[JsonProperty("lower")]
		public int LowerBound
		{
			get;
			set;
		}

		[JsonProperty("upper")]
		public int? UpperBound
		{
			get;
			set;
		}

		public bool IsInRange(int i)
		{
			if (RangeTransforms != null)
			{
				foreach (RangeTransform rangeTransform in RangeTransforms)
				{
					i = rangeTransform.Apply(i);
				}
			}
			if (i < 0)
			{
				i *= -1;
			}
			if (i < LowerBound)
			{
				return false;
			}
			if (UpperBound.HasValue)
			{
				return i <= UpperBound;
			}
			return true;
		}

		internal bool Validate()
		{
			if (RangeTransforms != null && RangeTransforms.Any((RangeTransform x) => !x.Validate()))
			{
				return false;
			}
			if (!UpperBound.HasValue)
			{
				return true;
			}
			return LowerBound <= UpperBound.Value;
		}

		public override string ToString()
		{
			return LowerBound.ToString() + "-" + UpperBound.ToString();
		}
	}
}
