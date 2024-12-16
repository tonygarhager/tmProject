using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Sdl.Versioning
{
	public static class Versions
	{
		public const int MajorVersion = 16;

		public const string Company = "SDL";

		public const string ProductHelpUrI = "https://docs.sdl.com/SDL_Trados_Studio_2021";

		public const string BaseProductIdentifier = "Studio16";

		public const string ProductIdentifier = "Studio16";

		public const string SamplesVersionId = "Samples16";

		public const string Edition = "";

		public const string StudioDocumentsFolderName = "Studio 2021";

		public const string ProjectTemplatesVersionId = "ProjectTemplates16";

		public const string LicensingVersion = "2021";

		public const string CopyrightYear = "2020";

		public const string ProductYear = "2021";

		public const string TermbaseDefinitionCLSID = "BA0FA330-AC80-4DCB-8F86-6D47BFE10B1E";

		public const string MultiTermApplicationCLSID = "DE9C39C5-7957-46C5-9DC9-B1B68CA55577";

		public const string BaseProductName = "SDL Trados Studio";

		public const string MarketingProductName = "SDL Trados Studio 2021";

		public const string ProductNameWithEdition = "SDL Trados Studio {0}";

		public const string PluginPackagePath = "SDL\\SDL Trados Studio\\{0}\\Plugins\\Packages";

		public const string PluginUnpackPath = "SDL\\SDL Trados Studio\\{0}\\Plugins\\Unpacked";

		public static readonly Dictionary<string, string> KnownStudioVersions = new Dictionary<string, string>
		{
			{
				"Studio2",
				"SDL Trados Studio 2011"
			},
			{
				"Studio3",
				"SDL Trados Studio 2014"
			},
			{
				"Studio4",
				"SDL Trados Studio 2015"
			},
			{
				"Studio5",
				"SDL Trados Studio 2017"
			},
			{
				"Studio15",
				"SDL Trados Studio 2019"
			},
			{
				"Studio16",
				"SDL Trados Studio 2021"
			},
			{
				"Studio17",
				"SDL Trados Studio Next"
			}
		};

		public static string Copyright => string.Format(VersionResources.Copyright, "2020");

		public static string ProductDescription
		{
			get
			{
				if (string.IsNullOrEmpty(""))
				{
					return string.Format(VersionResources.ProductDescription, "2021");
				}
				return string.Format(VersionResources.ProductDescriptionWithEdition, "2021", "");
			}
		}

		public static string BaseProductDescription => string.Format(VersionResources.ProductDescription, "2021");

		public static string GetBuildVersion()
		{
			string text = string.Empty;
			string text2 = string.Empty;
			try
			{
				Assembly entryAssembly = Assembly.GetEntryAssembly();
				if (entryAssembly != null)
				{
					string location = entryAssembly.Location;
					object[] customAttributes = entryAssembly.GetCustomAttributes(inherit: true);
					text = FileVersionInfo.GetVersionInfo(location).FileVersion;
					text2 = customAttributes.OfType<AssemblyConfigurationAttribute>().FirstOrDefault().Configuration;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + " (" + text2 + ")";
			}
			return text;
		}
	}
}
