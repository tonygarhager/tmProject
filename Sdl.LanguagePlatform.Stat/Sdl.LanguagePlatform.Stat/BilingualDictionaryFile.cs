using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Stat.WordAlignment;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.Stat
{
	public class BilingualDictionaryFile : IBilingualPhraseCounter
	{
		private List<PhrasePair> _phrasePairs;

		private NGramTree _phrasePairIndex;

		private readonly DataFileInfo _info;

		private const int Version = 1;

		private const int FileStamp = 20061218;

		internal static readonly int Magic = 20061219;

		public PhraseDictionary SourcePhrases
		{
			get;
			internal set;
		}

		public PhraseDictionary TargetPhrases
		{
			get;
			internal set;
		}

		public int Count => _phrasePairs.Count;

		public PhrasePair this[int idx] => _phrasePairs[idx];

		public bool Exists
		{
			get
			{
				if (SourcePhrases.Exists)
				{
					return TargetPhrases.Exists;
				}
				return false;
			}
		}

		public BilingualDictionaryFile(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_info = location.FindComponent(DataFileType.PhraseMappingFile, srcCulture, trgCulture);
			SourcePhrases = new PhraseDictionary(_info.Location, _info.SourceCulture);
			TargetPhrases = new PhraseDictionary(_info.Location, _info.TargetCulture);
			_phrasePairs = new List<PhrasePair>();
			_phrasePairIndex = new NGramTree("neversaved", 2);
		}

		public void CountBilingualPhrase(IList<int> srcPhrase, IList<int> trgPhrase)
		{
			Phrase phrase = SourcePhrases.AddPhrase(srcPhrase);
			phrase.Count++;
			Phrase phrase2 = TargetPhrases.AddPhrase(trgPhrase);
			phrase2.Count++;
			CountBilingualPhrase(phrase.ID, phrase2.ID, 1);
		}

		internal void CountBilingualPhrase(int srcPhraseId, int trgPhraseId, int inc)
		{
			if (inc <= 0)
			{
				inc = 1;
			}
			int num = FindInternal(srcPhraseId, trgPhraseId);
			if (num >= 0)
			{
				_phrasePairs[num].Count += inc;
				return;
			}
			PhrasePair phrasePair = new PhrasePair
			{
				SourcePhraseKey = srcPhraseId,
				TargetPhraseKey = trgPhraseId,
				Count = inc,
				ID = _phrasePairs.Count
			};
			_phrasePairs.Add(phrasePair);
			List<int> keys = new List<int>
			{
				phrasePair.SourcePhraseKey,
				phrasePair.TargetPhraseKey
			};
			_phrasePairIndex.Add(keys, phrasePair.ID);
		}

		public BilingualDictionaryFile FinishCounting()
		{
			return this;
		}

		public void Dump(VocabularyFile srcVocabulary, VocabularyFile trgVocabulary, TextWriter wtr)
		{
			wtr.WriteLine(" =========================== PHRASE DUMP =========================== ");
			foreach (PhrasePair phrasePair in _phrasePairs)
			{
				wtr.WriteLine("=================== Phrase Pair {0} (joint f={1})", phrasePair.ID, phrasePair.Count);
				Phrase phrase = SourcePhrases[phrasePair.SourcePhraseKey];
				Phrase phrase2 = TargetPhrases[phrasePair.TargetPhraseKey];
				phrase.Dump(srcVocabulary, wtr);
				wtr.WriteLine();
				phrase2.Dump(trgVocabulary, wtr);
				wtr.WriteLine();
			}
			wtr.WriteLine(" =================================================================== ");
		}

		public void PurgeAndUpdateIndex()
		{
			int[] array = new int[SourcePhrases.Count];
			int[] array2 = new int[TargetPhrases.Count];
			List<PhrasePair> list = new List<PhrasePair>();
			foreach (PhrasePair phrasePair2 in _phrasePairs)
			{
				if (phrasePair2.Count > 0)
				{
					array[phrasePair2.SourcePhraseKey]++;
					array2[phrasePair2.TargetPhraseKey]++;
					phrasePair2.ID = list.Count;
					list.Add(phrasePair2);
				}
			}
			_phrasePairs.Clear();
			_phrasePairs = list;
			MarkUnusedPhrases(SourcePhrases, array);
			MarkUnusedPhrases(TargetPhrases, array2);
			SourcePhrases.PurgeAndUpdateIndex();
			TargetPhrases.PurgeAndUpdateIndex();
			foreach (PhrasePair phrasePair3 in _phrasePairs)
			{
				phrasePair3.SourcePhraseKey = array[phrasePair3.SourcePhraseKey];
				phrasePair3.TargetPhraseKey = array2[phrasePair3.TargetPhraseKey];
			}
			_phrasePairIndex = new NGramTree("neversaved", 2);
			for (int i = 0; i < _phrasePairs.Count; i++)
			{
				PhrasePair phrasePair = _phrasePairs[i];
				List<int> keys = new List<int>
				{
					phrasePair.SourcePhraseKey,
					phrasePair.TargetPhraseKey
				};
				_phrasePairIndex.Add(keys, i);
			}
		}

		private static void MarkUnusedPhrases(PhraseDictionary phrases, IList<int> phraseCounts)
		{
			int num = 0;
			for (int i = 0; i < phrases.Count; i++)
			{
				if (phraseCounts[i] == 0)
				{
					phrases[i].Count = 0;
					num++;
				}
				else
				{
					phraseCounts[i] = i - num;
				}
			}
		}

		private int FindInternal(int srcKey, int trgKey)
		{
			List<int> sample = new List<int>
			{
				srcKey,
				trgKey
			};
			NGramTree.TreeIterator treeIterator = _phrasePairIndex.Find(sample);
			if (treeIterator == null || !treeIterator.IsValid)
			{
				return -1;
			}
			return treeIterator.UserData;
		}

		public PhrasePair FindPhrasePair(int srcKey, int trgKey)
		{
			int num = FindInternal(srcKey, trgKey);
			if (num < 0)
			{
				return null;
			}
			return _phrasePairs[num];
		}

		public List<PhrasePair> GetSourcePhrasePairs(int srcKey)
		{
			NGramTree.TreeIterator root = _phrasePairIndex.Root;
			if (root == null || !root.IsValid)
			{
				return null;
			}
			root.Navigate(srcKey);
			if (!root.IsValid)
			{
				return null;
			}
			List<PhrasePair> list = new List<PhrasePair>();
			for (int i = 0; i < root.ChildCount; i++)
			{
				NGramTree.TreeIterator treeIterator = root.NavigateToChild(i);
				list.Add(_phrasePairs[treeIterator.UserData]);
			}
			return list;
		}

		public List<PhrasePair> GetTargetPhrasePairs(int trgKey)
		{
			return _phrasePairs.Where((PhrasePair pp) => pp.TargetPhraseKey == trgKey).ToList();
		}

		public IEnumerable<PhrasePair> GetPhrasePairs()
		{
			foreach (PhrasePair phrasePair in _phrasePairs)
			{
				yield return phrasePair;
			}
		}

		public void Save()
		{
			if (!string.IsNullOrEmpty(_info.FileName))
			{
				SourcePhrases.Save();
				TargetPhrases.Save();
				using (Stream output = File.Create(_info.FileName))
				{
					BinaryWriter binaryWriter = new BinaryWriter(output);
					binaryWriter.Write(Magic);
					binaryWriter.Write(_phrasePairs.Count);
					foreach (PhrasePair phrasePair in _phrasePairs)
					{
						binaryWriter.Write(phrasePair.ID);
						binaryWriter.Write(phrasePair.SourcePhraseKey);
						binaryWriter.Write(phrasePair.TargetPhraseKey);
						binaryWriter.Write(phrasePair.Count);
					}
					binaryWriter.Close();
				}
			}
		}

		public void ReversePhraseMappingFile()
		{
			DataFileInfo dataFileInfo = new DataFileInfo(_info.Location, _info.DataType, _info.TargetCulture, _info.SourceCulture);
			using (Stream input = File.OpenRead(_info.FileName))
			{
				using (Stream output = File.Create(dataFileInfo.FileName))
				{
					BinaryReader binaryReader = new BinaryReader(input);
					BinaryWriter binaryWriter = new BinaryWriter(output);
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
					binaryWriter.Write(Magic);
					binaryWriter.Write(num2);
					for (int i = 0; i < num2; i++)
					{
						PhrasePair phrasePair = new PhrasePair
						{
							ID = binaryReader.ReadInt32(),
							SourcePhraseKey = binaryReader.ReadInt32(),
							TargetPhraseKey = binaryReader.ReadInt32(),
							Count = binaryReader.ReadInt32()
						};
						binaryWriter.Write(phrasePair.ID);
						binaryWriter.Write(phrasePair.TargetPhraseKey);
						binaryWriter.Write(phrasePair.SourcePhraseKey);
						binaryWriter.Write(phrasePair.Count);
					}
					binaryReader.Close();
					binaryWriter.Close();
				}
			}
		}

		public void Load()
		{
			if (Exists)
			{
				SourcePhrases.Load();
				TargetPhrases.Load();
				_phrasePairIndex = new NGramTree("never saved", 2);
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
					_phrasePairs.Clear();
					for (int i = 0; i < num2; i++)
					{
						PhrasePair phrasePair = new PhrasePair
						{
							ID = binaryReader.ReadInt32(),
							SourcePhraseKey = binaryReader.ReadInt32(),
							TargetPhraseKey = binaryReader.ReadInt32(),
							Count = binaryReader.ReadInt32()
						};
						_phrasePairs.Add(phrasePair);
						List<int> keys = new List<int>
						{
							phrasePair.SourcePhraseKey,
							phrasePair.TargetPhraseKey
						};
						_phrasePairIndex.Add(keys, i);
					}
					binaryReader.Close();
				}
			}
		}
	}
}
