using Newtonsoft.Json;
using System;

namespace Sdl.Core.Settings.Implementation.Json
{
	public class JsonSettingImpl<T> : Setting<T>, ICloneable
	{
		private string _internalId;

		private JsonSettingsGroup _settingsGroup;

		internal T _value;

		public override string Id => _internalId;

		internal string InternalId
		{
			set
			{
				_internalId = value;
			}
		}

		[JsonIgnore]
		public ISettingsGroup SettingsGroup
		{
			get
			{
				return _settingsGroup;
			}
			set
			{
				_settingsGroup = (value as JsonSettingsGroup);
			}
		}

		public override T Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				OnSettingUpdated();
			}
		}

		internal JsonSettingImpl(string id, ISettingsGroup group)
		{
			_internalId = id;
			_settingsGroup = (group as JsonSettingsGroup);
			_value = default(T);
		}

		public JsonSettingImpl(JsonSettingImpl<T> other)
		{
			_internalId = other._internalId;
			string value = JsonConvert.SerializeObject(other.Value);
			T val = _value = JsonConvert.DeserializeObject<T>(value);
		}

		public JsonSettingImpl()
		{
		}

		public override void Reset()
		{
			_value = default(T);
		}

		public object Clone()
		{
			return new JsonSettingImpl<T>(this);
		}

		protected void OnSettingUpdated()
		{
			if (_settingsGroup != null)
			{
				_settingsGroup.UpdateSetting(this, out _inherited);
			}
		}
	}
}
