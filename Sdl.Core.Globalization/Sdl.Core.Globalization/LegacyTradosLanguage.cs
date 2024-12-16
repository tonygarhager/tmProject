using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Core.Globalization
{
	public struct LegacyTradosLanguage : IXmlSerializable
	{
		private const int LcidInstalled = 1;

		private static Dictionary<int, string> _lazyNativeNameCache;

		private string _lazyIsoCode;

		private int _lazyLcid;

		private static readonly int[] ProprietaryLocales = new int[6]
		{
			1082,
			1124,
			1083,
			1072,
			1076,
			1077
		};

		private const string LanguageElementName = "Language";

		public bool IsValid
		{
			get
			{
				if (Lcid == 0 || IsoCode == null || IsoCode.Length == 0)
				{
					return false;
				}
				return true;
			}
		}

		public bool IsInstalled => IsInstalledLcid(Lcid);

		public bool IsProprietary => IsProprietaryLcid(Lcid);

		public string IsoCode
		{
			get
			{
				lock (typeof(Language))
				{
					if (_lazyIsoCode == null || (_lazyIsoCode.Length == 0 && _lazyLcid != 0))
					{
						_lazyIsoCode = GetIsoCodeFromLcid(_lazyLcid, includeCountry: true);
					}
				}
				return _lazyIsoCode;
			}
		}

		public int Lcid
		{
			get
			{
				lock (typeof(Language))
				{
					if (_lazyLcid == 0 && _lazyIsoCode != null && _lazyIsoCode.Length >= 2)
					{
						_lazyLcid = GetLcidFromIsoCode(_lazyIsoCode);
					}
				}
				return _lazyLcid;
			}
		}

		private static Dictionary<int, string> NativeNameCache
		{
			get
			{
				if (_lazyNativeNameCache == null)
				{
					_lazyNativeNameCache = new Dictionary<int, string>();
					CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
					CultureInfo[] array = cultures;
					foreach (CultureInfo cultureInfo in array)
					{
						_lazyNativeNameCache[cultureInfo.LCID] = cultureInfo.NativeName;
					}
				}
				return _lazyNativeNameCache;
			}
		}

		[DllImport("kernel32.dll")]
		private static extern bool IsValidLocale(int locale, int flags);

		public static bool IsInstalledLcid(int lcid)
		{
			return IsValidLocale(lcid, 1);
		}

		public static bool IsProprietaryLcid(int lcid)
		{
			if (!IsInstalledLcid(lcid))
			{
				return ProprietaryLocales.Any((int t) => lcid == t);
			}
			return false;
		}

		public LegacyTradosLanguage(string isoCode)
		{
			_lazyIsoCode = isoCode;
			_lazyLcid = 0;
		}

		public LegacyTradosLanguage(int lcid)
		{
			_lazyIsoCode = null;
			_lazyLcid = lcid;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || typeof(Language) != obj.GetType())
			{
				return false;
			}
			LegacyTradosLanguage legacyTradosLanguage = (LegacyTradosLanguage)obj;
			if (IsValid && legacyTradosLanguage.IsValid)
			{
				return legacyTradosLanguage.Lcid == Lcid;
			}
			return legacyTradosLanguage.IsoCode == IsoCode;
		}

		public override int GetHashCode()
		{
			if (IsValid)
			{
				return Lcid.GetHashCode();
			}
			return IsoCode.GetHashCode();
		}

		public override string ToString()
		{
			return IsoCode;
		}

		public string GetLocaleInfo(LanguageFormat languageFormat, bool includeCountry)
		{
			return GetLocaleInfo(Lcid, languageFormat, includeCountry);
		}

		public static string GetLocaleInfo(int lcid, LanguageFormat languageFormat, bool includeCountry)
		{
			switch (languageFormat)
			{
			case LanguageFormat.EnglishName:
				return GetEnglishFromLcid(lcid, includeCountry);
			case LanguageFormat.NativeName:
				return GetNativeFromLcid(lcid, includeCountry);
			case LanguageFormat.ISOCode:
				return GetIsoCodeFromLcid(lcid, includeCountry);
			default:
				return GetEnglishFromLcid(lcid, includeCountry);
			}
		}

		public static int[] GetAllLcids()
		{
			return TradosLanguage.GetAllLcids();
		}

		public static int GetLcidFromIsoCode(string isoCode)
		{
			if (TradosLanguage.GetLanguages(out TradosLanguage.et_Primary peo_primary, out TradosLanguage.et_Sub peo_sub, isoCode))
			{
				if (peo_sub == TradosLanguage.et_Sub.ec_SubNeutral)
				{
					peo_sub = TradosLanguage.et_Sub.ec_SubDefault;
				}
				return (int)TradosLanguage.MakeLanguageID(peo_primary, peo_sub);
			}
			return -1;
		}

		public static string GetIsoCodeFromLcid(int lcid, bool includeCountry)
		{
			TradosLanguage.GetLanguages((uint)lcid, out TradosLanguage.et_Primary peo_primary, out TradosLanguage.et_Sub peo_sub);
			if (peo_sub == TradosLanguage.et_Sub.ec_SubNeutral)
			{
				peo_sub = TradosLanguage.et_Sub.ec_SubDefault;
			}
			if (!includeCountry)
			{
				return TradosLanguage.GetLanguageCode(peo_primary);
			}
			return TradosLanguage.MakeLanguageCode(peo_primary, peo_sub);
		}

		public static bool IsDefaultForCountry(int lcid)
		{
			return lcid < 2048;
		}

		public string GetEnglishLanguageNameFromLcid()
		{
			return GetEnglishFromLcid(Lcid, includeCountry: false);
		}

		public static string GetEnglishLanguageNameFromLcid(int lcid)
		{
			return GetEnglishFromLcid(lcid, includeCountry: false);
		}

		public static string GetEnglishNameFromLcid(int lcid)
		{
			return GetEnglishFromLcid(lcid, includeCountry: true);
		}

		private static string GetEnglishFromLcid(int lcid, bool includeCountry)
		{
			TradosLanguage.GetLanguages((uint)lcid, out TradosLanguage.et_Primary peo_primary, out TradosLanguage.et_Sub peo_sub);
			string text = TradosLanguage.MakeLanguageString(peo_primary, peo_sub);
			if (!includeCountry)
			{
				if (text.IndexOf('(') != -1)
				{
					return text.Substring(0, text.IndexOf('(') - 1).Trim();
				}
				return text;
			}
			return text;
		}

		public string GetNativeLanguageNameFromLcid()
		{
			return GetNativeFromLcid(Lcid, includeCountry: false);
		}

		public static string GetNativeLanguageNameFromLcid(int lcid)
		{
			return GetNativeFromLcid(lcid, includeCountry: false);
		}

		public static string GetNativeNameFromLcid(int lcid)
		{
			return GetNativeFromLcid(lcid, includeCountry: true);
		}

		public static string GetLanguageCode(int primaryLanguageCode)
		{
			return TradosLanguage.TryGetLanguageCode((TradosLanguage.et_Primary)primaryLanguageCode);
		}

		public static int[] GetLcids(int primaryLanguageCode)
		{
			return TradosLanguage.GetLcids((TradosLanguage.et_Primary)primaryLanguageCode);
		}

		private static string GetNativeFromLcid(int lcid, bool includeCountry)
		{
			if (NativeNameCache.TryGetValue(lcid, out string value))
			{
				if (!includeCountry)
				{
					if (value.IndexOf('(') != -1)
					{
						return value.Substring(0, value.IndexOf('(') - 1).Trim();
					}
					return value;
				}
				return value;
			}
			return GetEnglishFromLcid(lcid, includeCountry);
		}

		public bool UsesCharacterCounts()
		{
			return UsesCharacterCounts(Lcid);
		}

		public static bool UsesCharacterCounts(int lcid)
		{
			switch (lcid)
			{
			case 4:
			case 17:
			case 30:
			case 1028:
			case 1041:
			case 1054:
			case 2052:
			case 3076:
			case 4100:
			case 5124:
			case 31748:
				return true;
			default:
				return false;
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			_lazyIsoCode = reader.ReadElementContentAsString("Language", string.Empty);
			_lazyLcid = 0;
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteElementString("Language", IsoCode);
		}
	}
}
