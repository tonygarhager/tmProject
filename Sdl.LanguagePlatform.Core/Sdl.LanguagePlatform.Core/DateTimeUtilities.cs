using System;
using System.Globalization;
using System.Threading;

namespace Sdl.LanguagePlatform.Core
{
	public static class DateTimeUtilities
	{
		public static DateTime Normalize(DateTime dt)
		{
			if (dt.Kind == DateTimeKind.Utc)
			{
				return dt;
			}
			if (dt == default(DateTime) || dt == DateTime.MinValue || dt == DateTime.MaxValue)
			{
				return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
			}
			return dt.ToUniversalTime();
		}

		public static bool TryParseWithFallback(string s, out DateTime result)
		{
			bool flag = DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result);
			if (!flag)
			{
				flag = DateTime.TryParse(s, Thread.CurrentThread.CurrentCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal, out result);
			}
			return flag;
		}
	}
}
