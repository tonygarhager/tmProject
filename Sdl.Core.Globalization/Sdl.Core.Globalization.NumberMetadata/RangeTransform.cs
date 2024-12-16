using Newtonsoft.Json;

namespace Sdl.Core.Globalization.NumberMetadata
{
	public class RangeTransform
	{
		[JsonProperty("op")]
		public RangeOperator RangeOperator
		{
			get;
			set;
		}

		[JsonProperty("arg")]
		public int Argument
		{
			get;
			set;
		}

		internal int Apply(int i)
		{
			switch (RangeOperator)
			{
			case RangeOperator.opDiv:
				return i / Argument;
			case RangeOperator.opMod:
				return i % Argument;
			default:
				throw new NumberMetadataRegistryException("Unknown RangeOperator: " + RangeOperator.ToString());
			}
		}

		internal bool Validate()
		{
			if (Argument > 0)
			{
				return RangeOperator > (RangeOperator)0;
			}
			return false;
		}
	}
}
