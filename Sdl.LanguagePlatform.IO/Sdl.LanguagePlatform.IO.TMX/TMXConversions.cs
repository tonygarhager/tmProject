using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.IO.TMX
{
	internal static class TMXConversions
	{
		private static string _TMXTimePattern = "yyyyMMdd'T'HHmmss'Z'";

		public static bool TryTMXToDateTime(string tmxDateTime, out DateTime result)
		{
			return DateTime.TryParseExact(tmxDateTime, _TMXTimePattern, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result);
		}

		public static string DateTimeToTMX(DateTime date)
		{
			return DateTimeUtilities.Normalize(date).ToString(_TMXTimePattern, DateTimeFormatInfo.InvariantInfo);
		}
	}
}
