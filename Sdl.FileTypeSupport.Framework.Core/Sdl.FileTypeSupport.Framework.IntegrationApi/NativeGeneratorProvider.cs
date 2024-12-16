using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public delegate IFileGenerator NativeGeneratorProvider(IPersistentFileConversionProperties fileProperties);
}
