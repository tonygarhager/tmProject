using Sdl.Core.Settings;
using Sdl.Core.Settings.Implementation.Json;
using System;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Core.Settings
{
	public class ComplexObservableList<T> : ObservableList<T> where T : ISerializableListItem, new()
	{
		public override void SaveToSettingsGroup(ISettingsGroup settingsGroup, string listSettingId)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			ClearListItemSettings(settingsGroup, listSettingId);
			settingsGroup.GetSetting<bool>(listSettingId).Value = true;
			int num = 0;
			using (IEnumerator<T> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					string text = listSettingId + num.ToString();
					settingsGroup.GetSetting<bool>(text).Value = true;
					current.SaveToSettingsGroup(settingsGroup, text);
					num++;
				}
			}
		}

		public override void PopulateFromSettingsGroup(ISettingsGroup settingsGroup, string listSettingId)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			base.PopulateFromSettingsGroup(settingsGroup, listSettingId);
		}

		public override void ClearListItemSettings(ISettingsGroup settingsGroup, string listSettingId)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			int num = 0;
			if (settingsGroup.ContainsSetting(listSettingId))
			{
				settingsGroup.RemoveSetting(listSettingId);
			}
			bool foundSetting;
			do
			{
				foundSetting = false;
				string text = listSettingId + num.ToString();
				T listItemFromSettings = GetListItemFromSettings(settingsGroup, text, out foundSetting);
				if (listItemFromSettings != null)
				{
					listItemFromSettings.ClearListItemSettings(settingsGroup, text);
					settingsGroup.RemoveSetting(text);
				}
				num++;
			}
			while (foundSetting);
		}

		protected override T GetListItemFromSettings(ISettingsGroup settingsGroup, string listItemSetting, out bool foundSetting)
		{
			if (settingsGroup is JsonSettingsGroup)
			{
				throw new NotSupportedException("Cannot read ISerializableListItem from JsonSettingsGroup");
			}
			T result = default(T);
			foundSetting = false;
			if (settingsGroup.ContainsSetting(listItemSetting))
			{
				foundSetting = true;
				result = new T();
				result.PopulateFromSettingsGroup(settingsGroup, listItemSetting);
			}
			return result;
		}
	}
}
