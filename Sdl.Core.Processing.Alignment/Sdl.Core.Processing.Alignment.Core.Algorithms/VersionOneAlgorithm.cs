using Sdl.Core.Processing.Alignment.Core.CostComputers;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal class VersionOneAlgorithm : OptimisticAlgorithm
	{
		public VersionOneAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings)
			: base(alignmentAlgorithmSettings)
		{
		}

		protected override IAlignmentCostComputer CreateCostComputer(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements)
		{
			IAlignmentCostComputer alignmentCostComputer = base.CreateCostComputer(leftInputElements, rightInputElements);
			return new CompositeCostComputer(new List<CompositeCostComponent>
			{
				new CompositeCostComponent(alignmentCostComputer, 10.0),
				new CompositeCostComponent(new TagEqualsCostComputer(), 10.0)
			});
		}
	}
}
