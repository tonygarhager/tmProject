using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public interface IFuzzyIndex
	{
		void Add(int key, IntFeatureVector fv);

		void Delete(int key);

		List<Hit> Search(IntFeatureVector fv, int maxResults, int minScore, int lastKey, ScoringMethod scoringMethod, Predicate<int> validateItemCallback, bool descendingOrder);
	}
}
