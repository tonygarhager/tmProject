using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal static class ILocalizableTokenExtensions
	{
		public static bool DoesFormatMatch(this ILocalizableToken token, ILocalizableToken other)
		{
			DateTimeToken dateTimeToken = token as DateTimeToken;
			NumberToken numberToken = token as NumberToken;
			MeasureToken measureToken = token as MeasureToken;
			DateTimeToken dateTimeToken2 = other as DateTimeToken;
			NumberToken numberToken2 = other as NumberToken;
			MeasureToken measureToken2 = other as MeasureToken;
			if (dateTimeToken != null && dateTimeToken2 != null)
			{
				if (dateTimeToken2.FormatString == null || dateTimeToken.FormatString == null)
				{
					return true;
				}
				if (string.CompareOrdinal(dateTimeToken2.FormatString, dateTimeToken.FormatString) == 0)
				{
					return true;
				}
				return DatePatternsDifferInLeadingZeroOnly(dateTimeToken.FormatString, dateTimeToken2.FormatString);
			}
			if (numberToken != null && numberToken2 != null)
			{
				bool num = !string.IsNullOrEmpty(numberToken.RawFractionalDigits);
				bool flag = !string.IsNullOrEmpty(numberToken2.RawFractionalDigits);
				bool num2 = numberToken.RawDecimalDigits != null && numberToken.RawDecimalDigits.Length > 3 && numberToken.GroupSeparator != NumericSeparator.None;
				bool flag2 = numberToken2.RawDecimalDigits != null && numberToken2.RawDecimalDigits.Length > 3 && numberToken2.GroupSeparator != NumericSeparator.None;
				bool flag3 = false;
				if (num2 & flag2)
				{
					flag3 = (numberToken.GroupSeparator != numberToken2.GroupSeparator || numberToken.AlternateGroupSeparator != numberToken2.AlternateGroupSeparator);
				}
				if (num & flag)
				{
					flag3 |= (numberToken.DecimalSeparator != numberToken2.DecimalSeparator || numberToken.AlternateDecimalSeparator != numberToken2.AlternateDecimalSeparator);
				}
				return !flag3;
			}
			if (measureToken != null)
			{
			}
			return true;
		}

		public static bool FormatHasMoreInformation(this ILocalizableToken token, ILocalizableToken other)
		{
			NumberToken numberToken = token as NumberToken;
			NumberToken numberToken2 = other as NumberToken;
			if (numberToken != null && numberToken2 != null)
			{
				bool flag = !string.IsNullOrEmpty(numberToken.RawFractionalDigits);
				bool flag2 = !string.IsNullOrEmpty(numberToken2.RawFractionalDigits);
				bool num = numberToken.RawDecimalDigits != null && numberToken.RawDecimalDigits.Length > 3;
				bool flag3 = numberToken2.RawDecimalDigits != null && numberToken2.RawDecimalDigits.Length > 3;
				if (num && !flag3)
				{
					return true;
				}
				if (flag && !flag2)
				{
					return true;
				}
			}
			return false;
		}

		private static bool DatePatternsDifferInLeadingZeroOnly(string pattern1, string pattern2)
		{
			DateTime dateTime = new DateTime(1990, 10, 10, 10, 10, 10);
			try
			{
				string strA = dateTime.ToString(pattern1, CultureInfo.InvariantCulture);
				string strB = dateTime.ToString(pattern2, CultureInfo.InvariantCulture);
				return string.CompareOrdinal(strA, strB) == 0;
			}
			catch (Exception)
			{
			}
			return false;
		}
	}
}
