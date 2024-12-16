using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.Segmentation
{
	internal class CheckContainsSegmentVisitor : IMarkupDataVisitor
	{
		public bool HasSegments
		{
			get;
			private set;
		}

		private void VisitChildren(IEnumerable<IAbstractMarkupData> container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitChildren(lockedContent.Content);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			VisitChildren(revisionMarker);
		}

		public void VisitSegment(ISegment segment)
		{
			HasSegments = true;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			VisitChildren(tagPair);
		}

		public void VisitText(IText text)
		{
		}
	}
}
