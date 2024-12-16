using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class HunAlignAlignmentTypeCostComputer : IAlignmentCostComputer
	{
		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			if (sourceElements.Count() != 1 || targetElements.Count() != 1)
			{
				return AlignmentCost.MaxValue;
			}
			return AlignmentCost.MinValue;
		}
	}
}
