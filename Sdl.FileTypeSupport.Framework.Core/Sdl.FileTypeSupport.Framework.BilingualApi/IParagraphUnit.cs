using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IParagraphUnit : ICloneable
	{
		IParagraphUnitProperties Properties
		{
			get;
			set;
		}

		bool IsStructure
		{
			get;
		}

		IParagraph Source
		{
			get;
			set;
		}

		IParagraph Target
		{
			get;
			set;
		}

		IEnumerable<ISegmentPair> SegmentPairs
		{
			get;
		}

		ISegment GetSourceSegment(SegmentId id);

		ISegment GetTargetSegment(SegmentId id);

		ISegmentPair GetSegmentPair(SegmentId id);

		Location GetSourceSegmentLocation(SegmentId id);

		Location GetTargetSegmentLocation(SegmentId id);
	}
}
