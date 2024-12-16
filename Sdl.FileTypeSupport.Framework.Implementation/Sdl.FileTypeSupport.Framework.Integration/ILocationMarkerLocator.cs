using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public interface ILocationMarkerLocator
	{
		IMessageLocation GetLocation(LocationMarkerId marker);
	}
}
