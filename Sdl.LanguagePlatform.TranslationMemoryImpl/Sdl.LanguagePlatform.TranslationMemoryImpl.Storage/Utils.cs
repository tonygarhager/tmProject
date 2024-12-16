using System;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal class Utils
	{
		public static string NormalizeToString(DateTime val)
		{
			if (val < DbStorageBase._MinDate)
			{
				val = DbStorageBase._MinDate;
			}
			return DateToSqlite1dot0dot60String(DateTime.SpecifyKind(new DateTime(val.Year, val.Month, val.Day, val.Hour, val.Minute, val.Second), DateTimeKind.Utc));
		}

		public static string DateToSqlite1dot0dot60String(DateTime val)
		{
			return val.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static DateTime StringToDate(string s)
		{
			if (s.Length < "yyyy-MM-dd HH-mm-ss".Length)
			{
				throw new Exception("Invalid date string: " + s);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(s.Substring(0, 4));
			stringBuilder.Append("-");
			stringBuilder.Append(s.Substring(5, 2));
			stringBuilder.Append("-");
			stringBuilder.Append(s.Substring(8, 2));
			stringBuilder.Append(" ");
			stringBuilder.Append(s.Substring(11, 2));
			stringBuilder.Append("-");
			stringBuilder.Append(s.Substring(14, 2));
			stringBuilder.Append("-");
			stringBuilder.Append(s.Substring(17, 2));
			s = stringBuilder.ToString();
			return DateTime.ParseExact(s, "yyyy-MM-dd HH-mm-ss", null);
		}
	}
}
