using Sdl.Core.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.FileTypeSupport.Framework.Core.Settings.Serialization.Implementation
{
	internal class XmlValueGetter : IValueGetter
	{
		private readonly ISettingsGroup _settingsGroup;

		private readonly string _keyPrefix;

		public XmlValueGetter(ISettingsGroup settingsGroup, string keyPrefix = "")
		{
			_settingsGroup = settingsGroup;
			_keyPrefix = keyPrefix;
		}

		public int GetValue(string key, int defaultValue)
		{
			return _settingsGroup.GetSetting(_keyPrefix + key, defaultValue);
		}

		public string GetValue(string key, string defaultValue)
		{
			return _settingsGroup.GetSetting(_keyPrefix + key, defaultValue);
		}

		public bool GetValue(string key, bool defaultValue)
		{
			return _settingsGroup.GetSetting(_keyPrefix + key, defaultValue);
		}

		public T GetValue<T>(string key, T defaultValue, bool discardKey) where T : ISettingsClass, new()
		{
			T result = (T)defaultValue.Clone();
			string keyPrefix = discardKey ? _keyPrefix : (_keyPrefix + key);
			XmlValueGetter valueGetter = new XmlValueGetter(_settingsGroup, keyPrefix);
			result.Read(valueGetter);
			return result;
		}

		public List<string> GetStringList(string key, List<string> defaultValue)
		{
			if (!_settingsGroup.ContainsSetting(_keyPrefix + key))
			{
				return new List<string>(defaultValue);
			}
			List<string> list = new List<string>();
			int num = 0;
			while (_settingsGroup.ContainsSetting(_keyPrefix + key + num.ToString()))
			{
				list.Add(_settingsGroup.GetSetting(_keyPrefix + key + num.ToString(), "").Value);
				num++;
			}
			return list;
		}

		public Dictionary<string, string> GetStringDictionary(string key, Dictionary<string, string> defaultValue)
		{
			if (!_settingsGroup.ContainsSetting(_keyPrefix))
			{
				return defaultValue.ToDictionary((KeyValuePair<string, string> item) => item.Key, (KeyValuePair<string, string> item) => item.Value);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = 0;
			while (_settingsGroup.ContainsSetting(_keyPrefix + num.ToString() + "_Key"))
			{
				string value = _settingsGroup.GetSetting(_keyPrefix + num.ToString() + "_Key", "").Value;
				string value2 = _settingsGroup.GetSetting(_keyPrefix + num.ToString() + "_Value", "").Value;
				if (!string.IsNullOrEmpty(value))
				{
					dictionary.Add(value, value2);
				}
				num++;
			}
			return dictionary;
		}

		public List<T> GetCompositeList<T>(string key, List<T> defaultValue) where T : ISettingsClass, new()
		{
			if (!_settingsGroup.ContainsSetting(_keyPrefix + key))
			{
				return new List<T>(defaultValue.Select((T x) => (T)x.Clone()));
			}
			List<T> list = new List<T>();
			int num = 0;
			while (_settingsGroup.ContainsSetting(_keyPrefix + key + num.ToString()))
			{
				T item = new T();
				item.ResetToDefaults();
				XmlValueGetter valueGetter = new XmlValueGetter(_settingsGroup, _keyPrefix + key + num.ToString());
				item.Read(valueGetter);
				list.Add(item);
				num++;
			}
			return list;
		}
	}
}
