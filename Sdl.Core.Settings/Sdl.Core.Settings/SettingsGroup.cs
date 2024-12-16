using Sdl.Core.Settings.Implementation;
using Sdl.Core.Settings.Implementation.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Sdl.Core.Settings
{
	[TypeDescriptionProvider(typeof(SettingsGroupTypeDescriptionProvider))]
	public abstract class SettingsGroup : AbstractSettingsGroupBase, ISettingsGroup, INotifyPropertyChanged, IEditableObject
	{
		private Sdl.Core.Settings.Implementation.Xml.SettingsGroup _settingsGroup;

		private ISettingsBundle _settingsBundle;

		public override string Id
		{
			get
			{
				return _settingsGroup.Id;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override ISettingsGroup Parent => ((Sdl.Core.Settings.Implementation.SettingsBundle)SettingsBundle.Parent)?.GetSettingsGroup(Id, GetType());

		private SettingsGroup ParentImpl => (SettingsGroup)Parent;

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

		internal Sdl.Core.Settings.Implementation.Xml.SettingsGroup XmlSettingsGroup
		{
			get
			{
				return _settingsGroup;
			}
			set
			{
				_settingsGroup = value;
			}
		}

		public new event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (!_parentPropertyChangedEventHandlerAttached)
				{
					ISettingsGroup parent = Parent;
					if (parent != null)
					{
						parent.PropertyChanged += parent_PropertyChanged;
					}
					_parentPropertyChangedEventHandlerAttached = true;
				}
				_propertyChangedDelegate = (PropertyChangedEventHandler)Delegate.Combine(_propertyChangedDelegate, value);
			}
			remove
			{
				_propertyChangedDelegate = (PropertyChangedEventHandler)Delegate.Remove(_propertyChangedDelegate, value);
			}
		}

		protected override void parent_SettingsChanged(object sender, SettingsChangedEventArgs e)
		{
			List<string> list = new List<string>();
			foreach (string settingId in e.SettingIds)
			{
				if (GetXmlSetting(settingId) == null)
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
			SettingsGroup settingsGroup2 = settingsGroup as SettingsGroup;
			foreach (Setting value in settingsGroup2.XmlSettingsGroup.Settings.Values)
			{
				if (!keys.Contains(value.Id))
				{
					keys.Add(value.Id);
				}
			}
			SettingsGroup parentImpl = settingsGroup2.ParentImpl;
			if (parentImpl != null)
			{
				CollectSettings(parentImpl, keys);
			}
		}

		protected virtual void OnInit(ISettingsBundle settingsBundle)
		{
		}

		public override bool ContainsSetting(string settingId)
		{
			Setting xmlSetting = GetXmlSetting(settingId);
			if (xmlSetting != null)
			{
				return true;
			}
			return ((SettingsGroup)Parent)?.ContainsSetting(settingId) ?? false;
		}

		public override IEnumerable<string> GetSettingIds()
		{
			return _settingsGroup.Settings.Keys;
		}

		public override Setting<T> GetSetting<T>(string id)
		{
			Setting<T> settingImpl = GetSettingImpl<T>(id);
			if (settingImpl != null)
			{
				return settingImpl;
			}
			object defaultValue = GetDefaultValue(id);
			if (defaultValue != null)
			{
				return CreateDefaultSetting(id, (T)defaultValue);
			}
			if (typeof(T).IsEnum)
			{
				return CreateDefaultSetting(id, (T)GetFirstEnumValue(typeof(T)));
			}
			return CreateDefaultSetting(id, default(T));
		}

		private static object GetFirstEnumValue(Type type)
		{
			if (!type.IsEnum)
			{
				throw new Exception($"{type.FullName} is not an enum.");
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
			return fields[0].GetValue(null);
		}

		public override bool GetSetting<T>(string settingId, out Setting<T> setting)
		{
			setting = GetSettingImpl<T>(settingId);
			return setting != null;
		}

		public override bool GetSetting<T>(string settingId, out T value)
		{
			Setting<T> settingImpl = GetSettingImpl<T>(settingId);
			if (settingImpl != null)
			{
				value = settingImpl.Value;
				return true;
			}
			value = default(T);
			return false;
		}

		public override Setting<T> GetSetting<T>(string settingId, T defaultValue)
		{
			Setting<T> settingImpl = GetSettingImpl<T>(settingId);
			if (settingImpl != null)
			{
				return settingImpl;
			}
			return CreateDefaultSetting(settingId, defaultValue);
		}

		public override bool RemoveSetting(string settingId)
		{
			return ResetSetting(settingId);
		}

		public override void Reset()
		{
			List<string> list = new List<string>();
			foreach (Setting value in _settingsGroup.Settings.Values)
			{
				list.Add(value.Id);
			}
			_settingsGroup.Settings.Clear();
			OnSettingsChanged(list.AsReadOnly(), isResumingEvents: false);
		}

		public override void ImportSettings(ISettingsGroup otherGroup)
		{
			SettingsGroup settingsGroup = otherGroup as SettingsGroup;
			if (settingsGroup != null)
			{
				Reset();
				List<string> list = new List<string>();
				foreach (Setting value in settingsGroup._settingsGroup.Settings.Values)
				{
					list.Add(value.Id);
					_settingsGroup.Settings.Add(value.Id, (Setting)value.Clone());
				}
				OnSettingsChanged(list.AsReadOnly(), isResumingEvents: false);
			}
		}

		internal Setting<T> GetSettingImpl<T>(string settingId)
		{
			Setting xmlSetting = GetXmlSetting(settingId);
			if (xmlSetting != null)
			{
				return SettingFactory.CreateSetting<T>(this, xmlSetting, SettingsBundle.IsDefault);
			}
			SettingsGroup settingsGroup = (SettingsGroup)Parent;
			if (settingsGroup != null)
			{
				XmlSettingImpl<T> xmlSettingImpl = settingsGroup.GetSettingImpl<T>(settingId) as XmlSettingImpl<T>;
				if (xmlSettingImpl != null)
				{
					return SettingFactory.CreateSetting<T>(this, (Setting)xmlSettingImpl.XmlSetting.Clone(), inherited: true);
				}
			}
			return null;
		}

		private Setting<T> CreateDefaultSetting<T>(string settingId, T defaultValue)
		{
			return SettingFactory.CreateSetting(this, settingId, defaultValue, inherited: true);
		}

		protected override void parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (GetXmlSetting(e.PropertyName) == null && _propertyChangedDelegate != null)
			{
				_propertyChangedDelegate(this, new PropertyChangedEventArgs(e.PropertyName));
			}
		}

		public override void BeginEdit()
		{
			if (!_settingsGroup.IsEditing)
			{
				_settingsGroup.BeginEdit();
				SuspendEvents();
			}
		}

		public override void CancelEdit()
		{
			if (_settingsGroup.IsEditing)
			{
				_settingsGroup.CancelEdit();
				DiscardEvents();
			}
		}

		public override void EndEdit()
		{
			if (_settingsGroup.IsEditing)
			{
				_settingsGroup.EndEdit();
				ResumeEvents();
			}
		}

		internal Setting ResolveXmlSetting(string settingId)
		{
			bool inherited;
			return ResolveXmlSetting(settingId, out inherited);
		}

		internal Setting ResolveXmlSetting(string settingId, bool includeDefault, out bool inherited)
		{
			Setting xmlSetting = GetXmlSetting(settingId);
			if (xmlSetting != null)
			{
				inherited = SettingsBundle.IsDefault;
				return xmlSetting;
			}
			inherited = true;
			SettingsGroup settingsGroup = (SettingsGroup)Parent;
			if (settingsGroup != null)
			{
				xmlSetting = settingsGroup.ResolveXmlSetting(settingId, includeDefault: false, out bool _);
				if (xmlSetting != null)
				{
					return xmlSetting;
				}
			}
			if (includeDefault)
			{
				object defaultValue = GetDefaultValue(settingId);
				if (defaultValue != null && SettingFactory.IsSimpleSettingValue(defaultValue))
				{
					string simpleSettingValueAsString = SettingFactory.GetSimpleSettingValueAsString(defaultValue);
					xmlSetting = Setting.CreateSetting(settingId);
					xmlSetting.Xml.Root.ReplaceAll(simpleSettingValueAsString);
					return xmlSetting;
				}
				return null;
			}
			return null;
		}

		internal Setting ResolveXmlSetting(string settingId, out bool inherited)
		{
			return ResolveXmlSetting(settingId, includeDefault: true, out inherited);
		}

		internal Setting ResolveNonDefaultXmlSetting(string settingId)
		{
			Setting xmlSetting = GetXmlSetting(settingId);
			if (xmlSetting != null)
			{
				return xmlSetting;
			}
			return ((SettingsGroup)Parent)?.ResolveNonDefaultXmlSetting(settingId);
		}

		internal Setting UpdateSetting(Setting updatedSetting, out bool isInherited)
		{
			isInherited = false;
			Setting setting = null;
			SettingsGroup settingsGroup = (SettingsGroup)Parent;
			if (settingsGroup != null && (setting = settingsGroup.ResolveXmlSetting(updatedSetting.Id, includeDefault: false, out bool _)) != null && setting.GetXmlString() == updatedSetting.GetXmlString())
			{
				isInherited = true;
				Setting xmlSetting = GetXmlSetting(updatedSetting.Id);
				if (xmlSetting != null)
				{
					_settingsGroup.Settings.Remove(xmlSetting.Id);
				}
				OnSettingChanged(updatedSetting.Id);
				return setting;
			}
			if (settingsGroup == null || setting == null)
			{
				object defaultValue = GetDefaultValue(updatedSetting.Id);
				if (defaultValue != null && SettingFactory.IsSimpleSettingValue(defaultValue))
				{
					string content = TypeConverterUtil.ConvertToInvariantString(defaultValue);
					Setting setting2 = Setting.CreateSetting(updatedSetting.Id);
					setting2.Xml.Root.ReplaceAll(content);
					if (setting2.GetXmlString() == updatedSetting.GetXmlString())
					{
						isInherited = true;
						Setting xmlSetting2 = GetXmlSetting(updatedSetting.Id);
						if (xmlSetting2 != null)
						{
							_settingsGroup.Settings.Remove(xmlSetting2.Id);
						}
						OnSettingChanged(updatedSetting.Id);
						return setting2;
					}
				}
			}
			setting = GetXmlSetting(updatedSetting.Id);
			if (setting == null)
			{
				setting = (Setting)updatedSetting.Clone();
				_settingsGroup.Settings.Add(setting.Id, setting);
			}
			else
			{
				setting.Xml = updatedSetting.Xml;
			}
			OnSettingChanged(updatedSetting.Id);
			return setting;
		}

		internal Setting<T> ResetSetting<T>(string settingId)
		{
			Setting xmlSetting = GetXmlSetting(settingId);
			if (xmlSetting != null)
			{
				_settingsGroup.Settings.Remove(xmlSetting.Id);
				OnSettingChanged(settingId);
			}
			return GetSetting<T>(settingId);
		}

		internal bool ResetSetting(string settingId)
		{
			Setting xmlSetting = GetXmlSetting(settingId);
			if (xmlSetting != null)
			{
				_settingsGroup.Settings.Remove(xmlSetting.Id);
				OnSettingChanged(settingId);
				return true;
			}
			return false;
		}

		protected virtual object GetDefaultValue(string settingId)
		{
			return null;
		}

		private Setting GetXmlSetting(string settingId)
		{
			_settingsGroup.Settings.TryGetValue(settingId, out Setting value);
			return value;
		}
	}
}
