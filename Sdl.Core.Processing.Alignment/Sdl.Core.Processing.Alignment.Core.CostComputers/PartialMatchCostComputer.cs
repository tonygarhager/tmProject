using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class PartialMatchCostComputer : IAlignmentCostComputer
	{
		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			IList<AlignmentElement> list = (sourceElements as IList<AlignmentElement>) ?? sourceElements.ToList();
			IList<AlignmentElement> list2 = (targetElements as IList<AlignmentElement>) ?? targetElements.ToList();
			if ((!list.Any() && list2.Any()) || (list.Any() && !list2.Any()) || (list.Count == 2 && list2.Count == 2))
			{
				return AlignmentCost.MaxValue;
			}
			if (list.Count == 1)
			{
				string text = list[0].TextContent.Trim();
				if (list2.Count == 2 && text.StartsWith(list2[0].TextContent.Trim()))
				{
					return AlignmentCost.MinValue;
				}
			}
			if (list.Count == 2 && list2.Count == 1)
			{
				string text2 = list2[0].TextContent.Trim();
				if (text2.StartsWith(list[0].TextContent.Trim()))
				{
					return AlignmentCost.MinValue;
				}
			}
			return AlignmentCost.MaxValue;
		}
	}
}
