using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Sdl.LanguagePlatform.Core
{
	public static class CultureInfoExtensions
	{
		public enum LanguageGroupInstallationStatus
		{
			Installed = 1,
			Supported
		}

		public enum LanguageGroupID
		{
			Unknown,
			WesternEurope,
			CentralEurope,
			Baltic,
			Greek,
			Cyrillic,
			Turkish,
			Japanese,
			Korean,
			TraditionalChinese,
			SimplifiedChinese,
			Thai,
			Hebrew,
			Arabic,
			Vietnamese,
			Indic,
			Georgian,
			Armenian
		}

		private delegate bool EnumLanguageGroupProc(LanguageGroupID grpId, [MarshalAs(UnmanagedType.LPTStr)] string groupString, [MarshalAs(UnmanagedType.LPTStr)] string groupNameString, LanguageGroupInstallationStatus status, IntPtr lParam);

		private delegate bool EnumLanguageGroupLocalesProc(LanguageGroupID grpId, int lcid, [MarshalAs(UnmanagedType.LPTStr)] string localeString, IntPtr lparam);

		private static Dictionary<int, LanguageGroupID> _localeToGroupMapping;

		private static readonly Dictionary<string, CultureInfo> LegacyLanguageMapping;

		private static readonly Dictionary<string, string> LegacyLanguageCodes;

		private static Dictionary<string, CultureInfo> _regionQualifiedCultureMapping;

		private static readonly HashSet<string> RomanceLanguages;

		private static readonly HashSet<string> UseBlankAsWordSeparatorExceptions;

		private static readonly HashSet<string> UseBlankAsSentenceSepatatorLanguagues;

		public static Dictionary<string, string> LegacyLanguageCodeMapping => LegacyLanguageCodes;

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern bool EnumSystemLanguageGroups(EnumLanguageGroupProc callback, LanguageGroupInstallationStatus status, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern bool EnumLanguageGroupLocales(EnumLanguageGroupLocalesProc callback, LanguageGroupID languageGroup, int flags, IntPtr lParam);

		static CultureInfoExtensions()
		{
			LegacyLanguageMapping = new Dictionary<string, CultureInfo>(StringComparer.OrdinalIgnoreCase);
			LegacyLanguageCodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			InitializeLanguageGroupMapping();
			InitializeRegionQualifiedCultureMappingUsingCreateSpecificCulture();
			RomanceLanguages = new HashSet<string>(new string[7]
			{
				"fr",
				"es",
				"pt",
				"it",
				"ro",
				"gl",
				"ca"
			}, StringComparer.OrdinalIgnoreCase);
			UseBlankAsWordSeparatorExceptions = new HashSet<string>(new string[6]
			{
				"th",
				"km",
				"ja",
				"chs",
				"cht",
				"zh"
			}, StringComparer.OrdinalIgnoreCase);
			UseBlankAsSentenceSepatatorLanguagues = new HashSet<string>(new string[2]
			{
				"th",
				"km"
			}, StringComparer.OrdinalIgnoreCase);
			LegacyLanguageCodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			try
			{
				LegacyLanguageCodes.Add("no-no", "nb-NO");
				LegacyLanguageCodes.Add("no-ny", "nn-NO");
				LegacyLanguageCodes.Add("no-xy", "nn-NO");
				LegacyLanguageCodes.Add("es-em", "es-ES");
				LegacyLanguageCodes.Add("es-xm", "es-ES");
				LegacyLanguageCodes.Add("es-xy", "es-ES");
				LegacyLanguageCodes.Add("es-xl", "es-419");
				LegacyLanguageCodes.Add("es-cn", "es-CL");
				LegacyLanguageCodes.Add("es-me", "es-MX");
				LegacyLanguageCodes.Add("mt", "mt-MT");
				LegacyLanguageCodes.Add("mt-01", "mt-MT");
				LegacyLanguageCodes.Add("cy", "cy-GB");
				LegacyLanguageCodes.Add("cy-01", "cy-GB");
				LegacyLanguageCodes.Add("ga", "ga-IE");
				LegacyLanguageCodes.Add("gd", "gd-GB");
				LegacyLanguageCodes.Add("ga-CT", "gd-GB");
				LegacyLanguageCodes.Add("iw", "he-IL");
				LegacyLanguageCodes.Add("rm", "rm-CH");
				LegacyLanguageCodes.Add("in", "id-ID");
				LegacyLanguageCodes.Add("mn-xc", "mn-MN");
				LegacyLanguageCodes.Add("ms-bx", "ms-BN");
				LegacyLanguageCodes.Add("qu", "quz-BO");
				LegacyLanguageCodes.Add("qu-BO", "quz-BO");
				LegacyLanguageCodes.Add("qu-EC", "quz-EC");
				LegacyLanguageCodes.Add("qu-PE", "quz-PE");
				LegacyLanguageCodes.Add("sz", "se-FI");
				LegacyLanguageCodes.Add("tl", "fil-PH");
				LegacyLanguageCodes.Add("fl", "fil-PH");
				LegacyLanguageCodes.Add("pt-pr", "pt-PT");
				LegacyLanguageCodes.Add("zh-_c", "zh-CN");
				LegacyLanguageCodes.Add("zh-_t", "zh-TW");
				LegacyLanguageCodes.Add("zh-XM", "zh-MO");
				LegacyLanguageCodes.Add("se", "se-NO");
				LegacyLanguageCodes.Add("nep", "ne-NP");
				LegacyLanguageCodes.Add("ne", "ne-NP");
				LegacyLanguageCodes.Add("div", "dv-MV");
				LegacyLanguageCodes.Add("mi", "mi-NZ");
				LegacyLanguageCodes.Add("bo", "bo-CN");
				LegacyLanguageCodes.Add("km", "km-KH");
				LegacyLanguageCodes.Add("lo", "lo-LA");
				LegacyLanguageCodes.Add("si", "si-LK");
				LegacyLanguageCodes.Add("am", "am-ET");
				LegacyLanguageCodes.Add("fy", "fy-NL");
				LegacyLanguageCodes.Add("ba", "ba-RU");
				LegacyLanguageCodes.Add("rw", "rw-RW");
				LegacyLanguageCodes.Add("wo", "wo-SN");
				LegacyLanguageCodes.Add("tk", "tk-TM");
				LegacyLanguageCodes.Add("kl", "kl-GL");
				LegacyLanguageCodes.Add("tg", "tg-Cyrl-TJ");
				LegacyLanguageCodes.Add("ug", "ug-CN");
				LegacyLanguageCodes.Add("ps", "ps-AF");
				LegacyLanguageCodes.Add("br", "br-FR");
				LegacyLanguageCodes.Add("oc", "oc-FR");
				LegacyLanguageCodes.Add("co", "co-FR");
				LegacyLanguageCodes.Add("ha", "ha-Latn-NG");
				LegacyLanguageCodes.Add("yo", "yo-NG");
				LegacyLanguageCodes.Add("ig", "ig-NG");
				LegacyLanguageCodes.Add("sh-hr", "hr-HR");
				LegacyLanguageCodes.Add("sh", "hr-HR");
				LegacyLanguageCodes.Add("sh-sr", "sr-Latn-CS");
				LegacyLanguageCodes.Add("sh-yu", "sr-Cyrl-CS");
				LegacyLanguageCodes.Add("sr-xc", "sr-Cyrl-CS");
				LegacyLanguageCodes.Add("bs", "bs-Latn-BA");
				LegacyLanguageCodes.Add("sh-B1", "hr-BA");
				LegacyLanguageCodes.Add("sh-B2", "bs-Latn-BA");
				LegacyLanguageCodes.Add("sh-B3", "sr-Latn-BA");
				LegacyLanguageCodes.Add("sh-B4", "sr-Cyrl-BA");
				LegacyLanguageCodes.Add("sr-SP", "sr-Cyrl-CS");
				LegacyLanguageCodes.Add("az-xc", "az-Cyrl-AZ");
				LegacyLanguageCodes.Add("az-xe", "az-Latn-AZ");
				LegacyLanguageCodes.Add("az-cy", "az-Cyrl-AZ");
				LegacyLanguageCodes.Add("az-lt", "az-Latn-AZ");
				LegacyLanguageCodes.Add("az-AZ", "az-Latn-AZ");
				LegacyLanguageCodes.Add("uz-xc", "uz-Cyrl-UZ");
				LegacyLanguageCodes.Add("uz-xl", "uz-Latn-UZ");
				LegacyLanguageCodes.Add("uz-lt", "uz-Latn-UZ");
				LegacyLanguageCodes.Add("uz-cy", "uz-Cyrl-UZ");
				LegacyLanguageCodes.Add("uz-UZ", "uz-Latn-UZ");
				LegacyLanguageCodes.Add("en-uk", "en-GB");
				LegacyLanguageCodes.Add("en-cr", "en-029");
				LegacyLanguageCodes.Add("en-cb", "en-029");
				LegacyLanguageCodes.Add("en-tr", "en-TT");
				LegacyLanguageCodes.Add("en-rh", "en-ZW");
				LegacyLanguageCodes.Add("bn", "bn-IN");
				LegacyLanguageCodes.Add("ml", "ml-IN");
				LegacyLanguageCodes.Add("or", "or-IN");
				LegacyLanguageCodes.Add("as", "as-IN");
				LegacyLanguageCodes.Add("tn", "tn-ZA");
				LegacyLanguageCodes.Add("xh", "xh-ZA");
				LegacyLanguageCodes.Add("zu", "zu-ZA");
				LegacyLanguageCodes.Add("ns", "nso-ZA");
				LegacyLanguageCodes.Add("ns-ZA", "nso-ZA");
			}
			catch (Exception)
			{
			}
		}

		public static CultureInfo GetCultureInfo(int lcid)
		{
			return GetCultureInfo(lcid, returnNullForUnknowns: false);
		}

		public static string GetMappedCultureCodeForLegacyCode(string legacyCode)
		{
			if (legacyCode == null)
			{
				throw new ArgumentNullException();
			}
			if (!LegacyLanguageCodes.TryGetValue(legacyCode, out string value))
			{
				return null;
			}
			return value;
		}

		public static CultureInfo GetCultureInfo(int lcid, bool returnNullForUnknowns)
		{
			try
			{
				if (lcid == 3068)
				{
					return CultureInfo.GetCultureInfo("hi-IN");
				}
				return CultureInfo.GetCultureInfo(lcid);
			}
			catch
			{
				if (!returnNullForUnknowns)
				{
					throw;
				}
				return null;
			}
		}

		public static CultureInfo GetCultureInfo(string name)
		{
			return GetCultureInfo(name, returnNullForUnknowns: false);
		}

		public static CultureInfo GetCultureInfo(string name, bool returnNullForUnknowns)
		{
			if (name == null)
			{
				return null;
			}
			if (name.Length == 0)
			{
				return CultureInfo.InvariantCulture;
			}
			if (int.TryParse(name, out int result))
			{
				return GetCultureInfo(result);
			}
			if (LegacyLanguageMapping.TryGetValue(name, out CultureInfo value))
			{
				return value;
			}
			Exception ex2;
			try
			{
				return CultureInfo.GetCultureInfo(name);
			}
			catch (Exception ex)
			{
				ex2 = ex;
			}
			if (!LegacyLanguageMapping.TryGetValue(name, out value))
			{
				lock (LegacyLanguageMapping)
				{
					string text = name.ToLower(CultureInfo.InvariantCulture);
					try
					{
						if (LegacyLanguageCodes.TryGetValue(text, out string value2))
						{
							value = CultureInfo.GetCultureInfo(value2);
						}
						else if (text.EndsWith("-01"))
						{
							CultureInfo cultureInfo = CultureInfo.GetCultureInfo(text.Substring(0, text.Length - 3));
							value = GetRegionQualifiedCulture(cultureInfo);
						}
						else
						{
							value = CultureInfo.GetCultureInfo(name);
						}
					}
					catch
					{
					}
					if (value != null)
					{
						LegacyLanguageMapping.Add(text, value);
					}
				}
			}
			if (value != null)
			{
				return value;
			}
			if (returnNullForUnknowns)
			{
				return null;
			}
			throw ex2;
		}

		public static bool IsRomanceLanguage(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			return RomanceLanguages.Contains(culture.TwoLetterISOLanguageName);
		}

		public static bool UseBlankAsWordSeparator(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			return !UseBlankAsWordSeparatorExceptions.Contains(culture.TwoLetterISOLanguageName);
		}

		public static bool UseBlankAsSentenceSeparator(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			return UseBlankAsSentenceSepatatorLanguagues.Contains(culture.TwoLetterISOLanguageName);
		}

		public static CultureInfo GetRegionQualifiedCulture(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (!culture.IsNeutralCulture)
			{
				return culture;
			}
			if (_regionQualifiedCultureMapping.TryGetValue(culture.Name, out CultureInfo value))
			{
				return value;
			}
			throw new LanguagePlatformException(ErrorCode.NoRegionSpecificLanguageFound, culture.Name);
		}

		public static string GetRegionQualifiedCulture(string cultureCode)
		{
			if (cultureCode == null)
			{
				throw new ArgumentNullException("cultureCode");
			}
			if (!_regionQualifiedCultureMapping.TryGetValue(cultureCode, out CultureInfo value))
			{
				return null;
			}
			return value.Name;
		}

		public static CultureInfo GetRegionNeutralCulture(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			CultureInfo cultureInfo = culture;
			while (!object.Equals(cultureInfo.Parent, CultureInfo.InvariantCulture))
			{
				cultureInfo = cultureInfo.Parent;
			}
			return cultureInfo;
		}

		public static bool AreCompatible(CultureInfo c1, CultureInfo c2)
		{
			if (c1 == null || c2 == null)
			{
				throw new ArgumentNullException();
			}
			if (object.Equals(c1, c2))
			{
				return true;
			}
			bool isTranditionalCompatible;
			bool flag = IsChinese(c1, out isTranditionalCompatible);
			bool isTranditionalCompatible2;
			bool flag2 = IsChinese(c2, out isTranditionalCompatible2);
			if (flag | flag2)
			{
				if (flag == flag2)
				{
					return isTranditionalCompatible == isTranditionalCompatible2;
				}
				return false;
			}
			c1 = GetRegionNeutralCulture(c1);
			c2 = GetRegionNeutralCulture(c2);
			return object.Equals(c1, c2);
		}

		private static bool IsChinese(CultureInfo cultureInfo, out bool isTranditionalCompatible)
		{
			switch (cultureInfo.Name.ToLower())
			{
			case "zh-hans":
			case "zh-cn":
			case "zh-sg":
			case "zh-chs":
			case "zh":
				isTranditionalCompatible = false;
				return true;
			case "zh-tw":
			case "zh-hk":
			case "zh-mo":
			case "zh-hant":
			case "zh-cht":
				isTranditionalCompatible = true;
				return true;
			default:
				isTranditionalCompatible = false;
				return false;
			}
		}

		public static bool UsesClitics(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (!culture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase))
			{
				return IsRomanceLanguage(culture);
			}
			return true;
		}

		private static void InitializeLanguageGroupMapping()
		{
			_localeToGroupMapping = new Dictionary<int, LanguageGroupID>();
			EnumSystemLanguageGroups(delegate(LanguageGroupID grpId, string groupString, string groupNameString, LanguageGroupInstallationStatus status, IntPtr lParam)
			{
				EnumLanguageGroupLocales(delegate(LanguageGroupID innerGrpId, int lcid, string localeString, IntPtr lparam)
				{
					if (!_localeToGroupMapping.ContainsKey(lcid))
					{
						_localeToGroupMapping.Add(lcid, innerGrpId);
					}
					int key = lcid & 0x3FF;
					if (!_localeToGroupMapping.ContainsKey(key))
					{
						_localeToGroupMapping.Add(key, innerGrpId);
					}
					return true;
				}, grpId, 0, IntPtr.Zero);
				return true;
			}, LanguageGroupInstallationStatus.Supported, IntPtr.Zero);
		}

		private static void InitializeRegionQualifiedCultureMappingUsingCreateSpecificCulture()
		{
			_regionQualifiedCultureMapping = new Dictionary<string, CultureInfo>(StringComparer.InvariantCultureIgnoreCase);
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
			foreach (CultureInfo cultureInfo in cultures)
			{
				if (!_regionQualifiedCultureMapping.ContainsKey(cultureInfo.Name))
				{
					try
					{
						CultureInfo value = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
						_regionQualifiedCultureMapping.Add(cultureInfo.Name, value);
					}
					catch (ArgumentException)
					{
					}
				}
			}
			string[] array = new string[8]
			{
				"zh-CHS",
				"zh-CN",
				"zh-Hans",
				"zh-CN",
				"zh-CHT",
				"zh-TW",
				"zh-Hant",
				"zh-TW"
			};
			for (int j = 0; j < array.Length; j += 2)
			{
				try
				{
					CultureInfo cultureInfo2 = CultureInfo.GetCultureInfo(array[j]);
					CultureInfo cultureInfo3 = CultureInfo.GetCultureInfo(array[j + 1]);
					if (!_regionQualifiedCultureMapping.ContainsKey(cultureInfo2.Name))
					{
						_regionQualifiedCultureMapping.Add(cultureInfo2.Name, cultureInfo3);
					}
				}
				catch
				{
				}
			}
		}

		public static LanguageGroupID GetLanguageGroupID(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (_localeToGroupMapping == null)
			{
				InitializeLanguageGroupMapping();
			}
			if (_localeToGroupMapping.TryGetValue(culture.LCID, out LanguageGroupID value))
			{
				return value;
			}
			return LanguageGroupID.Unknown;
		}

		public static string GetLanguageGroupName(CultureInfo culture)
		{
			LanguageGroupID languageGroupID = GetLanguageGroupID(culture);
			if (languageGroupID != 0)
			{
				return languageGroupID.ToString();
			}
			return null;
		}

		public static HashSet<string> GetLeadingClitics(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			switch (culture.TwoLetterISOLanguageName)
			{
			case "fr":
				return GetClitics("d'|l'|c'|j'|n'|qu'|s'|D'|L'|C'|J'|N'|Qu'|S'", applyQuoteSubstitution: true, addInitialUpperVariant: false);
			case "it":
				return GetClitics("d'|dall'|dell'|l'|nell'|quest'|sull'|un'|alcun'|al'|all'|s'|D'|Dall'|Dell'|L'|Nell'|Quest'|Sull'|Un'|Alcun'|Al'|All'|S'", applyQuoteSubstitution: true, addInitialUpperVariant: false);
			case "mt":
				return GetClitics("il-|l-|fil-|tal-|tas-|bir-|fl-|Il-|L-|Fil-|Tal-|Tas-|Bir-|Fl-", applyQuoteSubstitution: false, addInitialUpperVariant: false);
			default:
				return null;
			}
		}

		private static HashSet<string> GetClitics(string list, bool applyQuoteSubstitution, bool addInitialUpperVariant)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			string[] array = list.Split('|');
			foreach (string text in array)
			{
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				string text2 = text;
				for (int j = 0; j < 2; j++)
				{
					hashSet.Add(text2);
					if (applyQuoteSubstitution && text2.IndexOf('\'') >= 0)
					{
						string item = text2.Replace('\'', '’');
						hashSet.Add(item);
					}
					if (!addInitialUpperVariant || !char.IsLower(text[0]))
					{
						break;
					}
					text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
				}
			}
			return hashSet;
		}

		public static HashSet<string> GetTrailingClitics(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			string twoLetterISOLanguageName = culture.TwoLetterISOLanguageName;
			if (twoLetterISOLanguageName != null && twoLetterISOLanguageName == "en")
			{
				return GetClitics("'ve|'s|'m|'ll|n't|'re", applyQuoteSubstitution: true, addInitialUpperVariant: false);
			}
			return null;
		}

		public static bool UsesDefaultDigits(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (culture.IsNeutralCulture)
			{
				throw new ArgumentException("Invalid argument (neutral culture)");
			}
			string text = string.Join("", culture.NumberFormat.NativeDigits);
			return text.Equals("0123456789", StringComparison.Ordinal);
		}

		public static bool UsesStandardNumberGrouping(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (culture.IsNeutralCulture)
			{
				throw new ArgumentException("Invalid argument (neutral culture)");
			}
			if (culture.NumberFormat.NumberGroupSizes != null && culture.NumberFormat.NumberGroupSizes.Length == 1)
			{
				return culture.NumberFormat.NumberGroupSizes[0] == 3;
			}
			return false;
		}

		public static bool UseFullWidth(CultureInfo culture)
		{
			string twoLetterISOLanguageName = culture.TwoLetterISOLanguageName;
			if (!twoLetterISOLanguageName.Equals("ja", StringComparison.OrdinalIgnoreCase) && !twoLetterISOLanguageName.Equals("zh", StringComparison.OrdinalIgnoreCase))
			{
				return twoLetterISOLanguageName.Equals("ko", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}
	}
}
