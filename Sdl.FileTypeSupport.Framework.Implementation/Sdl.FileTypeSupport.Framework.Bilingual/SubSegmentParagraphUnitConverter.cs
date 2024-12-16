using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	public class SubSegmentParagraphUnitConverter : BilingualToNativeConverter
	{
		public override void VisitTagPair(ITagPair tag)
		{
			VisitContainerItems(tag);
		}

		public override void VisitPlaceholderTag(IPlaceholderTag tag)
		{
		}

		public override void VisitText(IText text)
		{
			base.InternalOutput.Text(text.Properties);
		}

		public override void VisitSegment(ISegment segment)
		{
			VisitContainerItems(GetSegmentContentToProcess(segment));
		}

		public override void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitContainerItems(commentMarker);
		}

		public override void VisitOtherMarker(IOtherMarker marker)
		{
			VisitContainerItems(marker);
		}

		public override void VisitLocationMarker(ILocationMarker location)
		{
		}

		public override void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitContainerItems(lockedContent.Content);
		}

		public override void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			switch (revisionMarker.Properties.RevisionType)
			{
			case RevisionType.Delete:
				break;
			case RevisionType.Insert:
				VisitContainerItems(revisionMarker);
				break;
			case RevisionType.Unchanged:
				VisitContainerItems(revisionMarker);
				break;
			default:
				VisitContainerItems(revisionMarker);
				break;
			}
		}
	}
}
