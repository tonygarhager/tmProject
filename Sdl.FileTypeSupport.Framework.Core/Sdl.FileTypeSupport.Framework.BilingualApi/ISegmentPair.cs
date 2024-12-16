using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface ISegmentPair
	{
		ISegment Source
		{
			get;
		}

		ISegment Target
		{
			get;
		}

		ISegmentPairProperties Properties
		{
			get;
			set;
		}
	}
}
