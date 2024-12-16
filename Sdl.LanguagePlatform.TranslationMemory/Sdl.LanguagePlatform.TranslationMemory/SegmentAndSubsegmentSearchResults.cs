using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SegmentAndSubsegmentSearchResults : SearchResults
	{
		[DataMember]
		public List<SubsegmentSearchResults> ResultsPerFragment => SubsegmentSearchResultsCollection?.ResultsPerFragment;

		[DataMember]
		public SubsegmentSearchResultsCollection SubsegmentSearchResultsCollection
		{
			get;
		}

		public SegmentAndSubsegmentSearchResults()
		{
		}

		public SegmentAndSubsegmentSearchResults(SearchResults results, SubsegmentSearchResultsCollection subsegResults)
		{
			CopyFrom(results);
			SubsegmentSearchResultsCollection = subsegResults;
		}
	}
}
