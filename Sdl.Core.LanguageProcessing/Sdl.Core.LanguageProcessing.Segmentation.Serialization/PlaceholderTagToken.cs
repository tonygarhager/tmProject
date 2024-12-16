using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.LanguageProcessing.Segmentation.Serialization
{
	internal class PlaceholderTagToken : Token
	{
		public SegmentationHint SegmentationHint
		{
			get;
		}

		public bool IsWordStop
		{
			get;
		}

		public PlaceholderTagToken(SegmentationHint segmentationHint, bool isWordStop)
		{
			SegmentationHint = segmentationHint;
			IsWordStop = isWordStop;
		}
	}
}
