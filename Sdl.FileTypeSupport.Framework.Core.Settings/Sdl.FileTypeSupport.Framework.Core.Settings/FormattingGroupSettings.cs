using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public class FormattingGroupSettings : AbstractSettingsClass, ISerializableListItem
	{
		private readonly ObservableDictionary<string, string> _formattingItems = new ObservableDictionary<string, string>();

		public override string SettingName => "Formattings";

		public string FormattingDescription
		{
			get
			{
				string text = string.Empty;
				if (_formattingItems != null)
				{
					foreach (KeyValuePair<string, string> formattingItem in _formattingItems)
					{
						text = text + formattingItem.Key + "=\"" + formattingItem.Value + "\"; ";
					}
					return text;
				}
				return text;
			}
		}

		public ObservableDictionary<string, string> FormattingItems => _formattingItems;

		public FormattingGroupSettings()
		{
		}

		public FormattingGroupSettings(FormattingGroupSettings sourceData)
		{
			foreach (KeyValuePair<string, string> formattingItem in sourceData.FormattingItems)
			{
				FormattingItems.Add(formattingItem.Key, formattingItem.Value);
			}
		}

		public override void Read(IValueGetter valueGetter)
		{
			Dictionary<string, string> stringDictionary = valueGetter.GetStringDictionary(SettingName, new Dictionary<string, string>());
			FormattingItems.Clear();
			foreach (KeyValuePair<string, string> item in stringDictionary)
			{
				FormattingItems.Add(item.Key, item.Value);
			}
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			Dictionary<string, string> value = FormattingItems.ToDictionary((KeyValuePair<string, string> item) => item.Key, (KeyValuePair<string, string> item) => item.Value);
			valueProcessor.Process(SettingName, value, new Dictionary<string, string>());
		}

		public override object Clone()
		{
			return new FormattingGroupSettings(this);
		}

		public void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			ClearListItemSettings(settingsGroup, listItemSetting);
			settingsGroup.GetSetting<bool>(listItemSetting).Value = true;
			int num = 0;
			foreach (string key in _formattingItems.Keys)
			{
				string str = listItemSetting + num.ToString();
				string settingName = str + "_Key";
				string settingName2 = str + "_Value";
				UpdateSettingInSettingsGroup(settingsGroup, settingName, key, "");
				UpdateSettingInSettingsGroup(settingsGroup, settingName2, _formattingItems[key], "");
				num++;
			}
		}

		public void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listItemSetting)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			int num = 0;
			_formattingItems.Clear();
			bool flag;
			do
			{
				flag = false;
				string str = listItemSetting + num.ToString();
				string text = str + "_Key";
				string settingName = str + "_Value";
				if (settingsGroup.ContainsSetting(text))
				{
					flag = true;
					string settingFromSettingsGroup = GetSettingFromSettingsGroup(settingsGroup, text, "");
					string settingFromSettingsGroup2 = GetSettingFromSettingsGroup(settingsGroup, settingName, "");
					_formattingItems.Add(settingFromSettingsGroup, settingFromSettingsGroup2);
				}
				num++;
			}
			while (flag);
		}

		public void ClearListItemSettings(ISettingsGroup settingsGroup, string listItemSetting)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			settingsGroup.RemoveSetting(listItemSetting);
			int num = 0;
			bool flag;
			do
			{
				flag = false;
				string str = listItemSetting + num.ToString();
				string settingId = str + "_Key";
				string settingId2 = str + "_Value";
				if (settingsGroup.ContainsSetting(settingId))
				{
					flag = true;
					settingsGroup.RemoveSetting(settingId);
					settingsGroup.RemoveSetting(settingId2);
				}
				num++;
			}
			while (flag);
		}

		public override void ResetToDefaults()
		{
			_formattingItems.Clear();
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
			else if (!settingValue.Equals(defaultValue))
			{
				settingsGroup.GetSetting<T>(settingName).Value = settingValue;
			}
		}

		public override bool Equals(ISettingsClass other)
		{
			return Equals(other);
		}

		public new bool Equals(object other)
		{
			FormattingGroupSettings formattingGroupSettings = other as FormattingGroupSettings;
			if (other == null)
			{
				return false;
			}
			if (_formattingItems.Count != formattingGroupSettings._formattingItems.Count)
			{
				return false;
			}
			for (int i = 0; i < _formattingItems.Count; i++)
			{
				if (!_formattingItems.ElementAt(i).Equals(formattingGroupSettings._formattingItems.ElementAt(i)))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			foreach (KeyValuePair<string, string> formattingItem in _formattingItems)
			{
				num ^= formattingItem.GetHashCode();
			}
			return num;
		}
	}
}
