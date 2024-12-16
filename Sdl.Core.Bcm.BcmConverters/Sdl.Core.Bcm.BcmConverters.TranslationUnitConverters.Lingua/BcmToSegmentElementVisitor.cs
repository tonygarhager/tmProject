using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Common;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.TranslationUnitConverters.Lingua
{
	internal class BcmToSegmentElementVisitor : BcmVisitor
	{
		private readonly List<SegmentElement> _container;

		private readonly FileSkeleton _fileSkeleton;

		private readonly Stack<int> _anchors = new Stack<int>();

		private int _currentAnchor;

		public bool CreateLinguaBcmMappings
		{
			get;
			set;
		}

		public LinguaToBcmMap LinguaToBcmMap
		{
			get;
		}

		public ConfirmationLevel ConfirmationLevel
		{
			get;
			set;
		}

		public BcmToSegmentElementVisitor(List<SegmentElement> container, FileSkeleton fileSkeleton)
		{
			_container = container;
			_fileSkeleton = fileSkeleton;
			LinguaToBcmMap = new LinguaToBcmMap();
		}

		public override void VisitStructure(StructureTag structureTag)
		{
		}

		public override void VisitTagPair(TagPair tagPair)
		{
			TagPairDefinition tagPairDefinition = _fileSkeleton.TagPairDefinitions[SkeletonCollectionKey.From(tagPair.TagPairDefinitionId)];
			string tagID = string.IsNullOrEmpty(tagPairDefinition.QuickInsertId) ? tagPair.FrameworkId : tagPairDefinition.QuickInsertId;
			Tag tag = new Tag
			{
				Type = TagType.Start,
				TagID = tagID,
				Anchor = ++_currentAnchor,
				TextEquivalent = tagPairDefinition.StartTagContent
			};
			_anchors.Push(_currentAnchor);
			RecordTag(tag, tagPair);
			VisitChildren(tagPair);
			Tag tag2 = new Tag
			{
				Type = TagType.End,
				TagID = tagID,
				Anchor = _anchors.Pop(),
				TextEquivalent = tagPairDefinition.EndTagContent
			};
			RecordTag(tag2, tagPair);
		}

		public override void VisitPlaceholderTag(PlaceholderTag tag)
		{
			PlaceholderTagDefinition placeholderTagDefinition = _fileSkeleton.PlaceholderTagDefinitions[SkeletonCollectionKey.From(tag.PlaceholderTagDefinitionId)];
			string tagID = string.IsNullOrEmpty(placeholderTagDefinition.QuickInsertId) ? tag.FrameworkId : placeholderTagDefinition.QuickInsertId;
			Tag tag2 = new Tag
			{
				Type = (string.IsNullOrEmpty(placeholderTagDefinition.TextEquivalent) ? TagType.Standalone : TagType.TextPlaceholder),
				TagID = tagID,
				Anchor = ++_currentAnchor,
				TextEquivalent = placeholderTagDefinition.TextEquivalent
			};
			RecordTag(tag2, tag);
		}

		public override void VisitText(TextMarkup text)
		{
			Text text2 = new Text
			{
				Value = text.Text
			};
			Text text3 = _container.LastOrDefault() as Text;
			if (text3 != null)
			{
				text3.Value += text.Text;
				return;
			}
			if (CreateLinguaBcmMappings)
			{
				LinguaToBcmMap.TextAssociations.Add(new KeyValuePair<Text, string>(text2, text.Id));
			}
			_container.Add(text2);
		}

		public override void VisitSegment(Sdl.Core.Bcm.BcmModel.Segment segment)
		{
			ConfirmationLevel = segment.ConfirmationLevel;
			VisitChildren(segment);
		}

		public override void VisitCommentContainer(CommentContainer commentContainer)
		{
			VisitChildren(commentContainer);
		}

		public override void VisitLockedContentContainer(LockedContentContainer lockedContentContainer)
		{
			Tag tag = new Tag
			{
				Type = TagType.LockedContent,
				TagID = lockedContentContainer.FrameworkId,
				Anchor = ++_currentAnchor,
				TextEquivalent = BcmTextCollector.CollectText(lockedContentContainer)
			};
			RecordTag(tag, lockedContentContainer);
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
			VisitChildren(paragraph);
		}

		public override void VisitTerminologyContainer(TerminologyAnnotationContainer terminologyAnnotation)
		{
			VisitChildren(terminologyAnnotation);
		}

		private void VisitChildren(MarkupDataContainer container)
		{
			container.ForEach(delegate(MarkupData markup)
			{
				markup.AcceptVisitor(this);
			});
		}

		private void RecordTag(Tag tag, MarkupData bcmTag)
		{
			_container.Add(tag);
			if (CreateLinguaBcmMappings)
			{
				LinguaToBcmMap.TagAssociations.Add(new KeyValuePair<Tag, string>(tag, bcmTag.Id));
			}
		}
	}
}
