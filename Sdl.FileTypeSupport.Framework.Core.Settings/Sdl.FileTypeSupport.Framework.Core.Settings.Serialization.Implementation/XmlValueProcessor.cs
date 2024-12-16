using Sdl.Core.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization.Implementation
{
	internal class XmlValueProcessor : IValueProcessor
	{
		private readonly ISettingsGroup _settingsGroup;

		private readonly string _keyPrefix;

		private readonly bool _forceWritingDefaultValues;

		public XmlValueProcessor(ISettingsGroup settingsGroup, string keyPrefix, bool forceWritingDefaultValues)
		{
			_settingsGroup = settingsGroup;
			_keyPrefix = keyPrefix;
			_forceWritingDefaultValues = forceWritingDefaultValues;
		}

		public void Process(string key, int value, int defaultValue)
		{
			if (_forceWritingDefaultValues || value != defaultValue || _settingsGroup.ContainsSetting(_keyPrefix + key))
			{
				_settingsGroup.GetSetting(_keyPrefix + key, defaultValue).Value = value;
			}
		}

		public void Process(string key, string value, string defaultValue)
		{
			if (_forceWritingDefaultValues || !(value == defaultValue) || _settingsGroup.ContainsSetting(_keyPrefix + key))
			{
				_settingsGroup.GetSetting(_keyPrefix + key, defaultValue).Value = value;
			}
		}

		public void Process(string key, bool value, bool defaultValue)
		{
			if (_forceWritingDefaultValues || value != defaultValue || _settingsGroup.ContainsSetting(_keyPrefix + key))
			{
				_settingsGroup.GetSetting(_keyPrefix + key, defaultValue).Value = value;
			}
		}

		public void Process(string key, ISettingsClass value, ISettingsClass defaultValue, bool discardKey)
		{
			if ((_forceWritingDefaultValues || ((value != null || defaultValue != null) && (value == null || !value.Equals(defaultValue))) || _settingsGroup.ContainsSetting(_keyPrefix + key)) && value != null)
			{
				if (discardKey)
				{
					_settingsGroup.GetSetting(_keyPrefix + key, defaultValue: false).Value = true;
				}
				string keyPrefix = discardKey ? _keyPrefix : (_keyPrefix + key);
				XmlValueProcessor valueProcessor = new XmlValueProcessor(_settingsGroup, keyPrefix, _forceWritingDefaultValues);
				value.Save(valueProcessor);
			}
		}

		public void Process(string key, List<string> value, List<string> defaultValue)
		{
			if (_forceWritingDefaultValues || ((value != null || defaultValue != null) && (value == null || defaultValue == null || !value.SequenceEqual(defaultValue))) || _settingsGroup.ContainsSetting(_keyPrefix + key))
			{
				List<string> list = (from x in _settingsGroup.GetSettingIds()
					where x.StartsWith(_keyPrefix + key)
					select x).ToList();
				foreach (string item in list)
				{
					_settingsGroup.RemoveSetting(item);
				}
				if (value != null)
				{
					_settingsGroup.GetSetting(_keyPrefix + key, defaultValue: false).Value = true;
					int num = 0;
					foreach (string item2 in value)
					{
						_settingsGroup.GetSetting(_keyPrefix + key + num.ToString(), string.Empty).Value = item2;
						num++;
					}
				}
			}
		}

		public void Process(string key, IReadOnlyDictionary<string, string> value, IReadOnlyDictionary<string, string> defaultValues)
		{
			if (_forceWritingDefaultValues || ((value != null || defaultValues != null) && (value == null || defaultValues == null || !value.SequenceEqual(defaultValues))) || _settingsGroup.ContainsSetting(_keyPrefix))
			{
				List<string> list = (from x in _settingsGroup.GetSettingIds()
					where x.StartsWith(_keyPrefix)
					select x).ToList();
				foreach (string item in list)
				{
					_settingsGroup.RemoveSetting(item);
				}
				int num = 0;
				if (value != null)
				{
					_settingsGroup.GetSetting(_keyPrefix, defaultValue: false).Value = true;
					foreach (KeyValuePair<string, string> item2 in value)
					{
						_settingsGroup.GetSetting(_keyPrefix + num.ToString() + "_Key", string.Empty).Value = item2.Key;
						_settingsGroup.GetSetting(_keyPrefix + num.ToString() + "_Value", string.Empty).Value = item2.Value;
						num++;
					}
				}
			}
		}

		public void Process(string key, IReadOnlyList<ISettingsClass> value, IReadOnlyList<ISettingsClass> defaultValue)
		{
			if (_forceWritingDefaultValues || ((value != null || defaultValue != null) && (value == null || defaultValue == null || !value.SequenceEqual(defaultValue))) || _settingsGroup.ContainsSetting(_keyPrefix + key))
			{
				List<string> list = (from x in _settingsGroup.GetSettingIds()
					where x.StartsWith(_keyPrefix + key)
					select x).ToList();
				foreach (string item in list)
				{
					_settingsGroup.RemoveSetting(item);
				}
				int num = 0;
				if (value != null)
				{
					_settingsGroup.GetSetting(_keyPrefix + key, defaultValue: false).Value = true;
					foreach (ISettingsClass item2 in value)
					{
						_settingsGroup.GetSetting(_keyPrefix + key + num.ToString(), defaultValue: false).Value = true;
						XmlValueProcessor valueProcessor = new XmlValueProcessor(_settingsGroup, _keyPrefix + key + num.ToString(), _forceWritingDefaultValues);
						item2.Save(valueProcessor);
						num++;
					}
				}
			}
		}
	}
}
