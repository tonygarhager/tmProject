namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeGenerationContentProcessor : INativeGenerationContentHandler, IAbstractNativeContentHandler
	{
		INativeGenerationContentHandler Output
		{
			get;
			set;
		}
	}
}
