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
	public class VocabularyFile : IKeyToStringMapper
	{
		private readonly CompactTrie<byte> _wordKeyTrie;

		private readonly List<string> _keyWordMap;

		private readonly DataFileInfo _info;

		private readonly SpecialTokenIDs _specialTokens;

		public int Count => _keyWordMap.Count;

		public bool Exists => _info.Exists;

		public SpecialTokenIDs SpecialTokenIDs => new SpecialTokenIDs(_specialTokens);

		public bool Dirty
		{
			get;
			private set;
		}

		public bool ReadOnly
		{
			get;
			set;
		}

		public VocabularyFile(DataLocation location, CultureInfo culture)
		{
			_info = location.FindComponent(DataFileType.VocabularyFile, culture);
			_wordKeyTrie = new CompactTrie<byte>();
			_keyWordMap = new List<string>();
			_specialTokens = new SpecialTokenIDs();
		}

		public void Load()
		{
			using (Stream input = File.OpenRead(_info.FileName))
			{
				BinaryReader binaryReader = new BinaryReader(input);
				_wordKeyTrie.Clear();
				_keyWordMap.Clear();
				int num = binaryReader.ReadInt32();
				if (num != 20062232)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData, _info.FileName);
				}
				int num2 = binaryReader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					int count = binaryReader.ReadInt32();
					string @string = Encoding.UTF8.GetString(binaryReader.ReadBytes(count));
					Add(@string);
				}
				binaryReader.Close();
			}
			LookupSpecialTokenIDs();
			bool readOnly = Dirty = false;
			ReadOnly = readOnly;
		}

		public void Save()
		{
			if (Dirty)
			{
				using (Stream output = File.Create(_info.FileName))
				{
					BinaryWriter binaryWriter = new BinaryWriter(output);
					binaryWriter.Write(BitConverter.GetBytes(20062232));
					binaryWriter.Write(_keyWordMap.Count);
					foreach (string item in _keyWordMap)
					{
						byte[] bytes = Encoding.UTF8.GetBytes(item);
						binaryWriter.Write(bytes.Length);
						binaryWriter.Write(bytes);
					}
					binaryWriter.Close();
				}
				Dirty = false;
			}
		}

		private void LookupSpecialTokenIDs()
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

		public int Add(string s)
		{
			int num = Lookup(s);
			if (num >= 0)
			{
				return num;
			}
			if (ReadOnly)
			{
				throw new LanguagePlatformException(ErrorCode.ReadonlyResource, _info.FileName);
			}
			Dirty = true;
			num = _keyWordMap.Count;
			_wordKeyTrie.Add(Encoding.UTF8.GetBytes(s), num);
			_keyWordMap.Add(s);
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

		public int LookupOrAdd(string s)
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
			if (key >= 0 && key < _keyWordMap.Count)
			{
				return _keyWordMap[key];
			}
			return null;
		}
	}
}
