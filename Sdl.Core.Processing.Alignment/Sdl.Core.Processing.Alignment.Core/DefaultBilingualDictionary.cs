using Sdl.Core.Processing.Alignment.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class DefaultBilingualDictionary : IBilingualDictionary
	{
		internal const string Signature = "BilingualDictionary";

		internal const char Delimiter = ',';

		private CultureInfo _sourceCulture;

		private CultureInfo _targetCulture;

		private IDictionary<string, IList<string>> _sourceWordTargetWords = new Dictionary<string, IList<string>>();

		public CultureInfo SourceCulture => _sourceCulture;

		public CultureInfo TargetCulture => _targetCulture;

		public bool IsReadOnly => false;

		public DefaultBilingualDictionary(string filePath)
		{
			Load(filePath);
		}

		public DefaultBilingualDictionary(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			if (sourceCulture == null)
			{
				throw new ArgumentNullException("sourceCulture");
			}
			if (targetCulture == null)
			{
				throw new ArgumentNullException("targetCulture");
			}
			_sourceCulture = sourceCulture;
			_targetCulture = targetCulture;
		}

		public IEnumerable<string> GetSourceWords()
		{
			return new List<string>(_sourceWordTargetWords.Keys);
		}

		public IEnumerable<string> GetTargetWords(string sourceWord)
		{
			if (IsValid(sourceWord) && _sourceWordTargetWords.TryGetValue(sourceWord, out IList<string> value))
			{
				return new List<string>(value);
			}
			return new List<string>();
		}

		public bool CanAddEntry(string sourceWord, string targetWord)
		{
			if (!IsReadOnly && IsValid(sourceWord))
			{
				return IsValid(targetWord);
			}
			return false;
		}

		public void AddEntry(string sourceWord, string targetWord)
		{
			CheckWord(sourceWord);
			CheckWord(targetWord);
			if (!_sourceWordTargetWords.TryGetValue(sourceWord, out IList<string> value))
			{
				value = new List<string>();
				_sourceWordTargetWords[sourceWord] = value;
			}
			if (!value.Contains(targetWord))
			{
				value.Add(targetWord);
			}
		}

		private void Load(string filename)
		{
			using (StreamReader reader = new StreamReader(filename))
			{
				LoadHeader(reader);
				LoadEntries(reader);
			}
		}

		private void LoadHeader(StreamReader reader)
		{
			string text = reader.ReadLine();
			string[] array = text.Split(',');
			if (array.Length != 3)
			{
				throw new BilingualDictionaryParseException("The bilingual dictionary file header is invalid; incorrect number of header items.");
			}
			string objA = array[0];
			if (!object.Equals(objA, "BilingualDictionary"))
			{
				throw new BilingualDictionaryParseException("The bilingual dictionary file header is invalid; incorrect signature.");
			}
			try
			{
				_sourceCulture = CultureInfo.GetCultureInfo(array[1]);
			}
			catch (ArgumentException innerException)
			{
				throw new BilingualDictionaryParseException("The bilingual dictionary file header is invalid; invalid source culture.", innerException);
			}
			try
			{
				_targetCulture = CultureInfo.GetCultureInfo(array[2]);
			}
			catch (ArgumentException innerException2)
			{
				throw new BilingualDictionaryParseException("The bilingual dictionary file header is invalid; invalid target culture.", innerException2);
			}
		}

		private void LoadEntries(StreamReader reader)
		{
			string text;
			while (true)
			{
				if ((text = reader.ReadLine()) != null)
				{
					string[] array = text.Split(',');
					if (array.Length < 2)
					{
						break;
					}
					string sourceWord = array[0];
					for (int i = 1; i < array.Length; i++)
					{
						string targetWord = array[i];
						try
						{
							AddEntry(sourceWord, targetWord);
						}
						catch (ArgumentException innerException)
						{
							throw new BilingualDictionaryParseException("The bilingual dictionary file contains an invalid entry; " + text, innerException);
						}
					}
					continue;
				}
				return;
			}
			throw new BilingualDictionaryParseException("The bilingual dictionary file contains an invalid entry; " + text);
		}

		public void Save(string filePath)
		{
			using (StreamWriter writer = new StreamWriter(filePath))
			{
				SaveHeader(writer);
				SaveEntries(writer);
			}
		}

		private void SaveHeader(StreamWriter writer)
		{
			string value = "BilingualDictionary," + SourceCulture.Name + "," + TargetCulture.Name;
			writer.WriteLine(value);
		}

		private void SaveEntries(StreamWriter writer)
		{
			foreach (KeyValuePair<string, IList<string>> sourceWordTargetWord in _sourceWordTargetWords)
			{
				string key = sourceWordTargetWord.Key;
				writer.Write(key);
				IList<string> value = sourceWordTargetWord.Value;
				foreach (string item in value)
				{
					writer.Write(',');
					writer.Write(item);
				}
				writer.WriteLine();
			}
		}

		private void CheckWord(string word)
		{
			if (word == null)
			{
				throw new ArgumentNullException("word");
			}
			if (word.Length == 0)
			{
				throw new ArgumentException("word cannot be empty", "word");
			}
			if (word.Contains(','))
			{
				throw new ArgumentException($"word cannot contain a '{','}'", "word");
			}
		}

		private bool IsValid(string word)
		{
			if (word != null && word.Length > 0)
			{
				return !word.Contains(',');
			}
			return false;
		}
	}
}
