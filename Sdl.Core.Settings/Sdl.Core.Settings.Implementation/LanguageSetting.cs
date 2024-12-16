using Sdl.Core.Globalization;
using Sdl.Core.Settings.Implementation.Xml;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Sdl.Core.Settings.Implementation
{
	internal class LanguageSetting : XmlSettingImpl<Language>
	{
		private const string IsoAbbreviationElementName = "IsoAbbreviation";

		public override Language Value
		{
			get
			{
				XElement xElement = base.Xml.Root.Elements().FirstOrDefault();
				if (xElement != null)
				{
					Language language = new Language();
					using (XmlReader xmlReader = xElement.CreateReader())
					{
						xmlReader.MoveToContent();
						ReadXml(language, xmlReader);
						return language;
					}
				}
				return null;
			}
			set
			{
				SetValueCore(value);
				OnXmlUpdated();
			}
		}

		public LanguageSetting(SettingsGroup settingsGroup, Setting setting, bool inherited)
			: base(settingsGroup, setting, inherited)
		{
		}

		public LanguageSetting(SettingsGroup settingsGroup, string settingId, Language value, bool inherited)
			: base(settingsGroup, settingId, inherited)
		{
			SetValueCore(value);
		}

		private static void ReadXml(Language language, XmlReader reader)
		{
			language.IsoAbbreviation = reader.ReadElementContentAsString("IsoAbbreviation", string.Empty);
		}

		private static void WriteXml(Language language, XmlWriter writer)
		{
			writer.WriteElementString("IsoAbbreviation", language.IsoAbbreviation);
		}

		private void SetValueCore(Language value)
		{
			XElement root = base.Xml.Root;
			if (value != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true;
				using (XmlWriter writer = XmlWriter.Create(stringBuilder, xmlWriterSettings))
				{
					WriteXml(value, writer);
				}
				XDocument xDocument = XDocument.Load(new StringReader(stringBuilder.ToString()), LoadOptions.None);
				root.ReplaceAll(xDocument.Root);
			}
			else
			{
				root.ReplaceAll(string.Empty);
			}
		}
	}
}
