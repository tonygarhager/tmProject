using System;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	public class LengthAlignmentCostComputer : IAlignmentCostComputer<int>
	{
		private readonly double _expansionFactor;

		private const double Variance = 6.8;

		private const double SubstitutionProbability = 0.89;

		private const double InsertionOrDeletionProbability = 0.01;

		private const double ExpansionOrContractionProbability = 0.09;

		private const double MeldingProbability = 0.01;

		public LengthAlignmentCostComputer(double expansionFactor)
		{
			_expansionFactor = expansionFactor;
		}

		public virtual int GetSubstitutionCosts(int s, int t)
		{
			return (int)(-100.0 * GetScore(s, t));
		}

		public virtual int GetDeletionCosts(int s)
		{
			int num = (int)(-100.0 * Math.Log(0.011235955056179775));
			return num + (int)(-100.0 * GetScore(s, 0));
		}

		public virtual int GetInsertionCosts(int t)
		{
			int num = (int)(-100.0 * Math.Log(0.011235955056179775));
			return num + (int)(-100.0 * GetScore(0, t));
		}

		public virtual int GetContractionCosts(int s1, int s2, int t)
		{
			int num = (int)(-100.0 * Math.Log(0.10112359550561797));
			return num + (int)(-100.0 * GetScore(s1 + s2, t));
		}

		public virtual int GetExpansionCosts(int s, int t1, int t2)
		{
			int num = (int)(-100.0 * Math.Log(0.10112359550561797));
			return num + (int)(-100.0 * GetScore(s, t1 + t2));
		}

		public virtual int GetMeldingCosts(int s1, int s2, int t1, int t2)
		{
			int num = (int)(-100.0 * Math.Log(0.011235955056179775));
			return num + (int)(-100.0 * GetScore(s1 + s2, t1 + t2));
		}

		private double GetScore(int srcLen, int trgLen)
		{
			double z = 1.0;
			if (srcLen == 0 && trgLen == 0)
			{
				return 2.0 * (1.0 - pnorm(z));
			}
			double num = (double)srcLen * _expansionFactor - (double)trgLen;
			double num2 = ((double)srcLen + (double)trgLen / _expansionFactor) / 2.0;
			z = Math.Abs(num / Math.Sqrt(num2 * 6.8));
			return 2.0 * (1.0 - pnorm(z));
		}

		public static double pnorm(double z)
		{
			double num = 1.0 / (1.0 + 0.2316419 * z);
			return 1.0 - 0.3989423 * Math.Exp((0.0 - z) * z / 2.0) * ((((1.330274429 * num - 1.821255978) * num + 1.781477937) * num - 0.356563782) * num + 0.31938153) * num;
		}
	}
}
