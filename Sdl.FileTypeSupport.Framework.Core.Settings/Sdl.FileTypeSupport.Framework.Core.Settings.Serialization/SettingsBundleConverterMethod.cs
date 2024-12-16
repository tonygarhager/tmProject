using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization
{
	public delegate void SettingsBundleConverterMethod(ISettingsBundle sourceSettingsBundle, ISettingsBundle targetSettingsBundle, string fileTypeId);
}
