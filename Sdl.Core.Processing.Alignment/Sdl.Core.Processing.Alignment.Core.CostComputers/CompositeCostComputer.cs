using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.CostComputers
{
	internal class CompositeCostComputer : IAlignmentCostComputer
	{
		internal readonly IEnumerable<CompositeCostComponent> CostComponents;

		internal readonly double MaximumCost;

		public CompositeCostComputer(IEnumerable<CompositeCostComponent> costComponents)
		{
			if (costComponents == null)
			{
				throw new ArgumentNullException("costComponents");
			}
			if (costComponents.Count() == 0)
			{
				throw new ArgumentException("costComponents must contain at least one cost component", "costComponents");
			}
			if (costComponents.Any((CompositeCostComponent x) => x.Weight <= 0.0))
			{
				throw new ArgumentException("The weight of at least one cost component is <= 0", "costComponents");
			}
			CostComponents = costComponents;
			MaximumCost = CostComponents.Sum((CompositeCostComponent costComponent) => costComponent.Weight);
		}

		public AlignmentCost GetAlignmentCost(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			double num = CostComponents.Sum((CompositeCostComponent costComponent) => costComponent.Weight * (double)costComponent.AlignmentCostComputer.GetAlignmentCost(sourceElements, targetElements));
			return (AlignmentCost)(num / MaximumCost);
		}
	}
}
