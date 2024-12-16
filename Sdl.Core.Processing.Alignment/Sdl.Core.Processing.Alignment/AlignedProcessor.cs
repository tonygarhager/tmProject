using Sdl.Core.Globalization;
using Sdl.Core.LanguageProcessing.SegmentTransferTools;
using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment
{
	internal class AlignedProcessor : AbstractBilingualContentProcessor
	{
		private readonly AbstractAlignmentProcessor _alignmentProcessor;

		private readonly byte _minimumAlignmentQuality;

		private List<AlignmentData> _alignments;

		private bool _insertBlankInSource;

		private bool _insertBlankInTarget;

		public AlignedProcessor(AbstractAlignmentProcessor alignmentProcessor, byte minimumAlignmentQuality)
		{
			if (alignmentProcessor == null)
			{
				throw new ArgumentNullException("alignmentProcessor");
			}
			_alignmentProcessor = alignmentProcessor;
			_minimumAlignmentQuality = minimumAlignmentQuality;
		}

		public AlignedProcessor(List<AlignmentData> alignments, byte minimumAlignmentQuality)
		{
			_alignments = alignments;
			_minimumAlignmentQuality = minimumAlignmentQuality;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}
			IParagraphUnit paragraphUnit2 = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
			ParagraphUnitId paragraphUnitId = paragraphUnit.Properties.ParagraphUnitId;
			IList<ISegment> clonedSegments = GetClonedSegments(paragraphUnit.Source);
			IList<ISegment> clonedSegments2 = GetClonedSegments(paragraphUnit.Target);
			Dictionary<string, ISegment> segmentIndex = CreateSegmentIndex(paragraphUnitId, clonedSegments);
			Dictionary<string, ISegment> segmentIndex2 = CreateSegmentIndex(paragraphUnitId, clonedSegments2);
			HashSet<string> consumedCollection = new HashSet<string>();
			HashSet<string> consumedCollection2 = new HashSet<string>();
			int num = 0;
			int num2 = 0;
			if (_alignments == null)
			{
				_alignments = _alignmentProcessor.Alignments;
			}
			_alignments.Sort();
			AlignmentData alignmentData = null;
			AlignmentData alignmentData2 = null;
			foreach (AlignmentData alignment in _alignments)
			{
				if ((byte)((1.0 - (double)alignment.Cost) * 100.0) < _minimumAlignmentQuality || alignment.AlignmentType == AlignmentType.Alignment01 || alignment.AlignmentType == AlignmentType.Alignment10)
				{
					alignmentData = alignment;
				}
				else
				{
					int mergedCount;
					ISegment segment = Merge(LocateSegments(alignment.LeftIds, segmentIndex), _insertBlankInSource, out mergedCount);
					int mergedCount2;
					ISegment segment2 = Merge(LocateSegments(alignment.RightIds, segmentIndex2), _insertBlankInTarget, out mergedCount2);
					num += alignment.LeftIds.Count;
					num2 += alignment.RightIds.Count;
					MarkConsumed(alignment.LeftIds, consumedCollection);
					MarkConsumed(alignment.RightIds, consumedCollection2);
					bool flag = alignment.AlignmentType == AlignmentType.Alignment22C || alignment.AlignmentType == AlignmentHelper.GetAlignmentType(mergedCount, mergedCount2);
					if (segment == null)
					{
						segment = CreateBlankSegment();
					}
					if (segment2 == null)
					{
						segment2 = CreateBlankSegment();
					}
					if (segment.Properties.TranslationOrigin == null)
					{
						segment.Properties.TranslationOrigin = ItemFactory.CreateTranslationOrigin();
					}
					segment.Properties.TranslationOrigin.OriginSystem = alignment.AlignmentType.ToString();
					segment.Properties.TranslationOrigin.SetMetaData("AlignmentQuality", alignment.Quality.ToString());
					if (alignmentData == null && alignment.AlignmentType == AlignmentType.Alignment11)
					{
						segment.Properties.TranslationOrigin.SetMetaData("PreviousSegmentId", "0");
						alignmentData2 = alignment;
					}
					else if (alignment.AlignmentType == AlignmentType.Alignment11)
					{
						if (alignmentData2 != null)
						{
							DocumentSegmentId documentSegmentId = alignmentData2.LeftIds.FirstOrDefault();
							if (documentSegmentId != null && object.Equals(alignmentData2.LeftIds.FirstOrDefault(), alignmentData.LeftIds.FirstOrDefault()))
							{
								segment.Properties.TranslationOrigin.SetMetaData("PreviousSegmentId", documentSegmentId.SegmentId.Id);
							}
						}
						alignmentData2 = alignment;
					}
					segment.Properties.ConfirmationLevel = ((!alignment.Confirmed) ? ConfirmationLevel.Draft : ConfirmationLevel.ApprovedSignOff);
					segment2.Properties = segment.Properties;
					AssignTranslationScore(segment, alignment);
					paragraphUnit2.Source.Add(segment);
					paragraphUnit2.Target.Add(segment2);
					alignmentData = alignment;
				}
			}
			SegmentPairsRepairer.RepairSegmentPairs(GetSegments(paragraphUnit2.Source), GetSegments(paragraphUnit2.Target));
			SegmentTransferHelper.RenumberTagIds(paragraphUnit2.Properties.ParagraphUnitId, paragraphUnit2.Target);
			SetAlignmentOrigin(paragraphUnit2);
			TagAligner.AlignTags(paragraphUnit2);
			base.ProcessParagraphUnit(paragraphUnit2);
		}

		private static void AssignTranslationScore(ISegment left, AlignmentData alignment)
		{
			left.Properties.TranslationOrigin.MatchPercent = (byte)((1.0 - (double)alignment.Cost) * 100.0);
		}

		private static string CreateCombinedSegmentKey(DocumentSegmentId id)
		{
			return id.ParagraphUnitId.Id + "@@@" + id.SegmentId.Id;
		}

		private static string CreateCombinedSegmentKey(ParagraphUnitId currentParagraphUnitId, ISegment segment)
		{
			return currentParagraphUnitId.Id + "@@@" + segment.Properties.Id.Id;
		}

		private static Dictionary<string, ISegment> CreateSegmentIndex(ParagraphUnitId currentParagraphUnitId, IEnumerable<ISegment> segments)
		{
			Dictionary<string, ISegment> dictionary = new Dictionary<string, ISegment>();
			foreach (ISegment segment in segments)
			{
				string key = CreateCombinedSegmentKey(currentParagraphUnitId, segment);
				dictionary.Add(key, segment);
			}
			return dictionary;
		}

		private static void MarkConsumed(IEnumerable<DocumentSegmentId> ids, HashSet<string> consumedCollection)
		{
			foreach (DocumentSegmentId id in ids)
			{
				string item = CreateCombinedSegmentKey(id);
				bool flag = consumedCollection.Add(item);
			}
		}

		private static bool AllSegmentsConsumed(ParagraphUnitId currentParagraphUnitId, IEnumerable<ISegment> segments, HashSet<string> consumed)
		{
			int num = 0;
			foreach (ISegment segment in segments)
			{
				string item = CreateCombinedSegmentKey(currentParagraphUnitId, segment);
				if (!consumed.Contains(item) && !IsPaddingSegment(segment))
				{
					return false;
				}
				num++;
			}
			if (num < consumed.Count)
			{
				return false;
			}
			return true;
		}

		private static bool IsPaddingSegment(ISegment segment)
		{
			if (segment == null || segment.Count != 1)
			{
				return false;
			}
			IOtherMarker otherMarker = segment[0] as IOtherMarker;
			if (otherMarker != null)
			{
				return string.Equals(otherMarker.MarkerType, "AlignmentReadOnlySegment");
			}
			return false;
		}

		private static IEnumerable<ISegment> LocateSegments(IEnumerable<DocumentSegmentId> ids, Dictionary<string, ISegment> segmentIndex)
		{
			return ids.Select((DocumentSegmentId currentId) => segmentIndex[CreateCombinedSegmentKey(currentId)]);
		}

		private static IList<ISegment> GetClonedSegments(IParagraph paragraph)
		{
			List<ISegment> list = new List<ISegment>();
			foreach (ISegment segment2 in GetSegments(paragraph))
			{
				ISegment segment = (ISegment)segment2.Clone();
				segment.Properties = (ISegmentPairProperties)segment2.Properties.Clone();
				list.Add(segment);
			}
			return list;
		}

		private static IEnumerable<ISegment> GetSegments(IParagraph paragraph)
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

		private ISegment Merge(IEnumerable<ISegment> segments, bool insertBlankBetweenSegments, out int mergedCount)
		{
			mergedCount = 0;
			if (segments == null || segments.Count() == 0)
			{
				return null;
			}
			mergedCount = segments.Count();
			if (segments.Count() == 1)
			{
				return segments.First();
			}
			return segments.Aggregate(delegate(ISegment segment0, ISegment segment1)
			{
				if (insertBlankBetweenSegments)
				{
					segment0.Add(ItemFactory.CreateText(ItemFactory.PropertiesFactory.CreateTextProperties(" ")));
				}
				while (segment1.Count > 0)
				{
					IAbstractMarkupData abstractMarkupData = segment1[0];
					abstractMarkupData.RemoveFromParent();
					abstractMarkupData.Parent = null;
					segment0.Add(abstractMarkupData);
				}
				return segment0;
			});
		}

		private ISegment CreateBlankSegment()
		{
			IOtherMarker otherMarker = ItemFactory.CreateOtherMarker();
			otherMarker.MarkerType = "AlignmentReadOnlySegment";
			ISegment segment = ItemFactory.CreateSegment(ItemFactory.CreateSegmentPairProperties());
			segment.Add(otherMarker);
			return segment;
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			if (documentInfo != null && documentInfo.SourceLanguage != null && documentInfo.TargetLanguage != null && documentInfo.SourceLanguage.CultureInfo != null && documentInfo.TargetLanguage.CultureInfo != null)
			{
				_insertBlankInSource = (CultureInfoExtensions.UseBlankAsSentenceSeparator(documentInfo.SourceLanguage.CultureInfo) || CultureInfoExtensions.UseBlankAsWordSeparator(documentInfo.SourceLanguage.CultureInfo));
				_insertBlankInTarget = (CultureInfoExtensions.UseBlankAsSentenceSeparator(documentInfo.TargetLanguage.CultureInfo) || CultureInfoExtensions.UseBlankAsWordSeparator(documentInfo.TargetLanguage.CultureInfo));
			}
			else
			{
				_insertBlankInSource = true;
				_insertBlankInTarget = true;
			}
			base.Initialize(documentInfo);
		}

		private void SetAlignmentOrigin(IParagraphUnit pu)
		{
			foreach (ISegmentPair segmentPair in pu.SegmentPairs)
			{
				if (segmentPair != null && segmentPair.Source != null && segmentPair.Target != null)
				{
					if (segmentPair.Properties.TranslationOrigin == null)
					{
						segmentPair.Properties.TranslationOrigin = ItemFactory.CreateTranslationOrigin();
					}
					segmentPair.Properties.TranslationOrigin.OriginType = "auto-aligned";
				}
			}
		}
	}
}
