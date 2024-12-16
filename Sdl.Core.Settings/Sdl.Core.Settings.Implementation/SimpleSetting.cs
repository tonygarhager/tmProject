using Sdl.Core.Settings.Implementation.Xml;

namespace Sdl.Core.Settings.Implementation
{
	internal class SimpleSetting<T> : XmlSettingImpl<T>
	{
		public override T Value
		{
			get
			{
				if (typeof(T) == typeof(string))
				{
					return (T)(object)base.Xml.Root.Value;
				}
				return (T)TypeConverterUtil.ConvertFromInvariantString(base.Xml.Root.Value, typeof(T));
			}
			set
			{
				SetValueCore(value);
				OnXmlUpdated();
			}
		}

		public SimpleSetting(SettingsGroup settingsGroup, Setting setting, bool inherited)
			: base(settingsGroup, setting, inherited)
		{
		}

		public SimpleSetting(SettingsGroup settingsGroup, string settingId, T value, bool inherited)
			: base(settingsGroup, settingId, inherited)
		{
			SetValueCore(value);
		}

		private void SetValueCore(T value)
		{
			if (typeof(T) == typeof(string))
			{
				base.Xml.Root.ReplaceAll((string)(object)value);
			}
			else
			{
				base.Xml.Root.ReplaceAll(TypeConverterUtil.ConvertToInvariantString(value));
			}
		}
	}
}
