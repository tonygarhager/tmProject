namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeGenerationContentHandler : IAbstractNativeContentHandler
	{
		void ParagraphUnitStart(IParagraphUnitProperties properties);

		void ParagraphUnitEnd();

		void SegmentStart(ISegmentPairProperties properties);

		void SegmentEnd();
	}
}
