using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class MarkupDataEditDistanceApplier
	{
		public enum MarkupDataAs
		{
			FromContent,
			ToContent
		}

		private Pair<ISegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>>> _markupDataFromSegment;

		private Pair<ISegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>>> _markupDataToSegment;

		public bool ShowDeleteOperations
		{
			get;
			set;
		}

		public bool ShowResolvedMoveOperations
		{
			get;
			set;
		} = true;


		public bool ShowResolvedDeleteOperations
		{
			get;
			set;
		} = true;


		public bool ShowResolvedSubstitutionOperations
		{
			get;
			set;
		}

		public bool ShowResolvedUnknownOperations
		{
			get;
			set;
		} = true;


		public EditDistance EditDistance
		{
			get;
			set;
		}

		public Segment LinguaFromSegment
		{
			get;
			set;
		}

		public Segment LinguaToSegment
		{
			get;
			set;
		}

		public IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		public Pair<ISegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>>> MarkupDataSegment
		{
			get;
			private set;
		}

		public MarkupDataAs MarkupDataIs
		{
			get;
			private set;
		}

		public MarkupDataEditDistanceApplier(IDocumentItemFactory factory)
		{
			ItemFactory = factory;
		}

		public void Execute(ISegment markupDataSegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>> tokenRanges, MarkupDataAs markupDataIs)
		{
			MarkupDataSegment = new Pair<ISegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>>>(markupDataSegment, tokenRanges);
			MarkupDataIs = markupDataIs;
			EdUtilities.MarkWhiteSpaceBetweenTextChangesAsChange(LinguaFromSegment, LinguaToSegment, EditDistance.Items);
			_markupDataFromSegment = CreateTokenizedMarkupDataSegment(LinguaFromSegment);
			_markupDataToSegment = CreateTokenizedMarkupDataSegment(LinguaToSegment);
			EditDistanceClassifier editDistanceClassifier = new EditDistanceClassifier(EditDistance.Items)
			{
				LinguaFromSegment = LinguaFromSegment,
				LinguaToSegment = LinguaToSegment
			};
			editDistanceClassifier.Execute();
			List<EditDistanceItem> list = new List<EditDistanceItem>();
			foreach (ClassifiedTagPairInfo deletedTagPair in editDistanceClassifier.DeletedTagPairs)
			{
				HandleDeletedTagPair(deletedTagPair);
			}
			foreach (ClassifiedTagPairInfo addedTagPair in editDistanceClassifier.AddedTagPairs)
			{
				List<EditDistanceItem> list2 = HandleAddedTagPair(addedTagPair);
				if (list2 != null)
				{
					foreach (EditDistanceItem item in list2)
					{
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			foreach (ClassifiedTagPairInfo movedTagPair in editDistanceClassifier.MovedTagPairs)
			{
				HandleMovedTagPair(movedTagPair);
			}
			foreach (EditDistanceChangeSequence changeSequence in editDistanceClassifier.ChangeSequences)
			{
				if (changeSequence.Items != null && !list.Intersect(changeSequence.Items).Any())
				{
					HandleChangeSequence(changeSequence);
				}
			}
			foreach (EditDistanceItem item2 in editDistanceClassifier.Other)
			{
				if (!list.Contains(item2))
				{
					HandleNonTagPair(item2);
				}
			}
		}

		private void HandleNonTagPair(EditDistanceItem item)
		{
			if (ShowThisOperation(item))
			{
				switch (item.Operation)
				{
				case EditOperation.Identity:
					break;
				case EditOperation.Change:
					HandleChange(item);
					break;
				case EditOperation.Move:
					HandleMove(item);
					break;
				case EditOperation.Insert:
					HandleInsert(item);
					break;
				case EditOperation.Delete:
					HandleDelete(item);
					break;
				}
			}
		}

		private bool ShowThisOperation(EditDistanceItem item)
		{
			switch (item.Resolution)
			{
			case EditDistanceResolution.None:
				return true;
			case EditDistanceResolution.Substitution:
				return ShowResolvedSubstitutionOperations;
			case EditDistanceResolution.Deletion:
				return ShowResolvedDeleteOperations;
			case EditDistanceResolution.Move:
				return ShowResolvedMoveOperations;
			case EditDistanceResolution.Other:
				return ShowResolvedUnknownOperations;
			default:
				return ShowResolvedUnknownOperations;
			}
		}

		private void HandleMovedTagPair(ClassifiedTagPairInfo info)
		{
			Pair<EditDistanceItem?, EditDistanceItem?> pair = new Pair<EditDistanceItem?, EditDistanceItem?>(info.Start, info.End);
			ITagPair tagPair;
			if (pair.First.HasValue)
			{
				tagPair = ((MarkupDataIs != 0) ? (MarkupDataSegment.Second[pair.First.Value.Target].Second.Parent as ITagPair) : (MarkupDataSegment.Second[pair.First.Value.Source].Second.Parent as ITagPair));
			}
			else
			{
				if (!pair.Second.HasValue)
				{
					return;
				}
				tagPair = ((MarkupDataIs != 0) ? (MarkupDataSegment.Second[pair.Second.Value.Target].First.Parent as ITagPair) : (MarkupDataSegment.Second[pair.Second.Value.Source].First.Parent as ITagPair));
			}
			if (tagPair == null)
			{
				return;
			}
			if (MarkupDataIs == MarkupDataAs.FromContent)
			{
				IPlaceholderTag placeholderTag = ItemFactory.CreatePlaceholderTag(ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(tagPair.StartTagProperties.TagContent));
				placeholderTag.TagProperties.TagId = tagPair.StartTagProperties.TagId;
				if (tagPair.HasSubSegmentReferences)
				{
					placeholderTag.AddSubSegmentReferences(tagPair.SubSegments);
				}
				placeholderTag.Properties.DisplayText = tagPair.StartTagProperties.DisplayText;
				IPlaceholderTag placeholderTag2 = ItemFactory.CreatePlaceholderTag(ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(tagPair.EndTagProperties.TagContent));
				if (tagPair.HasSubSegmentReferences)
				{
					placeholderTag2.AddSubSegmentReferences(tagPair.SubSegments);
				}
				placeholderTag2.TagProperties.TagId = tagPair.StartTagProperties.TagId;
				placeholderTag2.Properties.DisplayText = tagPair.EndTagProperties.DisplayText;
				tagPair.Parent.Insert(tagPair.IndexInParent, placeholderTag);
				tagPair.Parent.Insert(tagPair.IndexInParent + 1, placeholderTag2);
				tagPair.MoveAllItemsTo(tagPair.Parent, placeholderTag2.IndexInParent);
				tagPair.RemoveFromParent();
				IRevisionMarker revisionMarker = CreateInsertRevisionMarker();
				if (pair.First.HasValue && pair.Second.HasValue)
				{
					if (ShowDeleteOperations)
					{
						IRevisionMarker revisionMarker2 = CreateDeleteRevisionMarker();
						placeholderTag.Parent.Insert(placeholderTag.IndexInParent, revisionMarker2);
						placeholderTag.RemoveFromParent();
						revisionMarker2.Add(placeholderTag);
						revisionMarker2 = CreateDeleteRevisionMarker();
						placeholderTag2.Parent.Insert(placeholderTag2.IndexInParent, revisionMarker2);
						placeholderTag2.RemoveFromParent();
						revisionMarker2.Add(placeholderTag2);
					}
					tagPair.StartTagRevisionProperties = revisionMarker.Properties;
					tagPair.EndTagRevisionProperties = revisionMarker.Properties;
					IAbstractMarkupData abstractMarkupData = FindTokenStart(pair.First.Value.MoveTargetSource);
					IAbstractMarkupData abstractMarkupData2 = FindTokenStart(pair.Second.Value.MoveTargetSource);
					if (abstractMarkupData != null && abstractMarkupData2 != null)
					{
						if (abstractMarkupData.Parent == abstractMarkupData2.Parent)
						{
							InsertTagPair(tagPair, abstractMarkupData, abstractMarkupData2.IndexInParent - abstractMarkupData.IndexInParent);
						}
						else
						{
							InsertTagPair(tagPair, abstractMarkupData, abstractMarkupData.Parent.Count - abstractMarkupData.IndexInParent - 1);
						}
						if (!ShowDeleteOperations)
						{
							placeholderTag.RemoveFromParent();
							placeholderTag2.RemoveFromParent();
						}
					}
				}
				else if (pair.First.HasValue)
				{
					if (ShowDeleteOperations)
					{
						IRevisionMarker revisionMarker3 = CreateDeleteRevisionMarker();
						placeholderTag.Parent.Insert(placeholderTag.IndexInParent, revisionMarker3);
						placeholderTag.RemoveFromParent();
						revisionMarker3.Add(placeholderTag);
					}
					tagPair.StartTagRevisionProperties = revisionMarker.Properties;
					IAbstractMarkupData abstractMarkupData3 = FindTokenStart(pair.First.Value.MoveTargetSource);
					IAbstractMarkupData abstractMarkupData4 = placeholderTag2;
					if (abstractMarkupData3 != null)
					{
						if (abstractMarkupData3.Parent == abstractMarkupData4.Parent)
						{
							InsertTagPair(tagPair, abstractMarkupData3, abstractMarkupData4.IndexInParent - abstractMarkupData3.IndexInParent);
						}
						else
						{
							InsertTagPair(tagPair, abstractMarkupData3, abstractMarkupData3.Parent.Count - abstractMarkupData3.IndexInParent - 1);
						}
						if (ShowDeleteOperations)
						{
							placeholderTag2.RemoveFromParent();
							return;
						}
						placeholderTag.RemoveFromParent();
						placeholderTag2.RemoveFromParent();
					}
				}
				else
				{
					if (!pair.Second.HasValue)
					{
						return;
					}
					if (ShowDeleteOperations)
					{
						IRevisionMarker revisionMarker4 = CreateDeleteRevisionMarker();
						placeholderTag2.Parent.Insert(placeholderTag2.IndexInParent, revisionMarker4);
						placeholderTag2.RemoveFromParent();
						revisionMarker4.Add(placeholderTag2);
					}
					tagPair.EndTagRevisionProperties = revisionMarker.Properties;
					IAbstractMarkupData abstractMarkupData5 = placeholderTag;
					IAbstractMarkupData abstractMarkupData6 = FindTokenStart(pair.Second.Value.MoveTargetSource);
					if (abstractMarkupData5 != null && abstractMarkupData6 != null)
					{
						if (abstractMarkupData5.Parent == abstractMarkupData6.Parent)
						{
							InsertTagPair(tagPair, abstractMarkupData5, abstractMarkupData6.IndexInParent - abstractMarkupData5.IndexInParent);
						}
						else
						{
							InsertTagPair(tagPair, abstractMarkupData5, abstractMarkupData5.Parent.Count - abstractMarkupData5.IndexInParent - 1);
						}
						if (ShowDeleteOperations)
						{
							placeholderTag.RemoveFromParent();
							return;
						}
						placeholderTag.RemoveFromParent();
						placeholderTag2.RemoveFromParent();
					}
				}
				return;
			}
			if (pair.First.HasValue && ShowDeleteOperations)
			{
				IPlaceholderTag placeholderTag3 = ItemFactory.CreatePlaceholderTag(ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(tagPair.StartTagProperties.TagContent));
				placeholderTag3.TagProperties.TagId = tagPair.StartTagProperties.TagId;
				placeholderTag3.Properties.DisplayText = tagPair.StartTagProperties.DisplayText;
				if (tagPair.HasSubSegmentReferences)
				{
					placeholderTag3.AddSubSegmentReferences(tagPair.SubSegments);
				}
				IRevisionMarker revisionMarker5 = CreateDeleteRevisionMarker();
				revisionMarker5.Add(placeholderTag3);
				IAbstractMarkupData abstractMarkupData7 = FindTokenStart(pair.First.Value.MoveSourceTarget);
				if (abstractMarkupData7 != null)
				{
					abstractMarkupData7.Parent.Insert(abstractMarkupData7.IndexInParent, revisionMarker5);
				}
				else
				{
					MarkupDataSegment.First.Add(revisionMarker5);
				}
			}
			if (pair.Second.HasValue && ShowDeleteOperations)
			{
				IPlaceholderTag placeholderTag3 = ItemFactory.CreatePlaceholderTag(ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(tagPair.EndTagProperties.TagContent));
				placeholderTag3.TagProperties.TagId = tagPair.StartTagProperties.TagId;
				placeholderTag3.Properties.DisplayText = tagPair.EndTagProperties.DisplayText;
				if (tagPair.HasSubSegmentReferences)
				{
					placeholderTag3.AddSubSegmentReferences(tagPair.SubSegments);
				}
				IRevisionMarker revisionMarker5 = CreateDeleteRevisionMarker();
				revisionMarker5.Add(placeholderTag3);
				IAbstractMarkupData abstractMarkupData7 = FindTokenStart(pair.Second.Value.MoveSourceTarget);
				if (abstractMarkupData7 != null)
				{
					abstractMarkupData7.Parent.Insert(abstractMarkupData7.IndexInParent, revisionMarker5);
				}
				else
				{
					MarkupDataSegment.First.Add(revisionMarker5);
				}
			}
		}

		private List<EditDistanceItem> HandleAddedTagPair(ClassifiedTagPairInfo info)
		{
			Pair<EditDistanceItem?, EditDistanceItem?> pair = new Pair<EditDistanceItem?, EditDistanceItem?>(info.Start, info.End);
			int tokenIndex = pair.First?.Target ?? pair.Second.Value.Target;
			bool isStartTag;
			ITagPair tagPair = FindPairedTag((MarkupDataIs == MarkupDataAs.ToContent) ? MarkupDataSegment : _markupDataToSegment, tokenIndex, out isStartTag);
			if (tagPair == null)
			{
				return null;
			}
			if (MarkupDataIs == MarkupDataAs.ToContent)
			{
				IRevisionMarker revisionMarker = CreateInsertRevisionMarker();
				tagPair.StartTagRevisionProperties = revisionMarker.Properties;
				tagPair.EndTagRevisionProperties = revisionMarker.Properties;
			}
			else
			{
				ITagPair insertableTagPair = (ITagPair)tagPair.Clone();
				IRevisionMarker revisionMarker2 = CreateInsertRevisionMarker();
				insertableTagPair.StartTagRevisionProperties = revisionMarker2.Properties;
				insertableTagPair.EndTagRevisionProperties = revisionMarker2.Properties;
				IAbstractMarkupData abstractMarkupData = null;
				if (pair.First.HasValue)
				{
					abstractMarkupData = FindTokenStart(pair.First.Value.Source);
				}
				IAbstractMarkupData abstractMarkupData2 = null;
				if (pair.Second.HasValue)
				{
					abstractMarkupData2 = FindTokenStart(pair.Second.Value.Source);
				}
				if (abstractMarkupData != null)
				{
					int num = abstractMarkupData.Parent.Count;
					if (abstractMarkupData2 != null && abstractMarkupData.Parent == abstractMarkupData2.Parent)
					{
						num = abstractMarkupData2.IndexInParent;
					}
					List<EditDistanceItem> result;
					if (num - abstractMarkupData.IndexInParent == 0 && info.SubitemsCount > 0 && InsertOperationsOnly(info.Subitems))
					{
						result = info.Subitems;
						info.Subitems.ForEach(delegate(EditDistanceItem item)
						{
							WrapTokenContentInRevisionMarker(FindTokenMarker(insertableTagPair, item.Target), CreateInsertRevisionMarker());
						});
					}
					else
					{
						insertableTagPair.Clear();
						result = null;
					}
					InsertTagPair(insertableTagPair, abstractMarkupData, num - abstractMarkupData.IndexInParent);
					return result;
				}
				InsertTagPair(insertableTagPair, null, 0);
			}
			return null;
		}

		private static bool InsertOperationsOnly(IEnumerable<EditDistanceItem> list)
		{
			return list.All((EditDistanceItem item) => item.Operation == EditOperation.Insert);
		}

		private void InsertTagPair(ITagPair tagPair, IAbstractMarkupData fromLocation, int itemsToWrap)
		{
			if (fromLocation == null)
			{
				MarkupDataSegment.First.Add(tagPair);
				return;
			}
			fromLocation.Parent.Insert(fromLocation.IndexInParent, tagPair);
			List<IAbstractMarkupData> list = new List<IAbstractMarkupData>();
			for (int i = 0; i < itemsToWrap; i++)
			{
				list.Add(fromLocation.Parent[fromLocation.IndexInParent + i]);
			}
			foreach (IAbstractMarkupData item in list)
			{
				item.RemoveFromParent();
				tagPair.Add(item);
			}
		}

		private void HandleDeletedTagPair(ClassifiedTagPairInfo info)
		{
			if (!ShowDeleteOperations)
			{
				return;
			}
			Pair<EditDistanceItem?, EditDistanceItem?> pair = new Pair<EditDistanceItem?, EditDistanceItem?>(info.Start, info.End);
			int tokenIndex = pair.First?.Source ?? pair.Second.Value.Source;
			bool isStartTag;
			ITagPair tagPair = FindPairedTag((MarkupDataIs == MarkupDataAs.FromContent) ? MarkupDataSegment : _markupDataFromSegment, tokenIndex, out isStartTag);
			if (tagPair == null)
			{
				return;
			}
			if (MarkupDataIs == MarkupDataAs.FromContent)
			{
				IRevisionMarker revisionMarker = CreateDeleteRevisionMarker();
				tagPair.StartTagRevisionProperties = revisionMarker.Properties;
				tagPair.EndTagRevisionProperties = revisionMarker.Properties;
				return;
			}
			ITagPair tagPair2 = (ITagPair)tagPair.Clone();
			tagPair2.Clear();
			IRevisionMarker revisionMarker2 = CreateInsertRevisionMarker();
			tagPair2.StartTagRevisionProperties = revisionMarker2.Properties;
			tagPair2.EndTagRevisionProperties = revisionMarker2.Properties;
			IAbstractMarkupData abstractMarkupData = null;
			if (pair.First.HasValue)
			{
				abstractMarkupData = FindTokenStart(pair.First.Value.Target);
			}
			IAbstractMarkupData abstractMarkupData2 = null;
			if (pair.Second.HasValue)
			{
				abstractMarkupData2 = FindTokenStart(pair.Second.Value.Target);
			}
			if (abstractMarkupData == null)
			{
				InsertTagPair(tagPair2, null, 0);
				return;
			}
			int num = abstractMarkupData2?.IndexInParent ?? MarkupDataSegment.First.Count;
			InsertTagPair(tagPair2, abstractMarkupData, num - abstractMarkupData.IndexInParent);
		}

		private void HandleMove(EditDistanceItem item)
		{
			HandleDelete(item);
			EditDistanceItem editDistanceItem = default(EditDistanceItem);
			editDistanceItem.Source = item.MoveTargetSource;
			editDistanceItem.Target = item.Target;
			editDistanceItem.Costs = item.Costs;
			editDistanceItem.Resolution = item.Resolution;
			editDistanceItem.Operation = EditOperation.Insert;
			EditDistanceItem item2 = editDistanceItem;
			HandleInsert(item2);
		}

		private void HandleChange(EditDistanceItem item)
		{
			if (!ShowDeleteOperations)
			{
				if (MarkupDataIs == MarkupDataAs.FromContent)
				{
					FindTokenMarker(item.Source).Clear();
				}
				HandleInsert(item);
				return;
			}
			IRevisionMarker revisionMarker = CreateInsertRevisionMarker();
			IRevisionMarker revisionMarker2 = CreateDeleteRevisionMarker();
			switch (MarkupDataIs)
			{
			case MarkupDataAs.FromContent:
			{
				IOtherMarker otherMarker2 = FindTokenMarker(MarkupDataSegment.First, item.Source);
				WrapTokenContentInRevisionMarker(otherMarker2, revisionMarker2);
				CloneItemsTo(FindTokenMarker(_markupDataToSegment.First, item.Target), revisionMarker);
				otherMarker2?.Parent.Insert(otherMarker2.IndexInParent + 1, revisionMarker);
				break;
			}
			case MarkupDataAs.ToContent:
			{
				IOtherMarker otherMarker = FindTokenMarker(MarkupDataSegment.First, item.Target);
				WrapTokenContentInRevisionMarker(otherMarker, revisionMarker);
				CloneItemsTo(FindTokenMarker(_markupDataFromSegment.First, item.Source), revisionMarker2);
				otherMarker?.Parent.Insert(otherMarker.IndexInParent, revisionMarker2);
				break;
			}
			}
		}

		private void HandleChangeSequence(EditDistanceChangeSequence sequence)
		{
			if (!ShowThisOperation(sequence.Items[0]))
			{
				return;
			}
			if (!ShowDeleteOperations)
			{
				if (MarkupDataIs == MarkupDataAs.FromContent)
				{
					foreach (EditDistanceItem item in sequence.Items)
					{
						FindTokenMarker(item.Source).Clear();
					}
				}
				foreach (EditDistanceItem item2 in sequence.Items)
				{
					HandleInsert(item2);
				}
				return;
			}
			switch (MarkupDataIs)
			{
			case MarkupDataAs.FromContent:
			{
				IOtherMarker otherMarker2 = null;
				using (List<EditDistanceItem>.Enumerator enumerator = sequence.Items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						otherMarker2 = FindTokenMarker(tokenNumber: enumerator.Current.Source, lookIn: MarkupDataSegment.First);
						WrapTokenContentInRevisionMarker(otherMarker2, CreateDeleteRevisionMarker());
					}
				}
				IRevisionMarker revisionMarker2 = CreateInsertRevisionMarker();
				using (List<EditDistanceItem>.Enumerator enumerator = sequence.Items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CloneItemsTo(FindTokenMarker(tokenNumber: enumerator.Current.Target, lookIn: _markupDataToSegment.First), revisionMarker2);
					}
				}
				otherMarker2?.Parent.Insert(otherMarker2.IndexInParent + 1, revisionMarker2);
				break;
			}
			case MarkupDataAs.ToContent:
			{
				IOtherMarker otherMarker = FindTokenMarker(MarkupDataSegment.First, sequence.Items[0].Target);
				using (List<EditDistanceItem>.Enumerator enumerator = sequence.Items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WrapTokenContentInRevisionMarker(FindTokenMarker(tokenNumber: enumerator.Current.Target, lookIn: MarkupDataSegment.First), CreateInsertRevisionMarker());
					}
				}
				IRevisionMarker revisionMarker = CreateDeleteRevisionMarker();
				using (List<EditDistanceItem>.Enumerator enumerator = sequence.Items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CloneItemsTo(FindTokenMarker(tokenNumber: enumerator.Current.Source, lookIn: _markupDataFromSegment.First), revisionMarker);
					}
				}
				otherMarker?.Parent.Insert(otherMarker.IndexInParent, revisionMarker);
				break;
			}
			}
		}

		private static void CloneItemsTo(IOtherMarker token, IAbstractMarkupDataContainer container)
		{
			if (token != null)
			{
				((IOtherMarker)token.Clone()).MoveAllItemsTo(container);
			}
		}

		private Pair<ISegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>>> CreateTokenizedMarkupDataSegment(Segment linguaSegment)
		{
			MarkupDataSegmentBuilder markupDataSegmentBuilder = new MarkupDataSegmentBuilder(ItemFactory);
			markupDataSegmentBuilder.TreatAllTagsAsStandalone = false;
			markupDataSegmentBuilder.VisitLinguaSegment(linguaSegment);
			ISegment result = markupDataSegmentBuilder.Result;
			MarkupDataTokenApplier markupDataTokenApplier = new MarkupDataTokenApplier(ItemFactory);
			markupDataTokenApplier.Execute(result, linguaSegment);
			return new Pair<ISegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>>>(result, markupDataTokenApplier.TokenRanges);
		}

		private void HandleDelete(EditDistanceItem item)
		{
			if (!ShowDeleteOperations)
			{
				return;
			}
			IRevisionMarker revisionMarker = CreateDeleteRevisionMarker();
			switch (MarkupDataIs)
			{
			case MarkupDataAs.FromContent:
			{
				IOtherMarker otherMarker3 = FindTokenMarker(MarkupDataSegment.First, item.Source);
				if (otherMarker3 != null)
				{
					WrapTokenContentInRevisionMarker(otherMarker3, revisionMarker);
				}
				else
				{
					ApplyRevisionMarkerToTag(_markupDataFromSegment, item.Source, revisionMarker);
				}
				break;
			}
			case MarkupDataAs.ToContent:
			{
				IOtherMarker otherMarker = FindTokenMarker(_markupDataFromSegment.First, item.Source);
				if (otherMarker != null)
				{
					CloneItemsTo(otherMarker, revisionMarker);
					IOtherMarker otherMarker2 = FindTokenMarker(MarkupDataSegment.First, item.Target);
					if (otherMarker2 != null)
					{
						otherMarker2.Parent.Insert(otherMarker2.IndexInParent, revisionMarker);
					}
					else
					{
						MarkupDataSegment.First.Add(revisionMarker);
					}
				}
				break;
			}
			}
		}

		private static void ApplyRevisionMarkerToTag(Pair<ISegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>>> markupDataSegmentInfo, int tokenIndex, IRevisionMarker revisionMarker)
		{
			bool isStartTag;
			ITagPair tagPair = FindPairedTag(markupDataSegmentInfo, tokenIndex, out isStartTag);
			if (tagPair != null)
			{
				if (isStartTag)
				{
					tagPair.StartTagRevisionProperties = revisionMarker.Properties;
				}
				else
				{
					tagPair.EndTagRevisionProperties = revisionMarker.Properties;
				}
			}
		}

		private static ITagPair FindPairedTag(Pair<ISegment, IList<Pair<IAbstractMarkupData, IAbstractMarkupData>>> markupDataSegmentInfo, int tokenIndex, out bool isStartTag)
		{
			Pair<IAbstractMarkupData, IAbstractMarkupData> pair = markupDataSegmentInfo.Second[tokenIndex];
			if (pair == null)
			{
				isStartTag = false;
				return null;
			}
			ITagPair tagPair = pair.Second.Parent as ITagPair;
			if (tagPair != null && tagPair.Parent == pair.First.Parent)
			{
				isStartTag = true;
				return tagPair;
			}
			ITagPair tagPair2 = pair.First.Parent as ITagPair;
			if (tagPair2 != null && tagPair2.Parent == pair.Second.Parent)
			{
				isStartTag = false;
				return tagPair2;
			}
			isStartTag = false;
			return null;
		}

		private IRevisionMarker CreateDeleteRevisionMarker()
		{
			IRevisionProperties properties = ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Delete);
			return ItemFactory.CreateRevision(properties);
		}

		private void HandleInsert(EditDistanceItem item)
		{
			IRevisionMarker revisionMarker = CreateInsertRevisionMarker();
			switch (MarkupDataIs)
			{
			case MarkupDataAs.FromContent:
			{
				CloneItemsTo(FindTokenMarker(_markupDataToSegment.First, item.Target), revisionMarker);
				IAbstractMarkupData abstractMarkupData = FindTokenStart(item.Source);
				if (abstractMarkupData != null)
				{
					abstractMarkupData.Parent.Insert(abstractMarkupData.IndexInParent, revisionMarker);
				}
				else
				{
					MarkupDataSegment.First.Add(revisionMarker);
				}
				break;
			}
			case MarkupDataAs.ToContent:
				WrapTokenContentInRevisionMarker(FindTokenMarker(MarkupDataSegment.First, item.Target), revisionMarker);
				break;
			}
		}

		private static void WrapTokenContentInRevisionMarker(IAbstractMarkupDataContainer tokenMarker, IRevisionMarker revisionMarker)
		{
			if (tokenMarker != null)
			{
				tokenMarker.MoveAllItemsTo(revisionMarker);
				tokenMarker.Add(revisionMarker);
			}
		}

		private IRevisionMarker CreateInsertRevisionMarker()
		{
			IRevisionProperties properties = ItemFactory.PropertiesFactory.CreateRevisionProperties(RevisionType.Insert);
			return ItemFactory.CreateRevision(properties);
		}

		private IAbstractMarkupData FindTokenStart(int tokenNumber)
		{
			IAbstractMarkupData abstractMarkupData = FindTokenMarker(tokenNumber);
			return abstractMarkupData ?? GetTokenRange(tokenNumber).First;
		}

		private IOtherMarker FindTokenMarker(int tokenNumber)
		{
			return FindTokenMarker(MarkupDataSegment.First, tokenNumber);
		}

		private Pair<IAbstractMarkupData, IAbstractMarkupData> GetTokenRange(int tokenNumber)
		{
			if (tokenNumber < 0 || tokenNumber >= MarkupDataSegment.Second.Count)
			{
				return new Pair<IAbstractMarkupData, IAbstractMarkupData>(null, null);
			}
			return MarkupDataSegment.Second[tokenNumber];
		}

		private static IOtherMarker FindTokenMarker(IAbstractMarkupDataContainer lookIn, int tokenNumber)
		{
			return lookIn.Find(delegate(IAbstractMarkupData item)
			{
				IOtherMarker otherMarker = item as IOtherMarker;
				return otherMarker != null && otherMarker.Id == tokenNumber.ToString();
			}) as IOtherMarker;
		}
	}
}
