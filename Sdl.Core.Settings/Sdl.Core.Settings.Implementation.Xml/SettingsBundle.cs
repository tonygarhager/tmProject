using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Settings.Implementation.Xml
{
	internal class SettingsBundle : IXmlSerializable
	{
		private List<SettingsGroup> _settingsGroups = new List<SettingsGroup>();

		public List<SettingsGroup> SettingsGroups
		{
			get
			{
				return _settingsGroups;
			}
			set
			{
				_settingsGroups = value;
			}
		}

		public SettingsBundle()
		{
		}

		public SettingsBundle(SettingsBundle other)
		{
			foreach (SettingsGroup settingsGroup in other._settingsGroups)
			{
				_settingsGroups.Add(new SettingsGroup(settingsGroup));
			}
		}

		public XmlSchema GetSchema()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadToFollowing("SettingsBundle");
			while (reader.ReadToFollowing("SettingsGroup"))
			{
				XmlReader xmlReader = reader.ReadSubtree();
				SettingsGroup settingsGroup = new SettingsGroup();
				settingsGroup.ReadXml(xmlReader);
				_settingsGroups.Add(settingsGroup);
				xmlReader.Close();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("SettingsBundle");
			IEnumerator<SettingsGroup> enumerator = _settingsGroups.ToList().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Settings.Count > 0)
				{
					enumerator.Current.WriteXml(writer);
				}
			}
			writer.WriteEndElement();
		}

		public void RemoveSettingsGroup(string id)
		{
			SettingsGroup settingsGroup = _settingsGroups.FirstOrDefault((SettingsGroup group) => group.Id == id);
			if (settingsGroup != null)
			{
				_settingsGroups.Remove(settingsGroup);
			}
		}

		public object Clone()
		{
			return new SettingsBundle(this);
		}
	}
}
