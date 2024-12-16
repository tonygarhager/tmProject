using Sdl.Core.Bcm.BcmConverters.Common;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Extensions;
using Sdl.Core.Bcm.BcmModel;
using Sdl.Core.Bcm.BcmModel.Skeleton;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.FromBilingualApi.Helpers
{
	internal static class MarkupDataConverter
	{
		internal static Sdl.Core.Bcm.BcmModel.SegmentationHint Convert(Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint segmentationHint)
		{
			switch (segmentationHint)
			{
			case Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint.Exclude:
				return Sdl.Core.Bcm.BcmModel.SegmentationHint.Exclude;
			case Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint.Include:
				return Sdl.Core.Bcm.BcmModel.SegmentationHint.Include;
			case Sdl.FileTypeSupport.Framework.NativeApi.SegmentationHint.IncludeWithText:
				return Sdl.Core.Bcm.BcmModel.SegmentationHint.IncludeWithText;
			default:
				return Sdl.Core.Bcm.BcmModel.SegmentationHint.MayExclude;
			}
		}

		internal static Sdl.Core.Bcm.BcmModel.RevisionType ConvertToRevisionType(Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType revisionType)
		{
			switch (revisionType)
			{
			case Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.Insert:
				return Sdl.Core.Bcm.BcmModel.RevisionType.Inserted;
			case Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.Delete:
				return Sdl.Core.Bcm.BcmModel.RevisionType.Deleted;
			case Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.Unchanged:
				return Sdl.Core.Bcm.BcmModel.RevisionType.Unchanged;
			default:
				throw new ArgumentOutOfRangeException("revisionType");
			}
		}

		internal static FeedbackType ConvertToFeedbackType(Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType revisionType)
		{
			switch (revisionType)
			{
			case Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.FeedbackComment:
				return FeedbackType.Comment;
			case Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.FeedbackAdded:
				return FeedbackType.Added;
			case Sdl.FileTypeSupport.Framework.BilingualApi.RevisionType.FeedbackDeleted:
				return FeedbackType.Deleted;
			default:
				throw new ArgumentOutOfRangeException("revisionType");
			}
		}

		internal static SubContentCollection Convert(IEnumerable<ISubSegmentReference> subSegments)
		{
			SubContentCollection subContentCollection = new SubContentCollection();
			foreach (ISubSegmentReference subSegment in subSegments)
			{
				subContentCollection.Add(new LocalizableSubContent
				{
					Length = subSegment.Properties.Length,
					ParagraphUnitId = subSegment.ParagraphUnitId.Id,
					SourceTagContentOffset = subSegment.Properties.StartOffset
				});
			}
			return subContentCollection;
		}

		internal static Sdl.Core.Bcm.BcmModel.ConfirmationLevel Convert(Sdl.Core.Globalization.ConfirmationLevel confirmationLevel)
		{
			return (Sdl.Core.Bcm.BcmModel.ConfirmationLevel)confirmationLevel;
		}

		internal static TranslationOrigin Convert(ITranslationOrigin to)
		{
			if (to == null)
			{
				return null;
			}
			TranslationOrigin translationOrigin = new TranslationOrigin(to.OriginType, to.OriginSystem, to.MatchPercent, to.IsSIDContextMatch, to.IsStructureContextMatch, (Sdl.Core.Bcm.BcmModel.TextContextMatchLevel)to.TextContextMatchLevel, to.OriginalTranslationHash, Convert(to.OriginBeforeAdaptation));
			translationOrigin.CopyMetadataFrom(to);
			return translationOrigin;
		}

		internal static CommentDefinition Convert(IComment comment)
		{
			CommentDefinition commentDefinition = new CommentDefinition
			{
				Author = comment.Author,
				CommentSeverity = (CommentSeverity)comment.Severity,
				Date = comment.Date,
				Text = comment.Text
			};
			commentDefinition.CopyMetadataFrom(comment);
			return commentDefinition;
		}

		internal static Sdl.Core.Bcm.BcmModel.DependencyFileUsage Convert(Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage dependencyFileUsage)
		{
			switch (dependencyFileUsage)
			{
			case Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.None:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.None;
			case Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Extraction:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Extraction;
			case Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Generation:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Generation;
			case Sdl.FileTypeSupport.Framework.NativeApi.DependencyFileUsage.Final:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.Final;
			default:
				return Sdl.Core.Bcm.BcmModel.DependencyFileUsage.None;
			}
		}

		internal static Sdl.Core.Bcm.BcmConverters.Common.SniffInfo Convert(Sdl.FileTypeSupport.Framework.NativeApi.SniffInfo sniffInfo)
		{
			if (sniffInfo == null)
			{
				return null;
			}
			Sdl.Core.Bcm.BcmConverters.Common.SniffInfo sniffInfo2 = new Sdl.Core.Bcm.BcmConverters.Common.SniffInfo
			{
				IsSupported = sniffInfo.IsSupported,
				SuggestedTargetEncoding = sniffInfo.SuggestedTargetEncoding
			};
			Pair<Codepage, DetectionLevel> detectedEncoding = sniffInfo.DetectedEncoding;
			if (detectedEncoding.First.Encoding != null && detectedEncoding.Second != 0)
			{
				sniffInfo2.DetectedEncoding = new Tuple<string, DetectionLevel>(detectedEncoding.First.Encoding.BodyName, detectedEncoding.Second);
			}
			Pair<Language, DetectionLevel> detectedSourceLanguage = sniffInfo.DetectedSourceLanguage;
			if (detectedSourceLanguage.First.CultureInfo != null && detectedSourceLanguage.Second != 0)
			{
				sniffInfo2.DetectedSourceLanguage = new Tuple<string, DetectionLevel>(detectedSourceLanguage.First.CultureInfo.Name, detectedSourceLanguage.Second);
			}
			Pair<Language, DetectionLevel> detectedTargetLanguage = sniffInfo.DetectedTargetLanguage;
			if (detectedTargetLanguage.First.CultureInfo != null && detectedTargetLanguage.Second != 0)
			{
				sniffInfo2.DetectedTargetLanguage = new Tuple<string, DetectionLevel>(detectedTargetLanguage.First.CultureInfo.Name, detectedTargetLanguage.Second);
			}
			sniffInfo2.CopyMetadataFrom(sniffInfo);
			return sniffInfo2;
		}
	}
}
