using Snowball;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sdl.Core.LanguageProcessing.Stemmers
{
	public class StemmerFactory
	{
		private class StemmerInfo
		{
			public Func<Stemmer> createFn;

			public int signature;
		}

		private static readonly object _locker = new object();

		private static readonly Dictionary<string, StemmerInfo> _stemmerInfo = new Dictionary<string, StemmerInfo>();

		private static bool _initialized;

		public static Stemmer GetStemmer(string langCode, out int signature)
		{
			lock (_locker)
			{
				signature = 0;
				CheckInitialized();
				if (!_stemmerInfo.TryGetValue(langCode, out StemmerInfo value))
				{
					return null;
				}
				signature = value.signature;
				return value.createFn();
			}
		}

		private static void CheckInitialized()
		{
			if (_initialized)
			{
				return;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type[] types = executingAssembly.GetTypes();
			Type[] array = types;
			foreach (Type type in array)
			{
				if (type.IsSubclassOf(typeof(Stemmer)))
				{
					CallRegister(type);
				}
			}
			_initialized = true;
		}

		private static void CallRegister(Type t)
		{
			MethodInfo method = t.GetMethod("Register", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				method.Invoke(null, null);
			}
		}

		public static void Register(string langCode, Func<Stemmer> createFn, int signature)
		{
			lock (_locker)
			{
				if (_stemmerInfo.ContainsKey(langCode))
				{
					throw new Exception("Stemmer already registered for langCode: " + langCode);
				}
				StemmerInfo value = new StemmerInfo
				{
					createFn = createFn,
					signature = signature
				};
				_stemmerInfo.Add(langCode, value);
			}
		}
	}
}
