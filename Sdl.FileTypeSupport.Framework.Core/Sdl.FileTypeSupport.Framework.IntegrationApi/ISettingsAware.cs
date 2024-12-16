using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	public interface ISettingsAware
	{
		void InitializeSettings(ISettingsBundle settingsBundle, string configurationId);
	}
}
