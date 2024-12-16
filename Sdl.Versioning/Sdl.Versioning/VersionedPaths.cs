using Microsoft.Win32;
using System;
using System.IO;

namespace Sdl.Versioning
{
	public static class VersionedPaths
	{
		public static string UserAppDataPath
		{
			get
			{
				string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SDL\\SDL Trados Studio\\Studio16";
				Directory.CreateDirectory(text);
				return text;
			}
		}

		public static string LocalUserAppDataPath
		{
			get
			{
				string text = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SDL\\SDL Trados Studio\\Studio16";
				Directory.CreateDirectory(text);
				return text;
			}
		}

		public static string UnversionedUserAppDataPath
		{
			get
			{
				string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SDL\\SDL Trados Studio";
				Directory.CreateDirectory(text);
				return text;
			}
		}

		public static string CompanyAppDataPath
		{
			get
			{
				string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SDL";
				Directory.CreateDirectory(text);
				return text;
			}
		}

		public static string RetailProgramDataPath
		{
			get
			{
				string text = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SDL\\SDL Trados Studio\\Studio16";
				Directory.CreateDirectory(text);
				return text;
			}
		}

		public static string ProgramDataPath
		{
			get
			{
				string text = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SDL\\SDL Trados Studio\\Studio16";
				Directory.CreateDirectory(text);
				return text;
			}
		}

		public static RegistryKey UserAppDataRegistry
		{
			get
			{
				string text = "SOFTWARE\\SDL\\SDL Trados Studio\\Studio16";
				RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
				RegistryKey registryKey2 = registryKey.OpenSubKey(text, writable: true);
				if (registryKey2 == null)
				{
					registryKey2 = registryKey.CreateSubKey(text, writable: true);
				}
				return registryKey2;
			}
		}
	}
}
