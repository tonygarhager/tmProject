using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public interface INativeTextLocationInfoProvider
	{
		LocationInfo GetLocationInfo(NativeTextLocation nativeLocation);
	}
}
