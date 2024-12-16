using Sdl.Core.Bcm.BcmModel.Annotations;

namespace Sdl.Core.Bcm.BcmModel.Serialization
{
	public static class MarkupDataMapping
	{
		public static MarkupData GetType(string type)
		{
			switch (type)
			{
			case "paragraph":
				return new Paragraph();
			case "text":
				return new TextMarkup();
			case "segment":
				return new Segment();
			case "tagPair":
				return new TagPair();
			case "placeholderTag":
				return new PlaceholderTag();
			case "structureTag":
				return new StructureTag();
			case "locked":
				return new LockedContentContainer();
			case "comment":
				return new CommentContainer();
			case "revision":
				return new RevisionContainer();
			case "terminology":
				return new TerminologyAnnotationContainer();
			case "feedback":
				return new FeedbackContainer();
			default:
				return null;
			}
		}
	}
}
