using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Settings.Implementation.Xml
{
	internal class SettingsGroup : IXmlSerializable
	{
		private string _id;

		private Dictionary<string, Setting> _settings = new Dictionary<string, Setting>();

		private Dictionary<string, Setting> _backupSettings;

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

		public Dictionary<string, Setting> Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = value;
			}
		}

		public bool IsEditing => _backupSettings != null;

		public SettingsGroup()
		{
		}

		public SettingsGroup(SettingsGroup other)
		{
			_id = other._id;
			foreach (Setting value in other._settings.Values)
			{
				_settings.Add(value.Id, new Setting(value));
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadToFollowing("SettingsGroup");
			_id = reader.GetAttribute("Id");
			while (reader.ReadToFollowing("Setting"))
			{
				XmlReader xmlReader = reader.ReadSubtree();
				Setting setting = new Setting();
				setting.ReadXml(xmlReader);
				_settings.Add(setting.Id, setting);
				xmlReader.Close();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("SettingsGroup");
			writer.WriteAttributeString("Id", RemoveControlCharacters(_id));
			foreach (Setting value in _settings.Values)
			{
				value.WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		private static string RemoveControlCharacters(string inString)
		{
			if (inString == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in inString)
			{
				if (!char.IsControl(c))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public void BeginEdit()
		{
			if (_backupSettings == null)
			{
				_backupSettings = new Dictionary<string, Setting>();
				foreach (Setting value in _settings.Values)
				{
					_backupSettings.Add(value.Id, (Setting)value.Clone());
				}
			}
		}

		public void CancelEdit()
		{
			if (_backupSettings != null)
			{
				_settings = _backupSettings;
				_backupSettings = null;
			}
		}

		public void EndEdit()
		{
			if (_backupSettings != null)
			{
				_backupSettings = null;
			}
		}

		public object Clone()
		{
			return new SettingsGroup(this);
		}
	}
}
