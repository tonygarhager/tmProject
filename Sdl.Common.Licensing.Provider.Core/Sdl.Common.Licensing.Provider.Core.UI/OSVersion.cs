using System;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace Sdl.Common.Licensing.Provider.Core.UI
{
	public class OSVersion
	{
		private struct OSVERSIONINFOEX
		{
			public int dwOSVersionInfoSize;

			public int dwMajorVersion;

			public int dwMinorVersion;

			public int dwBuildNumber;

			public int dwPlatformId;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szCSDVersion;

			public short wServicePackMajor;

			public short wServicePackMinor;

			public short wSuiteMask;

			public byte wProductType;

			public byte wReserved;
		}

		public enum OSName
		{
			Windows95,
			Windows98SE,
			Windows98,
			WindowsMe,
			WindowsNT351,
			WindowsNT40,
			Windows2000,
			WindowsXP,
			WindowsServer2003,
			WindowsVista,
			WindowsServer2008,
			Windows7,
			WindowsServer2008R2,
			WindowsServer2012,
			WindowsServer2012R2,
			WindowsServer2016,
			Other
		}

		[DllImport("kernel32.dll")]
		private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);

		public static OSName GetOSName()
		{
			OperatingSystem oSVersion = Environment.OSVersion;
			OSName result = OSName.Other;
			OSVERSIONINFOEX osVersionInfo = default(OSVERSIONINFOEX);
			osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));
			GetVersionEx(ref osVersionInfo);
			byte wProductType = osVersionInfo.wProductType;
			switch (oSVersion.Platform)
			{
			case PlatformID.Win32Windows:
				switch (oSVersion.Version.Minor)
				{
				case 0:
					result = OSName.Windows95;
					break;
				case 10:
					result = ((oSVersion.Version.Revision.ToString() == "2222A") ? OSName.Windows98SE : OSName.Windows98);
					break;
				case 90:
					result = OSName.WindowsMe;
					break;
				}
				break;
			case PlatformID.Win32NT:
				switch (oSVersion.Version.Major)
				{
				case 3:
					result = OSName.WindowsNT351;
					break;
				case 4:
					result = OSName.WindowsNT40;
					break;
				case 5:
					switch (oSVersion.Version.Minor)
					{
					case 0:
						result = OSName.Windows2000;
						break;
					case 1:
						result = OSName.WindowsXP;
						break;
					case 2:
						result = OSName.WindowsServer2003;
						break;
					}
					break;
				case 6:
					switch (oSVersion.Version.Minor)
					{
					case 0:
						switch (wProductType)
						{
						case 1:
							result = OSName.WindowsVista;
							break;
						case 3:
							result = OSName.WindowsServer2008;
							break;
						}
						break;
					case 1:
						switch (wProductType)
						{
						case 1:
							result = OSName.Windows7;
							break;
						case 3:
							result = OSName.WindowsServer2008R2;
							break;
						}
						break;
					case 2:
						result = OSName.WindowsServer2012;
						break;
					case 3:
						result = OSName.WindowsServer2012R2;
						break;
					}
					break;
				case 10:
					if (oSVersion.Version.Minor == 0)
					{
						result = OSName.WindowsServer2016;
					}
					break;
				}
				break;
			default:
				result = OSName.Other;
				break;
			}
			return result;
		}

		public static bool isTerminalServicesInstalled()
		{
			switch (GetOSName())
			{
			case OSName.WindowsServer2003:
				return isTerminalServicesInstalled2003();
			case OSName.WindowsServer2008:
			case OSName.WindowsServer2008R2:
				return isTerminalServicesInstalled2008();
			case OSName.WindowsServer2012:
			case OSName.WindowsServer2012R2:
				return isTerminalServicesInstalled2012();
			case OSName.WindowsServer2016:
				return isTerminalServicesInstalled2016();
			default:
				return false;
			}
		}

		private static bool isTerminalServicesInstalled2008()
		{
			uint num = 18u;
			try
			{
				ManagementClass managementClass = new ManagementClass("Win32_ServerFeature");
				foreach (ManagementObject instance in managementClass.GetInstances())
				{
					if ((uint)instance["ID"] == num)
					{
						return true;
					}
				}
			}
			catch (ManagementException)
			{
			}
			return false;
		}

		private static bool isTerminalServicesInstalled2012()
		{
			return HasServerFeature("Remote Desktop Services");
		}

		private static bool isTerminalServicesInstalled2016()
		{
			return HasServerFeature("Remote Desktop Services");
		}

		private static bool isTerminalServicesInstalled2003()
		{
			try
			{
				ManagementClass managementClass = new ManagementClass("Win32_TerminalServiceSetting");
				foreach (ManagementObject instance in managementClass.GetInstances())
				{
					if ((uint)instance["TerminalServerMode"] == 1)
					{
						return true;
					}
				}
			}
			catch (ManagementException)
			{
			}
			return false;
		}

		private static bool HasServerFeature(string featureName)
		{
			try
			{
				ManagementObjectCollection instances = new ManagementClass("Win32_ServerFeature").GetInstances();
				return instances.Cast<ManagementBaseObject>().Any((ManagementBaseObject current) => string.Compare(current["Name"].ToString(), featureName, StringComparison.OrdinalIgnoreCase) == 0);
			}
			catch (ManagementException)
			{
			}
			return false;
		}
	}
}
