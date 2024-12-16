using System;
using System.Collections.Generic;

namespace Sdl.Core.Settings
{
	public class SettingsChangedEventArgs : EventArgs
	{
		private ISettingsGroup _settingsGroup;

		private IList<string> _settingIds;

		public ISettingsGroup SettingsGroup => _settingsGroup;

		public IList<string> SettingIds => _settingIds;

		public SettingsChangedEventArgs(ISettingsGroup settingsGroup, IList<string> settingIds)
		{
			_settingsGroup = settingsGroup;
			_settingIds = settingIds;
		}
	}
}
