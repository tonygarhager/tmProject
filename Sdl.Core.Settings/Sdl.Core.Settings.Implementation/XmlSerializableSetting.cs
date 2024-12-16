using Sdl.Core.Settings.Implementation.Xml;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Sdl.Core.Settings.Implementation
{
	internal class XmlSerializableSetting<T> : XmlSettingImpl<T>
	{
		public override T Value
		{
			get
			{
				XElement xElement = base.Xml.Root.Elements().FirstOrDefault();
				if (xElement != null)
				{
					T val = (T)Activator.CreateInstance(typeof(T));
					IXmlSerializable xmlSerializable = (IXmlSerializable)(object)val;
					using (XmlReader xmlReader = xElement.CreateReader())
					{
						xmlReader.MoveToContent();
						xmlSerializable.ReadXml(xmlReader);
					}
					return (T)xmlSerializable;
				}
				return default(T);
			}
			set
			{
				SetValueCore(value);
				OnXmlUpdated();
			}
		}

		public XmlSerializableSetting(SettingsGroup settingsGroup, Setting setting, bool inherited)
			: base(settingsGroup, setting, inherited)
		{
		}

		public XmlSerializableSetting(SettingsGroup settingsGroup, string settingId, T value, bool inherited)
			: base(settingsGroup, settingId, inherited)
		{
			SetValueCore(value);
		}

		private void SetValueCore(T value)
		{
			XElement root = base.Xml.Root;
			if (value != null)
			{
				IXmlSerializable xmlSerializable = (IXmlSerializable)(object)value;
				StringBuilder stringBuilder = new StringBuilder();
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true;
				using (XmlWriter writer = XmlWriter.Create(stringBuilder, xmlWriterSettings))
				{
					xmlSerializable.WriteXml(writer);
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
