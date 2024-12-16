using Sdl.Core.Bcm.BcmConverters.Common;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Bcm.BcmConverters.ToBilingualApi
{
	internal class ContextInfoPairIdComparer : IComparer<ContextInfoPair>
	{
		public int Compare(ContextInfoPair x, ContextInfoPair y)
		{
			if (x == null)
			{
				throw new ArgumentException("x");
			}
			if (y == null)
			{
				throw new ArgumentException("y");
			}
			return x.Id.CompareTo(y.Id);
		}
	}
}
