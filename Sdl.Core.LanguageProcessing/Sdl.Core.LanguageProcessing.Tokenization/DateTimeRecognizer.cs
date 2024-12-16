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
	internal class DateTimeRecognizer : Recognizer
	{
		private readonly List<CalendarDateTimePatterns> _calendarPatterns;

		private readonly LanguageMetadata _lm;

		public Dictionary<DateTimePatternType, DateTimeFSTEx> PatternsComputedPerType
		{
			get;
		} = new Dictionary<DateTimePatternType, DateTimeFSTEx>();


		public static Recognizer Create(RecognizerSettings settings, IResourceDataAccessor resourceDataAccessor, CultureInfo culture, DateTimePatternType types, int priority)
		{
			Dictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap = new Dictionary<DateTimePatternType, DateTimeFSTEx>();
			List<CalendarDateTimePatterns> patterns = DateTimePatternComputer.GetPatterns(culture, resourceDataAccessor, types, dateTimeFstExMap);
			return new DateTimeRecognizer(settings, priority, patterns, dateTimeFstExMap, culture)
			{
				OnlyIfFollowedByNonwordCharacter = CultureInfoExtensions.UseBlankAsWordSeparator(culture)
			};
		}

		public override string GetSignature(CultureInfo culture)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.GetSignature(culture));
			stringBuilder.Append("DateTimeRecognizer");
			stringBuilder.Append("1");
			if (culture.Name.StartsWith("en"))
			{
				stringBuilder.Append(".1");
			}
			if (PatternsComputedPerType == null)
			{
				return stringBuilder.ToString();
			}
			List<DateTimePatternType> list = new List<DateTimePatternType>(PatternsComputedPerType.Keys);
			list.Sort();
			foreach (DateTimePatternType item in list)
			{
				stringBuilder.Append(item.ToString());
				stringBuilder.Append(" ");
				int num = 0;
				if (PatternsComputedPerType[item].Patterns != null)
				{
					List<string> list2 = new List<string>(PatternsComputedPerType[item].Patterns);
					list2.Sort((string a, string b) => string.CompareOrdinal(a, b));
					foreach (string item2 in list2)
					{
						stringBuilder.Append(num.ToString());
						stringBuilder.Append(" ");
						stringBuilder.Append(item2);
						stringBuilder.Append(" ");
						num++;
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal DateTimeRecognizer(RecognizerSettings settings, int priority, List<CalendarDateTimePatterns> patterns, CultureInfo culture)
			: base(settings, TokenType.Date, priority, "DateTime", "DateTimeRecognizer", autoSubstitutable: false, culture)
		{
			if (patterns == null || patterns.Count == 0)
			{
				throw new ArgumentNullException("patterns");
			}
			_calendarPatterns = patterns;
		}

		public DateTimeRecognizer(RecognizerSettings settings, int priority, List<CalendarDateTimePatterns> patterns, Dictionary<DateTimePatternType, DateTimeFSTEx> dateTimeFstExMap, CultureInfo culture)
			: this(settings, priority, patterns, culture)
		{
			PatternsComputedPerType = dateTimeFstExMap;
			_lm = LanguageMetadata.GetMetadata(culture.Name);
		}

		public override Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
		{
			int index = -1;
			int num = -1;
			int num2 = -1;
			DateTime dateTime = default(DateTime);
			for (int i = 0; i < _calendarPatterns.Count; i++)
			{
				for (int j = 0; j < _calendarPatterns[i].Patterns.Count; j++)
				{
					DateTime output;
					Match match = _calendarPatterns[i].Patterns[j].Match(s, from, out output);
					if (match != null && match.Length >= num2 && VerifyContextConstraints(s, match.Index + match.Length) && !(output == default(DateTime)))
					{
						index = i;
						num2 = match.Length;
						num = j;
						dateTime = output;
					}
				}
			}
			if (num < 0)
			{
				return null;
			}
			consumedLength = num2;
			string text = s.Substring(from, num2);
			DateTimePatternType patternType = _calendarPatterns[index].Patterns[num].PatternType;
			DateTimeToken dateTimeToken = new DateTimeToken(text, dateTime, patternType, GetMatchingDateTimePattern(patternType, text));
			dateTimeToken.Culture = _Culture;
			return dateTimeToken;
		}

		private List<string> PatternsToTest(DateTimePatternType type)
		{
			List<string> result = DateTimeToken.GetValidDateTimePatterns(_lm).ToList();
			if (PatternsComputedPerType == null)
			{
				return result;
			}
			if (!PatternsComputedPerType.ContainsKey(type))
			{
				return result;
			}
			if (PatternsComputedPerType[type].Patterns == null)
			{
				return result;
			}
			return PatternsComputedPerType[type].Patterns;
		}

		private string GetMatchingDateTimePattern(DateTimePatternType type, string text)
		{
			List<string> patterns = PatternsToTest(type);
			string text2 = DateTimeToken.FindDateTimePattern(text, patterns, _lm?.DateTimeFormat, normalizeWhitespace: false);
			if (text2 == null)
			{
				text2 = DateTimeToken.FindDateTimePattern(text, patterns, _lm?.DateTimeFormat, normalizeWhitespace: true);
			}
			return text2;
		}
	}
}
