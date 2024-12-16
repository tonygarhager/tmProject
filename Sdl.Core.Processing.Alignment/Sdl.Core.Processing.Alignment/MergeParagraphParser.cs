using Sdl.Core.Globalization;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.ReverseAlignment;
using Sdl.Core.Processing.Processors.Storage;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Processing.Alignment
{
	internal class MergeParagraphParser : AbstractBilingualParser
	{
		private readonly ParagraphUnitStore _leftDocument;

		private readonly ParagraphUnitStore _rightDocument;

		private readonly CultureInfo _leftCulture;

		private readonly CultureInfo _rightCulture;

		private int _previousProgress;

		public EventHandler<ProgressEventArgs> ProgressHandler;

		public string FilterId
		{
			get;
		}

		public AlignmentToOriginalMapper ReverseMapper
		{
			get;
			private set;
		}

		public string OutputFile
		{
			get;
		}

		private bool IsReverseAlignment
		{
			get;
		}

		public MergeParagraphParser(ParagraphUnitStore leftDocument, ParagraphUnitStore rightDocument, CultureInfo leftCulture, CultureInfo rightCulture)
		{
			_leftDocument = (leftDocument ?? throw new ArgumentNullException("leftDocument"));
			_rightDocument = (rightDocument ?? throw new ArgumentNullException("rightDocument"));
			_leftCulture = leftCulture;
			_rightCulture = rightCulture;
		}

		public MergeParagraphParser(ParagraphUnitStore leftDocument, ParagraphUnitStore rightDocument, CultureInfo leftCulture, CultureInfo rightCulture, string outputFile, string filterId)
			: this(leftDocument, rightDocument, leftCulture, rightCulture)
		{
			OutputFile = outputFile;
			FilterId = filterId;
			ReverseMapper = new AlignmentToOriginalMapper();
			IsReverseAlignment = true;
		}

		private void InvokeProgressUpdate(int progress)
		{
			if (ProgressHandler != null && _previousProgress < progress)
			{
				if (progress < 0)
				{
					progress = 0;
				}
				if (progress > 100)
				{
					progress = 100;
				}
				_previousProgress = progress;
				ProgressHandler(this, new ProgressEventArgs((byte)progress));
			}
		}

		public override bool ParseNext()
		{
			InvokeProgressUpdate(0);
			InitializeOutput();
			MergeDocuments();
			CompleteOutput();
			return false;
		}

		private void InitializeOutput()
		{
			IFileExtractor fileExtractor = base.Output as IFileExtractor;
			if (fileExtractor == null)
			{
				throw new Exception("Output must implement IFileExtractor");
			}
			IDocumentProperties documentInfo = fileExtractor.DocumentInfo;
			documentInfo.SourceLanguage = new Language(_leftCulture);
			documentInfo.TargetLanguage = new Language(_rightCulture);
			base.Output.Initialize(documentInfo);
			IFileProperties fileProperties = ItemFactory.CreateFileProperties();
			base.Output.SetFileProperties(fileProperties);
		}

		private void MergeDocumentsForAlignment(IParagraphUnit outputParagraphUnit)
		{
			DocumentStructureBuilder structureBuilder = new DocumentStructureBuilder('\\', '#', _leftDocument);
			DocumentStructureBuilder structureBuilder2 = new DocumentStructureBuilder('\\', '#', _rightDocument);
			int num = Math.Max(_leftDocument.Count, _rightDocument.Count);
			for (int i = 0; i < num; i++)
			{
				IParagraphUnit paragraphUnit = (i < _leftDocument.Count) ? _leftDocument[i] : null;
				IParagraphUnit paragraphUnit2 = (i < _rightDocument.Count) ? _rightDocument[i] : null;
				if (paragraphUnit == null && paragraphUnit2 == null)
				{
					throw new Exception("Both left and right paragraph units cannot be null.");
				}
				if (paragraphUnit != null)
				{
					CopySegments(paragraphUnit.Source, outputParagraphUnit.Source, structureBuilder);
				}
				if (paragraphUnit2 != null)
				{
					CopySegments(paragraphUnit2.Source, outputParagraphUnit.Target, structureBuilder2);
				}
				InvokeProgressUpdate((int)((double)i / (double)num * 100.0));
			}
			RenumberParagraphTagIds(outputParagraphUnit);
		}

		private static void RenumberParagraphTagIds(IParagraphUnit outputParagraphUnit)
		{
			int maxTagId = GetMaxTagId(outputParagraphUnit.Source);
			maxTagId++;
			ReplaceTagId(outputParagraphUnit.Target, maxTagId);
		}

		private static int GetMaxTagId(IParagraph paragraph)
		{
			MaxTagIdVisitor maxTagIdVisitor = new MaxTagIdVisitor();
			foreach (IAbstractMarkupData item in paragraph)
			{
				if (!(item is StructureTag))
				{
					item.AcceptVisitor(maxTagIdVisitor);
				}
			}
			return maxTagIdVisitor.TagId;
		}

		private static void ReplaceTagId(IParagraph paragraph, int tagid)
		{
			ReplaceTagIdVisitor visitor = new ReplaceTagIdVisitor(tagid, paragraph.Parent.Properties.ParagraphUnitId.ToString());
			foreach (IAbstractMarkupData item in paragraph)
			{
				if (!(item is StructureTag))
				{
					item.AcceptVisitor(visitor);
				}
			}
		}

		private void MergeDocumentsForReverseAlignment(IParagraphUnit outputParagraphUnit)
		{
			ReverseMapper = new AlignmentToOriginalMapper();
			int id = 1;
			int id2 = 1;
			DocumentStructureBuilder structureBuilder = new DocumentStructureBuilder('\\', '#', _leftDocument);
			DocumentStructureBuilder structureBuilder2 = new DocumentStructureBuilder('\\', '#', _rightDocument);
			int num = Math.Max(_leftDocument.Count, _rightDocument.Count);
			for (int i = 0; i < num; i++)
			{
				IParagraphUnit paragraphUnit = (i < _leftDocument.Count) ? _leftDocument[i] : null;
				IParagraphUnit paragraphUnit2 = (i < _rightDocument.Count) ? _rightDocument[i] : null;
				if (paragraphUnit == null && paragraphUnit2 == null)
				{
					throw new Exception("Both left and right paragraph units cannot be null.");
				}
				if (paragraphUnit != null)
				{
					ReplaceParagraphSegments(paragraphUnit.Target, structureBuilder, isSource: true, ref id);
					CopyParagraph(paragraphUnit.Target, outputParagraphUnit.Source);
				}
				if (paragraphUnit2 != null)
				{
					IParagraph rightSideParagraph = GetRightSideParagraph(paragraphUnit2, FilterId);
					ReplaceParagraphSegments(rightSideParagraph, structureBuilder2, isSource: false, ref id2);
					CopyParagraph(rightSideParagraph, outputParagraphUnit.Target);
				}
				InvokeProgressUpdate((int)((double)i / (double)num * 100.0));
			}
			RenumberParagraphTagIds(outputParagraphUnit);
		}

		private static bool IsBilingualFilter(string filterId)
		{
			if (filterId == "Bilingual Excel v 1.0.0.0")
			{
				return true;
			}
			return false;
		}

		private static IParagraph GetRightSideParagraph(IParagraphUnit paragraphUnit, string filterId)
		{
			if (!IsBilingualFilter(filterId))
			{
				return paragraphUnit.Source;
			}
			return paragraphUnit.Target;
		}

		private void MergeDocuments()
		{
			IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
			if (!IsReverseAlignment)
			{
				MergeDocumentsForAlignment(paragraphUnit);
			}
			else
			{
				MergeDocumentsForReverseAlignment(paragraphUnit);
			}
			AddBlankSegments(paragraphUnit);
			SegmentPairsRepairer.RepairSegmentPairs(GetSegments(paragraphUnit.Source), GetSegments(paragraphUnit.Target));
			base.Output.ProcessParagraphUnit(paragraphUnit);
		}

		private static void CopySegments(IParagraph inputParagraph, IParagraph outputParagraph, DocumentStructureBuilder structureBuilder)
		{
			foreach (ISegment segment in GetSegments(inputParagraph))
			{
				RemoveSubsegmentReferences(inputParagraph);
				AlignmentSegment item = new AlignmentSegment(segment, structureBuilder.GetStructurePath(segment));
				outputParagraph.Add(item);
			}
		}

		private void ReplaceParagraphSegments(IParagraph inputParagraph, DocumentStructureBuilder structureBuilder, bool isSource, ref int id)
		{
			foreach (ISegment segment in GetSegments(inputParagraph))
			{
				RemoveSubsegmentReferences(inputParagraph);
				AlignmentSegment item = new AlignmentSegment(segment, structureBuilder.GetStructurePath(segment));
				IDictionary<int, MappingDocumentSegmentId> dictionary = isSource ? ReverseMapper.LeftSegmentIds : ReverseMapper.RightSegmentIds;
				string id2 = Convert.ToString(id);
				dictionary.Add(id, new MappingDocumentSegmentId(inputParagraph.Parent.Properties.ParagraphUnitId, segment.Properties.Id, new SegmentId(id2)));
				id++;
				int indexInParent = segment.IndexInParent;
				IAbstractMarkupDataContainer parent = segment.Parent;
				segment.RemoveFromParent();
				parent.Insert(indexInParent, item);
			}
		}

		private void CopyParagraph(IParagraph inputParagraph, IParagraph outputParagraph)
		{
			foreach (IAbstractMarkupData item in inputParagraph.ToList())
			{
				if (!(item is StructureTag))
				{
					item.RemoveFromParent();
					outputParagraph.Add(item);
				}
			}
		}

		private static void RemoveSubsegmentReferences(IParagraph paragraph)
		{
			foreach (IAbstractTag item in paragraph.AllSubItems.OfType<IAbstractTag>())
			{
				item.ClearSubSegmentReferences();
			}
		}

		private static IList<ISegment> GetSegments(IParagraph paragraph)
		{
			SegmentCollectionVisitor segmentCollectionVisitor = new SegmentCollectionVisitor();
			foreach (IAbstractMarkupData item in paragraph)
			{
				if (!(item is StructureTag))
				{
					item.AcceptVisitor(segmentCollectionVisitor);
				}
			}
			return segmentCollectionVisitor.Segments;
		}

		private void AddBlankSegments(IParagraphUnit outputParagraphUnit)
		{
			IList<ISegment> segments = GetSegments(outputParagraphUnit.Source);
			IList<ISegment> segments2 = GetSegments(outputParagraphUnit.Target);
			if (segments.Count > segments2.Count)
			{
				AddBlankSegments(outputParagraphUnit.Target, segments.Count - segments2.Count, segments);
			}
			if (segments2.Count > segments.Count)
			{
				AddBlankSegments(outputParagraphUnit.Source, segments2.Count - segments.Count, segments2);
			}
		}

		private void AddBlankSegments(IParagraph outputParagraph, int blankSegmentCount, IList<ISegment> inputSegments)
		{
			int num = inputSegments.Count - blankSegmentCount;
			for (int i = num; i < inputSegments.Count; i++)
			{
				ISegment segment = inputSegments[i];
				ISegment segment2 = ItemFactory.CreateSegment(segment.Properties);
				IOtherMarker otherMarker = ItemFactory.CreateOtherMarker();
				otherMarker.MarkerType = "AlignmentReadOnlySegment";
				segment2.Add(otherMarker);
				outputParagraph.Add(segment2);
			}
		}

		private void CompleteOutput()
		{
			base.Output.FileComplete();
			base.Output.Complete();
		}
	}
}
