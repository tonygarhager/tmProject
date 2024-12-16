using Sdl.Core.Processing.Alignment.Core;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment
{
	internal class AlignmentCrossoverLocator
	{
		private readonly IList<AlignmentData> _alignments;

		private readonly IList<SegmentId> _sourceSegmentIds;

		private readonly IList<SegmentId> _targetSegmentIds;

		private Dictionary<AlignmentData, AlignmentIndexInfo> _alignmentIndexes;

		public AlignmentCrossoverLocator(IList<AlignmentData> alignments, IList<SegmentId> sourceSegmentIds, IList<SegmentId> targetSegmentIds)
		{
			_alignments = alignments;
			_sourceSegmentIds = sourceSegmentIds;
			_targetSegmentIds = targetSegmentIds;
			ProcessAlignmentIndexes();
		}

		private void ProcessAlignmentIndexes()
		{
			_alignmentIndexes = new Dictionary<AlignmentData, AlignmentIndexInfo>();
			foreach (AlignmentData alignment in _alignments)
			{
				_alignmentIndexes[alignment] = new AlignmentIndexInfo
				{
					FirstSourceIndex = alignment.LeftIds.Min((DocumentSegmentId x) => _sourceSegmentIds.IndexOf(x.SegmentId)),
					FirstTargetIndex = alignment.RightIds.Min((DocumentSegmentId x) => _targetSegmentIds.IndexOf(x.SegmentId)),
					LastSourceIndex = alignment.LeftIds.Max((DocumentSegmentId x) => _sourceSegmentIds.IndexOf(x.SegmentId)),
					LastTargetIndex = alignment.RightIds.Max((DocumentSegmentId x) => _targetSegmentIds.IndexOf(x.SegmentId))
				};
			}
		}

		public IEnumerable<AlignmentData> FindAlignmentsInvolvedInCrossovers()
		{
			HashSet<AlignmentData> hashSet = new HashSet<AlignmentData>();
			for (int i = 0; i < _alignments.Count; i++)
			{
				for (int j = i + 1; j < _alignments.Count; j++)
				{
					if (AreSimpleAlignmentsCrossing(_alignments[i], _alignments[j], _sourceSegmentIds, _targetSegmentIds))
					{
						hashSet.Add(_alignments[i]);
						hashSet.Add(_alignments[j]);
					}
				}
			}
			return hashSet;
		}

		public bool AreSimpleAlignmentsCrossing(AlignmentData alignment1, AlignmentData alignment2, IList<SegmentId> sourceSegmentIds, IList<SegmentId> targetSegmentIds)
		{
			AlignmentIndexInfo alignmentIndexInfo = _alignmentIndexes[alignment1];
			AlignmentIndexInfo alignmentIndexInfo2 = _alignmentIndexes[alignment2];
			if (alignmentIndexInfo.LastSourceIndex > alignmentIndexInfo2.FirstSourceIndex && alignmentIndexInfo.LastTargetIndex > alignmentIndexInfo2.FirstTargetIndex)
			{
				return false;
			}
			if (alignmentIndexInfo.FirstSourceIndex < alignmentIndexInfo2.LastSourceIndex && alignmentIndexInfo.FirstTargetIndex < alignmentIndexInfo2.LastTargetIndex)
			{
				return false;
			}
			return true;
		}
	}
}
