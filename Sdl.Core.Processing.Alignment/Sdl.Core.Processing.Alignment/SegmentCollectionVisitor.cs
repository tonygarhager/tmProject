using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment
{
	internal class SegmentCollectionVisitor : IMarkupDataVisitor
	{
		private readonly IList<ISegment> _segments = new List<ISegment>();

		public IList<ISegment> Segments => _segments;

		public void VisitTagPair(ITagPair tagPair)
		{
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
		}

		public void VisitText(IText text)
		{
		}

		public void VisitSegment(ISegment segment)
		{
			_segments.Add(segment);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitChildren(lockedContent.Content);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			VisitChildren(revisionMarker);
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
