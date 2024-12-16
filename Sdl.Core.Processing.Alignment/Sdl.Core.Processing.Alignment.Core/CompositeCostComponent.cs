using Sdl.Core.Processing.Alignment.Core.CostComputers;
using System;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class CompositeCostComponent
	{
		public IAlignmentCostComputer AlignmentCostComputer
		{
			get;
			private set;
		}

		public double Weight
		{
			get;
			private set;
		}

		public CompositeCostComponent(IAlignmentCostComputer alignmentCostComputer, double weight)
		{
			if (alignmentCostComputer == null)
			{
				throw new ArgumentNullException("alignmentCostComputer");
			}
			if (weight <= 0.0)
			{
				throw new ArgumentOutOfRangeException("weight", "weight must be > 0.0");
			}
			AlignmentCostComputer = alignmentCostComputer;
			Weight = weight;
		}
	}
}
