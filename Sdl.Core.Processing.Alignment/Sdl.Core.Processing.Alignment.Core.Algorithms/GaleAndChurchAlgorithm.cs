using Sdl.Core.Processing.Alignment.Core.CostComputers;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal class GaleAndChurchAlgorithm : AbstractAlignmentAlgorithm
	{
		private IAlignmentCostComputer _costComputer;

		public GaleAndChurchAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings)
			: base(alignmentAlgorithmSettings)
		{
		}

		internal override IAlignmentCostComputer GetAlignmentCostComputer()
		{
			return _costComputer;
		}

		public override IList<AlignmentData> Align(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements)
		{
			_costComputer = CreateCostComputer(leftInputElements, rightInputElements);
			return base.Align(leftInputElements, rightInputElements);
		}

		protected virtual IAlignmentCostComputer CreateCostComputer(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements)
		{
			return new GaleAndChurchCostComputer(GetExpansionFactor(leftInputElements, rightInputElements));
		}

		private static double GetExpansionFactor(IEnumerable<AlignmentElement> leftInputElements, IEnumerable<AlignmentElement> rightInputElements)
		{
			long num = leftInputElements.Select((AlignmentElement leftInputElement) => leftInputElement.TextContent.Length).Sum();
			long num2 = rightInputElements.Select((AlignmentElement rightInputElement) => rightInputElement.TextContent.Length).Sum();
			if (num <= 0 || num2 <= 0)
			{
				return 1.0;
			}
			return (double)num2 / (double)num;
		}
	}
}
