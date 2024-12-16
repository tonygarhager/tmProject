using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal interface IAlignmentCostComputer
	{
		AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements);
	}
}
