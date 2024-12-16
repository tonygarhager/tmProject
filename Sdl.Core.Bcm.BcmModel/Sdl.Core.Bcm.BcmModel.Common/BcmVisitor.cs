using Sdl.Core.Bcm.BcmModel.Annotations;

namespace Sdl.Core.Bcm.BcmModel.Common
{
	public abstract class BcmVisitor
	{
		public abstract void VisitStructure(StructureTag structureTag);

		public abstract void VisitTagPair(TagPair tagPair);

		public abstract void VisitPlaceholderTag(PlaceholderTag tag);

		public abstract void VisitText(TextMarkup text);

		public abstract void VisitSegment(Segment segment);

		public abstract void VisitCommentContainer(CommentContainer commentContainer);

		public abstract void VisitLockedContentContainer(LockedContentContainer lockedContentContainer);

		public abstract void VisitRevisionContainer(RevisionContainer revisionContainer);

		public abstract void VisitFeedbackContainer(FeedbackContainer feedbackContainer);

		public abstract void VisitParagraph(Paragraph paragraph);

		public abstract void VisitTerminologyContainer(TerminologyAnnotationContainer terminologyAnnotation);
	}
}
