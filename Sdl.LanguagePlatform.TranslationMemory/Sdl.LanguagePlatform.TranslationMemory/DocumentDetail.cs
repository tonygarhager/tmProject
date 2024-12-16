using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class DocumentDetail
	{
		[DataMember]
		public WordCounts SourceWordCounts
		{
			get;
			set;
		}

		[DataMember]
		public List<Placeable> DocumentPlaceables
		{
			get;
			set;
		}

		[DataMember]
		public Segment SourceSegment
		{
			get;
			set;
		}

		[DataMember]
		public long SourceHash
		{
			get;
			set;
		}
	}
}
