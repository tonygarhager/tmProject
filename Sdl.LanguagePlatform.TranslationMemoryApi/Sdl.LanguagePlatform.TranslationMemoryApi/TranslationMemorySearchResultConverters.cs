using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public static class TranslationMemorySearchResultConverters
	{
		public static SegmentAndSubsegmentSearchResults[] ToSegmentAndSubsegmentSearchResults(SearchResults[] results)
		{
			if (results == null)
			{
				return null;
			}
			SegmentAndSubsegmentSearchResults[] array = new SegmentAndSubsegmentSearchResults[results.Length];
			for (int i = 0; i < results.Length; i++)
			{
				if (results[i] != null)
				{
					array[i] = new SegmentAndSubsegmentSearchResults(results[i], null);
				}
			}
			return array;
		}
	}
}
