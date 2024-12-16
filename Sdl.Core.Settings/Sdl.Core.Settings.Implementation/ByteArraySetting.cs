using Sdl.Core.Settings.Implementation.Xml;
using System;

namespace Sdl.Core.Settings.Implementation
{
	internal class ByteArraySetting : XmlSettingImpl<byte[]>
	{
		public override byte[] Value
		{
			get
			{
				return Convert.FromBase64String(base.Xml.Root.Value);
			}
			set
			{
				SetValueCore(value);
				OnXmlUpdated();
			}
		}

		public ByteArraySetting(SettingsGroup settingsGroup, Setting setting, bool inherited)
			: base(settingsGroup, setting, inherited)
		{
		}

		public ByteArraySetting(SettingsGroup settingsGroup, string settingId, byte[] value, bool inherited)
			: base(settingsGroup, settingId, inherited)
		{
			SetValueCore(value);
		}

		private void SetValueCore(byte[] value)
		{
			if (value != null)
			{
				base.Xml.Root.ReplaceAll(Convert.ToBase64String(value));
			}
			else
			{
				base.Xml.Root.ReplaceAll(string.Empty);
			}
		}
	}
}
