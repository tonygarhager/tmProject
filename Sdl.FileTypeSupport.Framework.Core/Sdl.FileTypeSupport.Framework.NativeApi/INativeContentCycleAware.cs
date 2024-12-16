using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.FileTypeSupport.Framework.NativeApi
{
	public interface INativeContentCycleAware
	{
		void SetFileProperties(IFileProperties properties);

		void StartOfInput();

		void EndOfInput();
	}
}
