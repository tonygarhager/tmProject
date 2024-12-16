using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public class LTPScoreProvider : IScoreProvider, IDisposable
	{
		private FloatSparseMatrix _srcToTrgTp;

		private FloatSparseMatrix _trgToSrcTp;

		private const double Minltp = 0.01;

		public bool Exists
		{
			get
			{
				if (_srcToTrgTp.Exists)
				{
					return _trgToSrcTp.Exists;
				}
				return false;
			}
		}

		public bool IsLoaded
		{
			get
			{
				if (!_srcToTrgTp.IsEmpty)
				{
					return !_trgToSrcTp.IsEmpty;
				}
				return false;
			}
		}

		public LTPScoreProvider(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_srcToTrgTp = new FloatSparseMatrix(location.GetComponentFileName(DataFileType.SimpleTranslationProbabilitiesFile, srcCulture, trgCulture));
			_trgToSrcTp = new FloatSparseMatrix(location.GetComponentFileName(DataFileType.SimpleTranslationProbabilitiesFile, trgCulture, srcCulture));
		}

		public void Load()
		{
			_srcToTrgTp.Load();
			_trgToSrcTp.Load();
		}

		public void Close()
		{
			_srcToTrgTp = null;
			_trgToSrcTp = null;
		}

		public double GetScore(int sW, int tW)
		{
			if (sW < 0 || tW < 0)
			{
				return 0.0;
			}
			float num = _srcToTrgTp[sW, tW];
			float num2 = _trgToSrcTp[tW, sW];
			double num3 = (double)(num + num2) / 2.0;
			if (!(num3 < 0.01))
			{
				return num3;
			}
			return 0.0;
		}

		public void Dispose()
		{
			_srcToTrgTp = null;
			_trgToSrcTp = null;
		}
	}
}
