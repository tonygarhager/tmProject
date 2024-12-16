using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public static class SdlXliffVersions
	{
		public const int CurrentMajorVersion = 1;

		public const int CurrentMinorVersion = 0;

		public const string SniffInfoOriginalVersionNumberKey = "OriginalVersionNumber";

		public static IEnumerable<string> KnownVersions
		{
			get
			{
				yield return MakeVersionString(0, 8);
				yield return MakeVersionString(1, 0);
			}
		}

		public static string CurrentVersionString => MakeVersionString(1, 0);

		public static string MakeVersionString(int majorVersion, int minorVersion)
		{
			return $"{majorVersion}.{minorVersion}";
		}

		public static bool ParseVersionString(string versionString, out int majorVersion, out int minorVersion)
		{
			string[] array = versionString.Split('.');
			if (array.Length < 2)
			{
				majorVersion = 0;
				minorVersion = 0;
				return false;
			}
			if (!int.TryParse(array[0], out majorVersion))
			{
				minorVersion = 0;
				return false;
			}
			if (!int.TryParse(array[1], out minorVersion))
			{
				return false;
			}
			return true;
		}
	}
}
