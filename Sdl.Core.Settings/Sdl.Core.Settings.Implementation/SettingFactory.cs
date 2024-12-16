using Sdl.Core.Globalization;
using Sdl.Core.Settings.Implementation.Xml;
using System;
using System.Xml.Serialization;

namespace Sdl.Core.Settings.Implementation
{
	internal static class SettingFactory
	{
		public static Setting<T> CreateSetting<T>(SettingsGroup settingsGroup, Setting setting, bool inherited)
		{
			if (typeof(Language).Equals(typeof(T)))
			{
				return (Setting<T>)(object)new LanguageSetting(settingsGroup, setting, inherited);
			}
			if (typeof(IXmlSerializable).IsAssignableFrom(typeof(T)))
			{
				return new XmlSerializableSetting<T>(settingsGroup, setting, inherited);
			}
			if (typeof(T) == typeof(byte[]))
			{
				return (Setting<T>)(object)new ByteArraySetting(settingsGroup, setting, inherited);
			}
			if (TypeConverterUtil.CanConvertToString(typeof(T)))
			{
				try
				{
					SimpleSetting<T> simpleSetting = new SimpleSetting<T>(settingsGroup, setting, inherited);
					T value = simpleSetting.Value;
					return simpleSetting;
				}
				catch (Exception)
				{
					return null;
				}
			}
			if (typeof(T).IsArray)
			{
				return new ArraySetting<T>(settingsGroup, setting, inherited);
			}
			return new DataContractSerializableSetting<T>(settingsGroup, setting, inherited);
		}

		public static Setting<T> CreateSetting<T>(SettingsGroup settingsGroup, string settingId, T defaultValue, bool inherited)
		{
			if (typeof(Language).Equals(typeof(T)))
			{
				return (Setting<T>)(object)new LanguageSetting(settingsGroup, settingId, (Language)(object)defaultValue, inherited);
			}
			if (typeof(IXmlSerializable).IsAssignableFrom(typeof(T)))
			{
				return new XmlSerializableSetting<T>(settingsGroup, settingId, defaultValue, inherited);
			}
			if (typeof(T) == typeof(byte[]))
			{
				return (Setting<T>)(object)new ByteArraySetting(settingsGroup, settingId, (byte[])(object)defaultValue, inherited);
			}
			if (TypeConverterUtil.CanConvertToString(typeof(T)))
			{
				return new SimpleSetting<T>(settingsGroup, settingId, defaultValue, inherited);
			}
			if (typeof(T).IsArray)
			{
				return new ArraySetting<T>(settingsGroup, settingId, defaultValue, inherited);
			}
			return new DataContractSerializableSetting<T>(settingsGroup, settingId, defaultValue, inherited);
		}

		public static bool IsSimpleSettingValue(object value)
		{
			if (value == null)
			{
				return false;
			}
			Type type = value.GetType();
			return TypeConverterUtil.CanConvertToString(type);
		}

		public static string GetSimpleSettingValueAsString(object value)
		{
			if (value == null)
			{
				return null;
			}
			string text = value as string;
			if (text != null)
			{
				return text;
			}
			Type type = value.GetType();
			return TypeConverterUtil.ConvertToInvariantString(value);
		}

		public static object DeserializeSettingValue(Type valueType, Setting xmlSetting)
		{
			return TypeConverterUtil.ConvertFromInvariantString(xmlSetting.Xml.Root.Value, valueType);
		}
	}
}
