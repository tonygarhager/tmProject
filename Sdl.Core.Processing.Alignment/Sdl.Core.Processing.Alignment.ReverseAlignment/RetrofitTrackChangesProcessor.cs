using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using Sdl.Core.Processing.Alignment.RetrofitApplier;
using Sdl.Core.Processing.Alignment.SdlAlignPackage;
using Sdl.Core.Processing.Processors.Storage;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	internal class RetrofitTrackChangesProcessor : AbstractBilingualContentProcessor
	{
		private readonly ParagraphUnitStore _xliffPuStore;

		private readonly SdlRetrofitPackage _package;

		private Dictionary<string, ISegmentPair> _xliffTargetIdToXliffSegmentPair = new Dictionary<string, ISegmentPair>();

		private Dictionary<SegmentId, SegmentId> _packageSourceIdToXliffSourceSegments = new Dictionary<SegmentId, SegmentId>();

		private Dictionary<SegmentId, SegmentId> _packageSourceIdToXliffTargetSegments = new Dictionary<SegmentId, SegmentId>();

		private EdApplier _retrofitTrackChangesApplier;

		private int _lastAlignmentProcessedIndex;

		private RetrofitUpdateSettings _retrofitUpdateSettings;

		public int UpdatedSegmentErrorCount
		{
			get;
			set;
		}

		public int UpdatedSegmentsCount
		{
			get;
			set;
		}

		private EdApplier RetrofitTrackChangesApplier
		{
			get
			{
				if (_retrofitTrackChangesApplier == null)
				{
					_retrofitTrackChangesApplier = new EdApplier(new CultureInfo(_package.AlignmentSettingsInfo.RightDocumentLanguage));
				}
				return _retrofitTrackChangesApplier;
			}
		}

		private RetrofitUpdateSettings GetDefaultUpdateSettings()
		{
			return new RetrofitUpdateSettings
			{
				SkipLockedSegment = false
			};
		}

		public RetrofitTrackChangesProcessor(SdlRetrofitPackage package, ParagraphUnitStore xliffPuStore, RetrofitUpdateSettings settings)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}
			if (xliffPuStore == null)
			{
				throw new ArgumentNullException("xliffPuStore");
			}
			_retrofitUpdateSettings = (settings ?? GetDefaultUpdateSettings());
			_package = package;
			_xliffPuStore = xliffPuStore;
		}

		public override void ProcessParagraphUnit(IParagraphUnit outputParagraphUnit)
		{
			CollectTagPropertiesVisitor collectTagPropertiesVisitor = new CollectTagPropertiesVisitor();
			CopyMetadataVisitor copyMetadataVisitor = new CopyMetadataVisitor();
			if (!outputParagraphUnit.IsStructure)
			{
				UpdateXliffParagraph(outputParagraphUnit);
			}
			foreach (IAbstractMarkupData item in outputParagraphUnit.Source)
			{
				IStructureTag structureTag = item as IStructureTag;
				if (structureTag == null)
				{
					item.AcceptVisitor(collectTagPropertiesVisitor);
				}
			}
			copyMetadataVisitor.StartTagProperties = collectTagPropertiesVisitor.StartTagProperties;
			copyMetadataVisitor.EndTagProperties = collectTagPropertiesVisitor.EndTagProperties;
			foreach (IAbstractMarkupData item2 in outputParagraphUnit.Target)
			{
				IStructureTag structureTag2 = item2 as IStructureTag;
				if (structureTag2 == null)
				{
					item2.AcceptVisitor(copyMetadataVisitor);
				}
			}
			base.ProcessParagraphUnit(outputParagraphUnit);
		}

		private void UpdateXliffParagraph(IParagraphUnit xliffParagraph)
		{
			bool skipParagraph = false;
			while (_lastAlignmentProcessedIndex <= _package.Alignments.Count - 1)
			{
				AlignmentData alignmentData = _package.Alignments[_lastAlignmentProcessedIndex];
				if (alignmentData != null)
				{
					if (alignmentData.LeftIds.Count == 1 && alignmentData.RightIds.Count == 1)
					{
						ProcessAlignment1To1(xliffParagraph, alignmentData, ref skipParagraph);
					}
					if (alignmentData.LeftIds.Count == 1 && alignmentData.RightIds.Count > 1)
					{
						ProcessAlignment1ToN(xliffParagraph, alignmentData, ref skipParagraph);
					}
					if (alignmentData.LeftIds.Count > 1 && alignmentData.RightIds.Count == 1)
					{
						ProcessAlignmentNTo1(xliffParagraph, alignmentData, ref skipParagraph);
					}
					if (alignmentData.LeftIds.Count == 2 && alignmentData.RightIds.Count == 2)
					{
						ProcessAlignment2To2(xliffParagraph, alignmentData, ref skipParagraph);
					}
					if (skipParagraph)
					{
						break;
					}
					_lastAlignmentProcessedIndex++;
				}
			}
			UpdatedSegmentsCount = RetrofitTrackChangesApplier.UpdatedSegmentsCount;
			UpdatedSegmentErrorCount = RetrofitTrackChangesApplier.UpdatedSegmentErrorCount;
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			InitializeWorkingCollections();
			base.Initialize(documentInfo);
		}

		private void InitializeWorkingCollections()
		{
			if (_xliffTargetIdToXliffSegmentPair != null && _xliffTargetIdToXliffSegmentPair.Count == 0)
			{
				_xliffTargetIdToXliffSegmentPair = GetSegmentPairMappings(_xliffPuStore);
			}
			if (_packageSourceIdToXliffSourceSegments == null || _packageSourceIdToXliffSourceSegments.Count == 0)
			{
				_packageSourceIdToXliffSourceSegments = new Dictionary<SegmentId, SegmentId>();
				_packageSourceIdToXliffTargetSegments = new Dictionary<SegmentId, SegmentId>();
				foreach (ISegment segment in GetSegments(_package.ParagraphUnit.Source))
				{
					SegmentId id = segment.Properties.Id;
					int key = int.Parse(id.Id);
					if (_package.ReverseMapper.LeftSegmentIds.ContainsKey(key))
					{
						string id2 = _package.ReverseMapper.LeftSegmentIds[key].OriginalSegmentId.Id;
						string id3 = _package.ReverseMapper.LeftSegmentIds[key].OriginalParagraphUnitId.Id;
						string key2 = CreateCombinedSegmentKeyFromStrings(id3, id2);
						ISegment source = _xliffTargetIdToXliffSegmentPair[key2].Source;
						ISegment target = _xliffTargetIdToXliffSegmentPair[key2].Target;
						_packageSourceIdToXliffSourceSegments.Add(id, source.Properties.Id);
						_packageSourceIdToXliffTargetSegments.Add(id, target.Properties.Id);
					}
				}
			}
		}

		private static Dictionary<string, ISegmentPair> GetSegmentPairMappings(IEnumerable<IParagraphUnit> puStore)
		{
			Dictionary<string, ISegmentPair> dictionary = new Dictionary<string, ISegmentPair>();
			foreach (ParagraphUnit item in puStore)
			{
				foreach (ISegmentPair segmentPair in item.SegmentPairs)
				{
					string key = CreateCombinedSegmentKey(item.Properties.ParagraphUnitId, segmentPair.Target);
					dictionary.Add(key, segmentPair);
				}
			}
			return dictionary;
		}

		private static string CreateCombinedSegmentKey(ParagraphUnitId currentParagraphUnitId, ISegment segment)
		{
			return currentParagraphUnitId.Id + "@@@" + segment.Properties.Id.Id;
		}

		private static string CreateCombinedSegmentKeyFromStrings(string currentParagraphUnitId, string segment)
		{
			return currentParagraphUnitId + "@@@" + segment;
		}

		private static IEnumerable<ISegment> GetSegments(IEnumerable<IAbstractMarkupData> paragraph)
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

		private void ProcessAlignment1To1(IParagraphUnit xliffParagraph, AlignmentData alignmentElement, ref bool skipParagraph)
		{
			SegmentId id = _packageSourceIdToXliffTargetSegments[alignmentElement.LeftIds[0].SegmentId];
			ISegment targetSegment = xliffParagraph.GetTargetSegment(id);
			if (targetSegment == null)
			{
				skipParagraph = true;
				return;
			}
			ISegment targetSegment2 = _package.ParagraphUnit.GetTargetSegment(alignmentElement.RightIds[0].SegmentId);
			RetrofitTrackChangesApplier.UpdateBcm(targetSegment, targetSegment2, _retrofitUpdateSettings);
		}

		private void ProcessAlignment1ToN(IParagraphUnit xliffParagraph, AlignmentData alignmentElement, ref bool skipParagraph)
		{
			SegmentId id = _packageSourceIdToXliffTargetSegments[alignmentElement.LeftIds[0].SegmentId];
			ISegment targetSegment = xliffParagraph.GetTargetSegment(id);
			if (targetSegment == null)
			{
				skipParagraph = true;
				return;
			}
			List<ISegment> list = new List<ISegment>();
			IParagraphUnit paragraphUnit = null;
			foreach (DocumentSegmentId rightId in alignmentElement.RightIds)
			{
				int key = int.Parse(rightId.SegmentId.Id);
				ParagraphUnitId paraId = _package.ReverseMapper.RightSegmentIds[key].OriginalParagraphUnitId;
				SegmentId originalSegmentId = _package.ReverseMapper.RightSegmentIds[key].OriginalSegmentId;
				paragraphUnit = _package.UpdatedTargetPUs.FirstOrDefault((IParagraphUnit pu) => pu.Properties.ParagraphUnitId == paraId);
				ISegment sourceSegment = paragraphUnit.GetSourceSegment(originalSegmentId);
				list.Add(sourceSegment);
			}
			ISegment updatedBilingualSegment = Merge(paragraphUnit.Source, list);
			RetrofitTrackChangesApplier.UpdateBcm(targetSegment, updatedBilingualSegment, _retrofitUpdateSettings);
		}

		private void ProcessAlignmentNTo1(IParagraphUnit xliffParagraph, AlignmentData alignmentElement, ref bool skipParagraph)
		{
			IList<ISegment> list = new List<ISegment>();
			IList<ISegment> list2 = new List<ISegment>();
			foreach (DocumentSegmentId leftId in alignmentElement.LeftIds)
			{
				SegmentId id = _packageSourceIdToXliffSourceSegments[leftId.SegmentId];
				ISegment sourceSegment = xliffParagraph.GetSourceSegment(id);
				if (sourceSegment == null)
				{
					skipParagraph = true;
					return;
				}
				list.Add(sourceSegment);
				SegmentId id2 = _packageSourceIdToXliffSourceSegments[leftId.SegmentId];
				list2.Add(xliffParagraph.GetTargetSegment(id2));
			}
			IParagraphUnit paragraphUnit = (IParagraphUnit)xliffParagraph.Clone();
			ISegment segment = Merge(xliffParagraph.Source, list);
			ISegment originalBilingualSegment = Merge(xliffParagraph.Target, list2);
			ISegment targetSegment = _package.ParagraphUnit.GetTargetSegment(alignmentElement.RightIds[0].SegmentId);
			RetrofitTrackChangesApplier.UpdateBcm(originalBilingualSegment, targetSegment, _retrofitUpdateSettings);
		}

		private void ProcessAlignment2To2(IParagraphUnit xliffParagraph, AlignmentData alignmentElement, ref bool skipParagraph)
		{
			int num = 0;
			while (true)
			{
				if (num < alignmentElement.LeftIds.Count)
				{
					SegmentId id = _packageSourceIdToXliffTargetSegments[alignmentElement.LeftIds[num].SegmentId];
					ISegment targetSegment = xliffParagraph.GetTargetSegment(id);
					if (targetSegment == null)
					{
						break;
					}
					SegmentId segmentId = alignmentElement.RightIds[num].SegmentId;
					if (alignmentElement.AlignmentType == AlignmentType.Alignment22C)
					{
						segmentId = alignmentElement.RightIds[Math.Abs(1 - num)].SegmentId;
					}
					ISegment targetSegment2 = _package.ParagraphUnit.GetTargetSegment(segmentId);
					RetrofitTrackChangesApplier.UpdateBcm(targetSegment, targetSegment2, _retrofitUpdateSettings);
					num++;
					continue;
				}
				return;
			}
			skipParagraph = true;
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

		private static ISegment Merge(IParagraph paragraph, IList<ISegment> segments)
		{
			if (segments == null || !segments.Any())
			{
				return null;
			}
			if (segments.Count() == 1)
			{
				return segments.First();
			}
			SegmentAndTagsMerger segmentAndTagsMerger = new SegmentAndTagsMerger();
			IParagraph paragraph2 = segmentAndTagsMerger.MergeSegmentsAndTags(paragraph, segments);
			for (int i = 1; i < segments.Count; i++)
			{
				segments[i].RemoveFromParent();
			}
			VoidSegment(segments[0]);
			paragraph2.MoveAllItemsTo(segments[0]);
			MoveTagOutsideSegment(segments[0]);
			return segments[0];
		}

		private static void VoidSegment(ISegment segment)
		{
			while (segment.Count > 0)
			{
				segment[0].RemoveFromParent();
			}
		}

		private static void MoveTagOutsideSegment(ISegment segment)
		{
			while (segment.Count == 1 && segment[0] is ITagPair)
			{
				ITagPair tagPair = segment[0] as ITagPair;
				tagPair.RemoveFromParent();
				tagPair.MoveAllItemsTo(segment);
				IAbstractMarkupDataContainer parent = segment.Parent;
				int indexInParent = segment.IndexInParent;
				segment.RemoveFromParent();
				tagPair.Add(segment);
				parent.Insert(indexInParent, tagPair);
			}
		}

		private static void RollbackParagraph(IParagraphUnit from, IParagraphUnit to)
		{
			to.Source.Clear();
			from.Source.MoveAllItemsTo(to.Source);
			to.Target.Clear();
			from.Target.MoveAllItemsTo(to.Target);
		}
	}
}
