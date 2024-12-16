using System;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public interface IScoreProvider : IDisposable
	{
		bool Exists
		{
			get;
		}

		bool IsLoaded
		{
			get;
		}

		void Load();

		void Close();

		double GetScore(int sW, int tW);
	}
}
