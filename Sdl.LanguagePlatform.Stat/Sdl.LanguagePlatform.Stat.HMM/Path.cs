using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat.HMM
{
	public class Path
	{
		public List<int> StateSequence;

		public double P;

		public Path()
		{
			StateSequence = new List<int>();
			P = 0.0;
		}
	}
}
