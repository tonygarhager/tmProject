using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class CalendarDateTimePatterns
	{
		public List<DateTimePattern> Patterns
		{
			get;
			set;
		}

		public string CultureName
		{
			get
			{
				return Culture.Name;
			}
			set
			{
				Culture = CultureInfoExtensions.GetCultureInfo(value);
			}
		}

		[XmlIgnore]
		public CultureInfo Culture
		{
			get;
			private set;
		}

		[XmlIgnore]
		public Calendar Calendar
		{
			get;
		}

		public CalendarDateTimePatterns(CultureInfo culture, Calendar cal)
		{
			Culture = culture;
			Calendar = cal;
			Patterns = new List<DateTimePattern>();
		}
	}
}
