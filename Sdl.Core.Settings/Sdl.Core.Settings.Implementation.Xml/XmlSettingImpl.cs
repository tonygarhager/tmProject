using System.Xml.Linq;

namespace Sdl.Core.Settings.Implementation.Xml
{
	public class XmlSettingImpl<T> : Setting<T>
	{
		private Sdl.Core.Settings.SettingsGroup _settingsGroup;

		private Setting _setting;

		public override string Id => _setting.Id;

		public override bool Inherited => _inherited;

		public override T Value
		{
			get;
			set;
		}

		internal XDocument Xml => _setting.Xml;

		internal Setting XmlSetting
		{
			get
			{
				return _setting;
			}
			set
			{
				_setting = value;
			}
		}

		internal XmlSettingImpl(Sdl.Core.Settings.SettingsGroup settingsGroup, Setting setting, bool inherited)
		{
			_settingsGroup = settingsGroup;
			_setting = setting;
			_inherited = inherited;
		}

		internal XmlSettingImpl(Sdl.Core.Settings.SettingsGroup settingsGroup, string settingId, bool inherited)
		{
			_settingsGroup = settingsGroup;
			_setting = Setting.CreateSetting(settingId);
			_inherited = inherited;
		}

		public override void Reset()
		{
			XmlSettingImpl<T> xmlSettingImpl = _settingsGroup.ResetSetting<T>(Id) as XmlSettingImpl<T>;
			_inherited = true;
			_setting = (Setting)xmlSettingImpl._setting.Clone();
		}

		protected void OnXmlUpdated()
		{
			_settingsGroup.UpdateSetting(_setting, out _inherited);
		}
	}
}
