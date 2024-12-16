using Sdl.LanguagePlatform.Core;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Lingua
{
	[DataContract]
	public class AlignedRange
	{
		[DataMember]
		public SegmentRange Source
		{
			get;
			set;
		}

		[DataMember]
		public SegmentRange Target
		{
			get;
			set;
		}

		public AlignedRange(SegmentRange source, SegmentRange target)
		{
			Source = source;
			Target = target;
		}
	}
}
