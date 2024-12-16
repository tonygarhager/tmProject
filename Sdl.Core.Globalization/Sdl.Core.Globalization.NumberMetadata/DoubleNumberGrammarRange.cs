using Newtonsoft.Json;

namespace Sdl.Core.Globalization.NumberMetadata
{
	public class DoubleNumberGrammarRange
	{
		[JsonProperty("lower")]
		public double LowerBound
		{
			get;
			set;
		}

		[JsonProperty("upper")]
		public double? UpperBound
		{
			get;
			set;
		}

		public bool IsInRange(double d)
		{
			if (d < 0.0)
			{
				d *= -1.0;
			}
			if (d < LowerBound)
			{
				return false;
			}
			if (UpperBound.HasValue)
			{
				return d <= UpperBound;
			}
			return true;
		}

		internal bool Validate()
		{
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
