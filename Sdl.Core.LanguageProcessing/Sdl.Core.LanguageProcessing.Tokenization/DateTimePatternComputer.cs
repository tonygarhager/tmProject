using Sdl.Core.LanguageProcessing.Tokenization.Transducer;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class DateTimePatternComputer
	{
		internal static CultureInfo WorkaroundCulture = GetWorkaroundCulture();

		private readonly CultureInfo _culture;

		private readonly IResourceDataAccessor _accessor;

		public static List<CalendarDateTimePatterns> GetPatterns(CultureInfo culture, IResourceDataAccessor accessor, DateTimePatternType types)
		{
			return GetPatterns(culture, accessor, types, new Dictionary<DateTimePatternType, DateTimeFSTEx>());
		}

		internal static List<CalendarDateTimePatterns> GetPatterns(CultureInfo culture, IResourceDataAccessor accessor, DateTimePatternType types, Dictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap)
		{
			return LoadDateTimeFsTs(culture, accessor, types, dateTimeFstExMap) ?? CreateDateTimeFsTs(culture, LanguageMetadata.GetOrCreateMetadata(culture.Name), types, new List<string>(), null, customOnly: false);
		}

		internal static List<CalendarDateTimePatterns> GetPatterns(CultureInfo culture, LanguageMetadata lm, DateTimePatternType types, List<string> customPatterns, Dictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap, bool customOnly)
		{
			return CreateDateTimeFsTs(culture, lm, types, customPatterns, dateTimeFstExMap, customOnly);
		}

		private static CultureInfo GetWorkaroundCulture()
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			Array.Sort(cultures, (CultureInfo x, CultureInfo y) => string.CompareOrdinal(x.Name, y.Name));
			return cultures.FirstOrDefault((CultureInfo x) => x.Name == "uz-Arab-AF");
		}

		public static HashSet<string> GetSupportedDateTimeFormatElements()
		{
			return new HashSet<string>
			{
				"d",
				"dd",
				"ddd",
				"dddd",
				"h",
				"hh",
				"H",
				"HH",
				"m",
				"mm",
				"M",
				"MM",
				"MMM",
				"MMMM",
				"s",
				"ss",
				"t",
				"tt",
				"gg",
				"y",
				"yy",
				"yyyy"
			};
		}

		public static List<(LanguageResourceType FSTLanguageResourceType, FST FST, LanguageResourceType FSTExLanguageResourceType, DateTimeFSTEx DateTimeFSTEx)> GetDateTimeLanguageResources(CultureInfo ci, LanguageMetadata lm, List<string> customPatterns, bool customOnly)
		{
			List<(LanguageResourceType, FST, LanguageResourceType, DateTimeFSTEx)> list = new List<(LanguageResourceType, FST, LanguageResourceType, DateTimeFSTEx)>();
			CultureInfo culture = ci;
			if (ci.Name == "uz-Arab")
			{
				culture = WorkaroundCulture;
			}
			if (customOnly && (customPatterns == null || customPatterns.Count == 0))
			{
				throw new ArgumentException("customOnly specified but no custom patterns were provided.");
			}
			Dictionary<DateTimePatternType, DateTimeFSTEx> dictionary = new Dictionary<DateTimePatternType, DateTimeFSTEx>();
			List<CalendarDateTimePatterns> patterns = GetPatterns(culture, lm, DateTimePatternType.LongDate | DateTimePatternType.ShortDate | DateTimePatternType.ShortTime | DateTimePatternType.LongTime, customPatterns, dictionary, customOnly);
			if (patterns.Count != 1)
			{
				throw new Exception("Unexpected no. of calendars while creating FSTs");
			}
			CalendarDateTimePatterns calendarDateTimePatterns = patterns[0];
			foreach (DateTimePattern pattern in calendarDateTimePatterns.Patterns)
			{
				FST fst = pattern.Fst;
				LanguageResourceType item;
				LanguageResourceType item2;
				switch (pattern.PatternType)
				{
				case DateTimePatternType.LongDate:
					item = LanguageResourceType.LongDateFST;
					item2 = LanguageResourceType.LongDateFSTEx;
					break;
				case DateTimePatternType.ShortDate:
					item = LanguageResourceType.ShortDateFST;
					item2 = LanguageResourceType.ShortDateFSTEx;
					break;
				case DateTimePatternType.LongTime:
					item = LanguageResourceType.LongTimeFST;
					item2 = LanguageResourceType.LongTimeFSTEx;
					break;
				case DateTimePatternType.ShortTime:
					item = LanguageResourceType.ShortTimeFST;
					item2 = LanguageResourceType.ShortTimeFSTEx;
					break;
				default:
					throw new Exception("Unknown PatternType " + pattern.PatternType.ToString());
				}
				DateTimeFSTEx item3 = dictionary[pattern.PatternType];
				list.Add((item, fst, item2, item3));
			}
			return list;
		}

		private static List<CalendarDateTimePatterns> CreateDateTimeFsTs(CultureInfo culture, LanguageMetadata lm, DateTimePatternType types, List<string> customPatterns, Dictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap, bool customOnly)
		{
			DateTimePatternComputer dateTimePatternComputer = new DateTimePatternComputer(culture, null);
			return dateTimePatternComputer.GetFsTsForMultipleCalendarsAndTypes(lm, types, customPatterns, dateTimeFstExMap, customOnly);
		}

		private static List<CalendarDateTimePatterns> LoadDateTimeFsTs(CultureInfo culture, IResourceDataAccessor accessor, DateTimePatternType types, IDictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap)
		{
			DateTimePatternComputer dateTimePatternComputer = new DateTimePatternComputer(culture, accessor);
			return dateTimePatternComputer.LoadPatterns(types, dateTimeFstExMap);
		}

		private DateTimePatternComputer(CultureInfo culture, IResourceDataAccessor accessor)
		{
			_culture = culture;
			_accessor = accessor;
		}

		private List<CalendarDateTimePatterns> LoadPatterns(DateTimePatternType types, IDictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap)
		{
			if (_accessor == null)
			{
				return null;
			}
			DateTimePatternType[] array = new DateTimePatternType[4]
			{
				DateTimePatternType.LongDate,
				DateTimePatternType.ShortDate,
				DateTimePatternType.LongTime,
				DateTimePatternType.ShortTime
			};
			List<CalendarDateTimePatterns> list = null;
			CalendarDateTimePatterns calendarDateTimePatterns = null;
			DateTimePatternType[] array2 = array;
			foreach (DateTimePatternType dateTimePatternType in array2)
			{
				if ((types & dateTimePatternType) == 0)
				{
					continue;
				}
				LanguageResourceType t;
				LanguageResourceType languageResourceType;
				switch (dateTimePatternType)
				{
				case DateTimePatternType.LongDate:
					t = LanguageResourceType.LongDateFST;
					languageResourceType = LanguageResourceType.LongDateFSTEx;
					break;
				case DateTimePatternType.ShortDate:
					t = LanguageResourceType.ShortDateFST;
					languageResourceType = LanguageResourceType.ShortDateFSTEx;
					break;
				case DateTimePatternType.ShortTime:
					t = LanguageResourceType.ShortTimeFST;
					languageResourceType = LanguageResourceType.ShortTimeFSTEx;
					break;
				case DateTimePatternType.LongTime:
					t = LanguageResourceType.LongTimeFST;
					languageResourceType = LanguageResourceType.LongTimeFSTEx;
					break;
				default:
					throw new Exception("Cannot map token type to corresponding resource type");
				}
				if (_accessor.GetResourceStatus(_culture, t, fallback: false) != ResourceStatus.NotAvailable)
				{
					byte[] resourceData = _accessor.GetResourceData(_culture, t, fallback: false);
					FST fst = FST.Create(resourceData);
					if (calendarDateTimePatterns == null)
					{
						list = new List<CalendarDateTimePatterns>();
						calendarDateTimePatterns = new CalendarDateTimePatterns(_culture, null);
						list.Add(calendarDateTimePatterns);
					}
					calendarDateTimePatterns.Patterns.Add(new DateTimePattern(dateTimePatternType, _culture, "(unavailable)", fst));
					if (languageResourceType != 0 && dateTimeFstExMap != null && _accessor.GetResourceStatus(_culture, languageResourceType, fallback: false) != ResourceStatus.NotAvailable)
					{
						resourceData = _accessor.GetResourceData(_culture, languageResourceType, fallback: false);
						DateTimeFSTEx value = DateTimeFSTEx.FromBinary(resourceData);
						dateTimeFstExMap.Add(dateTimePatternType, value);
					}
				}
			}
			return list;
		}

		private List<CalendarDateTimePatterns> GetFsTsForMultipleCalendarsAndTypes(LanguageMetadata lm, DateTimePatternType types, List<string> customPatterns, Dictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap, bool customOnly)
		{
			List<CalendarDateTimePatterns> list = new List<CalendarDateTimePatterns>();
			if (customPatterns != null)
			{
				DateTime now = DateTime.Now;
				foreach (string customPattern in customPatterns)
				{
					string s = now.ToString(customPattern, lm.DateTimeFormat);
					if (!DateTime.TryParseExact(s, customPattern, lm.DateTimeFormat, DateTimeStyles.None, out DateTime _))
					{
						throw new ArgumentException("customPatterns", "The format could not be parsed back to DateTime: " + customPattern);
					}
				}
			}
			list.Add(ComputeFsTsForSingleCalendar(lm, types, customPatterns, dateTimeFstExMap, customOnly));
			return list;
		}

		public static bool IgnorePattern(CultureInfo culture, string pattern)
		{
			switch (culture.Name)
			{
			case "en-US":
				return pattern.Equals("yy/MM/dd", StringComparison.Ordinal);
			case "de-DE":
				if (!pattern.Equals("dd. MMM yyyy", StringComparison.Ordinal) && !pattern.Equals("d. MMM yyyy", StringComparison.Ordinal) && !pattern.Equals("HH", StringComparison.Ordinal))
				{
					return pattern.Equals("HHmmss", StringComparison.Ordinal);
				}
				return true;
			case "de-AT":
			case "de-CH":
			case "de-LI":
				if (!pattern.Equals("HH", StringComparison.Ordinal) && !pattern.Equals("HHmm", StringComparison.Ordinal))
				{
					return pattern.Equals("HHmmss", StringComparison.Ordinal);
				}
				return true;
			case "de-LU":
				if (!pattern.Equals("HH", StringComparison.Ordinal))
				{
					return pattern.Equals("HHmmss", StringComparison.Ordinal);
				}
				return true;
			default:
				return pattern.Equals("HH", StringComparison.Ordinal);
			}
		}

		public static List<string> GetCustomPatterns(CultureInfo culture)
		{
			List<string> list = null;
			switch (culture.Name)
			{
			case "et-EE":
				list = new List<string>();
				list.Add("d. MMMM yyyy");
				list.Add("dd. MMMM yyyy");
				break;
			case "lv-LV":
				list = new List<string>();
				list.Add("yyyy'. gada 'd. MMMM");
				list.Add("yyyy'. gada 'dd. MMMM");
				list.Add("yyyy. MMMM dd");
				break;
			case "nl-NL":
			case "nl-BE":
				list = new List<string>();
				list.Insert(0, "d 'de' MMMM yyyy");
				list.Insert(0, "dd 'de' MMMM yyyy");
				break;
			case "da-DK":
				list = new List<string>();
				list.Add("d/M/yy");
				list.Add("d/M/yyyy");
				list.Add("dd/MM/yyyy");
				list.Add("d.M.yy");
				list.Add("d.M.yyyy");
				list.Add("dd.MM.yyyy");
				break;
			case "pl-PL":
				list = new List<string>();
				list.Add("dd.MM.yyyy");
				list.Add("dd.MM.yy");
				break;
			case "en-GB":
				list = new List<string>();
				list.Add("dddd, dd MMMM yyyy");
				list.Add("dddd, d MMMM yyyy");
				break;
			case "el-GR":
				list = new List<string>();
				list.Add("d.M.yy");
				list.Add("d.M.yyyy");
				list.Add("dd.MM.yyyy");
				break;
			case "hu-HU":
				list = new List<string>();
				list.Add("yyyy. MMMM dd.");
				break;
			case "cs-CZ":
				list = new List<string>();
				list.Add("h:mm tt");
				break;
			case "fr-CH":
				list = new List<string>();
				list.Add("dddd d MMMM yyyy");
				list.Add("d MMMM yyyy");
				list.Add("d MMM yy");
				break;
			}
			return list;
		}

		private CalendarDateTimePatterns ComputeFsTsForSingleCalendar(LanguageMetadata lm, DateTimePatternType types, List<string> customPatterns, IDictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap, bool customOnly)
		{
			List<string> list = new List<string>();
			CalendarDateTimePatterns calendarDateTimePatterns = new CalendarDateTimePatterns(_culture, _culture.Calendar);
			if (!customOnly)
			{
				list = new List<string>(lm.DateTimePatterns.Where((string x) => CheckPatternConsistency(x, lm.Name)));
			}
			if (customPatterns != null)
			{
				list.AddRange(customPatterns);
			}
			if (!customOnly)
			{
				List<string> customPatterns2 = GetCustomPatterns(_culture);
				if (customPatterns2 != null)
				{
					list.AddRange(customPatterns2);
				}
			}
			list = new List<string>(list.Select((string x) => x.Trim()));
			List<string> list2 = new List<string>();
			foreach (string item in list)
			{
				if (!list2.Contains(item) && !IgnorePattern(_culture, item))
				{
					list2.Add(item);
					string rx;
					DateTimePatternType dateTimePatternType = ClassifyFormatString(lm, item, out rx);
					if (dateTimePatternType != 0 && (types & dateTimePatternType) != 0)
					{
						if (dateTimeFstExMap != null)
						{
							if (!dateTimeFstExMap.ContainsKey(dateTimePatternType))
							{
								dateTimeFstExMap.Add(dateTimePatternType, new DateTimeFSTEx());
							}
							dateTimeFstExMap[dateTimePatternType].Patterns.Add(item);
						}
						FST fST = FST.Create(rx);
						fST.MakeDeterministic();
						calendarDateTimePatterns.Patterns.Add(new DateTimePattern(dateTimePatternType, _culture, item, fST));
					}
				}
			}
			HashSet<DateTimePatternType> hashSet = new HashSet<DateTimePatternType>
			{
				DateTimePatternType.LongDate,
				DateTimePatternType.ShortDate,
				DateTimePatternType.LongTime,
				DateTimePatternType.ShortTime
			};
			foreach (DateTimePatternType patternType in hashSet)
			{
				if (!calendarDateTimePatterns.Patterns.Any((DateTimePattern x) => x.PatternType == patternType))
				{
					FST fST2 = FST.Create("<X:X>");
					fST2.MakeDeterministic();
					calendarDateTimePatterns.Patterns.Add(new DateTimePattern(patternType, _culture, "NULL", fST2));
					if (dateTimeFstExMap != null)
					{
						if (!dateTimeFstExMap.ContainsKey(patternType))
						{
							dateTimeFstExMap.Add(patternType, new DateTimeFSTEx());
						}
						dateTimeFstExMap[patternType].Patterns.Add("");
					}
				}
			}
			if (calendarDateTimePatterns.Patterns.Count <= 0)
			{
				return calendarDateTimePatterns;
			}
			calendarDateTimePatterns.Patterns.Sort((DateTimePattern a, DateTimePattern b) => a.PatternType - b.PatternType);
			List<DateTimePattern> list3 = new List<DateTimePattern>();
			while (calendarDateTimePatterns.Patterns.Count > 0)
			{
				DateTimePatternType patternType2 = calendarDateTimePatterns.Patterns[0].PatternType;
				int i = 1;
				List<FST> list4 = new List<FST>();
				for (; i < calendarDateTimePatterns.Patterns.Count && calendarDateTimePatterns.Patterns[i].PatternType == patternType2; i++)
				{
					list4.Add(calendarDateTimePatterns.Patterns[i].Fst);
				}
				calendarDateTimePatterns.Patterns[0].Fst.Disjunct(list4);
				calendarDateTimePatterns.Patterns[0].Fst.MakeDeterministic();
				list3.Add(calendarDateTimePatterns.Patterns[0]);
				calendarDateTimePatterns.Patterns.RemoveRange(0, i);
			}
			calendarDateTimePatterns.Patterns.AddRange(list3);
			return calendarDateTimePatterns;
		}

		private static void AppendLiteral(StringBuilder builder, string s)
		{
			if (s.Length != 0)
			{
				foreach (char c in s)
				{
					AppendLiteral(builder, c);
				}
			}
		}

		private static void EmitChar(StringBuilder builder, char c)
		{
			if (FST.ReservedCharacters.IndexOf(c) >= 0)
			{
				builder.Append('\\');
			}
			builder.Append(c);
		}

		private static void AppendLiteral(StringBuilder builder, char c)
		{
			builder.Append('<');
			if (c != 0)
			{
				EmitChar(builder, c);
			}
			builder.Append(":>");
		}

		private void AppendLiterals(StringBuilder builder, IReadOnlyList<string> coll)
		{
			if (coll.Count == 0)
			{
				return;
			}
			builder.Append("(");
			for (int i = 0; i < coll.Count; i++)
			{
				if (i > 0)
				{
					builder.Append("|");
				}
				AppendLiteral(builder, coll[i]);
			}
			builder.Append(")");
		}

		public static DateTimePatternType ClassifyPattern(string formatString)
		{
			DateTimePatternComputer dateTimePatternComputer = new DateTimePatternComputer(CultureInfo.InvariantCulture, null);
			string rx;
			return dateTimePatternComputer.ClassifyFormatString(new LanguageMetadata(CultureInfo.InvariantCulture), formatString, out rx);
		}

		public static DateTimePatternType ClassifyPattern(string formatString, string langCode)
		{
			LanguageMetadata orCreateMetadata = LanguageMetadata.GetOrCreateMetadata(langCode);
			CultureInfo cultureInfo = CultureInfoExtensions.GetCultureInfo(langCode);
			DateTimePatternComputer dateTimePatternComputer = new DateTimePatternComputer(cultureInfo, null);
			string rx;
			return dateTimePatternComputer.ClassifyFormatString(orCreateMetadata, formatString, out rx);
		}

		public static bool CheckPatternConsistency(string formatString, string langCode)
		{
			LanguageMetadata orCreateMetadata = LanguageMetadata.GetOrCreateMetadata(langCode);
			DateTime now = DateTime.Now;
			try
			{
				string s = now.ToString(formatString, orCreateMetadata.DateTimeFormat);
				DateTime result;
				return DateTime.TryParseExact(s, formatString, orCreateMetadata.DateTimeFormat, DateTimeStyles.None, out result);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private DateTimePatternType GetPattern(LanguageMetadata lm, char designator, int length, ref bool hasDateComponents, ref bool hasTimeComponents, out string pattern)
		{
			pattern = string.Empty;
			switch (designator)
			{
			case 'd':
				hasDateComponents = true;
				switch (length)
				{
				case 1:
					pattern = "(<:" + designator.ToString() + ">(((1|2|3)(0|1|2|3|4|5|6|7|8|9))|(1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortDate;
				case 2:
					pattern = "(<:" + designator.ToString() + ">((0|1|2|3)(0|1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortDate;
				case 3:
					pattern = AppendDayNames(getAbbreviatedNames: true, lm);
					return DateTimePatternType.ShortDate;
				case 4:
					pattern = AppendDayNames(getAbbreviatedNames: false, lm);
					return DateTimePatternType.LongDate;
				default:
					return DateTimePatternType.Unknown;
				}
			case 'F':
			case 'f':
				return DateTimePatternType.Unknown;
			case 'h':
				hasTimeComponents = true;
				switch (length)
				{
				case 1:
					pattern = "(<:" + designator.ToString() + ">(((1)(0|1|2))|(1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortTime;
				case 2:
					pattern = "(<:" + designator.ToString() + ">((0|1)(0|1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortTime;
				default:
					return DateTimePatternType.Unknown;
				}
			case 'm':
			case 's':
				hasTimeComponents = true;
				switch (length)
				{
				case 1:
					pattern = "(<:" + designator.ToString() + ">(((1|2|3|4|5)(0|1|2|3|4|5|6|7|8|9))|(0|1|2|3|4|5|6|7|8|9)))";
					break;
				case 2:
					pattern = "(<:" + designator.ToString() + ">((0|1|2|3|4|5)(0|1|2|3|4|5|6|7|8|9)))";
					break;
				default:
					return DateTimePatternType.Unknown;
				}
				return (designator == 's') ? DateTimePatternType.LongTime : DateTimePatternType.ShortTime;
			case 't':
			{
				hasTimeComponents = true;
				if (string.IsNullOrWhiteSpace(lm.AMDesignator) || string.IsNullOrWhiteSpace(lm.PMDesignator))
				{
					return DateTimePatternType.Unknown;
				}
				string text2 = AppendAmpm(length == 1, lm);
				if (text2 != null)
				{
					pattern = text2;
				}
				return DateTimePatternType.ShortTime;
			}
			case 'y':
				hasDateComponents = true;
				switch (length)
				{
				case 1:
					pattern = "(<:y>((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|(0|1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortDate;
				case 2:
					pattern = "(<:y>((0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortDate;
				case 4:
					pattern = "(<:y>((0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortDate;
				default:
					return DateTimePatternType.Unknown;
				}
			case 'g':
				hasDateComponents = true;
				if (length == 2)
				{
					string text = AppendEras(lm);
					if (text != null)
					{
						pattern = text;
					}
					return DateTimePatternType.ShortDate;
				}
				return DateTimePatternType.Unknown;
			case 'z':
				return DateTimePatternType.Unknown;
			case 'H':
				hasTimeComponents = true;
				switch (length)
				{
				case 1:
					pattern = "(<:H>((1(0|1|2|3|4|5|6|7|8|9))|(2(0|1|2|3|4))|(1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortTime;
				case 2:
					pattern = "(<:H>((1(0|1|2|3|4|5|6|7|8|9))|(2(0|1|2|3|4))|(0(0|1|2|3|4|5|6|7|8|9))))";
					return DateTimePatternType.ShortTime;
				default:
					return DateTimePatternType.Unknown;
				}
			case 'M':
				hasDateComponents = true;
				switch (length)
				{
				case 1:
					pattern = "(<:M>(1(0|1|2)|(1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortDate;
				case 2:
					pattern = "(<:M>(1(0|1|2)|0(1|2|3|4|5|6|7|8|9)))";
					return DateTimePatternType.ShortDate;
				case 3:
					pattern = AppendMonthNames(getAbbreviatedNames: true, lm);
					return DateTimePatternType.ShortDate;
				case 4:
					pattern = AppendMonthNames(getAbbreviatedNames: false, lm);
					return DateTimePatternType.LongDate;
				default:
					return DateTimePatternType.Unknown;
				}
			default:
				return DateTimePatternType.Unknown;
			}
		}

		private DateTimePatternType ClassifyFormatString(LanguageMetadata lm, string formatString, out string rx)
		{
			int length = formatString.Length;
			int i = 0;
			DateTimePatternType dateTimePatternType = DateTimePatternType.Unknown;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool hasDateComponents = false;
			bool hasTimeComponents = false;
			bool flag4 = false;
			rx = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag5 = false;
			while (i < length)
			{
				char c = formatString[i];
				if (flag5)
				{
					if (c != '\'')
					{
						if (c == '\\')
						{
							if (i + 1 < length && formatString[i + 1] == '\'')
							{
								AppendLiteral(stringBuilder, '\'');
								flag4 = true;
								i++;
							}
							else
							{
								AppendLiteral(stringBuilder, c);
								flag4 = true;
							}
						}
						else
						{
							if (c == '年' || c == '月' || c == '日')
							{
								dateTimePatternType = DateTimePatternType.LongDate;
							}
							AppendLiteral(stringBuilder, c);
							flag4 = true;
						}
					}
					else if (i + 1 < length && formatString[i + 1] == '\'')
					{
						AppendLiteral(stringBuilder, c);
						flag4 = true;
						i++;
					}
					else
					{
						flag5 = false;
					}
					i++;
					continue;
				}
				if ("dfFhmstyzgHM".IndexOf(c) >= 0)
				{
					int num = 0;
					for (; i < length && formatString[i] == c; i++)
					{
						num++;
					}
					switch (c)
					{
					case 'd':
						flag |= (num == 1 || num == 2);
						break;
					case 'M':
						flag2 = true;
						break;
					case 'y':
						flag3 = true;
						break;
					}
					string pattern;
					DateTimePatternType pattern2 = GetPattern(lm, c, num, ref hasDateComponents, ref hasTimeComponents, out pattern);
					if (string.IsNullOrEmpty(pattern))
					{
						return DateTimePatternType.Unknown;
					}
					stringBuilder.Append(pattern);
					switch (dateTimePatternType)
					{
					case DateTimePatternType.Unknown:
						dateTimePatternType = pattern2;
						break;
					case DateTimePatternType.ShortDate:
						if (pattern2 == DateTimePatternType.LongDate)
						{
							dateTimePatternType = DateTimePatternType.LongDate;
						}
						break;
					case DateTimePatternType.LongTime:
						if (pattern2 == DateTimePatternType.LongDate || pattern2 == DateTimePatternType.ShortDate)
						{
							dateTimePatternType = pattern2;
						}
						break;
					case DateTimePatternType.ShortTime:
						if (pattern2 != 0)
						{
							dateTimePatternType = pattern2;
						}
						break;
					default:
						throw new Exception("Invalid switch constant");
					case DateTimePatternType.LongDate:
						break;
					}
					continue;
				}
				switch (c)
				{
				case '\'':
					i++;
					flag5 = true;
					break;
				case '%':
					i++;
					break;
				case ':':
					AppendLiteral(stringBuilder, c);
					flag4 = true;
					i++;
					break;
				case '/':
					AppendLiteral(stringBuilder, c);
					flag4 = true;
					i++;
					break;
				case '\\':
					if (i + 1 < length)
					{
						AppendLiteral(stringBuilder, formatString[i + 1]);
						flag4 = true;
						i++;
					}
					else
					{
						AppendLiteral(stringBuilder, c);
						flag4 = true;
						i++;
					}
					break;
				case ' ':
				case '\u00a0':
					stringBuilder.Append("<\\s:>");
					flag4 = true;
					i++;
					break;
				default:
					AppendLiteral(stringBuilder, c);
					flag4 = true;
					i++;
					break;
				}
			}
			if ((dateTimePatternType == DateTimePatternType.LongDate || dateTimePatternType == DateTimePatternType.ShortDate) && !((flag && flag2) & flag3))
			{
				return DateTimePatternType.Unknown;
			}
			if ((hasDateComponents && hasTimeComponents) || (!hasDateComponents && !hasTimeComponents))
			{
				return DateTimePatternType.Unknown;
			}
			if (!flag4)
			{
				return DateTimePatternType.Unknown;
			}
			rx = stringBuilder.ToString();
			return dateTimePatternType;
		}

		private static string AppendAmpm(bool abbreviated, LanguageMetadata lm)
		{
			string text = lm.AMDesignator ?? string.Empty;
			string text2 = lm.PMDesignator ?? string.Empty;
			if (text.Length == 0 && text2.Length == 0)
			{
				return string.Empty;
			}
			string str;
			if (abbreviated)
			{
				str = "t";
				if (text.Length > 1)
				{
					text = text.Substring(0, 1);
				}
				if (text2.Length > 1)
				{
					text2 = text2.Substring(0, 1);
				}
			}
			else
			{
				str = "tt";
			}
			StringBuilder stringBuilder = new StringBuilder("(");
			int num = 0;
			if (text.Length > 0)
			{
				stringBuilder.Append(FstCombine(text, str + "'" + text + "'"));
				num++;
			}
			if (text2.Length > 0)
			{
				if (num > 0)
				{
					stringBuilder.Append("|");
				}
				stringBuilder.Append(FstCombine(text2, str + "'" + text2 + "'"));
				num++;
			}
			stringBuilder.Append(")");
			if (num == 1)
			{
				stringBuilder.Append("?");
			}
			return stringBuilder.ToString();
		}

		private static string AppendMonthNames(bool getAbbreviatedNames, LanguageMetadata lm)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			stringBuilder.Append("(");
			for (int i = 0; i < 2; i++)
			{
				string[] array = (i != 0) ? (getAbbreviatedNames ? lm.AbbreviatedMonthGenitiveNames : lm.MonthGenitiveNames) : (getAbbreviatedNames ? lm.AbbreviatedMonthNames : lm.MonthNames);
				for (int j = 0; j < array.Length; j++)
				{
					if (!string.IsNullOrEmpty(array[j]))
					{
						string value = FstCombine(array[j], $"M{j + 1}");
						if (flag)
						{
							flag = false;
						}
						else
						{
							stringBuilder.Append("|");
						}
						stringBuilder.Append(value);
					}
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private string AppendDayNames(bool getAbbreviatedNames, LanguageMetadata lm)
		{
			List<string> list = new List<string>();
			AddStrings(list, getAbbreviatedNames ? lm.AbbreviatedDayNames : lm.DayNames);
			if (list.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			AppendLiterals(stringBuilder, list);
			return stringBuilder.ToString();
		}

		private static string AppendEras(LanguageMetadata lm)
		{
			if (lm.EraNames == null || lm.EraNames.Length == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < lm.EraNames.Length; i++)
			{
				string text = lm.EraNames[i];
				if (i > 0)
				{
					stringBuilder.Append('|');
				}
				stringBuilder.Append(FstCombine(text, "g'" + text + "'"));
			}
			return stringBuilder.ToString();
		}

		private static void AddStrings(ICollection<string> coll, IEnumerable<string> values)
		{
			foreach (string value in values)
			{
				if (!string.IsNullOrEmpty(value) && !coll.Contains(value))
				{
					coll.Add(value);
				}
			}
		}

		private static string FstCombine(string upper, string lower)
		{
			int length = upper.Length;
			int length2 = lower.Length;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < length || i < length2; i++)
			{
				stringBuilder.Append("<");
				if (i < length)
				{
					if (FST.ReservedCharacters.IndexOf(upper[i]) >= 0)
					{
						stringBuilder.Append('\\');
					}
					stringBuilder.Append(upper[i]);
				}
				stringBuilder.Append(":");
				if (i < length2)
				{
					if (FST.ReservedCharacters.IndexOf(lower[i]) >= 0)
					{
						stringBuilder.Append('\\');
					}
					stringBuilder.Append(lower[i]);
				}
				stringBuilder.Append(">");
			}
			return stringBuilder.ToString();
		}
	}
}
