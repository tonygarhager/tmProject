using Sdl.Core.LanguageProcessing.Resources;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class MonolingualChiSquareComputer
	{
		public class Settings
		{
			public double MinimumScore;

			public int MinimumFrequency;

			public int MinimumCooccurrenceCount;

			public int WindowSize;

			public bool SkipPunctuation;

			public bool SkipFrequentWords;

			public Settings()
				: this(3.841, 5, 5)
			{
			}

			public Settings(double minScore, int minFreq, int minCooc)
			{
				MinimumScore = minScore;
				MinimumFrequency = minFreq;
				MinimumCooccurrenceCount = minCooc;
				SkipPunctuation = true;
				SkipFrequentWords = true;
				WindowSize = 4;
			}
		}

		private enum Method
		{
			DirectAdjacency,
			Adjacency,
			Cooccurrence
		}

		private readonly DataLocation _location;

		private readonly CultureInfo _culture;

		private readonly string _filename;

		public MonolingualChiSquareComputer(DataLocation location, CultureInfo culture)
		{
			_location = (location ?? throw new ArgumentNullException("location"));
			_culture = (culture ?? throw new ArgumentNullException("culture"));
			DataFileInfo dataFileInfo = new DataFileInfo(location, DataFileType.MonolingualChiSquareScores, culture, null);
			_filename = dataFileInfo.FileName;
		}

		public SparseMatrix<double> Compute()
		{
			return Compute(new Settings());
		}

		public SparseMatrix<double> Compute(Settings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException();
			}
			SparseMatrix<double> sparseMatrix = ComputeUsingMergeSort(settings);
			SparseMatrixIO.Write(sparseMatrix, _filename);
			return sparseMatrix;
		}

		private static bool IsStopword(int id, List<int> stopwordIDs)
		{
			if (id < 0 || stopwordIDs == null)
			{
				return false;
			}
			return stopwordIDs.BinarySearch(id) >= 0;
		}

		private SparseMatrix<double> ComputeUsingMergeSort(Settings settings)
		{
			FrequencyFileReader frequencyFileReader = new FrequencyFileReader(_location, _culture);
			if (!frequencyFileReader.Exists)
			{
				FrequencyFileComputer frequencyFileComputer = new FrequencyFileComputer(_location, _culture);
				frequencyFileComputer.Compute();
			}
			frequencyFileReader.Open();
			SpecialTokenIDs specialTokenIDs = null;
			List<int> list = null;
			if (settings.SkipFrequentWords || settings.SkipPunctuation)
			{
				IResourceDataAccessor accessor = new ResourceFileResourceAccessor();
				VocabularyFile vocabulary = _location.GetVocabulary(_culture);
				vocabulary.Load();
				specialTokenIDs = vocabulary.SpecialTokenIDs;
				list = vocabulary.GetStopwordIDs(_culture, accessor);
			}
			SparseMatrix<double> sparseMatrix = new SparseMatrix<double>();
			using (CooccurrenceCounter cooccurrenceCounter = new CooccurrenceCounter(_location.Directory.FullName, 8388608))
			{
				using (TokenFileReader tokenFileReader = new TokenFileReader(_location, _culture))
				{
					tokenFileReader.Open();
					for (int i = 0; i < tokenFileReader.Segments; i++)
					{
						IntSegment segmentAt = tokenFileReader.GetSegmentAt(i);
						HashSet<Pair<int>> hashSet = new HashSet<Pair<int>>();
						for (int j = 0; j < segmentAt.Count - 1; j++)
						{
							int num = segmentAt[j];
							if ((specialTokenIDs == null || num != specialTokenIDs.PCT) && (list == null || !IsStopword(num, list)))
							{
								for (int k = j + 1; k < segmentAt.Count && (settings.WindowSize <= 0 || k - j <= settings.WindowSize); k++)
								{
									int num2 = segmentAt[k];
									if ((specialTokenIDs == null || num2 != specialTokenIDs.PCT) && (list == null || !IsStopword(num2, list)) && num != num2)
									{
										hashSet.Add(new Pair<int>(num, num2));
									}
								}
							}
						}
						foreach (Pair<int> item in hashSet)
						{
							cooccurrenceCounter.Count(item.Left, item.Right);
						}
					}
					tokenFileReader.Close();
				}
				VocabularyFile vocabulary2 = _location.GetVocabulary(_culture);
				vocabulary2.Load();
				int samples = cooccurrenceCounter.Samples;
				foreach (CooccurrenceCount item2 in cooccurrenceCounter.Merge(settings.MinimumCooccurrenceCount))
				{
					int first = item2.First;
					int second = item2.Second;
					int count = item2.Count;
					int num3 = frequencyFileReader[first];
					int num4 = frequencyFileReader[second];
					if (num3 >= settings.MinimumFrequency && num4 >= settings.MinimumFrequency && count >= settings.MinimumCooccurrenceCount)
					{
						ContingencyTable contingencyTable = new ContingencyTable(count, num3, num4, samples);
						double num5 = contingencyTable.ChiSquare();
						if (num5 >= settings.MinimumScore)
						{
							sparseMatrix[first, second] = num5;
						}
					}
				}
				frequencyFileReader.Close();
				return sparseMatrix;
			}
		}
	}
}
