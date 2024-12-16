using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sdl.Core.Settings.Implementation.Json
{
	public class JsonSettingsGroup : AbstractSettingsGroupBase, ISettingsGroup, INotifyPropertyChanged, IEditableObject, ICloneable
	{
		private ISettingsBundle _settingsBundle;

		[JsonProperty]
		private Dictionary<string, object> _settings = new Dictionary<string, object>();

		public override string Id
		{
			get;
			set;
		}

		private JsonSettingsGroup ParentImpl => (JsonSettingsGroup)Parent;

		[JsonIgnore]
		public override ISettingsBundle SettingsBundle
		{
			get
			{
				return _settingsBundle;
			}
			set
			{
				_settingsBundle = value;
				OnInit(_settingsBundle);
			}
		}

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public override ISettingsGroup Parent => (_settingsBundle?.Parent)?.GetSettingsGroup(Id);

		protected virtual void OnInit(ISettingsBundle settingsBundle)
		{
		}

		protected override void parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_settings.ContainsKey(e.PropertyName) && _propertyChangedDelegate != null)
			{
				_propertyChangedDelegate(this, new PropertyChangedEventArgs(e.PropertyName));
			}
		}

		public JsonSettingsGroup()
		{
		}

		internal JsonSettingsGroup(string id)
		{
			Id = id;
		}

		public JsonSettingsGroup(JsonSettingsGroup other)
		{
			foreach (KeyValuePair<string, object> setting in other._settings)
			{
				dynamic value = setting.Value;
				dynamic val = JsonConvert.SerializeObject(value);
				dynamic val2 = JsonConvert.DeserializeObject(val);
				_settings.Add(setting.Key, val2);
			}
			Id = (other.Id.Clone() as string);
		}

		public override bool ContainsSetting(string settingId)
		{
			if (_settings.ContainsKey(settingId))
			{
				return true;
			}
			return ((JsonSettingsGroup)Parent)?.ContainsSetting(settingId) ?? false;
		}

		public override Setting<T> GetSetting<T>(string id)
		{
			if (_settings.ContainsKey(id))
			{
				if (_settings[id] is JObject)
				{
					JObject jObject = _settings[id] as JObject;
					JsonSettingImpl<T> jsonSettingImpl = jObject.ToObject<JsonSettingImpl<T>>();
					jsonSettingImpl.InternalId = id;
					jsonSettingImpl.SettingsGroup = this;
					_settings[id] = jsonSettingImpl;
					return jsonSettingImpl;
				}
				return (Setting<T>)_settings[id];
			}
			if (Parent != null && Parent.ContainsSetting(id))
			{
				Setting<T> setting = Parent.GetSetting<T>(id);
				JsonSettingImpl<T> jsonSettingImpl2 = (JsonSettingImpl<T>)setting;
				JsonSettingImpl<T> jsonSettingImpl3 = (JsonSettingImpl<T>)jsonSettingImpl2.Clone();
				jsonSettingImpl3.InternalId = id;
				jsonSettingImpl3.SettingsGroup = this;
				return jsonSettingImpl3;
			}
			JsonSettingImpl<T> jsonSettingImpl4 = CreateDefaultSetting<T>(id, this);
			_settings.Add(id, jsonSettingImpl4);
			return jsonSettingImpl4;
		}

		private static JsonSettingImpl<T> CreateDefaultSetting<T>(string settingId, ISettingsGroup group)
		{
			return new JsonSettingImpl<T>(settingId, group);
		}

		internal Setting<T> UpdateSetting<T>(Setting<T> setting, out bool isInherited)
		{
			isInherited = false;
			if (Parent != null && Parent.ContainsSetting(setting.Id))
			{
				T value = setting.Value;
				Setting<T> setting2 = Parent.GetSetting<T>(setting.Id);
				T value2 = setting2.Value;
				if (value.Equals(value2))
				{
					_settings.Remove(setting.Id);
					isInherited = true;
					OnSettingChanged(setting.Id);
					return setting2;
				}
			}
			if (_settings.ContainsKey(setting.Id))
			{
				_settings[setting.Id] = setting;
			}
			else
			{
				JsonSettingImpl<T> jsonSettingImpl = (JsonSettingImpl<T>)setting;
				setting = (JsonSettingImpl<T>)jsonSettingImpl.Clone();
				_settings.Add(setting.Id, setting);
			}
			OnSettingChanged(setting.Id);
			return setting;
		}

		public override bool GetSetting<T>(string settingId, out Setting<T> setting)
		{
			setting = GetSetting<T>(settingId);
			return setting != null;
		}

		public override bool GetSetting<T>(string settingId, out T value)
		{
			if (ContainsSetting(settingId))
			{
				value = GetSetting<T>(settingId).Value;
				return true;
			}
			value = default(T);
			return false;
		}

		public override Setting<T> GetSetting<T>(string settingId, T defaultValue)
		{
			if (ContainsSetting(settingId))
			{
				return GetSetting<T>(settingId);
			}
			JsonSettingImpl<T> jsonSettingImpl = CreateDefaultSetting<T>(settingId, this);
			jsonSettingImpl._value = defaultValue;
			_settings.Add(settingId, jsonSettingImpl);
			return jsonSettingImpl;
		}

		public override IEnumerable<string> GetSettingIds()
		{
			return _settings.Keys;
		}

		public override void ImportSettings(ISettingsGroup otherGroup)
		{
			JsonSettingsGroup jsonSettingsGroup = otherGroup as JsonSettingsGroup;
			if (jsonSettingsGroup != null)
			{
				Reset();
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, object> setting in jsonSettingsGroup._settings)
				{
					list.Add(setting.Key);
					dynamic value = setting.Value;
					dynamic val = value.Clone();
					_settings.Add(setting.Key, val);
				}
				OnSettingsChanged(list.AsReadOnly(), isResumingEvents: false);
			}
		}

		public override bool RemoveSetting(string settingId)
		{
			if (_settings.ContainsKey(settingId))
			{
				_settings.Remove(settingId);
				return true;
			}
			return false;
		}

		public override void Reset()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, object> setting in _settings)
			{
				list.Add(setting.Key);
			}
			_settings.Clear();
			OnSettingsChanged(list.AsReadOnly(), isResumingEvents: false);
		}

		protected override void parent_SettingsChanged(object sender, SettingsChangedEventArgs e)
		{
			List<string> list = new List<string>();
			foreach (string settingId in e.SettingIds)
			{
				if (_settings.ContainsKey(settingId))
				{
					list.Add(settingId);
				}
			}
			if (list.Count > 0)
			{
				OnSettingsChanged(list.AsReadOnly(), isResumingEvents: false);
			}
		}

		protected override void CollectSettings(ISettingsGroup settingsGroup, List<string> keys)
		{
			JsonSettingsGroup jsonSettingsGroup = settingsGroup as JsonSettingsGroup;
			foreach (KeyValuePair<string, object> setting in jsonSettingsGroup._settings)
			{
				if (!keys.Contains(setting.Key))
				{
					keys.Add(setting.Key);
				}
			}
			JsonSettingsGroup parentImpl = jsonSettingsGroup.ParentImpl;
			if (parentImpl != null)
			{
				CollectSettings(parentImpl, keys);
			}
		}

		public override void BeginEdit()
		{
			throw new NotImplementedException();
		}

		public override void EndEdit()
		{
			throw new NotImplementedException();
		}

		public override void CancelEdit()
		{
			throw new NotImplementedException();
		}

		public object Clone()
		{
			return new JsonSettingsGroup(this);
		}
	}
}
