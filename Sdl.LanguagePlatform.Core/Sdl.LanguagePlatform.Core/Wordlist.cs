using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.LanguagePlatform.Core
{
	public class Wordlist : ICloneable
	{
		private List<string> _words;

		private HashSet<string> _wordIndex;

		public int Version
		{
			get;
			private set;
		}

		public SearchOption Flags
		{
			get;
			private set;
		}

		public IEnumerable<string> Items => _words;

		public int Count
		{
			get
			{
				List<string> words = _words;
				if (words == null)
				{
					return 0;
				}
				return words.Count;
			}
		}

		public Wordlist()
			: this(SearchOption.None)
		{
		}

		public Wordlist(SearchOption flags)
			: this(null)
		{
			Flags = flags;
			Init();
		}

		public Wordlist(Wordlist other)
		{
			Flags = (other?.Flags ?? SearchOption.None);
			Init();
			if (other != null)
			{
				foreach (string item in other.Items)
				{
					Add(item);
				}
			}
		}

		private void Init()
		{
			_words = new List<string>();
			_wordIndex = (((Flags & SearchOption.CaseInsensitive) == 0) ? new HashSet<string>() : new HashSet<string>(StringComparer.OrdinalIgnoreCase));
		}

		public bool Contains(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				return _wordIndex.Contains(s);
			}
			return false;
		}

		public void Clear()
		{
			_words.Clear();
			_wordIndex.Clear();
		}

		public void Merge(Wordlist other)
		{
			if (other != null && this != other)
			{
				foreach (string item in other.Items)
				{
					Add(item);
				}
			}
		}

		public static Wordlist Merge(IEnumerable<Wordlist> wordLists)
		{
			Wordlist wordlist = null;
			foreach (Wordlist wordList in wordLists)
			{
				if (wordList != null)
				{
					if (wordlist == null)
					{
						wordlist = new Wordlist(wordList);
					}
					else
					{
						wordlist.Merge(wordList);
					}
				}
			}
			return wordlist;
		}

		public string GetRegularExpression(out CharacterSet first)
		{
			first = new CharacterSet();
			bool flag = (Flags & SearchOption.CaseInsensitive) != 0;
			_words.Sort((string a, string b) => b.Length - a.Length);
			StringBuilder stringBuilder = new StringBuilder("(");
			if (flag)
			{
				stringBuilder.Append("?i-:");
			}
			bool flag2 = true;
			foreach (string word in _words)
			{
				if (word != null && word.Length > 0)
				{
					if (flag2)
					{
						flag2 = false;
					}
					else
					{
						stringBuilder.Append("|");
					}
					char c = word[0];
					first.Add(c);
					if (flag)
					{
						char c2 = char.ToUpper(c);
						char c3 = char.ToLower(c);
						if (c2 != c)
						{
							first.Add(c2);
						}
						if (c3 != c && c3 != c2)
						{
							first.Add(c3);
						}
					}
					stringBuilder.Append(Regex.Escape(word));
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public bool Add(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			if (Contains(s))
			{
				return false;
			}
			_words.Add(s);
			_wordIndex.Add(s);
			return true;
		}

		public bool Remove(string s)
		{
			if (!Contains(s))
			{
				return false;
			}
			_wordIndex.Remove(s);
			_words.Remove(s);
			return true;
		}

		public void Save(TextWriter writer)
		{
			if ((Flags & SearchOption.CaseInsensitive) != 0)
			{
				writer.WriteLine("%AddCaseVariants");
			}
			_words.Sort(StringComparer.OrdinalIgnoreCase);
			foreach (string item in _words.ToList())
			{
				writer.WriteLine(item);
			}
		}

		public static void CleanupList(string inputFileName, string outputFileName, bool ignoreComments = true)
		{
			Wordlist wordlist = new Wordlist();
			using (TextReader reader = new StreamReader(inputFileName, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
			{
				wordlist.LoadInternal(reader, ignoreComments);
			}
			using (TextWriter writer = new StreamWriter(outputFileName, append: false, Encoding.UTF8))
			{
				wordlist.Save(writer);
			}
		}

		private void LoadInternal(TextReader reader, bool ignoreComments)
		{
			Dictionary<char, char> dictionary = null;
			bool flag = true;
			string text;
			while ((text = reader.ReadLine()) != null)
			{
				text = text.Trim();
				if (flag && text.StartsWith("%version="))
				{
					Version = int.Parse(text.Substring(9));
				}
				flag = false;
				if (text.StartsWith("%"))
				{
					if (text.Length <= 1)
					{
						continue;
					}
					string text2 = text.Substring(1).Trim();
					if (text2.Equals("addcasevariants", StringComparison.OrdinalIgnoreCase))
					{
						Flags |= SearchOption.CaseInsensitive;
					}
					else if (text2.Length == 2)
					{
						if (dictionary == null)
						{
							dictionary = new Dictionary<char, char>();
						}
						dictionary.Add(text2[0], text2[1]);
					}
				}
				else if (ignoreComments && !text.StartsWith("#") && text.Length > 0)
				{
					int num = text.IndexOf('#');
					if (num >= 0)
					{
						text = text.Substring(0, num).TrimEnd();
						if (text.Length == 0)
						{
							continue;
						}
					}
					Add(text);
					if (dictionary != null)
					{
						HashSet<string> hashSet = new HashSet<string>
						{
							text
						};
						List<string> list = new List<string>();
						foreach (KeyValuePair<char, char> item in dictionary)
						{
							foreach (string item2 in hashSet)
							{
								string text3 = item2.Replace(item.Key, item.Value);
								if (!text3.Equals(item2, StringComparison.Ordinal))
								{
									list.Add(text3);
								}
							}
							foreach (string item3 in list)
							{
								hashSet.Add(item3);
							}
							list.Clear();
						}
						if (hashSet.Count > 1)
						{
							foreach (string item4 in hashSet)
							{
								Add(item4);
							}
						}
					}
				}
				else if (!ignoreComments)
				{
					Add(text);
				}
			}
		}

		public void Load(string filename, bool ignoreComments = true)
		{
			using (StreamReader reader = new StreamReader(filename, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
			{
				LoadInternal(reader, ignoreComments);
			}
		}

		public void Load(Stream stream, bool ignoreComments = true)
		{
			using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
			{
				LoadInternal(reader, ignoreComments);
			}
		}

		public void Load(byte[] data, bool ignoreComments = true)
		{
			using (MemoryStream stream = new MemoryStream(data))
			{
				Load(stream, ignoreComments);
			}
		}

		public byte[] GetBytes()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (TextWriter writer = new StreamWriter(memoryStream, Encoding.UTF8))
				{
					Save(writer);
				}
				return memoryStream.ToArray();
			}
		}

		public object Clone()
		{
			return new Wordlist(this);
		}
	}
}
