using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class Counter<T> : IEnumerable<T>, IEnumerable
	{
		private IDictionary<T, int> _itemCount = new Dictionary<T, int>();

		public int GetCount(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (!_itemCount.TryGetValue(item, out int value))
			{
				return 0;
			}
			return value;
		}

		private void SetCount(T item, int count)
		{
			_itemCount[item] = count;
		}

		public void Add(T item)
		{
			Add(item, 1);
		}

		public void Add(T item, int numberOfTimes)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (numberOfTimes < 1)
			{
				throw new ArgumentOutOfRangeException("numberOfTimes", "numberOfTimes must be > 0");
			}
			int count = GetCount(item);
			int num = count + numberOfTimes;
			if (num < 0)
			{
				throw new OverflowException("Adding the item would cause the item count to be greater than Int32.MaxValue");
			}
			SetCount(item, num);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _itemCount.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
