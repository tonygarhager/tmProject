using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.Core.Algorithms;
using Sdl.Core.Processing.Alignment.Core.Anchoring;
using Sdl.Core.Processing.Alignment.Tokenization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment
{
	internal class GapAligner : Aligner
	{
		private int _numberOfProcessedItems;

		private int _totalItemsInGaps;

		private int _currentGapSize;

		protected byte CurrentProgress;

		private readonly IEnumerable<AbstractAnchoringStrategy> _anchoringStrategies;

		private bool _overrideParagraphUnitIdForConfirmedAlignments;

		private TokensContainer _tokensContainer;

		private readonly AlignmentAlgorithmSettings _algorithmSettings;

		public bool OverrideParagraphUnitIdForConfirmedAlignments
		{
			set
			{
				_overrideParagraphUnitIdForConfirmedAlignments = value;
			}
		}

		public GapAligner(IAlignmentAlgorithm algorithm)
			: base(algorithm)
		{
			_anchoringStrategies = new List<AbstractAnchoringStrategy>();
			_algorithmSettings = new AlignmentAlgorithmSettings(algorithm.SourceCulture, algorithm.TargetCulture);
		}

		public GapAligner(IAlignmentAlgorithm algorithm, IEnumerable<AbstractAnchoringStrategy> anchoringStrategies, AlignmentAlgorithmSettings algorithmSettings)
			: base(algorithm)
		{
			_anchoringStrategies = anchoringStrategies;
			_algorithmSettings = algorithmSettings;
		}

		internal GapAligner()
			: base(null)
		{
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			IList<AlignmentElement> sourceElements = GetSourceElements(paragraphUnit, null, null);
			IList<AlignmentElement> targetElements = GetTargetElements(paragraphUnit, null, null);
			_tokensContainer = new TokensContainer(sourceElements, targetElements, _algorithmSettings.LeftCulture, _algorithmSettings.RightCulture);
			_anchoringStrategies.ToList().ForEach(delegate(AbstractAnchoringStrategy x)
			{
				x.TokensContainer = _tokensContainer;
			});
			ParagraphUnitId paragraphUnitId = paragraphUnit.Properties.ParagraphUnitId;
			if (_overrideParagraphUnitIdForConfirmedAlignments)
			{
				(_anchoringStrategies.FirstOrDefault((AbstractAnchoringStrategy strategy) => strategy is ConfirmedAlignmentsAnchorStrategy) as ConfirmedAlignmentsAnchorStrategy)?.SetParagraphUnitId(paragraphUnitId);
			}
			_totalItemsInGaps = sourceElements.Count();
			AlignRecursively(sourceElements, targetElements, paragraphUnitId, _anchoringStrategies);
			AssignQuality(base.Alignments);
			if (CurrentProgress < 100)
			{
				InvokeProgressHandler(new ProgressEventArgs(100));
			}
		}

		internal virtual bool AlignRecursively(IList<AlignmentElement> sourceElements, IList<AlignmentElement> targetElements, ParagraphUnitId paragraphUnitId, IEnumerable<IAnchoringStrategy> anchoringStrategies)
		{
			if (anchoringStrategies != null && anchoringStrategies.Count() > 0)
			{
				IEnumerable<AlignmentData> anchors = anchoringStrategies.First().GetAnchors(sourceElements, targetElements);
				IList<SegmentId> sourceSegmentIds = sourceElements.Select((AlignmentElement x) => x.Id.SegmentId).ToList();
				IList<SegmentId> targetSegmentIds = targetElements.Select((AlignmentElement x) => x.Id.SegmentId).ToList();
				IEnumerable<AlignmentData> validAlignments = GetValidAlignments(anchors, paragraphUnitId, sourceSegmentIds, targetSegmentIds);
				IList<AlignmentGap> list = CalculateAlignmentGaps(validAlignments, sourceSegmentIds, targetSegmentIds).ToList();
				foreach (AlignmentGap item in list)
				{
					AlignmentGap gap = item;
					IList<AlignmentElement> list2 = sourceElements.Where((AlignmentElement x, int idx) => gap.SourceContains(idx)).ToList();
					IList<AlignmentElement> list3 = targetElements.Where((AlignmentElement x, int idx) => gap.TargetContains(idx)).ToList();
					if (list2.Count > 0 || list3.Count > 0)
					{
						AlignRecursively(list2, list3, paragraphUnitId, anchoringStrategies.Skip(1));
					}
				}
				foreach (AlignmentData item2 in validAlignments)
				{
					base.Alignments.Add(item2);
				}
				return true;
			}
			LocalAlign(sourceElements, targetElements);
			return false;
		}

		internal IEnumerable<AlignmentGap> CalculateAlignmentGaps(IEnumerable<AlignmentData> alignments, IList<SegmentId> sourceSegmentIds, IList<SegmentId> targetSegmentIds)
		{
			List<AlignmentGap> list = new List<AlignmentGap>();
			IEnumerable<Pair<SegmentId, int>> alignmentSegmentIds = alignments.SelectMany((AlignmentData x, int i) => x.LeftIds.Select((DocumentSegmentId y) => new Pair<SegmentId, int>(y.SegmentId, i)));
			List<Pair<int, int>> list2 = FindRanges(alignmentSegmentIds, sourceSegmentIds).ToList();
			IEnumerable<Pair<SegmentId, int>> alignmentSegmentIds2 = alignments.SelectMany((AlignmentData x, int i) => x.RightIds.Select((DocumentSegmentId y) => new Pair<SegmentId, int>(y.SegmentId, i)));
			List<Pair<int, int>> list3 = FindRanges(alignmentSegmentIds2, targetSegmentIds).ToList();
			if (list2.Count != list3.Count)
			{
				throw new Exception("Some of the passed alignments are complex!");
			}
			for (int j = 0; j < list2.Count; j++)
			{
				list.Add(new AlignmentGap
				{
					FirstSourceIndex = list2[j].First,
					LastSourceIndex = list2[j].Second,
					FirstTargetIndex = list3[j].First,
					LastTargetIndex = list3[j].Second
				});
			}
			return list;
		}

		internal void LocalAlign(IList<AlignmentElement> sourceElements, IList<AlignmentElement> targetElements)
		{
			_currentGapSize = Math.Max(sourceElements.Count, targetElements.Count);
			base.Algorithm.OnProgress += ReportProgress;
			try
			{
				if (!base.CancelProcessing)
				{
					IList<AlignmentData> list = base.Algorithm.Align(sourceElements, targetElements);
					foreach (AlignmentData item in list)
					{
						base.Alignments.Add(item);
					}
					_numberOfProcessedItems += sourceElements.Count();
					_ = base.CancelProcessing;
				}
			}
			catch (UserCancelledException)
			{
				base.CancelProcessing = true;
				throw;
			}
			finally
			{
				base.Algorithm.OnProgress -= ReportProgress;
			}
		}

		private void ReportProgress(object sender, ProgressEventArgs algorithmProgress)
		{
			byte b = ProgressEstimator.CalculateProgressForIntervals(_currentGapSize, _numberOfProcessedItems, _totalItemsInGaps, algorithmProgress.ProgressValue);
			if (CurrentProgress < b)
			{
				CurrentProgress = b;
				InvokeProgressHandler(new ProgressEventArgs(b));
			}
		}

		internal static IEnumerable<Pair<int, int>> FindRanges(IEnumerable<Pair<SegmentId, int>> alignmentSegmentIds, IList<SegmentId> segmentIds)
		{
			List<Pair<int, int>> list = new List<Pair<int, int>>();
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int i;
			for (i = 0; i < segmentIds.Count; i++)
			{
				Pair<SegmentId, int> pair = alignmentSegmentIds.FirstOrDefault((Pair<SegmentId, int> x) => x.First == segmentIds[i]);
				if (pair == null)
				{
					if (num == -1)
					{
						num = i;
					}
					else
					{
						num2 = i;
					}
					continue;
				}
				if (num != -1)
				{
					list.Add(new Pair<int, int>(num, (num2 != -1) ? num2 : num));
				}
				else if (i == 0 || (i == segmentIds.Count - 1 && pair.Second != num3) || pair.Second != num3)
				{
					list.Add(new Pair<int, int>(-1, -1));
				}
				num3 = pair.Second;
				num = -1;
				num2 = -1;
			}
			if (num != -1)
			{
				list.Add(new Pair<int, int>(num, (num2 != -1) ? num2 : num));
			}
			else
			{
				list.Add(new Pair<int, int>(-1, -1));
			}
			return list;
		}

		internal static IEnumerable<AlignmentData> FindAlignmentsToIgnore(IEnumerable<AlignmentData> alignments, ParagraphUnitId paragraphUnitId, IEnumerable<SegmentId> sourceSegmentIds, IEnumerable<SegmentId> targetSegmentIds)
		{
			List<AlignmentData> list = new List<AlignmentData>();
			foreach (AlignmentData alignment in alignments)
			{
				if (!alignment.LeftIds.All((DocumentSegmentId x) => x.ParagraphUnitId == paragraphUnitId))
				{
					list.Add(alignment);
				}
				else if (!alignment.RightIds.All((DocumentSegmentId x) => x.ParagraphUnitId == paragraphUnitId))
				{
					list.Add(alignment);
				}
				else if (!alignment.LeftIds.All((DocumentSegmentId x) => sourceSegmentIds.Contains(x.SegmentId)))
				{
					list.Add(alignment);
				}
				else if (!alignment.RightIds.All((DocumentSegmentId x) => targetSegmentIds.Contains(x.SegmentId)))
				{
					list.Add(alignment);
				}
				else if (alignment.LeftIds.Count == 0 && alignment.RightIds.Count == 1)
				{
					list.Add(alignment);
				}
				else if (alignment.LeftIds.Count == 1 && alignment.RightIds.Count == 0)
				{
					list.Add(alignment);
				}
			}
			return list;
		}

		internal static bool IsAlignmentComplex(AlignmentData alignment, IList<SegmentId> sourceSegmentIds, IList<SegmentId> targetSegmentIds)
		{
			int[] indices = alignment.LeftIds.Select((DocumentSegmentId x) => sourceSegmentIds.IndexOf(x.SegmentId)).ToArray();
			bool flag = !AreContinious(indices);
			if (!flag)
			{
				indices = alignment.RightIds.Select((DocumentSegmentId x) => targetSegmentIds.IndexOf(x.SegmentId)).ToArray();
				flag = !AreContinious(indices);
			}
			return flag;
		}

		internal static bool AreContinious(int[] indices)
		{
			Array.Sort(indices);
			for (int i = 1; i < indices.Length; i++)
			{
				if (indices[i] != indices[i - 1] + 1)
				{
					return false;
				}
			}
			return true;
		}

		internal static IEnumerable<AlignmentData> GetValidAlignments(IEnumerable<AlignmentData> alignments, ParagraphUnitId paragraphUnitId, IList<SegmentId> sourceSegmentIds, IList<SegmentId> targetSegmentIds)
		{
			IEnumerable<AlignmentData> enumerable = alignments.Distinct();
			IEnumerable<AlignmentData> alignmentsToIgnore = FindAlignmentsToIgnore(enumerable, paragraphUnitId, sourceSegmentIds, targetSegmentIds);
			IList<AlignmentData> alignments2 = enumerable.Where((AlignmentData x) => !alignmentsToIgnore.Contains(x) && !IsAlignmentComplex(x, sourceSegmentIds, targetSegmentIds)).ToList();
			AlignmentCrossoverLocator alignmentCrossoverLocator = new AlignmentCrossoverLocator(alignments2, sourceSegmentIds, targetSegmentIds);
			IEnumerable<AlignmentData> crossovers = alignmentCrossoverLocator.FindAlignmentsInvolvedInCrossovers();
			return enumerable.Where((AlignmentData x) => !crossovers.Contains(x) && !alignmentsToIgnore.Contains(x));
		}

		internal static void AssignQuality(IEnumerable<AlignmentData> alignments)
		{
			AlignmentQualityCalculator alignmentQualityCalculator = new AlignmentQualityCalculator(alignments);
			foreach (AlignmentData alignment in alignments)
			{
				alignment.Quality = alignmentQualityCalculator.CalculateQuality(alignment);
			}
		}
	}
}
