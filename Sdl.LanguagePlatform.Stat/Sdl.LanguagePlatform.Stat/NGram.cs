using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.Stat
{
	internal class NGram : IComparable<NGram>, ICountable
	{
		public int[] Data
		{
			get;
		}

		public int Length => Data.Length;

		public int Count
		{
			get;
			set;
		}

		public NGram(IList<int> data)
			: this(data, 1)
		{
		}

		public NGram(IEnumerable<int> data, int count)
		{
			Data = data.ToArray();
			Count = count;
		}

		public int CompareTo(NGram other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			if (other.Data.Length != Data.Length)
			{
				throw new ArgumentException("NGram width differs");
			}
			int num = 0;
			for (int i = 0; i < Data.Length; i++)
			{
				if (num != 0)
				{
					break;
				}
				num = Data[i] - other.Data[i];
			}
			return num;
		}
	}
}
