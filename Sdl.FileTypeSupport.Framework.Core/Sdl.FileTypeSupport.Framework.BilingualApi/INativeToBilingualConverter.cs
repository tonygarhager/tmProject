using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface INativeToBilingualConverter : INativeExtractionContentHandler, IAbstractNativeContentHandler
	{
		IBilingualContentHandler Output
		{
			get;
			set;
		}

		IDocumentProperties DocumentInfo
		{
			get;
			set;
		}

		IFileProperties FileInfo
		{
			get;
			set;
		}
	}
}
