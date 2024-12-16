using Sdl.Core.Bcm.BcmConverters.ToBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi.MarkupDataConverters
{
	internal class FeedbackConverter : MarkupDataConverter
	{
		private readonly IPropertiesFactory _propertiesFactory;

		private readonly FileSkeleton _fileSkeleton;

		public FeedbackConverter(IPropertiesFactory propertiesFactory, FileSkeleton fileSkeleton)
		{
			_propertiesFactory = propertiesFactory;
			_fileSkeleton = fileSkeleton;
		}

		internal override IAbstractMarkupData Convert(MarkupData source)
		{
			FeedbackContainer feedbackContainer = source as FeedbackContainer;
			if (feedbackContainer == null)
			{
				return null;
			}
			Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType type = ConvertFeedbackType(feedbackContainer.FeedbackType);
			FeedbackProperties feedbackProperties = _propertiesFactory.CreateFeedbackProperties(type) as FeedbackProperties;
			feedbackProperties.Author = feedbackContainer.Author;
			feedbackProperties.Date = feedbackContainer.Timestamp;
			feedbackProperties.DocumentCategory = feedbackContainer.DocumentCategory;
			feedbackProperties.FeedbackCategory = feedbackContainer.Category;
			feedbackProperties.FeedbackSeverity = feedbackContainer.Severity;
			feedbackProperties.ReplacementId = feedbackContainer.ReplacementId;
			AddFeedbackComment(feedbackProperties, feedbackContainer);
			feedbackProperties.CopyMetadataFrom(feedbackContainer.Metadata);
			FeedbackMarker feedbackMarker = new FeedbackMarker
			{
				Properties = feedbackProperties
			};
			feedbackMarker.ConvertAndAddChildren(feedbackContainer.Children, base.ConverterFactory);
			return feedbackMarker;
		}

		private void AddFeedbackComment(FeedbackProperties resultProperties, FeedbackContainer feedback)
		{
			if (!string.IsNullOrEmpty(feedback.Comment))
			{
				IComment comment = _propertiesFactory.CreateComment(feedback.Comment, feedback.Author, Severity.Undefined);
				comment.Date = (feedback.Timestamp ?? DateTime.Now);
				resultProperties.FeedbackComment = comment;
			}
		}

		private static Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType ConvertFeedbackType(FeedbackType feedbackType)
		{
			switch (feedbackType)
			{
			case FeedbackType.Comment:
				return Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.FeedbackComment;
			case FeedbackType.Added:
				return Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.FeedbackAdded;
			case FeedbackType.Deleted:
				return Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.FeedbackDeleted;
			default:
				throw new ArgumentOutOfRangeException("feedbackType");
			}
		}
	}
}
