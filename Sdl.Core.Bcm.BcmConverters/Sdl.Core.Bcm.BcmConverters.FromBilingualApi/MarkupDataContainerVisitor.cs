using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Annotations;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi
{
	internal class MarkupDataContainerVisitor : IMarkupDataVisitor
	{
		private readonly MarkupDataContainerVisitorData _data;

		private readonly FileSkeleton _fileSkeleton;

		private readonly List<MarkupData> _children;

		public MarkupDataContainerVisitor(MarkupDataContainerVisitorData data)
		{
			_data = data;
			_fileSkeleton = data.File.Skeleton;
			_children = new List<MarkupData>();
		}

		public List<MarkupData> Visit(IEnumerable<IAbstractMarkupData> container)
		{
			VisitChildren(container);
			return _children;
		}

		private void VisitChildren(IEnumerable<IAbstractMarkupData> container)
		{
			foreach (IAbstractMarkupData item in container)
			{
				IStructureTag structureTag = item as IStructureTag;
				if (structureTag != null)
				{
					VisitStructureTag(structureTag);
				}
				else
				{
					item.AcceptVisitor(this);
				}
			}
		}

		public void VisitSegment(ISegment segment)
		{
			_data.IsInSegment = true;
			Sdl.Core.Bcm.BcmModel.Segment segment2 = new Sdl.Core.Bcm.BcmModel.Segment
			{
				Id = Guid.NewGuid().ToString(),
				SegmentNumber = segment.Properties.Id.Id,
				IsLocked = segment.Properties.IsLocked
			};
			if (segment.ParentParagraph.IsTarget)
			{
				segment2.ConfirmationLevel = MarkupDataConverter.Convert(segment.Properties.ConfirmationLevel);
				if (segment.Properties.TranslationOrigin != null)
				{
					segment2.TranslationOrigin = (segment2.TranslationOrigin ?? MarkupDataConverter.Convert(segment.Properties.TranslationOrigin));
				}
			}
			CopyMetaData(segment.Properties, segment2);
			Sdl.Core.Bcm.BcmModel.MarkupDataContainer markupDataContainer = segment2;
			while (_data.BufferedComments.Count > 0)
			{
				CommentContainer commentContainer = _data.BufferedComments.Dequeue();
				markupDataContainer.Add(commentContainer);
				markupDataContainer = commentContainer;
			}
			markupDataContainer.AddRange(GenerateMarkupDataContainer(segment));
			_children.Add(segment2);
			_data.IsInSegment = false;
		}

		private void CopyMetaData(ISegmentPairProperties segmentProperties, Sdl.Core.Bcm.BcmModel.Segment segment)
		{
			if (segmentProperties.TranslationOrigin != null && segmentProperties.TranslationOrigin.HasMetaData)
			{
				foreach (KeyValuePair<string, string> metaDatum in segmentProperties.TranslationOrigin.MetaData)
				{
					segment.SetMetadata(metaDatum.Key, metaDatum.Value);
				}
			}
		}

		private void VisitStructureTag(IStructureTag structureTag)
		{
			AddSubContentPUs(structureTag.SubSegments);
			StructureTagDefinition elem = TagDefinitionBuilder.BuildStructure(0, structureTag);
			StructureTagDefinition orAdd = _fileSkeleton.StructureTagDefinitions.GetOrAdd(elem);
			Sdl.Core.Bcm.BcmModel.StructureTag structureTag2 = new Sdl.Core.Bcm.BcmModel.StructureTag
			{
				Id = Guid.NewGuid().ToString(),
				StructureTagDefinitionId = orAdd.Id
			};
			structureTag2.CopyMetadataFrom(structureTag.TagProperties);
			SetFrameworkOriginalIdMetadata(structureTag2, structureTag.TagProperties.TagId.Id);
			_children.Add(structureTag2);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			AddSubContentPUs(tagPair.SubSegments);
			if (!QuickInsertHelper.TryCreateSkeletonTagPairDefinition(tagPair.StartTagProperties.TagId.Id, _fileSkeleton, out TagPairDefinition tagPairDefinition))
			{
				tagPairDefinition = TagDefinitionBuilder.BuildTagPair(_fileSkeleton.TagPairDefinitions.Count + 1, tagPair);
				if (tagPair.StartTagProperties.Formatting != null && tagPair.StartTagProperties.Formatting.Any())
				{
					tagPairDefinition.FormattingGroupId = FormattingGroupHelper.AddFormatting(_fileSkeleton, tagPair.StartTagProperties.Formatting);
				}
				tagPairDefinition = _fileSkeleton.TagPairDefinitions.GetOrAdd(tagPairDefinition);
			}
			Sdl.Core.Bcm.BcmModel.TagPair tagPair2 = new Sdl.Core.Bcm.BcmModel.TagPair
			{
				Id = Guid.NewGuid().ToString(),
				TagPairDefinitionId = tagPairDefinition.Id
			};
			tagPair2.AddRange(GenerateMarkupDataContainer(tagPair));
			tagPair2.CopyMetadataFrom(tagPair.StartTagProperties);
			SetFrameworkOriginalIdMetadata(tagPair2, tagPair.StartTagProperties.TagId.Id);
			SetOriginalSegmentationHint(tagPair2, tagPair.StartTagProperties.SegmentationHint);
			foreach (KeyValuePair<string, string> metaDatum in tagPair.EndTagProperties.MetaData)
			{
				tagPair2.SetMetadata("__end_" + metaDatum.Key, metaDatum.Value);
			}
			_children.Add(tagPair2);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			AddSubContentPUs(tag.SubSegments);
			if (!QuickInsertHelper.TryCreateSkeletonPlaceholderTagDefinition(tag.TagProperties.TagId.Id, _fileSkeleton, out PlaceholderTagDefinition placeholderTagDefinition))
			{
				placeholderTagDefinition = TagDefinitionBuilder.BuildPlaceholder(_fileSkeleton.PlaceholderTagDefinitions.Count + 1, tag);
				placeholderTagDefinition = _fileSkeleton.PlaceholderTagDefinitions.GetOrAdd(placeholderTagDefinition);
			}
			Sdl.Core.Bcm.BcmModel.PlaceholderTag placeholderTag = new Sdl.Core.Bcm.BcmModel.PlaceholderTag
			{
				Id = Guid.NewGuid().ToString(),
				PlaceholderTagDefinitionId = placeholderTagDefinition.Id
			};
			placeholderTag.CopyMetadataFrom(tag.Properties);
			SetFrameworkOriginalIdMetadata(placeholderTag, tag.TagProperties.TagId.Id);
			SetOriginalSegmentationHint(placeholderTag, tag.Properties.SegmentationHint);
			_children.Add(placeholderTag);
		}

		public void VisitText(IText text)
		{
			TextMarkup item = new TextMarkup
			{
				Id = Guid.NewGuid().ToString(),
				Text = text.Properties.Text
			};
			_children.Add(item);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			if (_data.BcmExtractionSettings.ProcessComments)
			{
				CommentMarkerConverter commentMarkerConverter = new CommentMarkerConverter(_data, _children);
				commentMarkerConverter.Convert(commentMarker);
			}
			else
			{
				VisitChildren(commentMarker);
			}
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			LockedContentContainer lockedContentContainer = new LockedContentContainer
			{
				Id = Guid.NewGuid().ToString()
			};
			_children.Add(lockedContentContainer);
			lockedContentContainer.AddRange(GenerateMarkupDataContainer(lockedContent.Content));
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			FeedbackMarker feedbackMarker = revisionMarker as FeedbackMarker;
			if (feedbackMarker != null)
			{
				VisitFeedback(feedbackMarker);
			}
			else
			{
				VisitRevision(revisionMarker as RevisionMarker);
			}
		}

		private List<MarkupData> GenerateMarkupDataContainer(IEnumerable<IAbstractMarkupData> container)
		{
			MarkupDataContainerVisitor markupDataContainerVisitor = new MarkupDataContainerVisitor(_data);
			return markupDataContainerVisitor.Visit(container);
		}

		private void AddSubContentPUs(IEnumerable<ISubSegmentReference> subSegmentReferences)
		{
			foreach (ISubSegmentReference subSegmentReference in subSegmentReferences)
			{
				_fileSkeleton.SubContentPUs.Add(subSegmentReference.ParagraphUnitId.Id);
			}
		}

		private void VisitRevision(RevisionMarker revisionMarker)
		{
			RevisionProperties revisionProperties = revisionMarker.Properties as RevisionProperties;
			RevisionContainer revisionContainer = new RevisionContainer
			{
				Id = Guid.NewGuid().ToString(),
				RevisionType = MarkupDataConverter.ConvertToRevisionType(revisionProperties.RevisionType),
				Author = revisionProperties.Author,
				Timestamp = revisionProperties.Date
			};
			revisionContainer.AddRange(GenerateMarkupDataContainer(revisionMarker));
			revisionContainer.CopyMetadataFrom(revisionMarker.Properties);
			_children.Add(revisionContainer);
		}

		private void VisitFeedback(FeedbackMarker feedbackMarker)
		{
			FeedbackProperties feedbackProperties = feedbackMarker.Properties as FeedbackProperties;
			FeedbackContainer feedbackContainer = new FeedbackContainer
			{
				Id = Guid.NewGuid().ToString(),
				Author = feedbackProperties.Author,
				Timestamp = feedbackProperties.Date,
				Category = feedbackProperties.FeedbackCategory,
				DocumentCategory = feedbackProperties.DocumentCategory,
				Severity = feedbackProperties.FeedbackSeverity,
				FeedbackType = MarkupDataConverter.ConvertToFeedbackType(feedbackProperties.RevisionType),
				ReplacementId = feedbackProperties.ReplacementId
			};
			feedbackContainer.AddRange(GenerateMarkupDataContainer(feedbackMarker));
			if (feedbackProperties.FeedbackComment != null)
			{
				feedbackContainer.Comment = feedbackProperties.FeedbackComment.Text;
			}
			feedbackContainer.CopyMetadataFrom(feedbackProperties);
			_children.Add(feedbackContainer);
		}

		private static void SetFrameworkOriginalIdMetadata(MetadataContainer tag, string originalTagId)
		{
			tag.SetMetadata("frameworkOriginalTagId", originalTagId);
		}

		private static void SetOriginalSegmentationHint(MetadataContainer tag, Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint segmentationHint)
		{
			tag.SetMetadata("_originalSegmentationHint", segmentationHint.ToString());
		}
	}
}
