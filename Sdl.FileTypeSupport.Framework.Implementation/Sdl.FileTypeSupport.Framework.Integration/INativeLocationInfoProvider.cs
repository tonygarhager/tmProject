using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public interface INativeLocationInfoProvider
	{
		LocationInfo GetLocationInfo(LocationMarkerId marker);
	}
}
