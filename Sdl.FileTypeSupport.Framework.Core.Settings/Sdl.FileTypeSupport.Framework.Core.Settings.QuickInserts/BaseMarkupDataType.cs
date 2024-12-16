using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.QuickInserts
{
	public abstract class BaseMarkupDataType : AbstractSettingsClass, ISerializableListItem, INotifyPropertyChanged
	{
		private enum QuickInsertContentType
		{
			Text,
			TextPair,
			PlaceholderTag,
			TagPair
		}

		private const string MarkupDataTypeSetting = "MarkupDataType";

		public override string SettingName => "MarkupDataType";

		public new event PropertyChangedEventHandler PropertyChanged;

		public abstract void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting);

		public abstract void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting);

		public abstract void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting);

		public override string ToString()
		{
			return base.ToString();
		}

		protected new void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		protected new T GetSettingFromSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T defaultValue)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			if (settingsGroup.ContainsSetting(settingName))
			{
				return settingsGroup.GetSetting<T>(settingName).Value;
			}
			return defaultValue;
		}

		protected new void UpdateSettingInSettingsGroup<T>(ISettingsGroup settingsGroup, string settingName, T settingValue, T defaultValue)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			if (settingsGroup.ContainsSetting(settingName))
			{
				settingsGroup.GetSetting<T>(settingName).Value = settingValue;
			}
			else if (!EqualityComparer<T>.Default.Equals(settingValue, default(T)) && !EqualityComparer<T>.Default.Equals(settingValue, defaultValue))
			{
				settingsGroup.GetSetting<T>(settingName).Value = settingValue;
			}
		}
	}
}
