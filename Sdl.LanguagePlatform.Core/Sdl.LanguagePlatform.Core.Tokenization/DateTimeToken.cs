using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[DataContract]
	public class DateTimeToken : Token, ILocalizableToken
	{
		private static object _locker = new object();

		private static Dictionary<string, string[]> _ValidDateTimePatterns = new Dictionary<string, string[]>();

		[DataMember]
		public DateTimePatternType DateTimePatternType
		{
			get;
			set;
		}

		[DataMember]
		public string FormatString
		{
			get;
			set;
		}

		[DataMember]
		public DateTime Value
		{
			get;
			set;
		}

		public override bool IsPlaceable => true;

		public override bool IsSubstitutable => true;

		public bool IsDateToken
		{
			get
			{
				if (DateTimePatternType != DateTimePatternType.LongDate)
				{
					return DateTimePatternType == DateTimePatternType.ShortDate;
				}
				return true;
			}
		}

		public bool IsTimeToken
		{
			get
			{
				if (DateTimePatternType != DateTimePatternType.LongTime)
				{
					return DateTimePatternType == DateTimePatternType.ShortTime;
				}
				return true;
			}
		}

		public DateTimeToken()
		{
		}

		public DateTimeToken(DateTimeToken other)
			: base(other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Value = other.Value;
			DateTimePatternType = other.DateTimePatternType;
		}

		public DateTimeToken(string text, DateTime dateTime, DateTimePatternType type)
			: this(text, dateTime, type, null)
		{
		}

		public DateTimeToken(string text, DateTime dateTime, DateTimePatternType type, string formatString)
			: base(text)
		{
			DateTimePatternType = type;
			Value = dateTime;
			FormatString = formatString;
		}

		public virtual bool SetValue(Token blueprint, bool keepNumericSeparators)
		{
			DateTimeToken dateTimeToken = blueprint as DateTimeToken;
			if (dateTimeToken == null)
			{
				throw new ArgumentException("blueprint token does not have the expected type");
			}
			if (Value == dateTimeToken.Value && DateTimePatternType == dateTimeToken.DateTimePatternType)
			{
				return false;
			}
			Value = dateTimeToken.Value;
			DateTimePatternType = dateTimeToken.DateTimePatternType;
			return true;
		}

		public override SegmentElement Duplicate()
		{
			return new DateTimeToken(this);
		}

		public virtual bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings)
		{
			return Localize(culture, autoLocalizationSettings, null, adjustCasing: false);
		}

		private static bool DateTimePatternCheck(string pattern)
		{
			return pattern.Length >= 3;
		}

		internal static string[] GetValidDateTimePatterns(LanguageMetadata lm)
		{
			string[] value = new string[0];
			if (lm == null)
			{
				return value;
			}
			string name = lm.Name;
			lock (_locker)
			{
				if (_ValidDateTimePatterns.TryGetValue(name, out value))
				{
					return value;
				}
				List<string> list = new List<string>();
				string[] dateTimePatterns = lm.DateTimePatterns;
				foreach (string text in dateTimePatterns)
				{
					if (DateTimePatternCheck(text))
					{
						list.Add(text);
					}
				}
				value = list.ToArray();
				_ValidDateTimePatterns.Add(name, value);
				return value;
			}
		}

		internal static string FindDateTimePattern(string text, List<string> patterns, DateTimeFormatInfo formatInfo, bool normalizeWhitespace)
		{
			if (normalizeWhitespace)
			{
				char[] whitespaceCharacters = CharacterProperties.WhitespaceCharacters;
				foreach (char oldChar in whitespaceCharacters)
				{
					text = text.Replace(oldChar, ' ');
				}
			}
			string text2 = null;
			foreach (string pattern in patterns)
			{
				string text3 = pattern;
				if (normalizeWhitespace)
				{
					char[] whitespaceCharacters2 = CharacterProperties.WhitespaceCharacters;
					foreach (char oldChar2 in whitespaceCharacters2)
					{
						text3 = text3.Replace(oldChar2, ' ');
					}
				}
				try
				{
					if (DateTime.TryParseExact(text, text3, formatInfo, DateTimeStyles.None, out DateTime _) && (text2 == null || pattern.Length > text2.Length))
					{
						text2 = pattern;
					}
				}
				catch (Exception)
				{
				}
			}
			return text2;
		}

		private string GetPatternToUse(AutoLocalizationSettings autoLocalizationSettings, DateTimeToken originalMemoryToken, LanguageMetadata lm)
		{
			string patternFromSettings = null;
			string result = null;
			switch (DateTimePatternType)
			{
			case DateTimePatternType.LongDate:
				patternFromSettings = autoLocalizationSettings?.LongDatePattern;
				result = "D";
				break;
			case DateTimePatternType.ShortDate:
				patternFromSettings = autoLocalizationSettings?.ShortDatePattern;
				result = "d";
				break;
			case DateTimePatternType.LongTime:
				patternFromSettings = autoLocalizationSettings?.LongTimePattern;
				result = "T";
				break;
			case DateTimePatternType.ShortTime:
				patternFromSettings = autoLocalizationSettings?.ShortTimePattern;
				result = "t";
				break;
			}
			bool flag = originalMemoryToken != null;
			if (autoLocalizationSettings != null)
			{
				flag &= !autoLocalizationSettings.FormatDateTimeUniformly;
			}
			if (flag)
			{
				patternFromSettings = null;
			}
			if (patternFromSettings != null && base.TokenizationContext?.DateTimeFormats != null && !base.TokenizationContext.DateTimeFormats.Any((KeyValuePair<DateTimePatternType, List<string>> x) => x.Value.Contains(patternFromSettings)))
			{
				patternFromSettings = null;
			}
			string text = null;
			if (patternFromSettings == null)
			{
				if (originalMemoryToken != null)
				{
					text = originalMemoryToken.FormatString;
					if (text == null)
					{
						if (base.TokenizationContext?.DateTimeFormats != null)
						{
							List<string> list = new List<string>();
							foreach (KeyValuePair<DateTimePatternType, List<string>> dateTimeFormat in base.TokenizationContext.DateTimeFormats)
							{
								list.AddRange(dateTimeFormat.Value);
							}
							text = FindDateTimePattern(originalMemoryToken.Text, list, lm.DateTimeFormat, normalizeWhitespace: false);
							if (text == null)
							{
								text = FindDateTimePattern(originalMemoryToken.Text, list, lm.DateTimeFormat, normalizeWhitespace: true);
							}
						}
						else
						{
							List<string> patterns = GetValidDateTimePatterns(lm).ToList();
							text = FindDateTimePattern(originalMemoryToken.Text, patterns, lm.DateTimeFormat, normalizeWhitespace: false);
							if (text == null)
							{
								text = FindDateTimePattern(originalMemoryToken.Text, patterns, lm.DateTimeFormat, normalizeWhitespace: true);
							}
						}
					}
				}
				if (text == null && base.TokenizationContext?.DateTimeFormats != null && base.TokenizationContext.DateTimeFormats[DateTimePatternType].Count > 0)
				{
					text = base.TokenizationContext.DateTimeFormats[DateTimePatternType][0];
				}
			}
			if (patternFromSettings != null)
			{
				return patternFromSettings;
			}
			if (text != null)
			{
				FormatString = text;
				return text;
			}
			return result;
		}

		public virtual bool Localize(CultureInfo culture, AutoLocalizationSettings autoLocalizationSettings, ILocalizableToken originalMemoryToken, bool adjustCasing)
		{
			string text = null;
			LanguageMetadata orCreateMetadata = LanguageMetadata.GetOrCreateMetadata(culture.Name);
			string patternToUse = GetPatternToUse(autoLocalizationSettings, originalMemoryToken as DateTimeToken, orCreateMetadata);
			try
			{
				text = Value.ToString(patternToUse, orCreateMetadata.DateTimeFormat);
			}
			catch (Exception)
			{
				return false;
			}
			if (originalMemoryToken != null && adjustCasing && text.Length > 0)
			{
				DateTimeToken dateTimeToken = originalMemoryToken as DateTimeToken;
				if (dateTimeToken != null)
				{
					CharacterProperties.Case @case = CharacterProperties.GetCase(text[0]);
					CharacterProperties.Case case2 = CharacterProperties.GetCase(dateTimeToken.Text[0]);
					if (@case != case2 && (@case == CharacterProperties.Case.Upper || @case == CharacterProperties.Case.Lower) && (case2 == CharacterProperties.Case.Upper || case2 == CharacterProperties.Case.Lower))
					{
						string str = (text.Length > 1) ? text.Substring(1) : string.Empty;
						text = CharacterProperties.ToCase(text[0], case2).ToString() + str;
					}
				}
			}
			base.Culture = culture;
			if (string.Equals(text, Text, StringComparison.Ordinal))
			{
				return false;
			}
			Text = text;
			return true;
		}

		protected override TokenType GetTokenType()
		{
			switch (DateTimePatternType)
			{
			case DateTimePatternType.Unknown:
				return TokenType.Unknown;
			case DateTimePatternType.LongDate:
			case DateTimePatternType.ShortDate:
				return TokenType.Date;
			case DateTimePatternType.ShortTime:
			case DateTimePatternType.LongTime:
				return TokenType.Time;
			default:
				return TokenType.Unknown;
			}
		}

		public override Similarity GetSimilarity(SegmentElement other)
		{
			Similarity bundleSimilarity = GetBundleSimilarity(other);
			if (other == null || other.GetType() != GetType())
			{
				return bundleSimilarity;
			}
			DateTimeToken dateTimeToken = other as DateTimeToken;
			if (dateTimeToken == null)
			{
				return Similarity.None;
			}
			if ((!IsDateToken || !dateTimeToken.IsDateToken) && (!IsTimeToken || !dateTimeToken.IsTimeToken))
			{
				return Similarity.None;
			}
			if (!(Math.Abs((Value - dateTimeToken.Value).TotalSeconds) < 10.0))
			{
				return Similarity.IdenticalType;
			}
			return Similarity.IdenticalValueAndType;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return GetSimilarity((DateTimeToken)obj) == Similarity.IdenticalValueAndType;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 31 + Value.GetHashCode();
			return num * 31 + DateTimePatternType.GetHashCode();
		}

		public override void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException();
			}
			visitor.VisitDateTimeToken(this);
		}
	}
}
