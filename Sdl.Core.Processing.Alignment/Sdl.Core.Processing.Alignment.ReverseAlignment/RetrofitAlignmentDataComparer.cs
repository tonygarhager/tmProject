using Sdl.Core.Processing.Alignment.Core;
using System;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	public class RetrofitAlignmentDataComparer : IComparer<AlignmentData>
	{
		public int Compare(AlignmentData x, AlignmentData y)
		{
			if (x == null)
			{
				throw new ArgumentNullException("x");
			}
			if (y == null)
			{
				throw new ArgumentNullException("y");
			}
			if ((x.LeftIds == null || x.LeftIds.Count == 0) && y.LeftIds != null && y.LeftIds.Count != 0)
			{
				return -1;
			}
			if ((y.LeftIds == null || y.LeftIds.Count == 0) && x.LeftIds != null && x.LeftIds.Count != 0)
			{
				return -1;
			}
			int num = int.Parse(x.LeftIds[0].SegmentId.Id).CompareTo(int.Parse(y.LeftIds[0].SegmentId.Id));
			if (num != 0)
			{
				return num;
			}
			if ((x.RightIds == null || x.RightIds.Count == 0) && y.RightIds != null && y.RightIds.Count != 0)
			{
				return 1;
			}
			if ((y.RightIds == null || y.RightIds.Count == 0) && x.RightIds != null && x.RightIds.Count != 0)
			{
				return -1;
			}
			return int.Parse(x.RightIds[0].SegmentId.Id).CompareTo(int.Parse(y.RightIds[0].SegmentId.Id));
		}
	}
}
