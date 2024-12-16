using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Stat
{
	public abstract class VocabularyFile2
	{
		protected bool _Dirty;

		private readonly CompactTrie<byte> _wordKeyTrie;

		protected Dictionary<int, string> KeyWordMap;

		private readonly SpecialTokenIDs _specialTokens;

		protected bool ContiguousKeys = true;

		public int Count => KeyWordMap.Count;

		public SpecialTokenIDs SpecialTokenIDs => new SpecialTokenIDs(_specialTokens);

		public bool Dirty => _Dirty;

		public VocabularyFile2()
		{
			_wordKeyTrie = new CompactTrie<byte>();
			KeyWordMap = new Dictionary<int, string>();
			_specialTokens = new SpecialTokenIDs();
		}

		public abstract void Load();

		public abstract void Save();

		protected void LookupSpecialTokenIDs()
		{
			_specialTokens.BOS = Lookup("{{BOS}}");
			_specialTokens.EOS = Lookup("{{EOS}}");
			_specialTokens.DAT = Lookup("{{DAT}}");
			_specialTokens.MSR = Lookup("{{MSR}}");
			_specialTokens.NUM = Lookup("{{NUM}}");
			_specialTokens.TIM = Lookup("{{TIM}}");
			_specialTokens.PCT = Lookup("{{PCT}}");
			_specialTokens.VAR = Lookup("{{VAR}}");
		}

		public bool HasAllSpecialIDs()
		{
			return _specialTokens.HasAllSpecialIDs();
		}

		public void Add(int key, string s)
		{
			if (KeyWordMap.ContainsKey(key))
			{
				throw new Exception("Duplicate vocabulary key");
			}
			_wordKeyTrie.Add(Encoding.UTF8.GetBytes(s), key);
			KeyWordMap.Add(key, s);
		}

		public int Add(string s)
		{
			int num = Lookup(s);
			if (num >= 0)
			{
				return num;
			}
			_Dirty = true;
			num = KeyWordMap.Count;
			Add(num, s);
			return num;
		}

		public int Lookup(string s)
		{
			if (_wordKeyTrie.Contains(Encoding.UTF8.GetBytes(s), out int key))
			{
				return key;
			}
			return -1;
		}

		public List<int> GetIDs(IEnumerable<string> words)
		{
			if (words == null)
			{
				return null;
			}
			List<int> list = null;
			foreach (string word in words)
			{
				int num = Lookup(word);
				if (num >= 0)
				{
					if (list == null)
					{
						list = new List<int>();
					}
					int num2 = list.BinarySearch(num);
					if (num2 < 0)
					{
						list.Insert(~num2, num);
					}
				}
			}
			return list;
		}

		public List<int> GetStopwordIDs(CultureInfo culture, IResourceDataAccessor accessor)
		{
			if (accessor == null)
			{
				return null;
			}
			if (accessor.GetResourceStatus(culture, LanguageResourceType.Stopwords, fallback: true) == ResourceStatus.NotAvailable)
			{
				return null;
			}
			byte[] resourceData = accessor.GetResourceData(culture, LanguageResourceType.Stopwords, fallback: true);
			Wordlist wordlist = new Wordlist();
			wordlist.Load(resourceData);
			return GetIDs(wordlist.Items);
		}

		public void Dump(string fileName)
		{
			using (TextWriter wtr = File.CreateText(fileName))
			{
				Dump(wtr);
			}
		}

		public void Dump(TextWriter wtr)
		{
			foreach (KeyValuePair<IList<byte>, int> item in _wordKeyTrie)
			{
				string @string = Encoding.UTF8.GetString(item.Key.ToArray());
				wtr.WriteLine("{0}\t{1}", @string, item.Value);
			}
		}

		public virtual int LookupOrAdd(string s)
		{
			if (!_wordKeyTrie.Contains(Encoding.UTF8.GetBytes(s), out int key))
			{
				return Add(s);
			}
			return key;
		}

		public IntSegment GetIntSegment(List<Token> tokens)
		{
			IntSegment intSegment = new IntSegment();
			foreach (Token token in tokens)
			{
				string tokenString = GetTokenString(token);
				int item = -1;
				if (tokenString != null)
				{
					item = Lookup(tokenString);
				}
				intSegment.Elements.Add(item);
			}
			return intSegment;
		}

		public static string GetTokenString(Token t)
		{
			if (t == null)
			{
				return null;
			}
			switch (t.Type)
			{
			case TokenType.Word:
			case TokenType.Abbreviation:
			case TokenType.CharSequence:
			case TokenType.Acronym:
			case TokenType.Uri:
			case TokenType.OtherTextPlaceable:
				return t.Text;
			case TokenType.Date:
				return "{{DAT}}";
			case TokenType.Measurement:
				return "{{MSR}}";
			case TokenType.Number:
				return "{{NUM}}";
			case TokenType.Time:
				return "{{TIM}}";
			case TokenType.GeneralPunctuation:
			case TokenType.OpeningPunctuation:
			case TokenType.ClosingPunctuation:
				return "{{PCT}}";
			case TokenType.Variable:
				return "{{VAR}}";
			default:
				return null;
			}
		}

		public string Lookup(int key)
		{
			if (!ContiguousKeys)
			{
				if (!KeyWordMap.TryGetValue(key, out string value))
				{
					return null;
				}
				return value;
			}
			if (key < 0 || key >= KeyWordMap.Count)
			{
				return null;
			}
			return KeyWordMap[key];
		}
	}
}
