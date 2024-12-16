using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class PhraseDictionary
	{
		internal static int Magic = 20061219;

		private List<Phrase> _data;

		private CompactTrie<byte> _index;

		private readonly DataFileInfo _info;

		public int Count => _data.Count;

		public Phrase this[int idx] => _data[idx];

		public bool Exists => _info.Exists;

		public PhraseDictionary(DataLocation location, CultureInfo culture)
		{
			_info = location.FindComponent(DataFileType.PhraseDictionaryFile, culture);
			_data = new List<Phrase>();
			_index = new CompactTrie<byte>();
		}

		public Phrase AddPhrase(IList<int> keys)
		{
			Phrase phrase = FindPhrase(keys);
			if (phrase != null)
			{
				return phrase;
			}
			int count = _data.Count;
			phrase = new Phrase(keys)
			{
				ID = _data.Count
			};
			_data.Add(phrase);
			AddToIndex(keys, count);
			return phrase;
		}

		private void AddToIndex(IList<int> keys, int id)
		{
			_index.Add(CompressedInt.GetBytes(keys), id);
		}

		private bool FindInIndex(IList<int> phrase, out bool isPrefix, out int key)
		{
			return _index.Find(CompressedInt.GetBytes(phrase), out isPrefix, out key);
		}

		public void PurgeAndUpdateIndex()
		{
			List<Phrase> list = new List<Phrase>();
			_index = new CompactTrie<byte>();
			foreach (Phrase datum in _data)
			{
				if (datum.Count > 0)
				{
					Phrase phrase = datum;
					phrase.ID = list.Count;
					AddToIndex(phrase.Keys, phrase.ID);
					list.Add(phrase);
				}
			}
			_data = list;
		}

		public void ResetCounts()
		{
			if (_data != null)
			{
				foreach (Phrase datum in _data)
				{
					datum.Count = 0;
				}
			}
		}

		public void Dump(VocabularyFile v, TextWriter output)
		{
			if (_data != null)
			{
				foreach (Phrase datum in _data)
				{
					datum.Dump(v, output);
				}
			}
		}

		public List<KeyValuePair<Phrase, int>> FindPhrasesInSegment(IntSegment segment)
		{
			List<KeyValuePair<Phrase, int>> list = new List<KeyValuePair<Phrase, int>>();
			for (int i = 0; i < segment.Count; i++)
			{
				List<int> list2 = new List<int>();
				bool isPrefix = true;
				for (int j = i; j < segment.Count; j++)
				{
					if (!isPrefix)
					{
						break;
					}
					list2.Add(segment[j]);
					if (FindInIndex(list2, out isPrefix, out int key) && key >= 0)
					{
						Phrase key2 = _data[key];
						list.Add(new KeyValuePair<Phrase, int>(key2, i));
					}
				}
			}
			return list;
		}

		public Phrase FindPhrase(IList<int> keys)
		{
			if (!FindInIndex(keys, out bool _, out int key))
			{
				return null;
			}
			return _data[key];
		}

		public void Save()
		{
			if (!string.IsNullOrEmpty(_info.FileName))
			{
				using (Stream output = File.Create(_info.FileName))
				{
					BinaryWriter binaryWriter = new BinaryWriter(output);
					binaryWriter.Write(Magic);
					binaryWriter.Write(_data.Count);
					for (int i = 0; i < _data.Count; i++)
					{
						Phrase phrase = _data[i];
						phrase.Write(binaryWriter);
					}
					binaryWriter.Close();
				}
			}
		}

		public void Load()
		{
			if (!string.IsNullOrEmpty(_info.FileName))
			{
				using (Stream input = File.OpenRead(_info.FileName))
				{
					BinaryReader binaryReader = new BinaryReader(input);
					int num = binaryReader.ReadInt32();
					if (num != Magic)
					{
						throw new LanguagePlatformException(ErrorCode.CorruptData);
					}
					int num2 = binaryReader.ReadInt32();
					if (num2 < 0)
					{
						throw new LanguagePlatformException(ErrorCode.CorruptData);
					}
					_data.Clear();
					_index = new CompactTrie<byte>();
					for (int i = 0; i < num2; i++)
					{
						Phrase phrase = Phrase.Read(binaryReader);
						if (phrase.ID != _data.Count)
						{
							throw new LanguagePlatformException(ErrorCode.CorruptData);
						}
						_data.Add(phrase);
						AddToIndex(phrase.Keys, phrase.ID);
					}
					binaryReader.Close();
				}
			}
		}
	}
}
