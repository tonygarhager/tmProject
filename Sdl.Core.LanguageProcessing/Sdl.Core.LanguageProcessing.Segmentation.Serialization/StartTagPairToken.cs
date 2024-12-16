using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Core.LanguageProcessing.Segmentation.Serialization
{
	internal class StartTagPairToken : Token
	{
		public SegmentationHint SegmentationHint
		{
			get;
		}

		public bool IsWordStop
		{
			get;
		}

		public bool CanHide
		{
			get;
		}

		public StartTagPairToken(SegmentationHint segmentationHint, bool isWordStop, bool canHide)
		{
			SegmentationHint = segmentationHint;
			IsWordStop = isWordStop;
			CanHide = canHide;
		}
	}
}
