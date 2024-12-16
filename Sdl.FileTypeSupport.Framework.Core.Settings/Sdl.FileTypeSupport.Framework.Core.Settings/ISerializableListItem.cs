using Sdl.Core.Settings;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public interface ISerializableListItem
	{
		void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting);

		void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting);

		void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting);
	}
}
