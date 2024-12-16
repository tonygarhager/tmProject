using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sdl.Core.Settings
{
	public interface ISettingsGroup : INotifyPropertyChanged, IEditableObject
	{
		string Id
		{
			get;
		}

		ISettingsBundle SettingsBundle
		{
			get;
		}

		ISettingsGroup Parent
		{
			get;
		}

		bool EventsSuspended
		{
			get;
		}

		event EventHandler<SettingsChangedEventArgs> SettingsChanged;

		bool ContainsSetting(string settingId);

		Setting<T> GetSetting<T>(string id);

		bool GetSetting<T>(string settingId, out Setting<T> setting);

		bool GetSetting<T>(string settingId, out T value);

		Setting<T> GetSetting<T>(string settingId, T defaultValue);

		void ImportSettings(ISettingsGroup otherGroup);

		bool RemoveSetting(string settingId);

		void Reset();

		void SuspendEvents();

		void ResumeEvents();

		IEnumerable<string> GetSettingIds();
	}
}
