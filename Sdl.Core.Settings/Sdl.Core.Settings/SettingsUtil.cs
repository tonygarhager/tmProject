using Newtonsoft.Json;
using Sdl.Core.Settings.Implementation;
using Sdl.Core.Settings.Implementation.Json;
using Sdl.Core.Settings.Implementation.Xml;
using System.Xml;

namespace Sdl.Core.Settings
{
	public static class SettingsUtil
	{
		public static ISettingsBundle DeserializeSettingsBundle(XmlReader reader, ISettingsBundle parent, bool isDefault)
		{
			Sdl.Core.Settings.Implementation.Xml.SettingsBundle settingsBundle = new Sdl.Core.Settings.Implementation.Xml.SettingsBundle();
			using (XmlReader reader2 = XmlReader.Create(reader, new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Auto,
				IgnoreWhitespace = true
			}))
			{
				settingsBundle.ReadXml(reader2);
			}
			return new Sdl.Core.Settings.Implementation.SettingsBundle(settingsBundle, parent, isDefault);
		}

		public static ISettingsBundle DeserializeSettingsBundle(XmlReader reader, ISettingsBundle parent)
		{
			return DeserializeSettingsBundle(reader, parent, isDefault: false);
		}

		public static void SerializeSettingsBundle(XmlWriter writer, ISettingsBundle iSettingsBundle)
		{
			SerializeSettingsBundle(writer, iSettingsBundle, includeInheritedSettings: false);
		}

		public static void SerializeSettingsBundle(XmlWriter writer, ISettingsBundle iSettingsBundle, bool includeInheritedSettings)
		{
			Sdl.Core.Settings.Implementation.SettingsBundle settingsBundle = (Sdl.Core.Settings.Implementation.SettingsBundle)iSettingsBundle;
			settingsBundle.WriteXml(writer, includeInheritedSettings);
		}

		public static ISettingsBundle CreateSettingsBundle(ISettingsBundle parent)
		{
			return CreateSettingsBundle(parent, isDefault: false);
		}

		public static string SerializeJsonSettingsBundle(ISettingsBundle jsonSettingsBundle)
		{
			return JsonConvert.SerializeObject(jsonSettingsBundle);
		}

		public static ISettingsBundle DeserializeJsonSettingsBundle(string jsonSettingsBundle)
		{
			JsonSettingsBundle jsonSettingsBundle2 = JsonConvert.DeserializeObject<JsonSettingsBundle>(jsonSettingsBundle);
			jsonSettingsBundle2.RemoveNullGroups();
			jsonSettingsBundle2.FixUpBundleRefs();
			return jsonSettingsBundle2;
		}

		public static ISettingsBundle CreateJsonSettingsBundle(ISettingsBundle parent)
		{
			return new JsonSettingsBundle(parent, isDefault: false);
		}

		public static ISettingsBundle CreateSettingsBundle(ISettingsBundle parent, ISettingsBundle source)
		{
			Sdl.Core.Settings.Implementation.SettingsBundle settingsBundle = (Sdl.Core.Settings.Implementation.SettingsBundle)source;
			Sdl.Core.Settings.Implementation.Xml.SettingsBundle xmlBundle = new Sdl.Core.Settings.Implementation.Xml.SettingsBundle(settingsBundle.XmlBundle);
			return new Sdl.Core.Settings.Implementation.SettingsBundle(xmlBundle, parent, isDefault: false);
		}

		public static ISettingsBundle CreateSettingsBundle(ISettingsBundle parent, bool isDefault)
		{
			Sdl.Core.Settings.Implementation.Xml.SettingsBundle xmlBundle = new Sdl.Core.Settings.Implementation.Xml.SettingsBundle();
			return new Sdl.Core.Settings.Implementation.SettingsBundle(xmlBundle, parent, isDefault);
		}
	}
}
