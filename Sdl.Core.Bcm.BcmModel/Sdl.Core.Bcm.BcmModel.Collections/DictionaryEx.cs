using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Collections
{
	public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>, IEquatable<DictionaryEx<TKey, TValue>>
	{
		public DictionaryEx()
		{
		}

		public DictionaryEx(DictionaryEx<TKey, TValue> other)
			: this(other.AsEnumerable())
		{
		}

		public DictionaryEx(IEnumerable<KeyValuePair<TKey, TValue>> items)
			: this()
		{
			foreach (KeyValuePair<TKey, TValue> item in items)
			{
				Add(item.Key, item.Value);
			}
		}

		public bool Equals(DictionaryEx<TKey, TValue> other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			if (base.Count != other.Count)
			{
				return false;
			}
			return this.OrderBy((KeyValuePair<TKey, TValue> kvp) => kvp.Key).SequenceEqual(other.OrderBy((KeyValuePair<TKey, TValue> kvp) => kvp.Key));
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((DictionaryEx<TKey, TValue>)obj);
		}

		public override int GetHashCode()
		{
			return this.Aggregate(0, (int current, KeyValuePair<TKey, TValue> pair) => current + GetItemHashCode(pair.Key) * 397 + GetItemHashCode(pair.Value));
		}

		private static int GetItemHashCode<T>(T item)
		{
			if (!EqualityComparer<T>.Default.Equals(item, default(T)))
			{
				return item.GetHashCode();
			}
			return 0;
		}
	}
}
