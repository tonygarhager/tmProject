using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.Core.IntegrationApi
{
	public interface IFileTypeSettingsConverterComponentBuilder
	{
		IFileTypeSettingsConverter BuildFileTypeSettingsConverter(string name);
	}
}
