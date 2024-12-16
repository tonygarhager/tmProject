using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdl.LanguagePlatform.Core
{
	public class LanguageMetadata
	{
		private const int CurrentVersion = 1;

		private string _name;

		private CultureInfo _ci;

		private static Dictionary<string, LanguageMetadata> _metadata;

		private static readonly object Locker = new object();

		public int Version
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				_ci = CultureInfoExtensions.GetCultureInfo(_name, returnNullForUnknowns: true);
			}
		}

		[JsonIgnore]
		public DateTimeFormatInfo DateTimeFormat
		{
			get
			{
				DateTimeFormatInfo dateTimeFormatInfo = (_ci?.DateTimeFormat?.Clone() as DateTimeFormatInfo) ?? new DateTimeFormatInfo();
				dateTimeFormatInfo.AbbreviatedDayNames = AbbreviatedDayNames;
				dateTimeFormatInfo.AbbreviatedMonthGenitiveNames = AbbreviatedMonthGenitiveNames;
				dateTimeFormatInfo.AbbreviatedMonthNames = AbbreviatedMonthNames;
				dateTimeFormatInfo.AMDesignator = AMDesignator;
				dateTimeFormatInfo.DateSeparator = "/";
				dateTimeFormatInfo.DayNames = DayNames;
				dateTimeFormatInfo.MonthGenitiveNames = MonthGenitiveNames;
				dateTimeFormatInfo.MonthNames = MonthNames;
				dateTimeFormatInfo.PMDesignator = PMDesignator;
				dateTimeFormatInfo.TimeSeparator = ":";
				dateTimeFormatInfo.FullDateTimePattern = FullDateTimePattern;
				dateTimeFormatInfo.LongDatePattern = LongDatePattern;
				dateTimeFormatInfo.LongTimePattern = LongTimePattern;
				dateTimeFormatInfo.ShortDatePattern = ShortDatePattern;
				dateTimeFormatInfo.ShortTimePattern = ShortTimePattern;
				dateTimeFormatInfo.ShortestDayNames = ShortestDayNames;
				return dateTimeFormatInfo;
			}
		}

		[JsonIgnore]
		public NumberFormatInfo NumberFormat => new NumberFormatInfo
		{
			CurrencyDecimalDigits = CurrencyDecimalDigits,
			CurrencyDecimalSeparator = CurrencyDecimalSeparator,
			CurrencyGroupSeparator = CurrencyGroupSeparator,
			CurrencyGroupSizes = CurrencyGroupSizes,
			CurrencyNegativePattern = CurrencyNegativePattern,
			CurrencyPositivePattern = CurrencyPositivePattern,
			CurrencySymbol = CurrencySymbol,
			DigitSubstitution = DigitSubstitution,
			NaNSymbol = NaNSymbol,
			NativeDigits = NativeDigits,
			NegativeInfinitySymbol = NegativeInfinitySymbol,
			NegativeSign = NegativeSign,
			NumberDecimalDigits = NumberDecimalDigits,
			NumberDecimalSeparator = NumberDecimalSeparator,
			NumberGroupSeparator = NumberGroupSeparator,
			NumberGroupSizes = NumberGroupSizes,
			NumberNegativePattern = NumberNegativePattern,
			PercentDecimalDigits = PercentDecimalDigits,
			PercentDecimalSeparator = PercentDecimalSeparator,
			PercentGroupSeparator = PercentGroupSeparator,
			PercentGroupSizes = PercentGroupSizes,
			PercentNegativePattern = PercentNegativePattern,
			PercentPositivePattern = PercentPositivePattern,
			PercentSymbol = PercentSymbol,
			PerMilleSymbol = PerMilleSymbol,
			PositiveInfinitySymbol = PositiveInfinitySymbol,
			PositiveSign = PositiveSign
		};

		public int CurrencyDecimalDigits
		{
			get;
			set;
		}

		public int[] CurrencyGroupSizes
		{
			get;
			set;
		}

		public int CurrencyNegativePattern
		{
			get;
			set;
		}

		public string CurrencySymbol
		{
			get;
			set;
		}

		public DigitShapes DigitSubstitution
		{
			get;
			set;
		}

		public string NaNSymbol
		{
			get;
			set;
		}

		public string NegativeInfinitySymbol
		{
			get;
			set;
		}

		public int NumberDecimalDigits
		{
			get;
			set;
		}

		public int PercentDecimalDigits
		{
			get;
			set;
		}

		public string PercentDecimalSeparator
		{
			get;
			set;
		}

		public string PercentGroupSeparator
		{
			get;
			set;
		}

		public int[] PercentGroupSizes
		{
			get;
			set;
		}

		public int PercentNegativePattern
		{
			get;
			set;
		}

		public int PercentPositivePattern
		{
			get;
			set;
		}

		public string PercentSymbol
		{
			get;
			set;
		}

		public string PerMilleSymbol
		{
			get;
			set;
		}

		public string PositiveInfinitySymbol
		{
			get;
			set;
		}

		public string[] ShortestDayNames
		{
			get;
			set;
		}

		public string FullDateTimePattern
		{
			get;
			set;
		}

		public string LongDatePattern
		{
			get;
			set;
		}

		public string LongTimePattern
		{
			get;
			set;
		}

		public string ShortDatePattern
		{
			get;
			set;
		}

		public string ShortTimePattern
		{
			get;
			set;
		}

		public string NativeCalendarName
		{
			get;
			set;
		}

		public int CurrencyPositivePattern
		{
			get;
			set;
		}

		public bool CurrencyPrecedesNumber
		{
			get;
			set;
		}

		public string[] NativeDigits
		{
			get;
			set;
		}

		public string NegativeSign
		{
			get;
			set;
		}

		public string PositiveSign
		{
			get;
			set;
		}

		public int[] NumberGroupSizes
		{
			get;
			set;
		}

		public int NumberNegativePattern
		{
			get;
			set;
		}

		public string NumberGroupSeparator
		{
			get;
			set;
		}

		public string NumberDecimalSeparator
		{
			get;
			set;
		}

		public string CurrencyGroupSeparator
		{
			get;
			set;
		}

		public string CurrencyDecimalSeparator
		{
			get;
			set;
		}

		public string DateSeparator
		{
			get;
			set;
		}

		public string TimeSeparator
		{
			get;
			set;
		}

		public string AMDesignator
		{
			get;
			set;
		}

		public string PMDesignator
		{
			get;
			set;
		}

		public string[] DateTimePatterns
		{
			get;
			set;
		}

		public string[] AbbreviatedMonthNames
		{
			get;
			set;
		}

		public string[] AbbreviatedMonthGenitiveNames
		{
			get;
			set;
		}

		public string[] MonthNames
		{
			get;
			set;
		}

		public string[] MonthGenitiveNames
		{
			get;
			set;
		}

		public string[] AbbreviatedDayNames
		{
			get;
			set;
		}

		public string[] DayNames
		{
			get;
			set;
		}

		public string[] EraNames
		{
			get;
			set;
		}

		public LanguageMetadata()
		{
			Version = 1;
		}

		public LanguageMetadata(CultureInfo culture)
		{
			Name = culture.Name;
			Version = 1;
			CurrencyPositivePattern = culture.NumberFormat.CurrencyPositivePattern;
			CurrencyPrecedesNumber = (CurrencyPositivePattern % 2 == 0);
			CurrencyDecimalDigits = culture.NumberFormat.CurrencyDecimalDigits;
			CurrencyGroupSizes = culture.NumberFormat.CurrencyGroupSizes;
			CurrencyNegativePattern = culture.NumberFormat.CurrencyNegativePattern;
			CurrencySymbol = culture.NumberFormat.CurrencySymbol;
			DigitSubstitution = culture.NumberFormat.DigitSubstitution;
			NaNSymbol = culture.NumberFormat.NaNSymbol;
			NegativeInfinitySymbol = culture.NumberFormat.NegativeInfinitySymbol;
			NumberDecimalDigits = culture.NumberFormat.NumberDecimalDigits;
			PercentDecimalDigits = culture.NumberFormat.PercentDecimalDigits;
			PercentDecimalSeparator = culture.NumberFormat.PercentDecimalSeparator;
			PercentGroupSeparator = culture.NumberFormat.PercentGroupSeparator;
			PercentGroupSizes = culture.NumberFormat.PercentGroupSizes;
			PercentNegativePattern = culture.NumberFormat.PercentNegativePattern;
			PercentPositivePattern = culture.NumberFormat.PercentPositivePattern;
			PercentSymbol = culture.NumberFormat.PercentSymbol;
			PerMilleSymbol = culture.NumberFormat.PerMilleSymbol;
			PositiveInfinitySymbol = culture.NumberFormat.PositiveInfinitySymbol;
			NativeDigits = culture.NumberFormat.NativeDigits;
			NegativeSign = culture.NumberFormat.NegativeSign;
			PositiveSign = culture.NumberFormat.PositiveSign;
			NumberGroupSizes = culture.NumberFormat.NumberGroupSizes;
			NumberNegativePattern = culture.NumberFormat.NumberNegativePattern;
			NumberGroupSeparator = culture.NumberFormat.NumberGroupSeparator;
			NumberDecimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
			CurrencyGroupSeparator = culture.NumberFormat.CurrencyGroupSeparator;
			CurrencyDecimalSeparator = culture.NumberFormat.CurrencyDecimalSeparator;
			DateTimePatterns = culture.DateTimeFormat.GetAllDateTimePatterns();
			DateSeparator = culture.DateTimeFormat.DateSeparator;
			TimeSeparator = culture.DateTimeFormat.TimeSeparator;
			AbbreviatedMonthNames = culture.DateTimeFormat.AbbreviatedMonthNames;
			AbbreviatedMonthGenitiveNames = culture.DateTimeFormat.AbbreviatedMonthGenitiveNames;
			MonthNames = culture.DateTimeFormat.MonthNames;
			MonthGenitiveNames = culture.DateTimeFormat.MonthGenitiveNames;
			DayNames = culture.DateTimeFormat.DayNames;
			AbbreviatedDayNames = culture.DateTimeFormat.AbbreviatedDayNames;
			AMDesignator = culture.DateTimeFormat.AMDesignator;
			PMDesignator = culture.DateTimeFormat.PMDesignator;
			FullDateTimePattern = culture.DateTimeFormat.FullDateTimePattern;
			LongDatePattern = culture.DateTimeFormat.LongDatePattern;
			LongTimePattern = culture.DateTimeFormat.LongTimePattern;
			NativeCalendarName = culture.DateTimeFormat.NativeCalendarName;
			ShortDatePattern = culture.DateTimeFormat.ShortDatePattern;
			ShortTimePattern = culture.DateTimeFormat.ShortTimePattern;
			ShortestDayNames = culture.DateTimeFormat.ShortestDayNames;
			EraNames = culture.DateTimeFormat.Calendar.Eras.Select((int era) => culture.DateTimeFormat.GetEraName(era)).ToArray();
		}

		public static LanguageMetadata GetOrCreateMetadata(string langCode)
		{
			return GetMetadata(langCode) ?? new LanguageMetadata(new CultureInfo(langCode));
		}

		public static LanguageMetadata GetMetadata(string langCode)
		{
			lock (Locker)
			{
				if (_metadata == null)
				{
					LoadMetadata();
				}
			}
			_metadata.TryGetValue(langCode.ToLowerInvariant(), out LanguageMetadata value);
			return value;
		}

		public static List<string> GetMetadataLanguageCodes()
		{
			lock (Locker)
			{
				if (_metadata == null)
				{
					LoadMetadata();
				}
			}
			List<string> list = new List<string>();
			if (_metadata == null)
			{
				return list;
			}
			foreach (KeyValuePair<string, LanguageMetadata> metadatum in _metadata)
			{
				list.Add(metadatum.Value.Name);
			}
			return list;
		}

		private static void LoadMetadata()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string folderName = executingAssembly.GetName().Name + ".Metadata";
			List<string> list = (from r in executingAssembly.GetManifestResourceNames()
				where r.StartsWith(folderName) && r.EndsWith(".json")
				select r).ToList();
			_metadata = new Dictionary<string, LanguageMetadata>();
			foreach (string item in list)
			{
				Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(item);
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					string value = streamReader.ReadToEnd();
					LanguageMetadata languageMetadata = JsonConvert.DeserializeObject<LanguageMetadata>(value);
					_metadata.Add(languageMetadata.Name.ToLowerInvariant(), languageMetadata);
				}
			}
		}
	}
}
