using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdl.LanguagePlatform.Core
{
	internal static class MapStringExWrapper
	{
		private struct LPNLSVERSIONINFO
		{
			public readonly uint dwNLSVersionInfoSize;

			public readonly uint dwNLSVersion;

			public readonly uint dwDefinedVersion;
		}

		internal enum MapFlag : uint
		{
			ByteReverse = 0x800,
			FullWidth = 0x800000,
			HalfWidth = 0x400000,
			Hiragana = 0x100000,
			Katakana = 0x200000,
			LinguisticCasing = 0x1000000,
			LowerCase = 0x100,
			SimplifiedChinese = 0x2000000,
			SortKey = 0x400,
			TitleCase = 768u,
			TraditionalChinese = 0x4000000,
			UpperCase = 0x200,
			NormIgnoreNonspace = 2u,
			NormIgnoreSymbols = 4u,
			LinguisticIgnoreCase = 0x10,
			LinguisticIgnoreDiacritic = 0x20,
			NormIgnoreCase = 1u,
			NormIgnoreKanaType = 0x10000,
			NormIgnoreWidth = 0x20000,
			NormLinguisticCasing = 0x8000000,
			SortDigitsAsNumbers = 8u,
			StringSort = 0x1000
		}

		private const string LocaleNameUserDefault = null;

		private const uint LcmapByterev = 2048u;

		private const uint LcmapFullwidth = 8388608u;

		private const uint LcmapHalfwidth = 4194304u;

		private const uint LcmapHiragana = 1048576u;

		private const uint LcmapKatakana = 2097152u;

		private const uint LcmapLinguisticCasing = 16777216u;

		private const uint LcmapLowercase = 256u;

		private const uint LcmapSimplifiedChinese = 33554432u;

		private const uint LcmapSortkey = 1024u;

		private const uint LcmapTitlecase = 768u;

		private const uint LcmapTraditionalChinese = 67108864u;

		private const uint LcmapUppercase = 512u;

		private const uint NormIgnorenonspace = 2u;

		private const uint NormIgnoresymbols = 4u;

		private const uint LinguisticIgnorecase = 16u;

		private const uint LinguisticIgnorediacritic = 32u;

		private const uint NormIgnorecase = 1u;

		private const uint NormIgnorekanatype = 65536u;

		private const uint NormIgnorewidth = 131072u;

		private const uint NormLinguisticCasing = 134217728u;

		private const uint SortDigitsasnumbers = 8u;

		private const uint SortStringsort = 4096u;

		[DllImport("kernel32.dll")]
		private static extern int LCMapStringEx([In] [MarshalAs(UnmanagedType.LPWStr)] string lpLocaleName, uint dwMapFlags, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpSrcStr, int cchSrc, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpDestStr, int cchDest, ref LPNLSVERSIONINFO lpVersionInformation, IntPtr lpReserved, int sortHandle);

		public static string MapString(string source, MapFlag flags, string locale = null)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length == 0)
			{
				throw new ArgumentException("source が空です。", "source");
			}
			StringBuilder stringBuilder = new StringBuilder(Math.Min(source.Length * 3, 32767));
			LPNLSVERSIONINFO lpVersionInformation = default(LPNLSVERSIONINFO);
			int num = LCMapStringEx(locale, (uint)flags, source, source.Length, stringBuilder, stringBuilder.Capacity, ref lpVersionInformation, (IntPtr)0, 0);
			if (num == 0)
			{
				throw new Exception("LastWin32Error Code" + Convert.ToString(Marshal.GetLastWin32Error()));
			}
			return stringBuilder.ToString().Substring(0, num);
		}
	}
}
