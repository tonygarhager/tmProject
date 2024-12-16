using System;

namespace Sdl.Core.Processing.Alignment
{
	internal class AlignmentGap
	{
		public int FirstSourceIndex
		{
			get;
			set;
		}

		public int LastSourceIndex
		{
			get;
			set;
		}

		public int FirstTargetIndex
		{
			get;
			set;
		}

		public int LastTargetIndex
		{
			get;
			set;
		}

		public AlignmentGap()
		{
			FirstSourceIndex = -1;
			LastSourceIndex = -1;
			FirstTargetIndex = -1;
			LastTargetIndex = -1;
		}

		public bool SourceContains(int index)
		{
			if (FirstSourceIndex > -1 && LastSourceIndex > -1)
			{
				if (FirstSourceIndex <= index)
				{
					return index <= LastSourceIndex;
				}
				return false;
			}
			return false;
		}

		public bool TargetContains(int index)
		{
			if (FirstTargetIndex > -1 && LastTargetIndex > -1)
			{
				if (FirstTargetIndex <= index)
				{
					return index <= LastTargetIndex;
				}
				return false;
			}
			return false;
		}

		public bool IsEmpty()
		{
			if (FirstSourceIndex == -1 && LastSourceIndex == -1 && FirstTargetIndex == -1)
			{
				return LastTargetIndex == -1;
			}
			return false;
		}

		public int MaxRangeSize()
		{
			if (IsEmpty())
			{
				return 0;
			}
			return Math.Max(LastSourceIndex - FirstSourceIndex, LastTargetIndex - FirstTargetIndex) + 1;
		}
	}
}
