using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface IDefaultFileTypeSettingsProvider
	{
		void PopulateDefaultSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);
	}
}
