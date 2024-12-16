using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	[DataContract]
	public class TermFinderResult
	{
		[DataMember]
		public List<SegmentRange> MatchingRanges
		{
			get;
			set;
		}

		[DataMember]
		public int Score
		{
			get;
			set;
		}
	}
}
