using Sdl.Core.Bcm.BcmModel.Common;

namespace Sdl.Core.Bcm.BcmModel
{
	public class SegmentPair : ExtensionDataContainer, ICloneable<SegmentPair>
	{
		public Segment Source
		{
			get;
			set;
		}

		public Segment Target
		{
			get;
			set;
		}

		public SegmentPair(Segment source, Segment target)
		{
			Source = source;
			Target = target;
		}

		public SegmentPair Clone()
		{
			return new SegmentPair(Source.Clone(), Target.Clone());
		}
	}
}
