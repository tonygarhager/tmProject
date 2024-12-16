using System.Globalization;

namespace Sdl.LanguagePlatform.Core
{
	internal static class HashCodeExtensions
	{
		public static int GetPlatformAgnosticHashCode(this string s)
		{
			char[] array = s.ToCharArray();
			int num = array.Length - 1;
			int num2 = 352654597;
			int num3 = num2;
			int num4 = 0;
			while (num4 <= num)
			{
				char c = array[num4];
				char c2 = (++num4 <= num) ? array[num4] : '\0';
				num2 = (((num2 << 5) + num2 + (num2 >> 27)) ^ (int)(((uint)c2 << 16) | c));
				if (++num4 > num)
				{
					break;
				}
				c = array[num4];
				c2 = ((++num4 <= num) ? array[num4++] : '\0');
				num3 = (((num3 << 5) + num3 + (num3 >> 27)) ^ (int)(((uint)c2 << 16) | c));
			}
			return num2 + num3 * 1566083941;
		}

		public static int GetPlatformAgnosticHashCode(this CultureInfo culture)
		{
			int platformAgnosticHashCode = culture.Name.GetPlatformAgnosticHashCode();
			return 2 * platformAgnosticHashCode;
		}
	}
}
