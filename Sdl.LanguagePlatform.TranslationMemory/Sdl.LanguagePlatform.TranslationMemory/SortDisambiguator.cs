using System;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class SortDisambiguator
	{
		public delegate int Disambiguator(SearchResult a, SearchResult b);

		private readonly SortSpecification _sortCriterium;

		private readonly Disambiguator _disambiguator;

		public SortDisambiguator(SortSpecification sortCriterium, Disambiguator disambiguator)
		{
			_sortCriterium = sortCriterium;
			_disambiguator = disambiguator;
		}

		public int Disambiguate(int comparisonResult, SearchResult a, SearchResult b)
		{
			bool flag = false;
			foreach (SortCriterium criterion in _sortCriterium.Criteria)
			{
				if (string.Compare(criterion.FieldName, "sco", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return comparisonResult;
			}
			if (!RequiresDisambiguation(a, b))
			{
				return comparisonResult;
			}
			return _disambiguator(a, b);
		}

		private static bool RequiresDisambiguation(SearchResult a, SearchResult b)
		{
			if (a.ScoringResult.Match != b.ScoringResult.Match)
			{
				return false;
			}
			if (a.MatchingPlaceholderTokens != b.MatchingPlaceholderTokens)
			{
				return false;
			}
			if (a.ScoringResult.TextContextMatch != b.ScoringResult.TextContextMatch)
			{
				return false;
			}
			if (a.ScoringResult.IsStructureContextMatch != b.ScoringResult.IsStructureContextMatch)
			{
				return false;
			}
			return a.ScoringResult.MemoryTagsDeleted == b.ScoringResult.MemoryTagsDeleted;
		}
	}
}
