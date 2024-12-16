using Sdl.Core.Processing.Alignment.Common;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core.Anchoring
{
	internal class ConfirmedAlignmentsAnchorStrategy : AbstractAnchoringStrategy
	{
		private List<AlignmentData> _confirmedAlignments;

		public IList<AlignmentData> ConfirmedAlignments
		{
			get
			{
				return _confirmedAlignments;
			}
			set
			{
				_confirmedAlignments.Clear();
				if (value != null)
				{
					_confirmedAlignments.AddRange(value);
				}
			}
		}

		public ConfirmedAlignmentsAnchorStrategy()
		{
			_confirmedAlignments = new List<AlignmentData>();
		}

		public void SetParagraphUnitId(ParagraphUnitId paragraphUnitId)
		{
			List<AlignmentData> list = new List<AlignmentData>();
			foreach (AlignmentData confirmedAlignment in _confirmedAlignments)
			{
				List<DocumentSegmentId> leftIds = confirmedAlignment.LeftIds.Select((DocumentSegmentId documentSegmentId) => new DocumentSegmentId(paragraphUnitId, documentSegmentId.SegmentId, documentSegmentId.Order)).ToList();
				List<DocumentSegmentId> rightIds = confirmedAlignment.RightIds.Select((DocumentSegmentId documentSegmentId) => new DocumentSegmentId(paragraphUnitId, documentSegmentId.SegmentId, documentSegmentId.Order)).ToList();
				list.Add(new AlignmentData(leftIds, rightIds, AlignmentCost.MinValue, confirmed: true)
				{
					Quality = AlignmentQuality.Good
				});
			}
			_confirmedAlignments = list;
		}

		public override IList<AlignmentData> GetAnchors(IList<AlignmentElement> leftElements, IList<AlignmentElement> rightElements)
		{
			return _confirmedAlignments;
		}
	}
}
