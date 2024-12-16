using Sdl.Core.Bcm.BcmModel.Skeleton;
using System;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel
{
	public static class DocumentExtractorExtensions
	{
		public static void ExtractSegmentPair(this Document document, string fileId, string paragraphUnitId, string segmentNumber, ref Document newDocument, ref File newFile, ref ParagraphUnit newPu, bool includeTarget = true)
		{
			File file = GetFile(document, fileId);
			CreateNewFile(document, file, ref newDocument, ref newFile);
			if (!newDocument.Files.Contains(newFile))
			{
				newDocument.Files.Add(newFile);
			}
			ExtractSourceSegment(file, paragraphUnitId, segmentNumber, out SegmentPair sp, ref newFile, ref newPu);
			ExtractTargetSegment(file, sp, ref newFile, ref newPu, includeTarget);
			ExtractDocumentContext(document, fileId, paragraphUnitId, newFile.Skeleton, newPu);
			if (!newFile.ParagraphUnits.Contains(newPu))
			{
				newFile.ParagraphUnits.Add(newPu);
			}
		}

		public static Document ExtractTwoSegmentPairs(this Document document, string fileId, string paragraphUnitId1, string segmentNumber1, string paragraphUnitId2, string segmentNumber2, out string newParagraphUnitId)
		{
			File newFile = null;
			ParagraphUnit newPu = null;
			Document newDocument = null;
			if (!string.IsNullOrEmpty(paragraphUnitId1))
			{
				document.ExtractSegmentPair(fileId, paragraphUnitId1, segmentNumber1, ref newDocument, ref newFile, ref newPu);
			}
			document.ExtractSegmentPair(fileId, paragraphUnitId2, segmentNumber2, ref newDocument, ref newFile, ref newPu);
			ExtractDocumentContext(document, fileId, paragraphUnitId2, newFile.Skeleton, newPu);
			newParagraphUnitId = newPu.Id;
			return newDocument;
		}

		public static Document ExtractThreeSegmentPairs(this Document document, string fileId, string paragraphUnitId1, string segmentNumber1, string paragraphUnitId2, string segmentNumber2, string paragraphUnitId3, string segmentNumber3, out string newParagraphUnitId)
		{
			File newFile = null;
			ParagraphUnit newPu = null;
			Document newDocument = null;
			if (!string.IsNullOrEmpty(paragraphUnitId1))
			{
				document.ExtractSegmentPair(fileId, paragraphUnitId1, segmentNumber1, ref newDocument, ref newFile, ref newPu);
			}
			document.ExtractSegmentPair(fileId, paragraphUnitId2, segmentNumber2, ref newDocument, ref newFile, ref newPu);
			ExtractDocumentContext(document, fileId, paragraphUnitId2, newFile.Skeleton, newPu);
			if (!string.IsNullOrEmpty(paragraphUnitId3))
			{
				document.ExtractSegmentPair(fileId, paragraphUnitId3, segmentNumber3, ref newDocument, ref newFile, ref newPu);
			}
			newParagraphUnitId = newPu.Id;
			ExtractQuickInsertIds(document, fileId, newFile);
			return newDocument;
		}

		private static void ExtractSourceSegment(File file, string paragraphUnitId, string segmentNumber, out SegmentPair sp, ref File newFile, ref ParagraphUnit newPu)
		{
			CloneSegmentAndParagraph(file, paragraphUnitId, segmentNumber, out sp, ref newFile, ref newPu, out Segment newSegment);
			newSegment.ExtractSegmentSkeleton(file.Skeleton, newFile.Skeleton);
		}

		private static void ExtractTargetSegment(File file, SegmentPair sp, ref File newFile, ref ParagraphUnit newPu, bool includeTarget)
		{
			Segment segment;
			if (includeTarget)
			{
				segment = sp.Target.Clone();
				segment.ExtractSegmentSkeleton(file.Skeleton, newFile.Skeleton);
			}
			else
			{
				segment = new Segment(sp.Source.SegmentNumber);
			}
			if (newPu.Target == null)
			{
				newPu.Target = new Paragraph(segment);
			}
			else
			{
				newPu.Target.Add(segment);
			}
		}

		private static void CreateNewFile(Document document, File file, ref Document newDocument, ref File newFile)
		{
			if (newFile == null)
			{
				newDocument = new Document
				{
					Name = document.Name,
					SourceLanguageCode = document.SourceLanguageCode,
					TargetLanguageCode = document.TargetLanguageCode
				};
				newFile = new File
				{
					OriginalFileName = file.OriginalFileName,
					FileTypeDefinitionId = file.FileTypeDefinitionId
				};
			}
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

		private static void CloneSegmentAndParagraph(File file, string puId, string segmentNumber, out SegmentPair currentSp, ref File newFile, ref ParagraphUnit newPu, out Segment newSegment)
		{
			ParagraphUnit paragraphUnit = file.ParagraphUnits.SingleOrDefault((ParagraphUnit item) => item.Id == puId);
			if (paragraphUnit == null)
			{
				throw new ArgumentException("Paragraph Unit not found", "paragraphUnitId");
			}
			currentSp = paragraphUnit.SegmentPairs.SingleOrDefault((SegmentPair sp) => sp.Source.SegmentNumber == segmentNumber);
			if (currentSp == null)
			{
				throw new ArgumentException("Segment Pair not found", "segmentNumber");
			}
			newSegment = currentSp.Source.Clone();
			if (newPu == null)
			{
				newPu = new ParagraphUnit
				{
					ParentFileId = newFile.Id,
					Source = new Paragraph(newSegment)
				};
			}
			else
			{
				newPu.Source.Add(newSegment);
			}
		}

		private static void ExtractDocumentContext(Document document, string fileId, string paragraphUnitId, FileSkeleton newSkeleton, ParagraphUnit newParagraphUnit)
		{
			File file = GetFile(document, fileId);
			ParagraphUnit paragraphUnit = file.ParagraphUnits.SingleOrDefault((ParagraphUnit item) => item.Id == paragraphUnitId);
			if (paragraphUnit != null)
			{
				newParagraphUnit.StructureContextId = paragraphUnit.StructureContextId;
				ContextExtractorExtensions.ExtractContext(file, paragraphUnit, newSkeleton);
				if (paragraphUnit.ContextList != null && paragraphUnit.ContextList.Any())
				{
					newParagraphUnit.ContextList = paragraphUnit.ContextList.ToList();
				}
			}
		}

		private static void ExtractQuickInsertIds(Document document, string fileId, File newFile)
		{
			File file = GetFile(document, fileId);
			newFile.Skeleton.QuickInsertIds = file.Skeleton?.QuickInsertIds;
		}
	}
}
