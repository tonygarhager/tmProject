using Sdl.Core.LanguageProcessing.Stemmers;
using Sdl.Core.LanguageProcessing.Stemming;
using Sdl.LanguagePlatform.Core;
using Snowball;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class SnowballWrapper : IStemmer
	{
		private readonly Stemmer _stemmer;

		private readonly Dictionary<string, string> _exceptions;

		private readonly CultureInfo _ci;

		private readonly bool _stripDiacritics;

		private readonly object _locker = new object();

		private readonly int _signature;

		private static readonly Dictionary<string, Dictionary<string, string>> Exceptions = new Dictionary<string, Dictionary<string, string>>
		{
			{
				"fr",
				new Dictionary<string, string>
				{
					{
						"la",
						"le"
					},
					{
						"l'",
						"le"
					},
					{
						"du",
						"de"
					},
					{
						"des",
						"de"
					},
					{
						"d'",
						"de"
					},
					{
						"qu'",
						"que"
					},
					{
						"s'",
						"se"
					},
					{
						"n'",
						"ne"
					},
					{
						"aux",
						"à"
					},
					{
						"au",
						"à"
					}
				}
			}
		};

		public string Signature => _signature.ToString();

		private SnowballWrapper(Stemmer stemmer, Dictionary<string, string> exceptions, CultureInfo ci, bool stripDiacritics, int signature)
		{
			_stemmer = stemmer;
			_ci = ci;
			_stripDiacritics = stripDiacritics;
			_exceptions = exceptions;
			_signature = signature;
		}

		public string Stem(string word)
		{
			word = word.ToLower(_ci);
			if (_exceptions != null)
			{
				word = word.Replace("`", "'");
				word = word.Replace("‘", "'");
				word = word.Replace("’", "'");
				if (_exceptions.ContainsKey(word))
				{
					return _exceptions[word];
				}
			}
			string text;
			lock (_locker)
			{
				text = _stemmer.Stem(word);
			}
			if (!_stripDiacritics)
			{
				return text;
			}
			text = CharacterProperties.ToBase(text, skipSurrogates: true);
			return StripPeripheralPunctuation(text);
		}

		public static SnowballWrapper Create(CultureInfo ci, bool stripDiacritics)
		{
			string twoLetterISOLanguageName = ci.TwoLetterISOLanguageName;
			int signature;
			Stemmer stemmer = StemmerFactory.GetStemmer(twoLetterISOLanguageName, out signature);
			if (stemmer == null)
			{
				return null;
			}
			Dictionary<string, string> exceptions = null;
			if (Exceptions.ContainsKey(twoLetterISOLanguageName))
			{
				exceptions = Exceptions[twoLetterISOLanguageName];
			}
			return new SnowballWrapper(stemmer, exceptions, ci, stripDiacritics, signature);
		}

		private static string StripPeripheralPunctuation(string form)
		{
			if (string.IsNullOrEmpty(form))
			{
				return form;
			}
			int i = 0;
			int length;
			for (length = form.Length; i < length && char.IsPunctuation(form, i); i++)
			{
			}
			if (i == length)
			{
				return form;
			}
			int num = length - 1;
			while (num > i && char.IsPunctuation(form, num))
			{
				num--;
			}
			if (i > 0 || num < length - 1)
			{
				return form.Substring(i, num - i + 1);
			}
			return form;
		}
	}
}
