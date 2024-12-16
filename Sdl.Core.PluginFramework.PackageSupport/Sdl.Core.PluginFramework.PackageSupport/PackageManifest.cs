using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Sdl.Core.PluginFramework.PackageSupport
{
	public class PackageManifest : IEquatable<object>
	{
		public const string MANIFEST_FILENAME = "pluginpackage.manifest.xml";

		private const string THIRDPARTY_MANIFEST_NAMESPACE = "http://www.sdl.com/Plugins/PluginPackage/1.0";

		private const string ROOT_ELEMENT = "PluginPackage";

		private const string AUTHOR_ELEMENT = "Author";

		private const string DESCRIPTION_ELEMENT = "Description";

		private const string PLUGINNAME_ELEMENT = "PlugInName";

		private const string REQUIREDPRODUCT_ELEMENT = "RequiredProduct";

		private const string MINVERSION_ATTRIBUTE = "minversion";

		private const string MAXVERSION_ATTRIBUTE = "maxversion";

		private const string VERSION_ELEMENT = "Version";

		private const string PRODUCTNAME_ATTRIBUTE = "name";

		private const string INCLUDE = "Include";

		private const string FILE = "File";

		public bool LoadedSucessfully
		{
			get;
			private set;
		}

		public string Author
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string PlugInName
		{
			get;
			set;
		}

		public Version Version
		{
			get;
			set;
		}

		public Version MinRequiredProductVersion
		{
			get;
			set;
		}

		public Version MaxRequiredProductVersion
		{
			get;
			set;
		}

		public string RequiredProductName
		{
			get;
			set;
		}

		public IEnumerable<string> AdditionalFiles
		{
			get;
			set;
		}

		public PackageManifest()
		{
		}

		public PackageManifest(string packageManifestFile)
		{
			if (packageManifestFile == null)
			{
				throw new ArgumentNullException("packageManifestFile");
			}
			try
			{
				if (File.Exists(packageManifestFile))
				{
					using (FileStream manifestStream = File.Open(packageManifestFile, FileMode.Open, FileAccess.Read))
					{
						LoadedSucessfully = LoadXmlManifest(manifestStream);
					}
				}
			}
			catch
			{
				LoadedSucessfully = false;
			}
		}

		public PackageManifest(Stream pluginManifestStream)
		{
			if (pluginManifestStream == null)
			{
				throw new ArgumentNullException("pluginManifestStream");
			}
			using (pluginManifestStream)
			{
				LoadedSucessfully = LoadXmlManifest(pluginManifestStream);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			PackageManifest packageManifest = (PackageManifest)obj;
			if (Author.Equals(packageManifest.Author) && object.Equals(Description, packageManifest.Description) && object.Equals(MinRequiredProductVersion, packageManifest.MinRequiredProductVersion) && object.Equals(MaxRequiredProductVersion, packageManifest.MaxRequiredProductVersion) && object.Equals(PlugInName, packageManifest.PlugInName) && object.Equals(RequiredProductName, packageManifest.RequiredProductName) && object.Equals(Version, packageManifest.Version))
			{
				return object.Equals(AdditionalFiles, packageManifest.AdditionalFiles);
			}
			return false;
		}

		public override int GetHashCode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Author);
			stringBuilder.Append(Description);
			stringBuilder.Append(MinRequiredProductVersion);
			stringBuilder.Append(PlugInName);
			stringBuilder.Append(RequiredProductName);
			stringBuilder.Append(Version);
			return stringBuilder.ToString().GetHashCode();
		}

		public void Save(Stream toStream)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(toStream, new XmlWriterSettings
			{
				Indent = true,
				Encoding = Encoding.UTF8
			}))
			{
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("PluginPackage", "http://www.sdl.com/Plugins/PluginPackage/1.0");
				xmlWriter.WriteElementString("PlugInName", "http://www.sdl.com/Plugins/PluginPackage/1.0", PlugInName);
				xmlWriter.WriteElementString("Version", "http://www.sdl.com/Plugins/PluginPackage/1.0", Version.ToString());
				xmlWriter.WriteElementString("Description", "http://www.sdl.com/Plugins/PluginPackage/1.0", Description);
				xmlWriter.WriteElementString("Author", "http://www.sdl.com/Plugins/PluginPackage/1.0", Author);
				xmlWriter.WriteStartElement("RequiredProduct");
				xmlWriter.WriteAttributeString("name", RequiredProductName);
				xmlWriter.WriteAttributeString("minversion", MinRequiredProductVersion.ToString());
				if (MaxRequiredProductVersion != null)
				{
					xmlWriter.WriteAttributeString("maxversion", MaxRequiredProductVersion.ToString());
				}
				xmlWriter.WriteEndElement();
				if (AdditionalFiles != null && AdditionalFiles.Any())
				{
					xmlWriter.WriteStartElement("Include");
					foreach (string additionalFile in AdditionalFiles)
					{
						xmlWriter.WriteStartElement("File");
						xmlWriter.WriteString(additionalFile);
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
				xmlWriter.Flush();
			}
		}

		private bool LoadXmlManifest(Stream manifestStream)
		{
			XDocument xDocument;
			try
			{
				using (XmlReader reader = XmlReader.Create(manifestStream))
				{
					xDocument = XDocument.Load(reader);
				}
			}
			catch
			{
				return false;
			}
			XElement root = xDocument.Root;
			if (root.Name.LocalName != "PluginPackage")
			{
				return false;
			}
			Author = GetRequiredElementContent(root, "Author");
			Description = GetRequiredElementContent(root, "Description");
			PlugInName = GetRequiredElementContent(root, "PlugInName");
			string requiredElementContent = GetRequiredElementContent(root, "Version");
			if (requiredElementContent != null)
			{
				Version = new Version(requiredElementContent);
				XElement xElement = root.Element(XName.Get("RequiredProduct", "http://www.sdl.com/Plugins/PluginPackage/1.0"));
				if (xElement == null)
				{
					return false;
				}
				RequiredProductName = GetAttribute(xElement, "name");
				if (RequiredProductName == null)
				{
					return false;
				}
				string attribute = GetAttribute(xElement, "minversion");
				if (attribute == null)
				{
					return false;
				}
				MinRequiredProductVersion = new Version(attribute);
				string attribute2 = GetAttribute(xElement, "maxversion");
				if (attribute2 != null)
				{
					MaxRequiredProductVersion = new Version(attribute2);
				}
				XElement xElement2 = root.Element(XName.Get("Include", "http://www.sdl.com/Plugins/PluginPackage/1.0"));
				if (xElement2 != null)
				{
					AdditionalFiles = (from el in xElement2.Elements(XName.Get("File", "http://www.sdl.com/Plugins/PluginPackage/1.0"))
						where !string.IsNullOrWhiteSpace(el.Value)
						select el.Value.Trim()).ToList();
				}
				else
				{
					AdditionalFiles = new List<string>();
				}
				return true;
			}
			return false;
		}

		private string GetAttribute(XElement rootElement, string attributeName)
		{
			XAttribute xAttribute = rootElement.Attribute(XName.Get(attributeName));
			if (xAttribute == null || string.IsNullOrEmpty(xAttribute.Value))
			{
				return null;
			}
			return xAttribute.Value;
		}

		private string GetRequiredElementContent(XElement rootElement, string attributeId)
		{
			XElement xElement = rootElement.Element(XName.Get(attributeId, "http://www.sdl.com/Plugins/PluginPackage/1.0"));
			if (xElement != null && !string.IsNullOrEmpty(xElement.Value))
			{
				return xElement.Value;
			}
			return null;
		}
	}
}
