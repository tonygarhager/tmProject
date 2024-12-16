using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class HunAlignLengthCostComputer : IAlignmentCostComputer
	{
		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			int val = sourceElements.Sum((AlignmentElement sourceElement) => sourceElement.TextContent.Length);
			int val2 = targetElements.Sum((AlignmentElement targetElement) => targetElement.TextContent.Length);
			int num = Math.Min(val, val2);
			int num2 = Math.Max(val, val2);
			if (num2 == 0)
			{
				return AlignmentCost.MinValue;
			}
			return (AlignmentCost)(1.0 - (double)num / (double)num2);
		}
	}
}
