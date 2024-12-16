using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.Stat.HMM
{
	public abstract class HMM
	{
		protected static readonly double ZeroDelta = 1E-05;

		protected int States;

		protected int Vocabulary;

		protected HMMType Type;

		protected Random Random;

		public static HMM Create(int states, int vocabulary)
		{
			return Create(HMMType.Ergodic, states, vocabulary);
		}

		public static HMM Create(HMMType t, int states, int vocabulary)
		{
			return new HMMImplMoore(t, states, vocabulary);
		}

		protected HMM(HMMType t, int states, int vocabulary)
		{
			if (t == HMMType.StrictLeftToRight)
			{
				throw new NotImplementedException();
			}
			Type = t;
			States = states;
			Vocabulary = vocabulary;
			Random = new Random();
		}

		public abstract void InitializeTraining(bool uniformDistribution);

		public abstract void SingleTrainingIteration(IList<IList<int>> observations);

		public virtual void SingleTrainingIteration(IList<int> observation)
		{
			List<IList<int>> observations = new List<IList<int>>
			{
				observation
			};
			SingleTrainingIteration(observations);
		}

		public virtual void Save(string fileName)
		{
			using (Stream output = File.Create(fileName))
			{
				Save(output);
			}
		}

		public abstract void Save(Stream output);

		public static HMM Load(string fileName)
		{
			using (Stream input = File.OpenRead(fileName))
			{
				return Load(input);
			}
		}

		public static HMM Load(Stream input)
		{
			throw new NotImplementedException();
		}

		public abstract double GetSequenceProbability(IList<int> observation);

		public abstract List<int> Generate(int length, out Path path);

		public abstract List<Path> GetBestStateSequences(IList<int> observation, int n);

		public abstract Path GetBestStateSequence(IList<int> observation);

		public abstract bool Verify();

		protected abstract void Normalize();

		protected void Normalize(IList<double> distribution)
		{
			double num = distribution.Sum();
			if (Math.Abs(1.0 - num) > ZeroDelta)
			{
				for (int i = 0; i < distribution.Count; i++)
				{
					distribution[i] /= num;
				}
			}
		}

		protected int PickIndexRandomly(IList<double> distribution)
		{
			double num = Random.NextDouble();
			double num2 = 0.0;
			int i;
			for (i = 0; i < distribution.Count; i++)
			{
				if (!(num2 < num))
				{
					break;
				}
				num2 += distribution[i];
			}
			if (i <= 0)
			{
				return 0;
			}
			return i - 1;
		}

		protected void InitializeDistributionUniform(IList<double> distribution)
		{
			InitializeDistributionUniform(distribution, 0);
		}

		protected void InitializeDistributionUniform(IList<double> distribution, int k)
		{
			if (k < 0)
			{
				k = 0;
			}
			double value = (k < distribution.Count) ? (1.0 / (double)(distribution.Count - k)) : 0.0;
			for (int i = k; i < distribution.Count; i++)
			{
				distribution[i] = value;
			}
		}

		protected void InitializeDistributionRandom(IList<double> distribution)
		{
			InitializeDistributionRandom(distribution, 0);
		}

		protected void InitializeDistributionRandom(IList<double> distribution, int k)
		{
			if (k < 0)
			{
				k = 0;
			}
			if (k >= distribution.Count)
			{
				for (int i = 0; i < distribution.Count; i++)
				{
					distribution[i] = 0.0;
				}
				return;
			}
			for (int j = k; j < distribution.Count; j++)
			{
				distribution[j] = Random.NextDouble();
			}
			Normalize(distribution);
		}
	}
}
