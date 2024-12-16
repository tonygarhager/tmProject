using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class HitAscendingComparer : IComparer<Hit>
	{
		public int Compare(Hit a, Hit b)
		{
			if (a == null)
			{
				throw new ArgumentNullException("a");
			}
			if (b == null)
			{
				throw new ArgumentNullException("b");
			}
			int num = b.Score - a.Score;
			if (num == 0)
			{
				num = a.Key - b.Key;
			}
			return num;
		}
	}
}
