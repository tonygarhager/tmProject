using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Sdl.Core.FineGrainedAlignment
{
	public abstract class VocabularyFile3
	{
		protected bool _Dirty;

		private readonly CompactTrie<byte> _WordKeyTrie;

		protected Dictionary<int, TokenWithCount> _KeyWordMap;

		private readonly SpecialTokenIDs _SpecialTokens;

		private static List<string> _SpecialIDs;

		protected bool _contiguousKeys;

		public int Count => _KeyWordMap.Count;

		public SpecialTokenIDs SpecialTokenIDs => new SpecialTokenIDs(_SpecialTokens);

		public bool Dirty => _Dirty;

		static VocabularyFile3()
		{
			_SpecialIDs = new List<string>(new string[8]
			{
				"{{BOS}}",
				"{{EOS}}",
				"{{DAT}}",
				"{{MSR}}",
				"{{NUM}}",
				"{{TIM}}",
				"{{PCT}}",
				"{{VAR}}"
			});
		}

		public VocabularyFile3()
		{
			_WordKeyTrie = new CompactTrie<byte>();
			_KeyWordMap = new Dictionary<int, TokenWithCount>();
			_SpecialTokens = new SpecialTokenIDs();
		}

		public abstract void Load();

		protected void LookupSpecialTokenIDs()
		{
			_SpecialTokens.BOS = Lookup("{{BOS}}");
			_SpecialTokens.EOS = Lookup("{{EOS}}");
			_SpecialTokens.DAT = Lookup("{{DAT}}");
			_SpecialTokens.MSR = Lookup("{{MSR}}");
			_SpecialTokens.NUM = Lookup("{{NUM}}");
			_SpecialTokens.TIM = Lookup("{{TIM}}");
			_SpecialTokens.PCT = Lookup("{{PCT}}");
			_SpecialTokens.VAR = Lookup("{{VAR}}");
		}

		private void AddSpecialTokenIDs()
		{
			_SpecialTokens.BOS = LookupOrAdd("{{BOS}}");
			_SpecialTokens.EOS = LookupOrAdd("{{EOS}}");
			_SpecialTokens.DAT = LookupOrAdd("{{DAT}}");
			_SpecialTokens.MSR = LookupOrAdd("{{MSR}}");
			_SpecialTokens.NUM = LookupOrAdd("{{NUM}}");
			_SpecialTokens.TIM = LookupOrAdd("{{TIM}}");
			_SpecialTokens.PCT = LookupOrAdd("{{PCT}}");
			_SpecialTokens.VAR = LookupOrAdd("{{VAR}}");
		}

		public bool HasAllSpecialIDs()
		{
			return _SpecialTokens.HasAllSpecialIDs();
		}

		public void Add(int key, string s)
		{
			if (_KeyWordMap.ContainsKey(key))
			{
				throw new Exception("Duplicate vocabulary key");
			}
			_WordKeyTrie.Add(Encoding.UTF8.GetBytes(s), key);
			_KeyWordMap.Add(key, new TokenWithCount
			{
				Token = s
			});
		}

		public void Add(int key, string s, int count)
		{
			if (_KeyWordMap.ContainsKey(key))
			{
				throw new Exception("Duplicate vocabulary key");
			}
			_WordKeyTrie.Add(Encoding.UTF8.GetBytes(s), key);
			_KeyWordMap.Add(key, new TokenWithCount
			{
				Token = s,
				Count = count
			});
		}

		public int Add(string s)
		{
			int num = Lookup(s);
			if (num >= 0)
			{
				return num;
			}
			_Dirty = true;
			num = _KeyWordMap.Count;
			Add(num, s);
			return num;
		}

		public int Lookup(string s)
		{
			if (_WordKeyTrie.Contains(Encoding.UTF8.GetBytes(s), out int key))
			{
				return key;
			}
			return -1;
		}

		public List<int> GetIDs(IEnumerable<string> words)
		{
			List<int> list = null;
			if (words != null)
			{
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
			foreach (KeyValuePair<IList<byte>, int> item in _WordKeyTrie)
			{
				string @string = Encoding.UTF8.GetString(item.Key.ToArray());
				wtr.WriteLine("{0}\t{1}", @string, item.Value);
			}
		}

		public virtual int LookupOrAdd(string s)
		{
			if (_WordKeyTrie.Contains(Encoding.UTF8.GetBytes(s), out int key))
			{
				return key;
			}
			return Add(s);
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
			if (_contiguousKeys)
			{
				if (key >= 0 && key < _KeyWordMap.Count)
				{
					return _KeyWordMap[key].Token;
				}
				return null;
			}
			if (_KeyWordMap.TryGetValue(key, out TokenWithCount value))
			{
				return value.Token;
			}
			return null;
		}

		public TokenWithCount LookupFull(int key)
		{
			if (_KeyWordMap.TryGetValue(key, out TokenWithCount value))
			{
				return value;
			}
			return null;
		}
	}
}
