using Icu;
using System;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.ICU2
{
	public class WordBoundaryFinder
	{
		private static readonly Locale JaLocale;

		private static readonly BreakIterator JaIterator;

		private static readonly Dictionary<string, Locale> Locales;

		private static readonly Dictionary<string, BreakIterator> Iterators;

		public static string IcuVersion => Wrapper.IcuVersion;

		static WordBoundaryFinder()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Locales = new Dictionary<string, Locale>();
			Iterators = new Dictionary<string, BreakIterator>();
			Wrapper.Init();
			JaLocale = Locale.AvailableLocales.Find((Locale x) => x.BaseName == "ja");
			if (JaLocale == null)
			{
				throw new Exception("Unable to find ICU locale 'ja'");
			}
			JaIterator = BreakIterator.CreateWordInstance(JaLocale);
		}

		private Locale CheckLocaleResult(Locale locale, string culture)
		{
			if (locale == null)
			{
				throw new Exception("ICU locale not found for culture: " + culture);
			}
			Locales.Add(culture, locale);
			return locale;
		}

		private Locale GetLocale(string culture)
		{
			lock (Locales)
			{
				if (Locales.TryGetValue(culture, out Locale value))
				{
					return value;
				}
				if (culture.Length == 2)
				{
					value = Locale.AvailableLocales.Find((Locale x) => x.Language == culture);
					return CheckLocaleResult(value, culture);
				}
				if (culture.Length < 4 || culture[2] != '-')
				{
					throw new Exception("Unexpected culture string format: " + culture);
				}
				string region = culture.Substring(3);
				string lang = culture.Substring(0, 2);
				value = Locale.AvailableLocales.Find((Locale x) => x.Language == lang && x.Country == region);
				if (value == null)
				{
					value = Locale.AvailableLocales.Find((Locale x) => x.Language == lang);
				}
				return CheckLocaleResult(value, culture);
			}
		}

		private BreakIterator GetBreakIterator(string culture)
		{
			lock (Iterators)
			{
				if (Iterators.TryGetValue(culture, out BreakIterator value))
				{
					return value;
				}
				Locale locale = GetLocale(culture);
				value = BreakIterator.CreateWordInstance(locale);
				Iterators.Add(culture, value);
				return value;
			}
		}

		public List<int> FindZhBoundaries(string s, string culture)
		{
			BreakIterator breakIterator = GetBreakIterator(culture);
			return FindBoundaries(s, breakIterator);
		}

		public List<int> FindJaBoundaries(string s)
		{
			return FindBoundaries(s, JaIterator);
		}

		public List<int> FindBoundaries(string s, BreakIterator bi)
		{
			List<int> list;
			lock (bi)
			{
				bi.SetText(s);
				list = new List<int>(bi.Boundaries);
			}
			list.RemoveAt(0);
			return list;
		}
	}
}
