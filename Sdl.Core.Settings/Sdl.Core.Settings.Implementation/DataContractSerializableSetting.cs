using Sdl.Core.Settings.Implementation.Xml;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Sdl.Core.Settings.Implementation
{
	internal class DataContractSerializableSetting<T> : XmlSettingImpl<T>
	{
		private DataContractSerializer _serializer;

		public override T Value
		{
			get
			{
				XElement xElement = base.Xml.Root.Elements().FirstOrDefault();
				if (xElement != null)
				{
					using (XmlReader xmlReader = xElement.CreateReader())
					{
						xmlReader.MoveToContent();
						return (T)_serializer.ReadObject(xmlReader);
					}
				}
				return default(T);
			}
			set
			{
				SetValueCore(value);
				OnXmlUpdated();
			}
		}

		public DataContractSerializableSetting(SettingsGroup settingsGroup, Setting setting, bool inherited)
			: base(settingsGroup, setting, inherited)
		{
			_serializer = new DataContractSerializer(typeof(T));
		}

		public DataContractSerializableSetting(SettingsGroup settingsGroup, string settingId, T value, bool inherited)
			: base(settingsGroup, settingId, inherited)
		{
			_serializer = new DataContractSerializer(typeof(T));
			SetValueCore(value);
		}

		private void SetValueCore(T value)
		{
			XElement root = base.Xml.Root;
			if (value != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true;
				using (RemoveControlCharactersXmlWriter writer = new RemoveControlCharactersXmlWriter(XmlWriter.Create(stringBuilder, xmlWriterSettings)))
				{
					_serializer.WriteObject(writer, value);
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
