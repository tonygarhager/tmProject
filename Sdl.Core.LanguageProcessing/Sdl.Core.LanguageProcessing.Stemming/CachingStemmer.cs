using System;
using System.Collections.Generic;

namespace Sdl.Core.LanguageProcessing.Stemming
{
	public class CachingStemmer : IStemmer
	{
		private readonly IStemmer _wrappedStemmer;

		private readonly Dictionary<string, string> _cache;

		private readonly object _cacheLock = new object();

		public string Signature => _wrappedStemmer.Signature;

		public CachingStemmer(IStemmer wrapped)
		{
			_wrappedStemmer = (wrapped ?? throw new ArgumentNullException("wrapped"));
			_cache = new Dictionary<string, string>();
		}

		public string Stem(string word)
		{
			lock (_cacheLock)
			{
				if (_cache.TryGetValue(word, out string value))
				{
					return value;
				}
				value = _wrappedStemmer.Stem(word);
				_cache.Add(word, value);
				return value;
			}
		}
	}
}
