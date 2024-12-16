using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core.CostComputers;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core.Algorithms
{
	internal class RetrofitAlignmentAlgorithm : AbstractAlignmentAlgorithm
	{
		public override int DiagonalThicknessOverride => 100;

		public RetrofitAlignmentAlgorithm(AlignmentAlgorithmSettings alignmentAlgorithmSettings)
			: base(alignmentAlgorithmSettings)
		{
		}

		internal override IAlignmentCostComputer GetAlignmentCostComputer()
		{
			return new CompositeCostComputer(new List<CompositeCostComponent>
			{
				new CompositeCostComponent(new TagCountCostComputer(), 1.0),
				new CompositeCostComponent(new LCSCostComputer(), 6.0)
			});
		}

		internal override void CalculateContractionMinimumCost(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentArray elements, int i, int j, IAlignmentCostComputer alignmentCostComputer, ref AlignmentArrayElement elementWithMinimumCost)
		{
			int k = 2;
			List<AlignmentElement> list = new List<AlignmentElement>
			{
				leftInputElements[i - 1]
			};
			List<AlignmentElement> targetElements = new List<AlignmentElement>
			{
				rightInputElements[j - 1]
			};
			for (; i - k >= 0; k++)
			{
				list.Insert(0, leftInputElements[i - k]);
				AlignmentCost alignmentCost = alignmentCostComputer.GetAlignmentCost(list, targetElements);
				double num = elements[i - k, j - 1].TotalCost + (double)alignmentCost;
				if (elementWithMinimumCost == null || elementWithMinimumCost.TotalCost > num)
				{
					elementWithMinimumCost = new AlignmentArrayElement(alignmentCost, num, AlignmentOperation.Contraction, k);
				}
			}
		}

		internal override void CalculateExpansionMinimumCost(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentArray elements, int i, int j, IAlignmentCostComputer alignmentCostComputer, ref AlignmentArrayElement elementWithMinimumCost)
		{
			int k = 2;
			List<AlignmentElement> sourceElements = new List<AlignmentElement>
			{
				leftInputElements[i - 1]
			};
			List<AlignmentElement> list = new List<AlignmentElement>
			{
				rightInputElements[j - 1]
			};
			for (; j - k >= 0; k++)
			{
				list.Insert(0, rightInputElements[j - k]);
				AlignmentCost alignmentCost = alignmentCostComputer.GetAlignmentCost(sourceElements, list);
				double num = elements[i - 1, j - k].TotalCost + (double)alignmentCost;
				if (elementWithMinimumCost == null || elementWithMinimumCost.TotalCost > num)
				{
					elementWithMinimumCost = new AlignmentArrayElement(alignmentCost, num, AlignmentOperation.Expansion, k);
				}
			}
		}

		internal override void CalculateMeldingCrossMinimumCost(IList<AlignmentElement> leftInputElements, IList<AlignmentElement> rightInputElements, IAlignmentArray elements, int i, int j, IAlignmentCostComputer alignmentCostComputer, ref AlignmentArrayElement elementWithMinimumCost)
		{
			AlignmentCost alignmentCost = alignmentCostComputer.GetAlignmentCost(new List<AlignmentElement>
			{
				leftInputElements[i - 2],
				leftInputElements[i - 1]
			}, new List<AlignmentElement>
			{
				rightInputElements[j - 1],
				rightInputElements[j - 2]
			});
			double num = elements[i - 2, j - 2].TotalCost + (double)alignmentCost;
			if (elementWithMinimumCost == null || elementWithMinimumCost.TotalCost > num)
			{
				elementWithMinimumCost = new AlignmentArrayElement(alignmentCost, num, AlignmentOperation.MeldingCross);
			}
		}

		internal override void AddExpansionAlignment(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements, AlignmentCost elementCost, LinkedList<AlignmentData> alignments, ref int i, ref int j, int factor)
		{
			int num = factor;
			AlignmentData alignmentData = (factor < 4) ? new AlignmentData() : new AlignmentData(AlignmentType.Alignment1N);
			alignmentData.LeftIds.Add(leftElements[i - 1].Id);
			while (j - factor <= j - 1)
			{
				alignmentData.RightIds.Add(rightElements[j - factor].Id);
				factor--;
			}
			alignmentData.Cost = elementCost;
			alignments.AddFirst(alignmentData);
			j -= num;
			i--;
		}

		internal override void AddContractionAlignment(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements, AlignmentCost elementCost, LinkedList<AlignmentData> alignments, ref int i, ref int j, int factor)
		{
			int num = factor;
			AlignmentData alignmentData = (factor < 4) ? new AlignmentData() : new AlignmentData(AlignmentType.AlignmentN1);
			alignmentData.RightIds.Add(rightElements[j - 1].Id);
			while (i - factor <= i - 1)
			{
				alignmentData.LeftIds.Add(leftElements[i - factor].Id);
				factor--;
			}
			alignmentData.Cost = elementCost;
			alignments.AddFirst(alignmentData);
			i -= num;
			j--;
		}

		internal override void AddMeldingCrossAlignment(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements, AlignmentCost elementCost, LinkedList<AlignmentData> alignments, ref int i, ref int j)
		{
			AlignmentData alignmentData = new AlignmentData(AlignmentType.Alignment22C);
			alignmentData.LeftIds.Add(leftElements[i - 2].Id);
			alignmentData.LeftIds.Add(leftElements[i - 1].Id);
			alignmentData.RightIds.Add(rightElements[j - 2].Id);
			alignmentData.RightIds.Add(rightElements[j - 1].Id);
			alignmentData.Cost = elementCost;
			alignments.AddFirst(alignmentData);
			i -= 2;
			j -= 2;
		}

		internal override bool IsOperationAllowed(IAlignmentArray elements, int i, int j, AlignmentOperation op)
		{
			if (op == AlignmentOperation.MeldingCross)
			{
				if (i > 1 && j > 1)
				{
					return elements.IsValidAndDefined(i - 2, j - 2);
				}
				return false;
			}
			return base.IsOperationAllowed(elements, i, j, op);
		}
	}
}
