namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal class OptimisticAlgorithm : GaleAndChurchAlgorithm
	{
		private const int MaxOptimisticDiagonalWidth = 25;

		public override int DiagonalThicknessOverride => 25;

		public OptimisticAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings)
			: base(alignmentAlgorithmSettings)
		{
		}
	}
}
