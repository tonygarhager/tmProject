namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public class ChiSquareScoreProvider2 : IScoreProvider2
	{
		private SparseMatrix<double> _scores;

		public ChiSquareScoreProvider2(SparseMatrix<double> doubleSparseMatrix)
		{
			_scores = doubleSparseMatrix;
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
			if (_scores.HasValue(sW, tW))
			{
				return _scores[sW, tW];
			}
			return 0.0;
		}

		public void Dispose()
		{
			_scores = null;
		}
	}
}
