namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IMarkupDataVisitor
	{
		void VisitTagPair(ITagPair tagPair);

		void VisitPlaceholderTag(IPlaceholderTag tag);

		void VisitText(IText text);

		void VisitSegment(ISegment segment);

		void VisitLocationMarker(ILocationMarker location);

		void VisitCommentMarker(ICommentMarker commentMarker);

		void VisitOtherMarker(IOtherMarker marker);

		void VisitLockedContent(ILockedContent lockedContent);

		void VisitRevisionMarker(IRevisionMarker revisionMarker);
	}
}
