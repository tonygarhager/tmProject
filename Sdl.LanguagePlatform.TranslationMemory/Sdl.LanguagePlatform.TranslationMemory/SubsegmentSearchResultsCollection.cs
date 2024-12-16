using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SubsegmentSearchResultsCollection
	{
		[DataMember]
		public Segment SourceSegment
		{
			get;
			set;
		}

		[DataMember]
		public List<string> SourceSegmentFeatures
		{
			get;
			set;
		}

		[DataMember]
		public List<SubsegmentSearchResults> ResultsPerFragment
		{
			get;
			set;
		}
	}
}
