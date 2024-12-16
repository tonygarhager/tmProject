using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public delegate void OutputPropertiesProvider(INativeOutputFileProperties outputProperties, IPersistentFileConversionProperties conversionProperties, IOutputFileInfo suggestedFileInfo);
}
