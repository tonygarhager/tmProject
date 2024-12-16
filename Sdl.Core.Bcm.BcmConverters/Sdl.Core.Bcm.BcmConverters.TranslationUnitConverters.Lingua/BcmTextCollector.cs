using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Text;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua
{
	internal class BcmTextCollector : BcmVisitor
	{
		private readonly StringBuilder _result = new StringBuilder();

		public string Result => _result.ToString();

		public static string CollectText(MarkupData markupData)
		{
			BcmTextCollector bcmTextCollector = new BcmTextCollector();
			markupData.AcceptVisitor(bcmTextCollector);
			return bcmTextCollector.Result;
		}

		public void VisitChildren(MarkupDataContainer container)
		{
			container.ForEach(delegate(MarkupData markupData)
			{
				markupData.AcceptVisitor(this);
			});
		}

		public override void VisitStructure(StructureTag structureTag)
		{
		}

		public override void VisitTagPair(TagPair tagPair)
		{
			VisitChildren(tagPair);
		}

		public override void VisitPlaceholderTag(PlaceholderTag tag)
		{
		}

		public override void VisitText(TextMarkup text)
		{
			_result.Append(text.Text);
		}

		public override void VisitSegment(Segment segment)
		{
			VisitChildren(segment);
		}

		public override void VisitCommentContainer(CommentContainer commentContainer)
		{
			VisitChildren(commentContainer);
		}

		public override void VisitLockedContentContainer(LockedContentContainer lockedContentContainer)
		{
			VisitChildren(lockedContentContainer);
		}

		public override void VisitRevisionContainer(RevisionContainer revisionContainer)
		{
			if (revisionContainer.RevisionType != RevisionType.Deleted)
			{
				VisitChildren(revisionContainer);
			}
		}

		public override void VisitFeedbackContainer(FeedbackContainer feedbackContainer)
		{
			if (feedbackContainer.FeedbackType != FeedbackType.Deleted)
			{
				VisitChildren(feedbackContainer);
			}
		}

		public override void VisitParagraph(Paragraph paragraph)
		{
			throw new NotImplementedException();
		}

		public override void VisitTerminologyContainer(TerminologyAnnotationContainer terminologyAnnotation)
		{
			VisitChildren(terminologyAnnotation);
		}
	}
}
