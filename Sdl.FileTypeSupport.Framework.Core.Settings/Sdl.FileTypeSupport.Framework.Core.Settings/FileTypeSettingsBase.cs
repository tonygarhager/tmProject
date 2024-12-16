using Sdl.Core.Settings;
using System.ComponentModel;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public abstract class FileTypeSettingsBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public abstract void PopulateFromSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);

		public abstract void SaveToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId);

		public abstract void ResetToDefaults();

		public virtual void SaveDefaultsToSettingsBundle(ISettingsBundle settingsBundle, string fileTypeConfigurationId)
		{
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected T GetSettingFromSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T defaultValue)
		{
			if (settingsGroup.ContainsSetting(settingName))
			{
				return settingsGroup.GetSetting<T>(settingName).Value;
			}
			return defaultValue;
		}

		protected void UpdateSettingInSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue, T defaultValue)
		{
			if (settingsGroup == null)
			{
				return;
			}
			if (settingsGroup.ContainsSetting(settingName))
			{
				SaveInGroup(settingsGroup, settingName, settingValue);
			}
			else if (settingValue == null)
			{
				if (defaultValue != null)
				{
					SaveInGroup(settingsGroup, settingName, settingValue);
				}
			}
			else if (!settingValue.Equals(defaultValue))
			{
				SaveInGroup(settingsGroup, settingName, settingValue);
			}
		}

		protected void SaveInGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue)
		{
			settingsGroup.GetSetting<T>(settingName).Value = settingValue;
		}
	}
}
