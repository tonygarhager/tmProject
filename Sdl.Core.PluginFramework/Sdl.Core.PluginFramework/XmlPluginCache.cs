using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Sdl.Core.PluginFramework
{
	public class XmlPluginCache : IPluginCache
	{
		private const string PLUGINCACHE_ELEMENT = "PluginCache";

		private const string PLUGINSTATE_ELEMENT = "PluginState";

		private const string PLUGINID_ATTRIBUTE = "pluginId";

		private const string ENABLED_ATTRIBUTE = "enabled";

		private readonly string _pluginCacheFilePath;

		private Dictionary<string, PluginState> _pluginStates;

		public string PluginCacheFilePath => _pluginCacheFilePath;

		public XmlPluginCache(string pluginCacheFilePath)
		{
			_pluginCacheFilePath = pluginCacheFilePath;
			Load();
		}

		public PluginState GetPluginState(string pluginId)
		{
			if (!_pluginStates.TryGetValue(pluginId, out PluginState value))
			{
				return new PluginState(enabled: true);
			}
			return value;
		}

		public void StorePluginState(IPlugin plugin)
		{
			_pluginStates[plugin.Id.Id] = new PluginState(plugin.Enabled);
		}

		public void Save()
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				Indent = true
			};
			using (Stream output = File.Create(_pluginCacheFilePath))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(output, settings))
				{
					xmlWriter.WriteStartDocument();
					xmlWriter.WriteStartElement("PluginCache");
					foreach (KeyValuePair<string, PluginState> pluginState in _pluginStates)
					{
						xmlWriter.WriteStartElement("PluginState");
						xmlWriter.WriteAttributeString("pluginId", pluginState.Key);
						xmlWriter.WriteAttributeString("enabled", pluginState.Value.Enabled.ToString());
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
					xmlWriter.WriteEndDocument();
				}
			}
		}

		private void Load()
		{
			_pluginStates = new Dictionary<string, PluginState>();
			if (File.Exists(_pluginCacheFilePath))
			{
				XmlReader xmlReader = null;
				try
				{
					xmlReader = XmlReader.Create(_pluginCacheFilePath);
					while (xmlReader.Read())
					{
						if (xmlReader.Name == "PluginState")
						{
							string attribute = xmlReader.GetAttribute("pluginId");
							bool enabled = Convert.ToBoolean(xmlReader.GetAttribute("enabled"), CultureInfo.InvariantCulture);
							_pluginStates[attribute] = new PluginState(enabled);
						}
					}
				}
				catch (XmlException)
				{
					if (_pluginStates.Count > 0)
					{
						_pluginStates.Clear();
					}
					if (xmlReader != null)
					{
						xmlReader.Close();
						xmlReader = null;
					}
					string backupFileName = GetBackupFileName(_pluginCacheFilePath);
					File.Move(_pluginCacheFilePath, backupFileName);
				}
				finally
				{
					if (xmlReader != null)
					{
						xmlReader.Close();
						xmlReader = null;
					}
				}
			}
		}

		private static string GetBackupFileName(string filePath)
		{
			string searchPattern = Path.GetFileName(filePath) + ".*";
			int num = 1;
			foreach (string item in Directory.EnumerateFiles(Path.GetDirectoryName(filePath), searchPattern))
			{
				string extension = Path.GetExtension(item);
				if (!string.IsNullOrEmpty(extension) && extension.Length > 1)
				{
					extension = extension.Substring(1);
					int result = 0;
					if (int.TryParse(extension, out result) && num <= result)
					{
						num = result + 1;
					}
				}
			}
			return filePath + $".{num}";
		}
	}
}
