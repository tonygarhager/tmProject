using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class DataFileInfo
	{
		private readonly DataFileType _dataType;

		public DataLocation Location
		{
			get;
			set;
		}

		public string FileName => Location.GetComponentFileName(_dataType, SourceCulture, TargetCulture);

		public bool Exists => File.Exists(FileName);

		public DataFileType DataType => _dataType;

		public CultureInfo SourceCulture
		{
			get;
		}

		public CultureInfo TargetCulture
		{
			get;
		}

		public DataFileInfo(DataLocation location, DataFileType t, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_dataType = t;
			SourceCulture = srcCulture;
			TargetCulture = trgCulture;
			Location = location;
		}

		public DataFileInfo(string fileName)
			: this(new FileInfo(fileName))
		{
		}

		public DataFileInfo(FileInfo fileInfo)
		{
			_dataType = DataFileType.Unknown;
			SourceCulture = (TargetCulture = null);
			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException(fileInfo.FullName);
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
			string text = null;
			if (!DataLocation.ExtensionToTypeMapping.TryGetValue(fileInfo.Extension.ToLowerInvariant(), out _dataType))
			{
				throw new LanguagePlatformException(ErrorCode.InternalError);
			}
			Location = new DataLocation(fileInfo.Directory);
			string name;
			if (DataLocation.IsBilingualDataType(_dataType))
			{
				int num = fileNameWithoutExtension.IndexOf("_", StringComparison.Ordinal);
				if (num <= 0)
				{
					throw new LanguagePlatformException(ErrorCode.InternalError);
				}
				name = fileNameWithoutExtension.Substring(0, num);
				text = fileNameWithoutExtension.Substring(num + 1);
			}
			else
			{
				name = fileNameWithoutExtension;
			}
			SourceCulture = CultureInfoExtensions.GetCultureInfo(name);
			if (text != null)
			{
				TargetCulture = CultureInfoExtensions.GetCultureInfo(text);
			}
			if (DataLocation.IsBilingualDataType(_dataType) && TargetCulture == null)
			{
				throw new ArgumentException("Bilingual data file type requires target culture to be set");
			}
		}

		public void Dump(TextWriter output)
		{
			if (!Exists)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentMissing);
			}
			switch (_dataType)
			{
			case DataFileType.VocabularyFile:
			{
				VocabularyFile vocabulary9 = Location.GetVocabulary(SourceCulture);
				vocabulary9.Load();
				vocabulary9.Dump(output);
				break;
			}
			case DataFileType.TokenFile:
			{
				TokenFileReader tokenFileReader3 = new TokenFileReader(Location, SourceCulture);
				VocabularyFile vocabulary7 = Location.GetVocabulary(SourceCulture);
				if (!vocabulary7.Exists)
				{
					throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "vocabulary file");
				}
				tokenFileReader3.Open();
				vocabulary7.Load();
				for (int m = 0; m < tokenFileReader3.Segments; m++)
				{
					IntSegment segmentAt3 = tokenFileReader3.GetSegmentAt(m);
					segmentAt3.Dump(vocabulary7, output);
					output.WriteLine();
				}
				tokenFileReader3.Close();
				break;
			}
			case DataFileType.FrequencyCountsFile:
			{
				FrequencyFileReader frequencyFileReader = new FrequencyFileReader(Location, SourceCulture);
				frequencyFileReader.Open();
				VocabularyFile vocabulary5 = Location.GetVocabulary(SourceCulture);
				if (!vocabulary5.Exists)
				{
					throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "vocabulary file");
				}
				vocabulary5.Load();
				for (int k = 0; k < frequencyFileReader.Items; k++)
				{
					output.WriteLine("{0}\t{1}\t{2}", k, vocabulary5.Lookup(k), frequencyFileReader[k]);
				}
				frequencyFileReader.Close();
				break;
			}
			case DataFileType.NGram2CountsFile:
			case DataFileType.NGram3CountsFile:
			{
				VocabularyFile vocabulary6 = Location.GetVocabulary(SourceCulture);
				if (!vocabulary6.Exists)
				{
					throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "vocabulary file");
				}
				vocabulary6.Load();
				using (FileStream input = File.OpenRead(FileName))
				{
					BinaryReader reader = new BinaryReader(input);
					NGramReaderWriter nGramReaderWriter = new NGramReaderWriter();
					NGram item;
					while (nGramReaderWriter.Read(reader, out item))
					{
						for (int l = 0; l < item.Length; l++)
						{
							string text = vocabulary6.Lookup(item.Data[l]);
							output.Write((!string.IsNullOrEmpty(text)) ? text : "<<UNK>>");
							output.Write("\t");
						}
						output.WriteLine("f={0}", item.Count);
					}
				}
				break;
			}
			case DataFileType.PhraseDictionaryFile:
			{
				PhraseDictionary phraseDictionary = new PhraseDictionary(Location, SourceCulture);
				phraseDictionary.Load();
				VocabularyFile vocabulary8 = Location.GetVocabulary(SourceCulture);
				if (!vocabulary8.Exists)
				{
					throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "vocabulary file");
				}
				vocabulary8.Load();
				phraseDictionary.Dump(vocabulary8, output);
				break;
			}
			case DataFileType.WordAlignmentFile:
			{
				WordAlignmentFileReader wordAlignmentFileReader = new WordAlignmentFileReader(Location, SourceCulture, TargetCulture);
				wordAlignmentFileReader.Open();
				TokenFileReader tokenFileReader = new TokenFileReader(Location, SourceCulture);
				tokenFileReader.Open();
				VocabularyFile vocabulary3 = Location.GetVocabulary(SourceCulture);
				if (!vocabulary3.Exists)
				{
					throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "source vocabulary file");
				}
				vocabulary3.Load();
				TokenFileReader tokenFileReader2 = new TokenFileReader(Location, TargetCulture);
				tokenFileReader2.Open();
				VocabularyFile vocabulary4 = Location.GetVocabulary(TargetCulture);
				if (!vocabulary4.Exists)
				{
					throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "target vocabulary file");
				}
				vocabulary4.Load();
				if (tokenFileReader.Segments != tokenFileReader2.Segments)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData);
				}
				for (int j = 0; j < wordAlignmentFileReader.Items; j++)
				{
					AlignmentTable alignmentTable = wordAlignmentFileReader[j];
					IntSegment segmentAt = tokenFileReader.GetSegmentAt(j);
					IntSegment segmentAt2 = tokenFileReader2.GetSegmentAt(j);
					alignmentTable.Dump(segmentAt, segmentAt2, vocabulary3, vocabulary4, output);
				}
				wordAlignmentFileReader.Close();
				tokenFileReader.Close();
				tokenFileReader2.Close();
				break;
			}
			case DataFileType.PhraseMappingFile:
			{
				BilingualDictionaryFile bilingualDictionaryFile = new BilingualDictionaryFile(Location, SourceCulture, TargetCulture);
				bilingualDictionaryFile.Load();
				VocabularyFile vocabulary = Location.GetVocabulary(SourceCulture);
				vocabulary.Load();
				VocabularyFile vocabulary2 = Location.GetVocabulary(TargetCulture);
				vocabulary2.Load();
				int num = 0;
				foreach (PhrasePair phrasePair in bilingualDictionaryFile.GetPhrasePairs())
				{
					num++;
					Phrase phrase = bilingualDictionaryFile.SourcePhrases[phrasePair.SourcePhraseKey];
					Phrase phrase2 = bilingualDictionaryFile.TargetPhrases[phrasePair.TargetPhraseKey];
					output.WriteLine("=============================================== #{0} (f={1})", num - 1, phrasePair.Count);
					phrase.Dump(vocabulary, output);
					phrase2.Dump(vocabulary2, output);
				}
				break;
			}
			case DataFileType.PlainFile:
			{
				using (PlainFileReader plainFileReader = new PlainFileReader(Location, SourceCulture))
				{
					plainFileReader.Open();
					for (int i = 0; i < plainFileReader.Items; i++)
					{
						output.WriteLine(plainFileReader[i]);
					}
					plainFileReader.Close();
				}
				break;
			}
			case DataFileType.MonolingualChiSquareScores:
				DumpMonolingualChi2(Location, SourceCulture, output);
				break;
			case DataFileType.TokenInvertedFile:
			case DataFileType.SimpleTranslationProbabilitiesFile:
				throw new LanguagePlatformException(ErrorCode.NotImplemented);
			case DataFileType.PlainFileIndex:
			case DataFileType.TokenFileIndex:
			case DataFileType.TokenInvertedFileIndex:
			case DataFileType.WordAlignmentFileIndex:
				throw new LanguagePlatformException(ErrorCode.InvalidOperation);
			default:
				throw new LanguagePlatformException(ErrorCode.InvalidOperation);
			}
		}

		private static void DumpMonolingualChi2(DataLocation location, CultureInfo culture, TextWriter output)
		{
			using (FrequencyFileReader frequencyFileReader = new FrequencyFileReader(location, culture))
			{
				frequencyFileReader.Open();
				VocabularyFile vocabulary = location.GetVocabulary(culture);
				if (!vocabulary.Exists)
				{
					throw new LanguagePlatformException(ErrorCode.DataComponentMissing, "vocabulary file");
				}
				vocabulary.Load();
				SparseMatrix<double> data = new SparseMatrix<double>();
				string componentFileName = location.GetComponentFileName(DataFileType.MonolingualChiSquareScores, culture);
				SparseMatrixIO.Load(ref data, componentFileName);
				for (int i = 0; i < data.RowCount; i++)
				{
					int num = data.KeyAt(i);
					SparseArray<double> sparseArray = data.ColumnAt(i);
					for (int j = 0; j < sparseArray.Count; j++)
					{
						int num2 = sparseArray.KeyAt(j);
						double num3 = sparseArray.ValueAt(j);
						string text = vocabulary.Lookup(num);
						string text2 = vocabulary.Lookup(num2);
						int num4 = frequencyFileReader[num];
						int num5 = frequencyFileReader[num2];
						output.WriteLine("{0}\t{1}\tf(s)={2}\tf(t)={3}\tc2={4}", text, text2, num4, num5, num3);
					}
				}
			}
		}
	}
}
