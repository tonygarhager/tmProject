using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IBilingualToNativeConverter : IBilingualContentHandler
	{
		INativeGenerationContentHandler Output
		{
			get;
			set;
		}

		void Flush();
	}
}
