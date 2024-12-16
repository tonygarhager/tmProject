using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class HitDescendingComparer : IComparer<Hit>
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
				num = b.Key - a.Key;
			}
			return num;
		}
	}
}
