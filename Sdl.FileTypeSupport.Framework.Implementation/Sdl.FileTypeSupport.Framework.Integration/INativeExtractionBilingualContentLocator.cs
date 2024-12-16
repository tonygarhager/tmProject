using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public interface INativeExtractionBilingualContentLocator
	{
		ILocationMarker FindLocationMarker(LocationMarkerId markerId);
	}
}
