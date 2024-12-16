namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeExtractionContentProcessor : INativeExtractionContentHandler, IAbstractNativeContentHandler
	{
		INativeExtractionContentHandler Output
		{
			get;
			set;
		}
	}
}
