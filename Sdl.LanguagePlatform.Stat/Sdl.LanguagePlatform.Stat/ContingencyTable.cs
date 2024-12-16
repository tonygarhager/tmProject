using System;

namespace Sdl.LanguagePlatform.Stat
{
	public class ContingencyTable
	{
		private readonly double[,] _observed;

		private readonly double[,] _expected;

		public double N
		{
			get;
			private set;
		}

		public double R1 => _observed[0, 0] + _observed[0, 1];

		public double R2 => _observed[1, 0] + _observed[1, 1];

		public double C1 => _observed[0, 0] + _observed[1, 0];

		public double C2 => _observed[0, 1] + _observed[1, 1];

		public ContingencyTable(int joint, int aFreq, int bFreq, int sampleSize)
			: this(joint, aFreq - joint, bFreq - joint, sampleSize - aFreq - bFreq + joint, sampleSize)
		{
		}

		public ContingencyTable(int joint, int aNotb, int notAb, int notAnotB, int sampleSize)
		{
			_observed = new double[2, 2];
			_expected = new double[2, 2];
			Set(joint, aNotb, notAb, notAnotB, sampleSize);
		}

		public void Set(int joint, int aFreq, int bFreq, int sampleSize)
		{
			Set(joint, aFreq - joint, bFreq - joint, sampleSize - aFreq - bFreq + joint, sampleSize);
		}

		public void Set(int joint, int aNotb, int notAb, int notAnotB, int sampleSize)
		{
			_observed[0, 0] = joint;
			_observed[0, 1] = aNotb;
			_observed[1, 0] = notAb;
			_observed[1, 1] = notAnotB;
			N = _observed[0, 0] + _observed[0, 1] + _observed[1, 0] + _observed[1, 1];
			_expected[0, 0] = R1 * C1 / N;
			_expected[0, 1] = R1 * C2 / N;
			_expected[1, 0] = R2 * C1 / N;
			_expected[1, 1] = R2 * C2 / N;
		}

		public double ChiSquare()
		{
			double num = (_observed[0, 0] + _observed[0, 1]) * (_observed[1, 0] + _observed[1, 1]) * (_observed[0, 0] + _observed[1, 0]) * (_observed[0, 1] + _observed[1, 1]);
			return Math.Pow(_observed[0, 0] * _observed[1, 1] - _observed[0, 1] * _observed[1, 0], 2.0) * N / num;
		}
	}
}
