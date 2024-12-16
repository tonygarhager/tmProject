using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public interface IFileTypeSettingsConverter
	{
		ISettingsBundle ConvertXmlToJson(ISettingsBundle xmlSettingsBundle, string fileTypeConfigurationId);

		void ConvertXmlToJson(ISettingsBundle xmlSettingsBundle, ISettingsBundle jsonSettingsBundle, string fileTypeConfigurationId);

		ISettingsBundle ConvertJsonToXml(ISettingsBundle jsonSettingsBundle, string fileTypeConfigurationId);

		void ConvertJsonToXml(ISettingsBundle jsonSettingsBundle, ISettingsBundle xmlSettingsBundle, string fileTypeConfigurationId);
	}
}
