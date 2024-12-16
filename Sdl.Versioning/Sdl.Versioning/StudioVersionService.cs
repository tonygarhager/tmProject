using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Sdl.Versioning
{
	public class StudioVersionService : IStudioVersionService
	{
		private const string InstallLocation64Bit = "SOFTWARE\\Wow6432Node\\SDL\\";

		private const string InstallLocation32Bit = "SOFTWARE\\SDL";

		private readonly List<StudioVersion> _installedStudioVersions;

		private readonly StudioVersion DefaultStudioVersion;

		public StudioVersionService()
		{
			_installedStudioVersions = new List<StudioVersion>();
			DefaultStudioVersion = new StudioVersion
			{
				Version = "Studio16",
				PublicVersion = Versions.BaseProductDescription,
				InstallPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\SDL\\SDL Trados Studio\\Studio16\\",
				Edition = "",
				ExecutableVersion = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion),
				StudioDocumentsFolderName = "Studio 2021",
				ShortVersion = "2021"
			};
			Initialize();
		}

		public List<StudioVersion> GetInstalledStudioVersions()
		{
			return _installedStudioVersions;
		}

		public bool StudioVersionSuported(Version minVersion, Version maxVersion, Version studioVersion)
		{
			if (studioVersion >= minVersion)
			{
				if (!(maxVersion == null))
				{
					return studioVersion <= maxVersion;
				}
				return true;
			}
			return false;
		}

		public StudioVersion GetStudioVersion()
		{
			StudioVersion studioVersion = _installedStudioVersions.Where((StudioVersion v) => v.ExecutableVersion.Major == 16 && v.Edition == "")?.FirstOrDefault();
			if (studioVersion == null)
			{
				studioVersion = DefaultStudioVersion;
			}
			return studioVersion;
		}

		private void Initialize()
		{
			PopulateStudioVersions();
		}

		private void PopulateStudioVersions()
		{
			string text = Environment.Is64BitOperatingSystem ? "SOFTWARE\\Wow6432Node\\SDL\\" : "SOFTWARE\\SDL";
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text);
			if (registryKey != null)
			{
				string[] subKeyNames = registryKey.GetSubKeyNames();
				foreach (string text2 in subKeyNames)
				{
					foreach (KeyValuePair<string, string> knownStudioVersion in Versions.KnownStudioVersions)
					{
						if (text2.StartsWith(knownStudioVersion.Key))
						{
							string edition = text2.Substring(knownStudioVersion.Key.Length);
							FindAndCreateStudioVersion(text, knownStudioVersion.Key, knownStudioVersion.Value, edition);
						}
					}
				}
			}
		}

		private void FindAndCreateStudioVersion(string registryPath, string studioVersion, string studioPublicVersion, string edition)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryPath + "\\" + studioVersion + edition);
			if (registryKey != null)
			{
				CreateStudioVersion(registryKey, studioVersion, studioPublicVersion, edition);
			}
		}

		private void CreateStudioVersion(RegistryKey studioKey, string version, string publicVersion, string edition)
		{
			if (studioKey.GetValue("InstallLocation") != null)
			{
				try
				{
					string text = studioKey.GetValue("InstallLocation").ToString();
					string studioFullVersion = GetStudioFullVersion(text);
					_installedStudioVersions.Add(new StudioVersion
					{
						Version = version,
						PublicVersion = publicVersion,
						InstallPath = text,
						Edition = edition,
						ExecutableVersion = new Version(studioFullVersion),
						ShortVersion = GetShortVersion(publicVersion),
						StudioDocumentsFolderName = GetDocumentsFolderName(publicVersion)
					});
				}
				catch
				{
				}
			}
		}

		private static string GetStudioFullVersion(string installLocation)
		{
			return FileVersionInfo.GetVersionInfo(Assembly.LoadFile(string.Format("{0}\\{1}", installLocation, "SDLTradosStudio.exe"))?.Location)?.FileVersion;
		}

		private static string GetDocumentsFolderName(string publicVersion)
		{
			return publicVersion.Substring(11);
		}

		private static string GetShortVersion(string publicVersion)
		{
			return publicVersion.Substring(18);
		}
	}
}
