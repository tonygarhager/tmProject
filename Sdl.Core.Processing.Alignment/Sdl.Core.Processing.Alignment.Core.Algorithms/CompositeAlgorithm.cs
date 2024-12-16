using Sdl.Core.Processing.Alignment.Core.CostComputers;
using System;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal class CompositeAlgorithm : AbstractAlignmentAlgorithm
	{
		private readonly CompositeCostComputer _compositeCostComputer;

		public CompositeAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings, CompositeCostComputer compositeCostComputer)
			: base(alignmentAlgorithmSettings)
		{
			if (compositeCostComputer == null)
			{
				throw new ArgumentNullException("compositeCostComputer");
			}
			_compositeCostComputer = compositeCostComputer;
		}

		internal override IAlignmentCostComputer GetAlignmentCostComputer()
		{
			return _compositeCostComputer;
		}
	}
}
