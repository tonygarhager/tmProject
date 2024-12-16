using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.Stat.HMM
{
	internal class HMMImplMealy : HMM
	{
		private readonly double[] _p;

		private readonly double[][] _a;

		private readonly double[][][] _b;

		public HMMImplMealy(int states, int vocabulary)
			: this(HMMType.Ergodic, states, vocabulary)
		{
		}

		public HMMImplMealy(HMMType t, int states, int vocabulary)
			: base(t, states, vocabulary)
		{
			if (t == HMMType.StrictLeftToRight)
			{
				throw new NotImplementedException();
			}
			_p = new double[States];
			_a = new double[States][];
			_b = new double[States][][];
			for (int i = 0; i < States; i++)
			{
				_a[i] = new double[States];
				_b[i] = new double[States][];
				for (int j = 0; j < States; j++)
				{
					_b[i][j] = new double[Vocabulary];
				}
			}
		}

		public void SetA(int i, int j, double v)
		{
			_a[i][j] = v;
		}

		public void SetB(int i, int j, int o, double v)
		{
			_b[i][j][o] = v;
		}

		public void SetP(int i, double v)
		{
			_p[i] = v;
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
				}
				else
				{
					InitializeDistributionRandom(_a[i], k);
				}
				int num;
				switch (Type)
				{
				case HMMType.LeftToRight:
					num = i;
					break;
				case HMMType.StrictLeftToRight:
					num = i + 1;
					break;
				default:
					num = 0;
					break;
				}
				for (int j = num; j < States; j++)
				{
					if (uniformDistribution)
					{
						InitializeDistributionUniform(_b[i][j]);
					}
					else
					{
						InitializeDistributionRandom(_b[i][j]);
					}
				}
			}
		}

		public override void SingleTrainingIteration(IList<IList<int>> observations)
		{
			if (!Verify())
			{
				throw new InvalidOperationException("The model is inconsistent and cannot be trained.");
			}
			HMMImplMealy hMMImplMealy = new HMMImplMealy(States, Vocabulary);
			HMMImplMealy hMMImplMealy2 = new HMMImplMealy(States, Vocabulary);
			foreach (IList<int> observation in observations)
			{
				double[] scaleFactors;
				double[][] array = ComputeAlpha(observation, out scaleFactors);
				double[][] array2 = ComputeBeta(observation, scaleFactors);
				double[][][] array3 = new double[States][][];
				for (int i = 0; i < States; i++)
				{
					array3[i] = new double[States][];
					for (int j = 0; j < States; j++)
					{
						array3[i][j] = new double[observation.Count];
					}
				}
				for (int k = 0; k < observation.Count; k++)
				{
					double num = 0.0;
					for (int l = 0; l < States; l++)
					{
						for (int m = 0; m < States; m++)
						{
							double num2 = array[l][k] * _a[l][m] * _b[l][m][observation[k]] * array2[m][k + 1];
							array3[l][m][k] = num2;
							num += num2;
						}
					}
					for (int n = 0; n < States; n++)
					{
						for (int num3 = 0; num3 < States; num3++)
						{
							array3[n][num3][k] /= num;
						}
					}
				}
				for (int num4 = 0; num4 < States; num4++)
				{
					double num5 = 0.0;
					for (int num6 = 0; num6 < States; num6++)
					{
						double expectedTransitionCount = GetExpectedTransitionCount(array3, num4, num6);
						hMMImplMealy._a[num4][num6] += expectedTransitionCount / 1.0;
						num5 += expectedTransitionCount;
						double num7 = 0.0;
						HashSet<int> hashSet = new HashSet<int>();
						foreach (int item in observation)
						{
							if (!hashSet.Contains(item))
							{
								hashSet.Add(item);
								double expectedTransitionCountWithSymbol = GetExpectedTransitionCountWithSymbol(array3, num4, num6, item, observation);
								num7 += expectedTransitionCountWithSymbol;
								hMMImplMealy._b[num4][num6][item] += expectedTransitionCountWithSymbol / 1.0;
							}
						}
						for (int num8 = 0; num8 < Vocabulary; num8++)
						{
							hMMImplMealy2._b[num4][num6][num8] += expectedTransitionCount / 1.0;
						}
						Math.Abs(num7 - expectedTransitionCount);
						_ = HMM.ZeroDelta;
					}
					for (int num9 = 0; num9 < States; num9++)
					{
						hMMImplMealy2._a[num4][num9] += num5 / 1.0;
					}
				}
			}
			for (int num10 = 0; num10 < States; num10++)
			{
				for (int num11 = 0; num11 < States; num11++)
				{
					if (hMMImplMealy._a[num10][num11] == 0.0)
					{
						_a[num10][num11] = 0.0;
					}
					else
					{
						_a[num10][num11] = hMMImplMealy._a[num10][num11] / hMMImplMealy2._a[num10][num11];
					}
					for (int num12 = 0; num12 < Vocabulary; num12++)
					{
						if (hMMImplMealy._b[num10][num11][num12] == 0.0)
						{
							_b[num10][num11][num12] = 0.0;
						}
						else
						{
							_b[num10][num11][num12] = hMMImplMealy._b[num10][num11][num12] / hMMImplMealy2._b[num10][num11][num12];
						}
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
			double[][] array = new double[States][];
			scaleFactors = new double[observation.Count + 1];
			for (int i = 0; i < States; i++)
			{
				array[i] = new double[observation.Count + 1];
				array[i][0] = _p[i];
				scaleFactors[0] += _p[i];
			}
			for (int j = 0; j < States; j++)
			{
				array[j][0] /= scaleFactors[0];
			}
			for (int k = 0; k < observation.Count; k++)
			{
				for (int l = 0; l < States; l++)
				{
					double num = 0.0;
					for (int m = 0; m < States; m++)
					{
						double num2 = array[m][k] * _a[m][l] * _b[m][l][observation[k]];
						num += num2;
					}
					array[l][k + 1] = num;
					scaleFactors[k + 1] += num;
				}
				for (int n = 0; n < States; n++)
				{
					array[n][k + 1] /= scaleFactors[k + 1];
				}
			}
			for (int num3 = 0; num3 <= observation.Count; num3++)
			{
				double num4 = 0.0;
				for (int num5 = 0; num5 < States; num5++)
				{
					num4 += array[num5][num3];
				}
				if (Math.Abs(1.0 - num4) > HMM.ZeroDelta)
				{
					throw new Exception("Internal error (scaling)");
				}
			}
			return array;
		}

		private double[][] ComputeBeta(IList<int> observation, IList<double> scaleFactors)
		{
			double[][] array = new double[States][];
			for (int i = 0; i < States; i++)
			{
				array[i] = new double[observation.Count + 1];
				array[i][observation.Count] = 1.0 / scaleFactors[observation.Count];
			}
			for (int num = observation.Count - 1; num >= 0; num--)
			{
				for (int j = 0; j < States; j++)
				{
					double num2 = 0.0;
					for (int k = 0; k < States; k++)
					{
						double num3 = array[k][num + 1] * _a[j][k] * _b[j][k][observation[num]];
						num2 += num3;
					}
					array[j][num] = num2 / scaleFactors[num];
				}
			}
			return array;
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
			double[] outputStateProbabilities = null;
			InitializeForward(out double[] stateProbabilities);
			foreach (int item in observation)
			{
				Forward(ref stateProbabilities, ref outputStateProbabilities, item);
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
				int num5 = PickIndexRandomly(_b[num][num3]);
				int num6 = 0;
				while (_b[num][num3][num5] > 0.0 && num6 < 10)
				{
					num5 = PickIndexRandomly(_b[num][num3]);
					num6++;
				}
				if (_b[num][num3][num5] == 0.0)
				{
					return null;
				}
				list.Add(num5);
				path.StateSequence.Add(num3);
				path.P = path.P * _a[num][num3] * _b[num][num3][num5];
				length--;
				num = num3;
			}
			return list;
		}

		public override List<Path> GetBestStateSequences(IList<int> observation, int n)
		{
			if (observation == null)
			{
				return null;
			}
			InitializeForward(out double[] stateProbabilities);
			List<Path> list = new List<Path>();
			for (int i = 0; i < States; i++)
			{
				list.Add(new Path());
				list[i].P = stateProbabilities[i];
				list[i].StateSequence.Add(i);
			}
			foreach (int item2 in observation)
			{
				for (int j = 0; j < States; j++)
				{
					stateProbabilities[j] = list[j].P;
				}
				for (int k = 0; k < States; k++)
				{
					int item = 0;
					double num = 0.0;
					for (int l = 0; l < States; l++)
					{
						double num2 = stateProbabilities[l] * _a[l][k] * _b[l][k][item2];
						if (l == 0 || num2 > num)
						{
							item = l;
							num = num2;
						}
					}
					list[k].P = num;
					list[k].StateSequence.Add(item);
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

		private void InitializeForward(out double[] stateProbabilities)
		{
			stateProbabilities = new double[States];
			for (int i = 0; i < States; i++)
			{
				stateProbabilities[i] = _p[i];
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
					num += inputStateProbabilities[j] * _a[j][i] * _b[j][i][symbol];
				}
				outputStateProbabilities[i] = num;
			}
		}

		private static double GetExpectedTransitionCount(IReadOnlyList<double[][]> pijt, int i, int j)
		{
			return pijt[i][j].Sum();
		}

		private static double GetExpectedTransitionCountWithSymbol(IReadOnlyList<double[][]> pijt, int i, int j, int symbol, IList<int> observation)
		{
			double num = 0.0;
			for (int k = 0; k < observation.Count; k++)
			{
				if (observation[k] == symbol)
				{
					num += pijt[i][j][k];
				}
			}
			return num;
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
					double num2 = _b[i][j].Sum();
					switch (Type)
					{
					case HMMType.Ergodic:
						if (Math.Abs(1.0 - num2) > HMM.ZeroDelta)
						{
							return false;
						}
						break;
					case HMMType.LeftToRight:
						if (j >= i)
						{
							if (Math.Abs(1.0 - num2) > HMM.ZeroDelta)
							{
								return false;
							}
						}
						else if (num2 != 0.0)
						{
							return false;
						}
						break;
					case HMMType.StrictLeftToRight:
						if (j > i)
						{
							if (Math.Abs(1.0 - num2) > HMM.ZeroDelta)
							{
								return false;
							}
						}
						else if (num2 != 0.0)
						{
							return false;
						}
						break;
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
						if (_b[m][n].Sum() != 0.0)
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
						if (_b[k][l].Sum() != 0.0)
						{
							return false;
						}
					}
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			case HMMType.Ergodic:
				break;
			}
			return true;
		}

		protected override void Normalize()
		{
			Normalize(_p);
			for (int i = 0; i < States; i++)
			{
				Normalize(_a[i]);
				for (int j = 0; j < States; j++)
				{
					Normalize(_b[i][j]);
				}
			}
		}
	}
}
