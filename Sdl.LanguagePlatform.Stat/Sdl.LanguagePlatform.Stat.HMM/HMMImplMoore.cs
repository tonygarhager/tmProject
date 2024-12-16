using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.Stat.HMM
{
	internal class HMMImplMoore : HMM
	{
		private readonly double[] _p;

		private readonly double[][] _a;

		private readonly double[][] _b;

		public HMMImplMoore(int states, int vocabulary)
			: this(HMMType.Ergodic, states, vocabulary)
		{
		}

		public HMMImplMoore(HMMType t, int states, int vocabulary)
			: base(t, states, vocabulary)
		{
			if (t == HMMType.StrictLeftToRight)
			{
				throw new NotImplementedException();
			}
			_p = new double[States];
			_a = new double[States][];
			_b = new double[States][];
			for (int i = 0; i < States; i++)
			{
				_a[i] = new double[States];
				_b[i] = new double[Vocabulary];
			}
		}

		public override void InitializeTraining(bool uniformDistribution)
		{
			_p.Initialize();
			_p[0] = 1.0;
			for (int i = 0; i < States; i++)
			{
				int k;
				switch (Type)
				{
				case HMMType.LeftToRight:
					k = i;
					break;
				case HMMType.StrictLeftToRight:
					k = i + 1;
					break;
				default:
					k = 0;
					break;
				}
				if (uniformDistribution)
				{
					InitializeDistributionUniform(_a[i], k);
					InitializeDistributionUniform(_b[i]);
				}
				else
				{
					InitializeDistributionRandom(_a[i], k);
					InitializeDistributionRandom(_b[i]);
				}
			}
		}

		public override void SingleTrainingIteration(IList<IList<int>> observations)
		{
			if (!Verify())
			{
				throw new InvalidOperationException("The model is inconsistent and cannot be trained.");
			}
			HMMImplMoore hMMImplMoore = new HMMImplMoore(States, Vocabulary);
			HMMImplMoore hMMImplMoore2 = new HMMImplMoore(States, Vocabulary);
			foreach (IList<int> observation in observations)
			{
				double[] scaleFactors;
				double[][] array = ComputeAlpha(observation, out scaleFactors);
				double[][] array2 = ComputeBeta(observation, scaleFactors);
				double[][][] array3 = new double[observation.Count][][];
				for (int i = 0; i < observation.Count; i++)
				{
					array3[i] = new double[States][];
					for (int j = 0; j < States; j++)
					{
						array3[i][j] = new double[States];
					}
				}
				for (int k = 0; k < observation.Count - 1; k++)
				{
					array3[k] = new double[States][];
					for (int l = 0; l < States; l++)
					{
						array3[k][l] = new double[States];
					}
					double num = 0.0;
					for (int m = 0; m < States; m++)
					{
						for (int n = 0; n < States; n++)
						{
							double num2 = array[k][m] * _a[m][n] * _b[n][observation[k + 1]] * array2[k + 1][n];
							array3[k][m][n] = num2;
							num += num2;
						}
					}
					for (int num3 = 0; num3 < States; num3++)
					{
						for (int num4 = 0; num4 < States; num4++)
						{
							array3[k][num3][num4] /= num;
						}
					}
				}
				for (int num5 = 0; num5 < States; num5++)
				{
					double num6 = 0.0;
					for (int num7 = 0; num7 < States; num7++)
					{
						double expectedTransitionCount = GetExpectedTransitionCount(array3, num5, num7, observation.Count);
						hMMImplMoore._a[num5][num7] += expectedTransitionCount / 1.0;
						num6 += expectedTransitionCount;
						double num8 = 0.0;
						HashSet<int> hashSet = new HashSet<int>();
						foreach (int item in observation)
						{
							if (!hashSet.Contains(item))
							{
								hashSet.Add(item);
								double expectedTransitionCountWithSymbol = GetExpectedTransitionCountWithSymbol(array3, num5, num7, item, observation);
								num8 += expectedTransitionCountWithSymbol;
								hMMImplMoore._b[num7][item] += expectedTransitionCountWithSymbol / 1.0;
							}
						}
						for (int num9 = 0; num9 < Vocabulary; num9++)
						{
							hMMImplMoore2._b[num7][num9] += expectedTransitionCount / 1.0;
						}
						Math.Abs(num8 - expectedTransitionCount);
						_ = HMM.ZeroDelta;
					}
					for (int num10 = 0; num10 < States; num10++)
					{
						hMMImplMoore2._a[num5][num10] += num6 / 1.0;
					}
				}
			}
			for (int num11 = 0; num11 < States; num11++)
			{
				for (int num12 = 0; num12 < States; num12++)
				{
					if (hMMImplMoore._a[num11][num12] == 0.0)
					{
						_a[num11][num12] = 0.0;
					}
					else
					{
						_a[num11][num12] = hMMImplMoore._a[num11][num12] / hMMImplMoore2._a[num11][num12];
					}
				}
				for (int num13 = 0; num13 < Vocabulary; num13++)
				{
					if (hMMImplMoore._b[num11][num13] == 0.0)
					{
						_b[num11][num13] = 0.0;
					}
					else
					{
						_b[num11][num13] = hMMImplMoore._b[num11][num13] / hMMImplMoore2._b[num11][num13];
					}
				}
			}
			if (!Verify())
			{
				Normalize();
			}
		}

		public override void Save(Stream output)
		{
			throw new NotImplementedException();
		}

		private double[][] ComputeAlpha(IList<int> observation, out double[] scaleFactors)
		{
			double[][] array = new double[observation.Count][];
			scaleFactors = new double[observation.Count];
			for (int i = 0; i < observation.Count; i++)
			{
				scaleFactors[i] = 0.0;
				array[i] = new double[States];
				for (int j = 0; j < States; j++)
				{
					if (i == 0)
					{
						array[i][j] = _p[j] * _b[j][observation[i]];
					}
					else
					{
						for (int k = 0; k < States; k++)
						{
							array[i][j] += array[i - 1][k] * _a[k][j];
						}
						array[i][j] *= _b[j][observation[i]];
					}
					scaleFactors[i] += array[i][j];
				}
				for (int l = 0; l < States; l++)
				{
					array[i][l] /= scaleFactors[i];
				}
			}
			return array;
		}

		private double[][] ComputeBeta(IList<int> observation, IList<double> scaleFactors)
		{
			double[][] array = new double[observation.Count][];
			for (int num = observation.Count - 1; num >= 0; num--)
			{
				array[num] = new double[States];
				for (int i = 0; i < States; i++)
				{
					if (num == observation.Count - 1)
					{
						array[num][i] = 1.0;
						continue;
					}
					for (int j = 0; j < States; j++)
					{
						array[num][i] += _a[i][j] * _b[j][observation[num + 1]] * array[num + 1][j];
					}
				}
				for (int k = 0; k < States; k++)
				{
					array[num][k] /= scaleFactors[num];
				}
			}
			return array;
		}

		private static double GetExpectedTransitionCount(IReadOnlyList<double[][]> ptij, int i, int j, int maxT)
		{
			double num = 0.0;
			for (int k = 0; k < maxT; k++)
			{
				num += ptij[k][i][j];
			}
			return num;
		}

		private static double GetExpectedTransitionCountWithSymbol(IReadOnlyList<double[][]> ptij, int i, int j, int symbol, IList<int> observation)
		{
			double num = 0.0;
			for (int k = 0; k < observation.Count; k++)
			{
				if (observation[k] == symbol)
				{
					num += ptij[k][i][j];
				}
			}
			return num;
		}

		public override double GetSequenceProbability(IList<int> observation)
		{
			if (observation == null)
			{
				throw new ArgumentNullException("observation");
			}
			if (observation.Count == 0)
			{
				throw new ArgumentOutOfRangeException("observation");
			}
			double[] stateProbabilities = null;
			double[] outputStateProbabilities = null;
			InitializeForward(ref stateProbabilities, observation[0]);
			for (int i = 1; i < observation.Count; i++)
			{
				Forward(ref stateProbabilities, ref outputStateProbabilities, observation[i]);
				stateProbabilities = outputStateProbabilities;
			}
			return stateProbabilities.Sum();
		}

		public override List<int> Generate(int length, out Path path)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = 0;
			List<int> list;
			do
			{
				list = GenerateInternal(length, out path);
				num++;
				if (num > 100)
				{
					throw new Exception("Can't find path");
				}
			}
			while (list == null || list.Count < length);
			return list;
		}

		private List<int> GenerateInternal(int length, out Path path)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			List<int> list = new List<int>();
			path = new Path();
			int num = PickIndexRandomly(_p);
			int num2 = 0;
			while (_p[num] == 0.0 && num2 < 10)
			{
				num = PickIndexRandomly(_p);
				num2++;
			}
			if (_p[num] == 0.0)
			{
				return null;
			}
			path.StateSequence.Add(num);
			path.P = _p[num];
			length--;
			while (length > 0)
			{
				int num3 = PickIndexRandomly(_a[num]);
				int num4 = 0;
				while (_a[num][num3] == 0.0 && num4 < 10)
				{
					num3 = PickIndexRandomly(_a[num]);
					num4++;
				}
				if (_a[num][num3] == 0.0)
				{
					return null;
				}
				int num5 = PickIndexRandomly(_b[num]);
				int num6 = 0;
				while (_b[num][num5] > 0.0 && num6 < 10)
				{
					num5 = PickIndexRandomly(_b[num]);
					num6++;
				}
				if (_b[num][num5] == 0.0)
				{
					return null;
				}
				list.Add(num5);
				path.StateSequence.Add(num3);
				path.P = path.P * _a[num][num3] * _b[num][num5];
				length--;
				num = num3;
			}
			return list;
		}

		public override List<Path> GetBestStateSequences(IList<int> observation, int n)
		{
			if (observation == null || observation.Count == 0)
			{
				return null;
			}
			double[] stateProbabilities = null;
			InitializeForward(ref stateProbabilities, observation[0]);
			List<Path> list = new List<Path>();
			for (int i = 0; i < States; i++)
			{
				list.Add(new Path());
				list[i].P = stateProbabilities[i];
				list[i].StateSequence.Add(i);
			}
			for (int j = 1; j < observation.Count; j++)
			{
				for (int k = 0; k < States; k++)
				{
					stateProbabilities[k] = list[k].P;
				}
				for (int l = 0; l < States; l++)
				{
					int item = 0;
					double num = 0.0;
					for (int m = 0; m < States; m++)
					{
						double num2 = stateProbabilities[m] * _a[m][l] * _b[l][observation[j]];
						if (m == 0 || num2 > num)
						{
							item = m;
							num = num2;
						}
					}
					list[l].P = num;
					list[l].StateSequence.Add(item);
				}
			}
			list.Sort((Path x, Path y) => Math.Sign(y.P - x.P));
			if (n < list.Count)
			{
				list.RemoveRange(n, list.Count - n);
			}
			return list;
		}

		public override Path GetBestStateSequence(IList<int> observation)
		{
			List<Path> bestStateSequences = GetBestStateSequences(observation, 1);
			if (bestStateSequences != null && bestStateSequences.Count != 0)
			{
				return bestStateSequences[0];
			}
			return null;
		}

		private void InitializeForward(ref double[] stateProbabilities, int symbol)
		{
			if (stateProbabilities == null)
			{
				stateProbabilities = new double[States];
			}
			for (int i = 0; i < States; i++)
			{
				stateProbabilities[i] = _p[i] * _b[i][symbol];
			}
		}

		private void Forward(ref double[] inputStateProbabilities, ref double[] outputStateProbabilities, int symbol)
		{
			if (outputStateProbabilities == null)
			{
				outputStateProbabilities = new double[States];
			}
			for (int i = 0; i < States; i++)
			{
				double num = 0.0;
				for (int j = 0; j < States; j++)
				{
					num += inputStateProbabilities[j] * _a[j][i];
				}
				outputStateProbabilities[i] = num * _b[i][symbol];
			}
		}

		public override bool Verify()
		{
			double num = _p.Sum();
			if (Math.Abs(1.0 - num) > HMM.ZeroDelta)
			{
				return false;
			}
			for (int i = 0; i < States; i++)
			{
				num = _a[i].Sum();
				if (Math.Abs(1.0 - num) > HMM.ZeroDelta)
				{
					return false;
				}
				for (int j = 0; j < States; j++)
				{
					double num2 = _b[i].Sum();
					if (Math.Abs(1.0 - num2) > HMM.ZeroDelta)
					{
						return false;
					}
				}
			}
			switch (Type)
			{
			case HMMType.LeftToRight:
			{
				if (_p[0] != 1.0)
				{
					return false;
				}
				for (int m = 0; m < States; m++)
				{
					for (int n = 0; n < m; n++)
					{
						if (_a[m][n] != 0.0)
						{
							return false;
						}
					}
				}
				break;
			}
			case HMMType.StrictLeftToRight:
			{
				if (_p[0] != 1.0)
				{
					return false;
				}
				for (int k = 0; k < States; k++)
				{
					for (int l = 0; l <= k; l++)
					{
						if (_a[k][l] != 0.0)
						{
							return false;
						}
					}
				}
				break;
			}
			}
			return true;
		}

		protected override void Normalize()
		{
			Normalize(_p);
			for (int i = 0; i < States; i++)
			{
				Normalize(_a[i]);
				Normalize(_b[i]);
			}
		}
	}
}
