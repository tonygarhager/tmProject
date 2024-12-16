using Sdl.LanguagePlatform.Stat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sdl.Core.FineGrainedAlignment
{
	internal class TrainedModelComputer
	{
		private class Counts
		{
			public readonly SparseMatrix<double> t_given_s = new SparseMatrix<double>();

			public readonly SparseArray<double> any_t_given_s = new SparseArray<double>();
		}

		private readonly DataLocation2 _Location;

		private readonly CultureInfo _SourceCulture;

		private readonly CultureInfo _TargetCulture;

		private const int _ReportProgressPeriod = 100;

		private readonly SparseMatrix<double> _translationTable = new SparseMatrix<double>();

		private readonly SparseMatrix<double> _reversedTranslationTable = new SparseMatrix<double>();

		private readonly VocabularyFile3 _srcVocab;

		private readonly VocabularyFile3 _trgVocab;

		public const double MIN_PROB = 1E-12;

		private double _initialProb;

		private double _reversedInitialProb;

		private readonly int _srcNullTokenKey;

		private readonly int _trgNullTokenKey;

		public SparseMatrix<double> TranslationTable => _translationTable;

		public SparseMatrix<double> ReversedTranslationTable => _reversedTranslationTable;

		public event EventHandler<TranslationModelProgressEventArgs> Progress;

		public TrainedModelComputer(DbDataLocation location, CultureInfo srcCulture, CultureInfo trgCulture, VocabularyFile3 srcVocab, VocabularyFile3 trgVocab)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			if (srcCulture == null)
			{
				throw new ArgumentNullException("srcCulture");
			}
			if (trgCulture == null)
			{
				throw new ArgumentNullException("trgCulture");
			}
			_Location = location;
			_SourceCulture = srcCulture;
			_TargetCulture = trgCulture;
			_srcVocab = srcVocab;
			_trgVocab = trgVocab;
			_srcNullTokenKey = srcVocab.Lookup(" ");
			if (_srcNullTokenKey == -1)
			{
				throw new Exception("Missing source null token in vocab");
			}
			_trgNullTokenKey = trgVocab.Lookup(" ");
			if (_trgNullTokenKey == -1)
			{
				throw new Exception("Missing target null token in vocab");
			}
		}

		public void Compute(int iterations)
		{
			_initialProb = 1.0 / (double)_trgVocab.Count;
			_reversedInitialProb = 1.0 / (double)_srcVocab.Count;
			if (_initialProb < 1E-12 || _reversedInitialProb < 1E-12)
			{
				throw new Exception("Vocab too big");
			}
			int totalProgressSteps;
			using (TokenFileReader2 tokenFileReader = new TokenFileReader2(_Location, _SourceCulture))
			{
				tokenFileReader.Open();
				totalProgressSteps = tokenFileReader.Segments * iterations;
			}
			int progress = 0;
			for (int i = 0; i < iterations; i++)
			{
				Train(ref progress, totalProgressSteps);
			}
		}

		private void DumpTranslationTable(bool reversed)
		{
			IList<int> list = reversed ? _reversedTranslationTable.RowKeys : _translationTable.RowKeys;
			foreach (int item in list)
			{
				string value = reversed ? _srcVocab.Lookup(item) : _trgVocab.Lookup(item);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(value);
				stringBuilder.Append(":\t");
				IList<int> list2 = reversed ? _reversedTranslationTable.ColumnKeys(item) : _translationTable.ColumnKeys(item);
				foreach (int item2 in list2)
				{
					value = (reversed ? _trgVocab.Lookup(item2) : _srcVocab.Lookup(item2));
					stringBuilder.Append(value);
					stringBuilder.Append("(");
					stringBuilder.Append(reversed ? _reversedTranslationTable[item, item2] : _translationTable[item, item2]);
					stringBuilder.Append(")\t");
				}
			}
		}

		private double TranslationTableEntry(int target, int source)
		{
			if (_translationTable.HasValue(target, source))
			{
				return _translationTable[target, source];
			}
			return _initialProb;
		}

		private double ReversedTranslationTableEntry(int target, int source)
		{
			if (_reversedTranslationTable.HasValue(target, source))
			{
				return _reversedTranslationTable[target, source];
			}
			return _reversedInitialProb;
		}

		private List<int> SentencePlusNull(bool isTarget, IntSegment sentence)
		{
			List<int> list = new List<int>(sentence.Elements);
			if (isTarget)
			{
				list.Add(_trgNullTokenKey);
			}
			else
			{
				list.Add(_srcNullTokenKey);
			}
			return list;
		}

		private List<int> Sentence(IntSegment sentence)
		{
			return sentence.Elements;
		}

		private void Train(ref int progress, int totalProgressSteps)
		{
			Counts counts = new Counts();
			Counts counts2 = new Counts();
			using (TokenFileReader2 tokenFileReader = new TokenFileReader2(_Location, _SourceCulture))
			{
				using (TokenFileReader2 tokenFileReader2 = new TokenFileReader2(_Location, _TargetCulture))
				{
					tokenFileReader.Open();
					tokenFileReader2.Open();
					if (tokenFileReader.Segments != tokenFileReader2.Segments)
					{
						throw new Exception("Unexpected: difference in segment count");
					}
					for (int i = 0; i < tokenFileReader.Segments; i++)
					{
						IntSegment segmentAt = tokenFileReader.GetSegmentAt(i);
						IntSegment segmentAt2 = tokenFileReader2.GetSegmentAt(i);
						List<int> list = SentencePlusNull(isTarget: false, segmentAt);
						List<int> list2 = Sentence(segmentAt2);
						List<int> list3 = Sentence(segmentAt);
						Dictionary<int, double> dictionary = AlignmentProbForEachUniqueTargetToken(list, list2, isReversed: false);
						foreach (int item in list2)
						{
							foreach (int item2 in list)
							{
								double num = TranslationTableEntry(item, item2);
								double num2 = num / dictionary[item];
								num = 0.0;
								if (counts.t_given_s.HasValue(item, item2))
								{
									num = counts.t_given_s[item, item2];
								}
								num += num2;
								counts.t_given_s[item, item2] = num;
								num = 0.0;
								if (counts.any_t_given_s.HasValue(item2))
								{
									num = counts.any_t_given_s[item2];
								}
								num += num2;
								counts.any_t_given_s[item2] = num;
							}
						}
						List<int> list4 = SentencePlusNull(isTarget: true, segmentAt2);
						Dictionary<int, double> dictionary2 = AlignmentProbForEachUniqueTargetToken(list4, list3, isReversed: true);
						foreach (int item3 in list3)
						{
							foreach (int item4 in list4)
							{
								double num3 = ReversedTranslationTableEntry(item3, item4);
								double num4 = num3 / dictionary2[item3];
								num3 = 0.0;
								if (counts2.t_given_s.HasValue(item3, item4))
								{
									num3 = counts2.t_given_s[item3, item4];
								}
								num3 += num4;
								counts2.t_given_s[item3, item4] = num3;
								num3 = 0.0;
								if (counts2.any_t_given_s.HasValue(item4))
								{
									num3 = counts2.any_t_given_s[item4];
								}
								num3 += num4;
								counts2.any_t_given_s[item4] = num3;
							}
						}
						progress++;
						if (progress % 100 == 0)
						{
							OnProgress(TranslationModelProgressStage.Computing, progress, totalProgressSteps);
						}
					}
				}
			}
			MaximiseLexicalTranslationProbs(counts, isReversed: false);
			MaximiseLexicalTranslationProbs(counts2, isReversed: true);
		}

		private void MaximiseLexicalTranslationProbs(Counts counts, bool isReversed)
		{
			VocabularyFile3 trgVocab = _trgVocab;
			VocabularyFile3 srcVocab = _srcVocab;
			SparseMatrix<double> sparseMatrix = _translationTable;
			if (isReversed)
			{
				trgVocab = _srcVocab;
				srcVocab = _trgVocab;
				sparseMatrix = _reversedTranslationTable;
			}
			foreach (int rowKey in counts.t_given_s.RowKeys)
			{
				IList<int> list = counts.t_given_s.ColumnKeys(rowKey);
				foreach (int item in list)
				{
					double val = counts.t_given_s[rowKey, item] / counts.any_t_given_s[item];
					double num2 = sparseMatrix[rowKey, item] = Math.Max(val, 1E-12);
				}
			}
		}

		private Dictionary<int, double> AlignmentProbForEachUniqueTargetToken(List<int> srcSentence, List<int> trgSentence, bool isReversed)
		{
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			foreach (int item in trgSentence)
			{
				foreach (int item2 in srcSentence)
				{
					double num = isReversed ? ReversedTranslationTableEntry(item, item2) : TranslationTableEntry(item, item2);
					if (!dictionary.ContainsKey(item))
					{
						dictionary.Add(item, 0.0);
					}
					dictionary[item] += num;
				}
			}
			return dictionary;
		}

		private void OnProgress(TranslationModelProgressStage progressStage)
		{
			OnProgress(progressStage, 0);
		}

		private void OnProgress(TranslationModelProgressStage progressStage, int progressNumber)
		{
			TranslationModelProgressEventArgs progressEventArgs = new TranslationModelProgressEventArgs(progressStage, progressNumber, -1);
			OnProgress(progressEventArgs);
		}

		private void OnProgress(TranslationModelProgressStage progressStage, int progressNumber, int progressLimit)
		{
			TranslationModelProgressEventArgs progressEventArgs = new TranslationModelProgressEventArgs(progressStage, progressNumber, progressLimit);
			OnProgress(progressEventArgs);
		}

		private void OnProgress(TranslationModelProgressEventArgs progressEventArgs)
		{
			OnProgress(this, progressEventArgs);
		}

		private void OnProgress(object sender, TranslationModelProgressEventArgs progressEventArgs)
		{
			if (this.Progress != null)
			{
				this.Progress(this, progressEventArgs);
				if (progressEventArgs.Cancel)
				{
					throw new TranslationModelCancelException();
				}
			}
		}
	}
}
