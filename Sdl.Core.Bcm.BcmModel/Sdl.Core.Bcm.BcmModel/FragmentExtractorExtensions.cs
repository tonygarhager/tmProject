using Sdl.Core.Bcm.BcmModel.Skeleton;
using System;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel
{
	public static class FragmentExtractorExtensions
	{
		public static Fragment ExtractSourceFragment(this Document document, string fileId, string paragraphUnitId, string segmentNumber, bool extractContext = true)
		{
			return document.ExtractFragmentDetails(fileId, paragraphUnitId, segmentNumber, includeSource: true, includeTarget: false, extractContext);
		}

		public static Fragment ExtractTargetFragment(this Document document, string fileId, string paragraphUnitId, string segmentNumber, bool extractContext = true)
		{
			return document.ExtractFragmentDetails(fileId, paragraphUnitId, segmentNumber, includeSource: false, includeTarget: true, extractContext);
		}

		public static Fragment ExtractFragment(this Document document, string fileId, string paragraphUnitId, string segmentNumber, bool includeTarget = true, bool extractContext = true)
		{
			return document.ExtractFragmentDetails(fileId, paragraphUnitId, segmentNumber, includeSource: true, includeTarget, extractContext);
		}

		private static Fragment ExtractFragmentDetails(this Document document, string fileId, string paragraphUnitId, string segmentNumber, bool includeSource = true, bool includeTarget = true, bool extractContext = true)
		{
			File file = GetFile(document, fileId);
			Fragment fragment = CreateNewFragment(document, file);
			SegmentPair segmentPair = GetSegmentPair(file, paragraphUnitId, segmentNumber);
			if (includeSource)
			{
				fragment.SourceContent = ExtractSourceSegment(file.Skeleton, segmentPair, fragment.Skeleton);
			}
			if (includeTarget)
			{
				fragment.TargetContent = ExtractTargetSegment(file.Skeleton, segmentPair, fragment.Skeleton);
			}
			return ExtractFragmentProperties(document, fileId, paragraphUnitId, extractContext, file, fragment);
		}

		public static Fragment ExtractSourceFragmentParagraphLine(this Document document, string fileId, string paragraphUnitId, string segmentNumber, bool extractContext = true)
		{
			return document.ExtractFragmentParagraphLineDetails(fileId, paragraphUnitId, segmentNumber, includeSource: true, includeTarget: false, extractContext);
		}

		public static Fragment ExtractTargetFragmentParagraphLine(this Document document, string fileId, string paragraphUnitId, string segmentNumber, bool extractContext = true)
		{
			return document.ExtractFragmentParagraphLineDetails(fileId, paragraphUnitId, segmentNumber, includeSource: false, includeTarget: true, extractContext);
		}

		public static Fragment ExtractFragmentParagraphLine(this Document document, string fileId, string paragraphUnitId, string segmentNumber, bool includeTarget = true, bool extractContext = true)
		{
			return document.ExtractFragmentParagraphLineDetails(fileId, paragraphUnitId, segmentNumber, includeSource: true, includeTarget, extractContext);
		}

		private static Fragment ExtractFragmentParagraphLineDetails(this Document document, string fileId, string paragraphUnitId, string segmentNumber, bool includeSource = true, bool includeTarget = true, bool extractContext = true)
		{
			File file = GetFile(document, fileId);
			Fragment fragment = CreateNewFragment(document, file);
			SegmentPair segmentPair = GetSegmentPair(file, paragraphUnitId, segmentNumber);
			if (includeSource)
			{
				fragment.SourceContent = ExtractSourceSegmentParentLine(file.Skeleton, segmentPair, fragment.Skeleton);
			}
			if (includeTarget)
			{
				fragment.TargetContent = ExtractTargetSegmentParentLine(file.Skeleton, segmentPair, fragment.Skeleton);
			}
			return ExtractFragmentProperties(document, fileId, paragraphUnitId, extractContext, file, fragment);
		}

		private static Fragment ExtractFragmentProperties(Document document, string fileId, string paragraphUnitId, bool extractContext, File file, Fragment fragment)
		{
			if (extractContext)
			{
				ExtractFragmentContext(document, fileId, paragraphUnitId, fragment);
			}
			fragment.Skeleton.QuickInsertIds = file.Skeleton?.QuickInsertIds;
			ExtractFragmentCommentDefinitions(file, paragraphUnitId, fragment);
			ExtractFragmentMetadata(file, paragraphUnitId, fragment);
			return fragment;
		}

		private static Segment ExtractSourceSegment(FileSkeleton fileSkeleton, SegmentPair sp, FileSkeleton fragmentSkeleton)
		{
			Segment segment = sp.Source.Clone();
			segment.ExtractSegmentSkeleton(fileSkeleton, fragmentSkeleton);
			return segment;
		}

		private static Segment ExtractTargetSegment(FileSkeleton fileSkeleton, SegmentPair sp, FileSkeleton fragmentSkeleton)
		{
			Segment segment = sp.Target.Clone();
			segment.ExtractSegmentSkeleton(fileSkeleton, fragmentSkeleton);
			return segment;
		}

		private static MarkupData ExtractSourceSegmentParentLine(FileSkeleton fileSkeleton, SegmentPair sp, FileSkeleton fragmentSkeleton)
		{
			Segment newSegment = sp.Source.Clone();
			MarkupDataContainer markupDataContainer = ExtractSegmentParentLine(sp.Source, newSegment);
			markupDataContainer.ExtractMarkupDataContainerSkeleton(fileSkeleton, fragmentSkeleton);
			return markupDataContainer;
		}

		private static MarkupData ExtractTargetSegmentParentLine(FileSkeleton fileSkeleton, SegmentPair sp, FileSkeleton fragmentSkeleton)
		{
			Segment newSegment = sp.Target.Clone();
			MarkupDataContainer markupDataContainer = ExtractSegmentParentLine(sp.Source, newSegment);
			markupDataContainer.ExtractMarkupDataContainerSkeleton(fileSkeleton, fragmentSkeleton);
			return markupDataContainer;
		}

		private static MarkupDataContainer ExtractSegmentParentLine(Segment oldSegment, Segment newSegment)
		{
			MarkupDataContainer parent = oldSegment.Parent;
			MarkupDataContainer markupDataContainer = newSegment;
			while (parent != null)
			{
				MarkupDataContainer markupDataContainer2 = parent.CloneWithoutChildren();
				markupDataContainer2?.Add(markupDataContainer);
				parent = parent.Parent;
				markupDataContainer = markupDataContainer2;
			}
			return markupDataContainer;
		}

		private static Fragment CreateNewFragment(Document document, File file)
		{
			Fragment fragment = new Fragment
			{
				DocumentId = document.Id,
				SourceLanguageCode = document.SourceLanguageCode,
				TargetLanguageCode = document.TargetLanguageCode,
				Skeleton = new File
				{
					Id = file.Id
				}.Skeleton
			};
			fragment.Skeleton.FileId = file.Id;
			return fragment;
		}

		private static File GetFile(Document document, string fileId)
		{
			File file = document.Files.SingleOrDefault((File f) => f.Id == fileId);
			if (file == null)
			{
				throw new ArgumentException("File not found", "fileId");
			}
			return file;
		}

		private static SegmentPair GetSegmentPair(File file, string puId, string segmentNumber)
		{
			ParagraphUnit paragraphUnit = file.ParagraphUnits.SingleOrDefault((ParagraphUnit item) => item.Id == puId);
			if (paragraphUnit == null)
			{
				throw new ArgumentException("Paragraph Unit not found", "puId");
			}
			SegmentPair segmentPair = paragraphUnit.SegmentPairs.SingleOrDefault((SegmentPair sp) => sp.Source.SegmentNumber == segmentNumber);
			if (segmentPair == null)
			{
				throw new ArgumentException("Segment Pair not found", "segmentNumber");
			}
			return segmentPair;
		}

		private static void ExtractFragmentContext(Document document, string fileId, string paragraphUnitId, Fragment fragment)
		{
			File file = GetFile(document, fileId);
			ParagraphUnit paragraphUnit = file.ParagraphUnits.SingleOrDefault((ParagraphUnit item) => item.Id == paragraphUnitId);
			if (paragraphUnit != null)
			{
				fragment.StructureContextId = paragraphUnit.StructureContextId;
				ContextExtractorExtensions.ExtractContext(file, paragraphUnit, fragment.Skeleton);
				if (paragraphUnit.ContextList != null && paragraphUnit.ContextList.Any())
				{
					fragment.ContextList = paragraphUnit.ContextList.ToList();
				}
			}
		}

		private static void ExtractFragmentCommentDefinitions(File oldFile, string paragraphUnitId, Fragment fragment)
		{
			ParagraphUnit paragraphUnit = oldFile.ParagraphUnits.SingleOrDefault((ParagraphUnit item) => item.Id == paragraphUnitId);
			if (paragraphUnit?.CommentDefinitionIds != null && paragraphUnit.CommentDefinitionIds.Any() && oldFile.Skeleton?.CommentDefinitions != null)
			{
				foreach (int commentDefinitionId in paragraphUnit.CommentDefinitionIds)
				{
					CommentDefinition commentDefinition = oldFile.Skeleton.CommentDefinitions.SingleOrDefault((CommentDefinition cd) => cd.Id == commentDefinitionId);
					if (commentDefinition != null)
					{
						CommentDefinition elem = commentDefinition.Clone();
						fragment.Skeleton.CommentDefinitions.GetOrAddWithExistingId(elem);
					}
				}
				fragment.CommentDefinitionIds = paragraphUnit.CommentDefinitionIds.ToList();
			}
		}

		private static void ExtractFragmentMetadata(File oldFile, string paragraphUnitId, Fragment fragment)
		{
			ParagraphUnit paragraphUnit = oldFile.ParagraphUnits.SingleOrDefault((ParagraphUnit item) => item.Id == paragraphUnitId);
			if (paragraphUnit?.Metadata != null && paragraphUnit.Metadata.Any())
			{
				fragment.AddMetadataFrom(paragraphUnit.Metadata);
			}
		}
	}
}
