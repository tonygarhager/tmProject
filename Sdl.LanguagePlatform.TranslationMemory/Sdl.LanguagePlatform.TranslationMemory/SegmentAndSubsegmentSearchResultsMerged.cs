using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class SegmentAndSubsegmentSearchResultsMerged : SearchResultsMerged
	{
		private readonly Dictionary<string, SubsegmentSearchResultDedupInfo> _subsegmentSearchResultDedupInfoDictionary = new Dictionary<string, SubsegmentSearchResultDedupInfo>();

		public List<SubsegmentSearchResultsCollection> SubsegmentSearchResultsCollectionList
		{
			get;
		} = new List<SubsegmentSearchResultsCollection>();


		public SegmentAndSubsegmentSearchResultsMerged()
		{
		}

		public SegmentAndSubsegmentSearchResultsMerged(SegmentAndSubsegmentSearchResults results)
			: base(results)
		{
			if (results.SubsegmentSearchResultsCollection != null)
			{
				MergeSubsegmentSearchResultsCollection(results.SubsegmentSearchResultsCollection, removeDuplicates: true);
			}
		}

		private void MergeSubsegmentSearchResultsCollection(SubsegmentSearchResultsCollection other, bool removeDuplicates)
		{
			foreach (SubsegmentSearchResults item in other.ResultsPerFragment)
			{
				string sourceFeatureString = SourceFeatureString(other, item);
				List<SubsegmentSearchResult> list = new List<SubsegmentSearchResult>();
				foreach (SubsegmentSearchResult item2 in item.OrderBy((SubsegmentSearchResult a) => (int)a.MatchType))
				{
					string key = DedupKey(item2.MatchType.ToString(), sourceFeatureString, item2.TranslationFeatureString);
					if (_subsegmentSearchResultDedupInfoDictionary.TryGetValue(key, out SubsegmentSearchResultDedupInfo value))
					{
						if (item2.ScoringResult.Match > value.BestSubsegmentSearchResult.ScoringResult.Match)
						{
							item2.Repetitions += value.BestSubsegmentSearchResult.Repetitions;
							if (removeDuplicates)
							{
								value.SubsegmentSearchResults.Remove(value.BestSubsegmentSearchResult);
							}
							value.BestSubsegmentSearchResult = item2;
							value.SubsegmentSearchResults = item;
						}
						else
						{
							value.BestSubsegmentSearchResult.Repetitions++;
							if (removeDuplicates)
							{
								list.Add(item2);
							}
						}
					}
					else
					{
						value = new SubsegmentSearchResultDedupInfo
						{
							BestSubsegmentSearchResult = item2,
							SubsegmentSearchResults = item
						};
						_subsegmentSearchResultDedupInfoDictionary.Add(key, value);
					}
				}
				foreach (SubsegmentSearchResult item3 in list)
				{
					item.Remove(item3);
				}
			}
			SubsegmentSearchResultsCollectionList.Add(other);
		}

		private static string DedupKey(string type, string sourceFeatureString, string translationFeatureString)
		{
			return type + " " + sourceFeatureString + " " + translationFeatureString;
		}

		private static string SourceFeatureString(SubsegmentSearchResultsCollection collection, SubsegmentSearchResults results)
		{
			List<string> range = collection.SourceSegmentFeatures.GetRange(results.SourceFeatureStartIndex, results.SourceFeatureCount);
			return string.Join("|", range.ToArray());
		}

		public void Merge(SegmentAndSubsegmentSearchResults other, bool removeDuplicates, int cascadeEntryIndex)
		{
			Merge((SearchResults)other, removeDuplicates, cascadeEntryIndex);
			if (other.SubsegmentSearchResultsCollection != null)
			{
				MergeSubsegmentSearchResultsCollection(other.SubsegmentSearchResultsCollection, removeDuplicates);
			}
		}
	}
}
