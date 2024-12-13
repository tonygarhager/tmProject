using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// A converter helper class that converts from SearchResults to Subsegmentsearch results
	/// </summary>
	public static class TranslationMemorySearchResultConverters
	{
		/// <summary>
		/// Converts the given SearchResults array to an SegmentAndSubsegmentSearchResults array
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
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
