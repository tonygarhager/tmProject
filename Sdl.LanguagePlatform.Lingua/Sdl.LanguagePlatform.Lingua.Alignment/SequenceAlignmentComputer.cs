using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class SequenceAlignmentComputer<T>
	{
		private enum Operation
		{
			Align,
			Noise,
			Skip
		}

		private struct Cell
		{
			public int Score;

			public int BackI;

			public int BackJ;

			public Operation Op;

			public int UlMaxScore;

			public override string ToString()
			{
				return $"{Score}->{BackI}/{BackJ}";
			}
		}

		private readonly IList<T> _source;

		private readonly IList<T> _target;

		private readonly ISequenceAlignmentItemScoreProvider<T> _scorer;

		private readonly IExtensionDisambiguator _picker;

		private readonly int _minLength;

		private readonly int _maxItems;

		private int[,] _alignmentScores;

		private Cell[,] _table;

		private int[] _sourceSkipScores;

		private int[] _targetSkipScores;

		public static List<AlignedSubstring> ComputeCoverage(IList<T> source, IList<T> target, ISequenceAlignmentItemScoreProvider<T> scorer, IExtensionDisambiguator picker)
		{
			return ComputeCoverage(source, target, 1, scorer, picker, 0);
		}

		public static List<AlignedSubstring> ComputeLongestCommonSubsequence(IList<T> source, IList<T> target, int minLength, ISequenceAlignmentItemScoreProvider<T> scorer, IExtensionDisambiguator picker)
		{
			return ComputeCoverage(source, target, minLength, scorer, picker, 1);
		}

		private static int[,] ComputeScores(IList<T> source, IList<T> target, ISequenceAlignmentItemScoreProvider<T> scorer)
		{
			int[,] array = new int[source.Count, target.Count];
			for (int i = 0; i < source.Count; i++)
			{
				for (int j = 0; j < target.Count; j++)
				{
					T a = source[i];
					T b = target[j];
					array[i, j] = scorer.GetAlignScore(a, b);
				}
			}
			return array;
		}

		public static List<AlignedSubstring> ComputeCoverage(IList<T> source, IList<T> target, int minLength, ISequenceAlignmentItemScoreProvider<T> scorer, IExtensionDisambiguator picker, int maxItems)
		{
			SequenceAlignmentComputer<T> sequenceAlignmentComputer = new SequenceAlignmentComputer<T>(source, target, scorer, picker, minLength, maxItems);
			return sequenceAlignmentComputer.Compute();
		}

		public SequenceAlignmentComputer(IList<T> source, IList<T> target, ISequenceAlignmentItemScoreProvider<T> scorer, IExtensionDisambiguator picker, int minLength, int maxItems)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (scorer == null)
			{
				throw new ArgumentNullException("scorer");
			}
			if (minLength <= 0)
			{
				minLength = 1;
			}
			if (maxItems < 0)
			{
				maxItems = 0;
			}
			_minLength = minLength;
			_maxItems = maxItems;
			_source = source;
			_target = target;
			_scorer = scorer;
			_picker = picker;
		}

		public List<AlignedSubstring> Compute()
		{
			return Compute(_source.Count, _target.Count);
		}

		private void ComputeSkipScoreCaches()
		{
			_sourceSkipScores = new int[_source.Count];
			for (int i = 0; i < _source.Count; i++)
			{
				_sourceSkipScores[i] = _scorer.GetSourceSkipScore(_source[i]);
			}
			_targetSkipScores = new int[_target.Count];
			for (int j = 0; j < _target.Count; j++)
			{
				_targetSkipScores[j] = _scorer.GetTargetSkipScore(_target[j]);
			}
		}

		private void ComputeFullTable(bool maySkip)
		{
			for (int i = 1; i <= _source.Count; i++)
			{
				for (int j = 1; j <= _target.Count; j++)
				{
					_table[i, j].Score = 0;
					int num = _alignmentScores[i - 1, j - 1];
					int num2 = _table[i - 1, j - 1].Score + num;
					if (num2 > _table[i, j].Score)
					{
						_table[i, j].Score = num2;
						_table[i, j].BackI = i - 1;
						_table[i, j].BackJ = j - 1;
						_table[i, j].Op = ((num <= 0) ? Operation.Noise : Operation.Align);
					}
					if (maySkip)
					{
						int num3 = _table[i - 1, j].Score + _sourceSkipScores[i - 1];
						if (num3 > _table[i, j].Score)
						{
							_table[i, j].Score = num3;
							_table[i, j].BackI = i - 1;
							_table[i, j].BackJ = j;
							_table[i, j].Op = Operation.Skip;
						}
						int num4 = _table[i, j - 1].Score + _targetSkipScores[j - 1];
						if (num4 > _table[i, j].Score)
						{
							_table[i, j].Score = num4;
							_table[i, j].BackI = i;
							_table[i, j].BackJ = j - 1;
							_table[i, j].Op = Operation.Skip;
						}
					}
					int score = _table[i, j].Score;
					int num5 = score;
					if (_table[i - 1, j - 1].UlMaxScore > num5)
					{
						num5 = _table[i - 1, j - 1].UlMaxScore;
					}
					if (_table[i - 1, j].UlMaxScore > num5)
					{
						num5 = _table[i - 1, j].UlMaxScore;
					}
					if (_table[i, j - 1].UlMaxScore > num5)
					{
						num5 = _table[i, j - 1].UlMaxScore;
					}
					_table[i, j].UlMaxScore = num5;
				}
			}
		}

		private void ComputeMaximaForCoverage(ICollection<Pair<int>> maxima, out int globalMax, int uptoSource, int uptoTarget, bool maySkip, bool[,] blocked)
		{
			maxima.Clear();
			globalMax = 0;
			for (int i = 1; i <= uptoSource; i++)
			{
				for (int j = 1; j <= uptoTarget; j++)
				{
					_table[i, j].Score = 0;
					if (blocked != null && blocked[i, j])
					{
						continue;
					}
					int num = _alignmentScores[i - 1, j - 1];
					int num2 = _table[i - 1, j - 1].Score + num;
					if (num2 > _table[i, j].Score)
					{
						_table[i, j].Score = num2;
						_table[i, j].BackI = i - 1;
						_table[i, j].BackJ = j - 1;
						_table[i, j].Op = ((num <= 0) ? Operation.Noise : Operation.Align);
					}
					if (maySkip)
					{
						int num3 = _table[i - 1, j].Score + _sourceSkipScores[i - 1];
						if (num3 > _table[i, j].Score)
						{
							_table[i, j].Score = num3;
							_table[i, j].BackI = i - 1;
							_table[i, j].BackJ = j;
							_table[i, j].Op = Operation.Skip;
						}
						int num4 = _table[i, j - 1].Score + _targetSkipScores[j - 1];
						if (num4 > _table[i, j].Score)
						{
							_table[i, j].Score = num4;
							_table[i, j].BackI = i;
							_table[i, j].BackJ = j - 1;
							_table[i, j].Op = Operation.Skip;
						}
					}
					if (_table[i, j].Score > globalMax)
					{
						maxima.Clear();
						globalMax = _table[i, j].Score;
						maxima.Add(new Pair<int>(i, j));
					}
					else if (_table[i, j].Score > 0 && _table[i, j].Score == globalMax)
					{
						maxima.Add(new Pair<int>(i, j));
					}
				}
			}
		}

		private void ComputeMaximaForLcs(ICollection<Pair<int>> maxima, out int globalMax, int uptoSource, int uptoTarget)
		{
			maxima.Clear();
			globalMax = _table[uptoSource, uptoTarget].UlMaxScore;
			for (int num = uptoSource; num > 0; num--)
			{
				if (_table[num, uptoTarget].UlMaxScore >= globalMax)
				{
					for (int num2 = uptoTarget; num2 > 0; num2--)
					{
						if (_table[num, num2].Score == globalMax)
						{
							maxima.Add(new Pair<int>(num, num2));
							return;
						}
					}
				}
			}
		}

		public List<AlignedSubstring> Compute(int uptoSource, int uptoTarget)
		{
			if (uptoSource <= 0 || uptoSource > _source.Count)
			{
				throw new ArgumentOutOfRangeException("uptoSource");
			}
			if (uptoTarget <= 0 || uptoTarget > _target.Count)
			{
				throw new ArgumentOutOfRangeException("uptoTarget");
			}
			List<AlignedSubstring> list = new List<AlignedSubstring>();
			List<Pair<int>> list2 = new List<Pair<int>>();
			if (_alignmentScores == null)
			{
				_alignmentScores = ComputeScores(_source, _target, _scorer);
			}
			bool maySkip = _scorer.MaySkip;
			if (maySkip && (_sourceSkipScores == null || _targetSkipScores == null))
			{
				ComputeSkipScoreCaches();
			}
			bool flag = _maxItems != 1;
			if (_table == null)
			{
				_table = new Cell[_source.Count + 1, _target.Count + 1];
				if (!flag)
				{
					ComputeFullTable(maySkip);
				}
			}
			bool[,] array = null;
			if (flag)
			{
				array = new bool[_source.Count + 1, _target.Count + 1];
			}
			do
			{
				int globalMax;
				if (flag)
				{
					ComputeMaximaForCoverage(list2, out globalMax, uptoSource, uptoTarget, maySkip, array);
				}
				else
				{
					ComputeMaximaForLcs(list2, out globalMax, uptoSource, uptoTarget);
				}
				if (list2.Count <= 0)
				{
					continue;
				}
				List<AlignedSubstring> list3 = new List<AlignedSubstring>();
				foreach (Pair<int> item2 in list2)
				{
					int num = item2.Left;
					int num2 = item2.Right;
					int num3 = 0;
					while (_table[num, num2].Score > 0)
					{
						Cell cell = _table[num, num2];
						if (cell.Op == Operation.Align)
						{
							num3++;
						}
						num = cell.BackI;
						num2 = cell.BackJ;
					}
					AlignedSubstring item = new AlignedSubstring(num, item2.Left - num, num2, item2.Right - num2, globalMax, num3);
					if (num3 >= _minLength)
					{
						list3.Add(item);
					}
				}
				if (list3.Count == 0)
				{
					list2.Clear();
					continue;
				}
				AlignedSubstring alignedSubstring = (_picker == null) ? list3[0] : _picker.PickExtension(list, list3);
				if (alignedSubstring == null)
				{
					break;
				}
				if (array != null)
				{
					for (int i = 1; i <= uptoSource; i++)
					{
						for (int j = 1; j <= uptoTarget; j++)
						{
							if ((i > alignedSubstring.Source.Start && i <= alignedSubstring.Source.Start + alignedSubstring.Source.Length) || (j > alignedSubstring.Target.Start && j <= alignedSubstring.Target.Start + alignedSubstring.Target.Length))
							{
								array[i, j] = true;
							}
						}
					}
				}
				list.Add(alignedSubstring);
			}
			while (list2.Count > 0 && (_maxItems == 0 || list.Count < _maxItems));
			return list;
		}
	}
}
