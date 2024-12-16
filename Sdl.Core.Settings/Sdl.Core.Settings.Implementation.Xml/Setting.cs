using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Settings.Implementation.Xml
{
	internal class Setting : IXmlSerializable
	{
		private string _id;

		private XDocument _xml;

		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public XDocument Xml
		{
			get
			{
				return _xml;
			}
			set
			{
				_xml = value;
			}
		}

		public static Setting CreateSetting(string Id)
		{
			Setting setting = new Setting();
			setting.Id = Id;
			XDocument xDocument = new XDocument();
			xDocument.Add(new XElement(XName.Get("Setting")));
			setting.Xml = xDocument;
			return setting;
		}

		public Setting()
		{
		}

		public Setting(Setting copy)
		{
			_id = copy._id;
			_xml = new XDocument(copy.Xml);
		}

		public string GetXmlString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(stringBuilder))
			{
				Xml.WriteTo(writer);
			}
			return stringBuilder.ToString();
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadToFollowing("Setting");
			_id = reader.GetAttribute("Id");
			_xml = XDocument.Load(reader.ReadSubtree(), LoadOptions.None);
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Setting");
			writer.WriteAttributeString("Id", _id);
			if (_xml != null)
			{
				foreach (XNode item in _xml.Root.Nodes())
				{
					item.WriteTo(writer);
				}
			}
			writer.WriteEndElement();
		}

		public object Clone()
		{
			return new Setting(this);
		}
	}
}
