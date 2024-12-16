using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public class DiskBasedBilingualPhraseCounter2 : IBilingualPhraseCounter2
	{
		private readonly DiskBasedMergeCounter<Sdl.LanguagePlatform.Stat.BilingualPhrase> _bilingualPhrases;

		private readonly DiskBasedMergeCounter<Phrase> _sourcePhrases;

		private readonly DiskBasedMergeCounter<Phrase> _targetPhrases;

		private readonly DataLocation2 _location;

		private readonly CultureInfo _sourceCulture;

		private readonly CultureInfo _targetCulture;

		public DiskBasedBilingualPhraseCounter2(DataLocation2 location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_location = (location ?? throw new ArgumentNullException("location"));
			_sourceCulture = (srcCulture ?? throw new ArgumentNullException("srcCulture"));
			_targetCulture = (trgCulture ?? throw new ArgumentNullException("trgCulture"));
			_sourcePhrases = new DiskBasedMergeCounter<Phrase>(new PhraseReaderWriter(), "sp", location.Directory.FullName, 1572864);
			_targetPhrases = new DiskBasedMergeCounter<Phrase>(new PhraseReaderWriter(), "tp", location.Directory.FullName, 1572864);
			_bilingualPhrases = new DiskBasedMergeCounter<Sdl.LanguagePlatform.Stat.BilingualPhrase>(new BilingualPhraseReaderWriter(), "bp", location.Directory.FullName, 524288);
		}

		public void CountBilingualPhrase(IList<int> srcPhrase, IList<int> trgPhrase)
		{
			Phrase phrase = new Phrase(srcPhrase);
			Phrase phrase2 = new Phrase(trgPhrase);
			Sdl.LanguagePlatform.Stat.BilingualPhrase item = new Sdl.LanguagePlatform.Stat.BilingualPhrase(phrase, phrase2);
			_sourcePhrases.Count(phrase);
			_targetPhrases.Count(phrase2);
			_bilingualPhrases.Count(item);
		}

		public BilingualDictionaryFile2 FinishCounting()
		{
			string ifn = _sourcePhrases.FinishCounting();
			string ifn2 = _targetPhrases.FinishCounting();
			string sortedDataFileName = _bilingualPhrases.FinishCounting();
			GeneratePhraseDictionary(ifn, _sourceCulture, 2);
			GeneratePhraseDictionary(ifn2, _targetCulture, 2);
			return GenerateBilingualDictionary(sortedDataFileName, 2);
		}

		private BilingualDictionaryFile2 GenerateBilingualDictionary(string sortedDataFileName, int minFreq)
		{
			PhraseDictionary2 phraseDictionary = new PhraseDictionary2(_location, _sourceCulture);
			phraseDictionary.Load();
			PhraseDictionary2 phraseDictionary2 = new PhraseDictionary2(_location, _targetCulture);
			phraseDictionary2.Load();
			BilingualDictionaryFile2 bilingualDictionaryFile = new BilingualDictionaryFile2(_location, _sourceCulture, _targetCulture)
			{
				SourcePhrases = phraseDictionary,
				TargetPhrases = phraseDictionary2
			};
			using (FileStream input = File.OpenRead(sortedDataFileName))
			{
				BinaryReader reader = new BinaryReader(input);
				BilingualPhraseReaderWriter bilingualPhraseReaderWriter = new BilingualPhraseReaderWriter();
				Sdl.LanguagePlatform.Stat.BilingualPhrase item;
				while (bilingualPhraseReaderWriter.Read(reader, out item))
				{
					Phrase phrase = phraseDictionary.FindPhrase(item.Source.Keys);
					Phrase phrase2 = phraseDictionary2.FindPhrase(item.Target.Keys);
					if (phrase != null && phrase2 != null && item.Count >= minFreq)
					{
						bilingualDictionaryFile.CountBilingualPhrase(phrase.ID, phrase2.ID, item.Count);
					}
				}
			}
			File.Delete(sortedDataFileName);
			return bilingualDictionaryFile;
		}

		private void GeneratePhraseDictionary(string ifn, CultureInfo culture, int minFreq)
		{
			string componentFileName = _location.GetComponentFileName(DataFileType.PhraseDictionaryFile, culture);
			GeneratePhraseDictionary(ifn, culture, componentFileName, minFreq);
			File.Delete(ifn);
		}

		public void GeneratePhraseDictionary(string sortedDatafileName, CultureInfo culture, string outputFileName, int minFreq)
		{
			PhraseDictionary2 phraseDictionary = new PhraseDictionary2(_location, culture);
			using (FileStream input = File.OpenRead(sortedDatafileName))
			{
				BinaryReader reader = new BinaryReader(input);
				PhraseReaderWriter phraseReaderWriter = new PhraseReaderWriter();
				int num = 0;
				Phrase item;
				while (phraseReaderWriter.Read(reader, out item))
				{
					item.ID = num;
					phraseDictionary.AddPhrase(item.Keys);
					num++;
				}
			}
			using (TokenFileReader2 tokenFileReader = new TokenFileReader2(_location, culture))
			{
				tokenFileReader.Open();
				for (int i = 0; i < tokenFileReader.Segments; i++)
				{
					IntSegment segmentAt = tokenFileReader.GetSegmentAt(i);
					foreach (KeyValuePair<Phrase, int> item2 in phraseDictionary.FindPhrasesInSegment(segmentAt))
					{
						item2.Key.Count++;
					}
				}
				tokenFileReader.Close();
			}
			int num2 = 0;
			for (int j = 0; j < phraseDictionary.Count; j++)
			{
				if (phraseDictionary[j].Count >= minFreq)
				{
					num2++;
				}
			}
			using (FileStream output = File.Create(outputFileName))
			{
				BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(PhraseDictionary.Magic);
				binaryWriter.Write(num2);
				int num3 = 0;
				for (int k = 0; k < phraseDictionary.Count; k++)
				{
					if (phraseDictionary[k].Count >= minFreq)
					{
						phraseDictionary[k].ID = num3;
						phraseDictionary[k].Write(binaryWriter);
						num3++;
					}
				}
			}
		}
	}
}
