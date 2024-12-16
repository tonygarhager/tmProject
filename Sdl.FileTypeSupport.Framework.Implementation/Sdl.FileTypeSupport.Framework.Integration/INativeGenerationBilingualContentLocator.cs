using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	public interface INativeGenerationBilingualContentLocator
	{
		IMessageLocation FindLocation(LocationInfo info);
	}
}
