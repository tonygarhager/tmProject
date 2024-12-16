using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.Core.Algorithms;
using Sdl.Core.Processing.Alignment.Core.CostComputers;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	internal class ReverseGapAligner : GapAligner
	{
		private MergeParagraphParser _mergeParagraphParser;

		private byte _minimumAlignmentQuality;

		public ReverseGapAligner(IAlignmentAlgorithm algorithm, AlignmentAlgorithmSettings algorithmSettings, MergeParagraphParser mergeParagraphParser, byte minimumAlignmentQuality)
			: base(algorithm, null, algorithmSettings)
		{
			_mergeParagraphParser = mergeParagraphParser;
			_minimumAlignmentQuality = minimumAlignmentQuality;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			IList<AlignmentElement> sourceElements = GetSourceElements(paragraphUnit, null, null, isRetrofit: true);
			IList<AlignmentElement> targetElements = GetTargetElements(paragraphUnit, null, null, isRetrofit: true);
			AlignRecursively(sourceElements, targetElements, paragraphUnit);
			GapAligner.AssignQuality(base.Alignments);
			if (CurrentProgress < 100)
			{
				InvokeProgressHandler(new ProgressEventArgs(100));
			}
		}

		internal bool AlignRecursively(IList<AlignmentElement> sourceElements, IList<AlignmentElement> targetElements, IParagraphUnit paragraphUnit)
		{
			IList<AlignmentGap> list = CalculateAlignmentGaps(sourceElements, targetElements).ToList();
			foreach (AlignmentGap item in list)
			{
				AlignmentGap gap = item;
				IList<AlignmentElement> list2 = sourceElements.Where((AlignmentElement x, int idx) => gap.SourceContains(idx)).ToList();
				IList<AlignmentElement> list3 = targetElements.Where((AlignmentElement x, int idx) => gap.TargetContains(idx)).ToList();
				if ((list2.Count > 0 || list3.Count > 0) && ValidateGap(list2, list3))
				{
					LocalAlign(list2, list3);
				}
			}
			return true;
		}

		public bool ValidateGap(IEnumerable<AlignmentElement> sourceElements, IEnumerable<AlignmentElement> targetElements)
		{
			LCSCostComputer lCSCostComputer = new LCSCostComputer();
			AlignmentCost alignmentCost = lCSCostComputer.GetAlignmentCost(sourceElements, targetElements);
			double num = 1.0 - (double)(int)_minimumAlignmentQuality / 100.0;
			return (double)alignmentCost < num;
		}

		internal IEnumerable<AlignmentGap> CalculateAlignmentGaps(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements)
		{
			List<AlignmentGap> list = new List<AlignmentGap>();
			int firstSourceIndex = 0;
			int firstTargetIndex = 0;
			if (leftElements.Count == 0 || rightElements.Count == 0)
			{
				return list;
			}
			_mergeParagraphParser.ReverseMapper.SetParagraphAssociations();
			int i = 1;
			int j = 1;
			for (int k = 1; k <= _mergeParagraphParser.ReverseMapper.ParagraphAssociations.Count; k++)
			{
				ParagraphUnitId left = _mergeParagraphParser.ReverseMapper.ParagraphAssociations[k].Left;
				ParagraphUnitId right = _mergeParagraphParser.ReverseMapper.ParagraphAssociations[k].Right;
				for (; i < leftElements.Count && _mergeParagraphParser.ReverseMapper.LeftSegmentIds[int.Parse(leftElements[i].Id.SegmentId.Id)].OriginalParagraphUnitId == left; i++)
				{
				}
				for (; j < rightElements.Count && _mergeParagraphParser.ReverseMapper.RightSegmentIds[int.Parse(rightElements[j].Id.SegmentId.Id)].OriginalParagraphUnitId == right; j++)
				{
				}
				list.Add(new AlignmentGap
				{
					FirstSourceIndex = firstSourceIndex,
					LastSourceIndex = i - 1,
					FirstTargetIndex = firstTargetIndex,
					LastTargetIndex = j - 1
				});
				firstSourceIndex = i;
				firstTargetIndex = j;
			}
			list.Add(new AlignmentGap
			{
				FirstSourceIndex = firstSourceIndex,
				LastSourceIndex = i - 1,
				FirstTargetIndex = firstTargetIndex,
				LastTargetIndex = j - 1
			});
			return list;
		}
	}
}
