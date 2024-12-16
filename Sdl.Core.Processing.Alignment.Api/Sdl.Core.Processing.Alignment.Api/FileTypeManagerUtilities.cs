using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Sdl.Core.Processing.Alignment.Api
{
	public static class FileTypeManagerUtilities
	{
		private enum ConfigurationFileType
		{
			Unknown,
			Template,
			FileTypeSettings
		}

		public static void Configure(IFileTypeManager fileTypeManager, string configurationFilePath)
		{
			if (fileTypeManager == null)
			{
				throw new ArgumentNullException("fileTypeManager");
			}
			if (string.IsNullOrEmpty(configurationFilePath))
			{
				throw new ArgumentOutOfRangeException("configurationFilePaths", "Parameter must not contain null or empty strings");
			}
			FileInfo fileInfo = new FileInfo(configurationFilePath);
			if (!fileInfo.Exists)
			{
				throw new ArgumentOutOfRangeException("configurationFilePaths", "At least one of the specified configuration files does not exist.");
			}
			switch (DetermineType(fileInfo))
			{
			case ConfigurationFileType.Unknown:
				throw new ArgumentOutOfRangeException("configurationFilePaths", "At least one configuration file is not supported");
			case ConfigurationFileType.Template:
				ApplyTemplate(fileTypeManager, fileInfo);
				break;
			case ConfigurationFileType.FileTypeSettings:
				ApplyFileTypeSettings(fileTypeManager, fileInfo);
				break;
			default:
				throw new Exception("Unknown enum");
			}
		}

		private static ConfigurationFileType DetermineType(FileInfo configurationFile)
		{
			switch (configurationFile.Extension.ToLowerInvariant())
			{
			case ".sdltpl":
				return ConfigurationFileType.Template;
			case ".sdlftsettings":
				return ConfigurationFileType.FileTypeSettings;
			default:
				return ConfigurationFileType.Unknown;
			}
		}

		private static void ApplyTemplate(IFileTypeManager fileTypeManager, FileInfo configurationFile)
		{
			ISettingsBundle settingsBundle2 = fileTypeManager.SettingsBundle = ReadSettingsFromTemplate(configurationFile);
		}

		private static ISettingsBundle ReadSettingsFromTemplate(FileInfo file)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Auto
			};
			using (XmlReader reader = XmlReader.Create(file.FullName, settings))
			{
				XPathDocument xPathDocument = new XPathDocument(reader);
				XPathNodeIterator xPathNodeIterator = xPathDocument.CreateNavigator().Select("/ProjectTemplate/SettingsBundles");
				if (xPathNodeIterator.MoveNext())
				{
					using (XmlReader reader2 = xPathNodeIterator.Current.ReadSubtree())
					{
						return SettingsUtil.DeserializeSettingsBundle(reader2, null);
					}
				}
				return null;
			}
		}

		private static void ApplyFileTypeSettings(IFileTypeManager fileTypeManager, FileInfo configurationFile)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Auto
			};
			using (XmlReader reader = XmlReader.Create(configurationFile.FullName, settings))
			{
				ISettingsBundle settingsBundle2 = fileTypeManager.SettingsBundle = SettingsUtil.DeserializeSettingsBundle(reader, null);
			}
		}
	}
}
