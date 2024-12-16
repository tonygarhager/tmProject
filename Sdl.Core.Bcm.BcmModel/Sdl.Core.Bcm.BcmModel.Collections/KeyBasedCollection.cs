using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Collections
{
	public class KeyBasedCollection<TKey, TValue> : KeyedCollection<TKey, TValue>, IEquatable<KeyBasedCollection<TKey, TValue>>
	{
		protected Func<TValue, TKey> KeySelector;

		protected KeyBasedCollection()
		{
		}

		public KeyBasedCollection(Func<TValue, TKey> keySelector)
		{
			KeySelector = keySelector;
		}

		public void ForEach(Action<TValue> action)
		{
			foreach (TValue item in base.Items)
			{
				action(item);
			}
		}

		public bool Equals(KeyBasedCollection<TKey, TValue> other)
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
			return this.SequenceEqual(other);
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
			return Equals((KeyBasedCollection<TKey, TValue>)obj);
		}

		public override int GetHashCode()
		{
			int num = 0;
			foreach (TValue item in base.Items)
			{
				num ^= item.GetHashCode();
			}
			return num;
		}

		protected override TKey GetKeyForItem(TValue item)
		{
			return KeySelector(item);
		}
	}
}
