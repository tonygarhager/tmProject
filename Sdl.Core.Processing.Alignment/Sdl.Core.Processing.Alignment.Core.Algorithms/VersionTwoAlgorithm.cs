using Sdl.Core.Processing.Alignment.Core.CostComputers;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal class VersionTwoAlgorithm : AbstractAlignmentAlgorithm
	{
		public override int DiagonalThicknessOverride => 100;

		public VersionTwoAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings)
			: base(alignmentAlgorithmSettings)
		{
		}

		internal override IAlignmentCostComputer GetAlignmentCostComputer()
		{
			return new CompositeCostComputer(new List<CompositeCostComponent>
			{
				new CompositeCostComponent(new HunAlignLengthCostComputer(), 5.0),
				new CompositeCostComponent(new HunAlignAlignmentTypeCostComputer(), 8.0),
				new CompositeCostComponent(new TagCountCostComputer(), 3.0),
				new CompositeCostComponent(new GaleAndChurchCostComputer(), 1.0),
				new CompositeCostComponent(new PartialMatchCostComputer(), 1.0)
			});
		}
	}
}
