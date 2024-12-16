using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public class ChiSquareScoreProvider : IScoreProvider, IDisposable
	{
		private BinaryDoubleSparseMatrix _scores;

		private readonly string _fileName;

		public bool Exists => File.Exists(_fileName);

		public bool IsLoaded => !_scores.IsEmpty;

		public ChiSquareScoreProvider(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_fileName = location.GetComponentFileName(DataFileType.BilingualChiSquareScores, srcCulture, trgCulture);
			_scores = new BinaryDoubleSparseMatrix();
		}

		public void Load()
		{
			_scores.Load(_fileName);
		}

		public void Close()
		{
			_scores = null;
		}

		public double GetScore(int sW, int tW)
		{
			if (sW < 0 || tW < 0)
			{
				return 0.0;
			}
			return _scores[sW, tW];
		}

		public void Dispose()
		{
			_scores = null;
		}
	}
}
