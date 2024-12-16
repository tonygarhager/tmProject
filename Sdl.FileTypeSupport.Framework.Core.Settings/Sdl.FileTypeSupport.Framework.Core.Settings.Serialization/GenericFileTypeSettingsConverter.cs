using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization
{
	public class GenericFileTypeSettingsConverter : IFileTypeSettingsConverter
	{
		private readonly SettingsBundleConverterMethod[] _conversionMethods;

		public GenericFileTypeSettingsConverter(params SettingsBundleConverterMethod[] conversionMethods)
		{
			_conversionMethods = conversionMethods;
		}

		public ISettingsBundle ConvertXmlToJson(ISettingsBundle xmlSettingsBundle, string fileTypeConfigurationId)
		{
			ISettingsBundle settingsBundle = SettingsUtil.CreateJsonSettingsBundle(null);
			ConvertXmlToJson(xmlSettingsBundle, settingsBundle, fileTypeConfigurationId);
			return settingsBundle;
		}

		public void ConvertXmlToJson(ISettingsBundle xmlSettingsBundle, ISettingsBundle jsonSettingsBundle, string fileTypeConfigurationId)
		{
			if (jsonSettingsBundle.ContainsSettingsGroup(fileTypeConfigurationId))
			{
				jsonSettingsBundle.RemoveSettingsGroup(fileTypeConfigurationId);
			}
			SettingsBundleConverterMethod[] conversionMethods = _conversionMethods;
			foreach (SettingsBundleConverterMethod settingsBundleConverterMethod in conversionMethods)
			{
				settingsBundleConverterMethod(xmlSettingsBundle, jsonSettingsBundle, fileTypeConfigurationId);
			}
		}

		public ISettingsBundle ConvertJsonToXml(ISettingsBundle jsonSettingsBundle, string fileTypeConfigurationId)
		{
			ISettingsBundle settingsBundle = SettingsUtil.CreateSettingsBundle(null);
			ConvertJsonToXml(jsonSettingsBundle, settingsBundle, fileTypeConfigurationId);
			return settingsBundle;
		}

		public void ConvertJsonToXml(ISettingsBundle jsonSettingsBundle, ISettingsBundle xmlSettingsBundle, string fileTypeConfigurationId)
		{
			if (xmlSettingsBundle.ContainsSettingsGroup(fileTypeConfigurationId))
			{
				xmlSettingsBundle.RemoveSettingsGroup(fileTypeConfigurationId);
			}
			SettingsBundleConverterMethod[] conversionMethods = _conversionMethods;
			foreach (SettingsBundleConverterMethod settingsBundleConverterMethod in conversionMethods)
			{
				settingsBundleConverterMethod(jsonSettingsBundle, xmlSettingsBundle, fileTypeConfigurationId);
			}
		}
	}
}
