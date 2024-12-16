using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SearchResultsMerged : SearchResults
	{
		[DataMember]
		public Dictionary<int, DocumentDetail> DocumentDetails
		{
			get;
			set;
		}

		public SearchResultsMerged()
			: base(null)
		{
			DocumentDetails = new Dictionary<int, DocumentDetail>();
		}

		public SearchResultsMerged(SearchResults searchResults)
			: this()
		{
			CopyFromSearchResults(searchResults);
		}

		public void Merge(SearchResults other, bool removeDuplicates, int cascadeEntryIndex)
		{
			if (!DocumentDetails.ContainsKey(cascadeEntryIndex))
			{
				DocumentDetail value = new DocumentDetail
				{
					DocumentPlaceables = other.DocumentPlaceables,
					SourceHash = other.SourceHash,
					SourceSegment = other.SourceSegment,
					SourceWordCounts = other.SourceWordCounts
				};
				DocumentDetails.Add(cascadeEntryIndex, value);
			}
			Merge(other, removeDuplicates);
		}

		public void CopyFromSearchResults(SearchResults other)
		{
			base.DocumentPlaceables = other.DocumentPlaceables;
			base.SourceHash = other.SourceHash;
			base.SourceSegment = other.SourceSegment.Duplicate();
			base.SourceWordCounts = other.SourceWordCounts;
			base.Results = other.Results;
			base.MultipleTranslations = other.MultipleTranslations;
		}
	}
}
