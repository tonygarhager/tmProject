using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;

namespace Sdl.Core.FineGrainedAlignment
{
	public class BilingualChiSquareComputer3
	{
		public EventHandler<TranslationModelProgressEventArgs> Progress;

		private const int REPORT_PROGRESS_PERIOD = 1000;

		private readonly DataLocation2 _Location;

		private readonly IEnumerable<Pair<IntSegment>> _segmentReaders;

		public BilingualChiSquareComputer3(DataLocation2 location, IEnumerable<Pair<IntSegment>> segmentReaders)
		{
			_segmentReaders = segmentReaders;
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			_Location = location;
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
			using (CooccurrenceCounter2 cooccurrenceCounter = new CooccurrenceCounter2(_Location.Directory.FullName))
			{
				int num = 0;
				int num2 = 0;
				foreach (Pair<IntSegment> segmentReader in _segmentReaders)
				{
					num++;
					num2++;
					if (num2 > 1000)
					{
						num2 = 0;
						OnProgress(TranslationModelProgressStage.Analysing, num);
					}
					IntSegment left = segmentReader.Left;
					IntSegment right = segmentReader.Right;
					left.Uniq();
					right.Uniq();
					for (int i = 0; i < left.Count; i++)
					{
						int key = left[i];
						if (dictionary.ContainsKey(key))
						{
							dictionary[key]++;
						}
						else
						{
							dictionary.Add(key, 1);
						}
					}
					for (int j = 0; j < right.Count; j++)
					{
						int key2 = right[j];
						if (dictionary2.ContainsKey(key2))
						{
							dictionary2[key2]++;
						}
						else
						{
							dictionary2.Add(key2, 1);
						}
					}
					for (int k = 0; k < left.Count; k++)
					{
						int v = left[k];
						for (int l = 0; l < right.Count; l++)
						{
							int v2 = right[l];
							cooccurrenceCounter.Count(v, v2);
						}
					}
				}
				ContingencyTable contingencyTable = null;
				int num3 = 0;
				cooccurrenceCounter.Progress += delegate(object sender, TranslationModelProgressEventArgs args)
				{
					OnProgress(args.ProgressStage, args.ProgressNumber);
				};
				foreach (CooccurrenceCount item in cooccurrenceCounter.Merge(minCooc))
				{
					num3++;
					if (num3 % 1000 == 0)
					{
						OnProgress(TranslationModelProgressStage.Computing, num3);
					}
					int first = item.First;
					int second = item.Second;
					int count = item.Count;
					int num4 = dictionary[first];
					int num5 = dictionary2[second];
					if (count >= minCooc && num4 >= minFreq && num5 >= minFreq)
					{
						if (contingencyTable == null)
						{
							contingencyTable = new ContingencyTable(count, dictionary[first], dictionary2[second], num);
						}
						else
						{
							contingencyTable.Set(count, dictionary[first], dictionary2[second], num);
						}
						double num6 = contingencyTable.ChiSquare();
						if (num6 >= minScore)
						{
							sparseMatrix[first, second] = num6;
						}
					}
				}
				return sparseMatrix;
			}
		}

		private void OnProgress(TranslationModelProgressStage progressStage, int progressNumber)
		{
			if (Progress != null)
			{
				TranslationModelProgressEventArgs e = new TranslationModelProgressEventArgs(progressStage, progressNumber, 0);
				Progress(this, e);
			}
		}
	}
}
