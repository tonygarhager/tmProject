using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class SegmentBuilderWithTagApplier
	{
		private readonly MarkupDataSegmentBuilder _segmentBuilder;

		private MarkupDataTagApplier _tagMatcher;

		private readonly IDocumentItemFactory _itemFactory;

		private readonly IQuickTags _quickTags;

		private readonly bool _excludeTagsInLockedContentText;

		public bool TagsRemoved => _tagMatcher.UnmatchedItems.Count > 0;

		public bool PenaltyApplied
		{
			get;
			set;
		}

		public ISegment Result => _segmentBuilder.Result;

		public SegmentBuilderWithTagApplier(IDocumentItemFactory itemFactory, IQuickTags quickTags)
			: this(itemFactory, quickTags, excludeTagsInLockedContentText: true)
		{
		}

		public SegmentBuilderWithTagApplier(IDocumentItemFactory itemFactory, IQuickTags quickTags, bool excludeTagsInLockedContentText)
		{
			_itemFactory = itemFactory;
			_quickTags = quickTags;
			_segmentBuilder = new MarkupDataSegmentBuilder(itemFactory);
			_tagMatcher = new MarkupDataTagApplier(excludeTagsInLockedContentText);
			_excludeTagsInLockedContentText = excludeTagsInLockedContentText;
		}

		public void ApplyTags(SearchResult searchResult, ISegment segmentToSearchIn, IAbstractMarkupDataContainer documentContent, Penalty memoryTagDeletedPenalty)
		{
			PenaltyApplied = false;
			if (searchResult == null)
			{
				throw new ArgumentNullException("searchResult");
			}
			Segment targetSegment;
			if (searchResult.TranslationProposal != null)
			{
				targetSegment = searchResult.TranslationProposal.TargetSegment;
			}
			else
			{
				if (searchResult.MemoryTranslationUnit == null)
				{
					throw new ArgumentNullException("MemoryTranslationUnit");
				}
				targetSegment = searchResult.MemoryTranslationUnit.TargetSegment;
			}
			ApplyTags(targetSegment, segmentToSearchIn, documentContent);
			if (TagsRemoved && memoryTagDeletedPenalty != null && !HasTagPenaltiesApplied(searchResult.ScoringResult))
			{
				searchResult.ScoringResult.ApplyPenalty(memoryTagDeletedPenalty);
				PenaltyApplied = true;
			}
		}

		public void ApplyTags(Segment segment, ISegment segmentToSearchIn, IAbstractMarkupDataContainer documentContent)
		{
			if (segment == null)
			{
				throw new ArgumentNullException("segment");
			}
			_segmentBuilder.VisitLinguaSegment(segment);
			_tagMatcher = new MarkupDataTagApplier(_excludeTagsInLockedContentText)
			{
				DocumentContent = documentContent,
				SegmentToSearchIn = segmentToSearchIn
			};
			if (!_tagMatcher.ApplyDocumentTags(_segmentBuilder.Result))
			{
				ApplyQuickTags();
				RemoveUnmatchedTags();
			}
		}

		public void ApplyQuickTags(MarkupDataTagApplier tagMatcher)
		{
			_tagMatcher = tagMatcher;
			ApplyQuickTags();
		}

		private void ApplyQuickTags()
		{
			new MarkupDataQuickTagApplier(_quickTags).ApplyQuickTags(_tagMatcher.UnmatchedItems);
		}

		public void RemoveUnmatchedTags()
		{
			if (_tagMatcher != null && _tagMatcher.UnmatchedItems.Count != 0)
			{
				List<IAbstractMarkupData> list = new List<IAbstractMarkupData>();
				foreach (IAbstractMarkupData unmatchedItem in _tagMatcher.UnmatchedItems)
				{
					if (unmatchedItem is ILockedContent)
					{
						list.Add(unmatchedItem);
					}
				}
				foreach (IAbstractMarkupData item in list)
				{
					_tagMatcher.UnmatchedItems.Remove(item);
				}
				_tagMatcher.RemoveUnmatchedContent();
			}
		}

		private static bool HasTagPenaltiesApplied(ScoringResult scoringResult)
		{
			if (scoringResult?.AppliedPenalties == null || scoringResult.AppliedPenalties.Count == 0)
			{
				return false;
			}
			return scoringResult.AppliedPenalties.Find((AppliedPenalty penalty) => penalty.Type == PenaltyType.TagMismatch || penalty.Type == PenaltyType.MemoryTagsDeleted) != null;
		}
	}
}
