using System;
using System.Diagnostics;
using System.Globalization;

namespace Sdl.Core.Processing.Alignment.Core
{
	public struct AlignmentCost
	{
		public static readonly AlignmentCost MinValue = new AlignmentCost(0.0);

		public static readonly AlignmentCost MaxValue = new AlignmentCost(1.0);

		private readonly double _value;

		public AlignmentCost(double value)
		{
			_value = value;
		}

		public static implicit operator double(AlignmentCost alignmentCost)
		{
			return alignmentCost._value;
		}

		public static explicit operator AlignmentCost(double value)
		{
			return new AlignmentCost(value);
		}

		[Conditional("DEBUG")]
		private static void Check(double value)
		{
			if (value < 0.0 || value > 1.0)
			{
				throw new ArgumentOutOfRangeException("value", "value must be between 0.0 and 1.0 inclusive");
			}
		}

		public override string ToString()
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
