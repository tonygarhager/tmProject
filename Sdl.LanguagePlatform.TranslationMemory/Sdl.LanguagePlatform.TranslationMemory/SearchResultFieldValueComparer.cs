using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class SearchResultFieldValueComparer : IFieldValueComparer<SearchResult>
	{
		public int Compare(SearchResult a, SearchResult b, string fieldName)
		{
			if (a == null || b == null || string.IsNullOrEmpty(fieldName))
			{
				throw new ArgumentNullException();
			}
			string text = fieldName.ToLower(CultureInfo.InvariantCulture);
			int num;
			switch (text)
			{
			case "sco":
				num = a.ScoringResult.Match - b.ScoringResult.Match;
				if (num == 0 && a.ScoringResult.IdContextMatch != b.ScoringResult.IdContextMatch)
				{
					num = (a.ScoringResult.IdContextMatch ? 1 : (-1));
				}
				if (num == 0 && a.ScoringResult.TargetSegmentDiffers != b.ScoringResult.TargetSegmentDiffers)
				{
					num = ((!a.ScoringResult.TargetSegmentDiffers) ? 1 : (-1));
				}
				if (num == 0)
				{
					num = a.MatchingPlaceholderTokens - b.MatchingPlaceholderTokens;
				}
				if (num == 0)
				{
					num = a.ScoringResult.TextContextMatch - b.ScoringResult.TextContextMatch;
				}
				if (num == 0 && a.ScoringResult.IsStructureContextMatch != b.ScoringResult.IsStructureContextMatch)
				{
					num = (a.ScoringResult.IsStructureContextMatch ? 1 : (-1));
				}
				if (num == 0 && a.ScoringResult.MemoryTagsDeleted != b.ScoringResult.MemoryTagsDeleted)
				{
					num = ((!a.ScoringResult.MemoryTagsDeleted) ? 1 : (-1));
				}
				if (num == 0 && a.ScoringResult.TextReplacements != b.ScoringResult.TextReplacements)
				{
					num = b.ScoringResult.TextReplacements.CompareTo(a.ScoringResult.TextReplacements);
				}
				if (num == 0 && a.ScoringResult.PlaceableFormatChanges != b.ScoringResult.PlaceableFormatChanges)
				{
					num = b.ScoringResult.PlaceableFormatChanges.CompareTo(a.ScoringResult.PlaceableFormatChanges);
				}
				break;
			case "usc":
				num = a.MemoryTranslationUnit.SystemFields.UseCount - b.MemoryTranslationUnit.SystemFields.UseCount;
				break;
			case "usd":
				num = DateTime.Compare(a.MemoryTranslationUnit.SystemFields.UseDate, b.MemoryTranslationUnit.SystemFields.UseDate);
				break;
			case "crd":
				num = DateTime.Compare(a.MemoryTranslationUnit.SystemFields.CreationDate, b.MemoryTranslationUnit.SystemFields.CreationDate);
				break;
			case "chd":
				num = DateTime.Compare(a.MemoryTranslationUnit.SystemFields.ChangeDate, b.MemoryTranslationUnit.SystemFields.ChangeDate);
				break;
			default:
			{
				AppliedPenalty appliedPenalty = a.ScoringResult.FindAppliedFilter(text);
				AppliedPenalty appliedPenalty2 = b.ScoringResult.FindAppliedFilter(text);
				num = (((appliedPenalty != null || appliedPenalty2 != null) && (appliedPenalty == null || appliedPenalty2 == null)) ? ((appliedPenalty == null) ? 1 : (-1)) : 0);
				break;
			}
			}
			return num;
		}
	}
}
