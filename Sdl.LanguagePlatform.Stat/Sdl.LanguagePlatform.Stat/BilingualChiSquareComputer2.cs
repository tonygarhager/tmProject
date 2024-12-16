using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class BilingualChiSquareComputer2
	{
		public class Settings
		{
			public Func<DataLocation2, CultureInfo, TokenFileReader2> TokenFileReaderCreator = (DataLocation2 loc, CultureInfo ci) => new TokenFileReader2(loc, ci);
		}

		public EventHandler<ProgressEventArgs> Progress;

		private const int ReportProgressPeriod = 100;

		private readonly DataLocation2 _location;

		private readonly CultureInfo _sourceCulture;

		private readonly CultureInfo _targetCulture;

		private readonly Settings _settings;

		public BilingualChiSquareComputer2(DataLocation2 location, CultureInfo srcCulture, CultureInfo trgCulture, Settings settings = null)
		{
			if (settings == null)
			{
				settings = new Settings();
			}
			_settings = settings;
			_location = (location ?? throw new ArgumentNullException("location"));
			_sourceCulture = (srcCulture ?? throw new ArgumentNullException("srcCulture"));
			_targetCulture = (trgCulture ?? throw new ArgumentNullException("trgCulture"));
		}

		public SparseMatrix<double> Compute()
		{
			return Compute(3.841, 5, 3);
		}

		public SparseMatrix<double> Compute(double minScore, int minFreq, int minCooc)
		{
			return ComputeUsingMergeSort(minScore, minFreq, minCooc);
		}

		private SparseMatrix<double> ComputeUsingMergeSort(double minScore, int minFreq, int minCooc)
		{
			SparseMatrix<double> sparseMatrix = new SparseMatrix<double>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
			using (CooccurrenceCounter cooccurrenceCounter = new CooccurrenceCounter(_location.Directory.FullName))
			{
				int segments;
				using (TokenFileReader2 tokenFileReader = _settings.TokenFileReaderCreator(_location, _sourceCulture))
				{
					using (TokenFileReader2 tokenFileReader2 = _settings.TokenFileReaderCreator(_location, _targetCulture))
					{
						tokenFileReader.Open();
						tokenFileReader2.Open();
						segments = tokenFileReader.Segments;
						if (tokenFileReader.Segments != tokenFileReader2.Segments)
						{
							throw new Exception("Unexpected: difference in segment count");
						}
						for (int i = 0; i < tokenFileReader.Segments; i++)
						{
							if (i % 100 == 0)
							{
								OnProgress(ProgressStage.Probability, i);
							}
							IntSegment segmentAt = tokenFileReader.GetSegmentAt(i);
							IntSegment segmentAt2 = tokenFileReader2.GetSegmentAt(i);
							segmentAt.Uniq();
							segmentAt2.Uniq();
							for (int j = 0; j < segmentAt.Count; j++)
							{
								int key = segmentAt[j];
								if (dictionary.ContainsKey(key))
								{
									dictionary[key]++;
								}
								else
								{
									dictionary.Add(key, 1);
								}
							}
							for (int k = 0; k < segmentAt2.Count; k++)
							{
								int key2 = segmentAt2[k];
								if (dictionary2.ContainsKey(key2))
								{
									dictionary2[key2]++;
								}
								else
								{
									dictionary2.Add(key2, 1);
								}
							}
							for (int l = 0; l < segmentAt.Count; l++)
							{
								int v = segmentAt[l];
								for (int m = 0; m < segmentAt2.Count; m++)
								{
									int v2 = segmentAt2[m];
									cooccurrenceCounter.Count(v, v2);
								}
							}
						}
						tokenFileReader.Close();
						tokenFileReader2.Close();
					}
				}
				ContingencyTable contingencyTable = null;
				foreach (CooccurrenceCount item in cooccurrenceCounter.Merge(minCooc))
				{
					int first = item.First;
					int second = item.Second;
					int count = item.Count;
					int num = dictionary[first];
					int num2 = dictionary2[second];
					if (count >= minCooc && num >= minFreq && num2 >= minFreq)
					{
						if (contingencyTable == null)
						{
							contingencyTable = new ContingencyTable(count, dictionary[first], dictionary2[second], segments);
						}
						else
						{
							contingencyTable.Set(count, dictionary[first], dictionary2[second], segments);
						}
						double num3 = contingencyTable.ChiSquare();
						if (num3 >= minScore)
						{
							sparseMatrix[first, second] = num3;
						}
					}
				}
				return sparseMatrix;
			}
		}

		private void OnProgress(ProgressStage progressStage, int progressNumber)
		{
			if (Progress != null)
			{
				ProgressEventArgs e = new ProgressEventArgs(progressStage, progressNumber);
				Progress(this, e);
			}
		}
	}
}
