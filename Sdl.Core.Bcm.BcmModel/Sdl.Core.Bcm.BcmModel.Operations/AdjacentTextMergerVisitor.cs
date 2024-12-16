using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Core.Bcm.BcmModel.Operations
{
	internal sealed class AdjacentTextMergerVisitor : BcmVisitor
	{
		private readonly List<TextMarkup> _adjacentTextMarkups = new List<TextMarkup>();

		private void MergeTextMarkups()
		{
			if (_adjacentTextMarkups.Count < 2)
			{
				_adjacentTextMarkups.Clear();
				return;
			}
			foreach (TextMarkup item in _adjacentTextMarkups.Where((TextMarkup markup) => string.IsNullOrEmpty(markup.Id)))
			{
				item.Id = Guid.NewGuid().ToString();
			}
			TextMarkup textMarkup = _adjacentTextMarkups[0];
			StringBuilder stringBuilder = new StringBuilder(textMarkup.Text);
			for (int i = 1; i < _adjacentTextMarkups.Count; i++)
			{
				stringBuilder.Append(_adjacentTextMarkups[i].Text);
				_adjacentTextMarkups[i].RemoveFromParent();
			}
			_adjacentTextMarkups.Clear();
			textMarkup.Text = stringBuilder.ToString();
		}

		public void VisitChildren(MarkupDataContainer container)
		{
			MergeTextMarkups();
			for (int i = 0; i < container.Count; i++)
			{
				container[i].AcceptVisitor(this);
			}
			MergeTextMarkups();
		}

		public override void VisitSegment(Segment segment)
		{
			VisitChildren(segment);
			MergeTextMarkups();
		}

		public override void VisitStructure(StructureTag structureTag)
		{
			MergeTextMarkups();
		}

		public override void VisitTagPair(TagPair tagPair)
		{
			VisitChildren(tagPair);
		}

		public override void VisitPlaceholderTag(PlaceholderTag tag)
		{
			MergeTextMarkups();
		}

		public override void VisitText(TextMarkup text)
		{
			_adjacentTextMarkups.Add(text);
		}

		public override void VisitCommentContainer(CommentContainer commentContainer)
		{
			VisitChildren(commentContainer);
		}

		public override void VisitLockedContentContainer(LockedContentContainer lockedContentContainer)
		{
			MergeTextMarkups();
		}

		public override void VisitRevisionContainer(RevisionContainer revisionContainer)
		{
			VisitChildren(revisionContainer);
		}

		public override void VisitFeedbackContainer(FeedbackContainer feedbackContainer)
		{
			VisitChildren(feedbackContainer);
		}

		public override void VisitParagraph(Paragraph paragraph)
		{
			VisitChildren(paragraph);
		}

		public override void VisitTerminologyContainer(TerminologyAnnotationContainer terminologyAnnotation)
		{
			VisitChildren(terminologyAnnotation);
		}
	}
}
