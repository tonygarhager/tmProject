using Sdl.Core.Bcm.BcmModel.Collections;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Skeleton
{
	public class SkeletonCollection<T> : KeyedCollection<SkeletonCollectionKey, T>, IEquatable<SkeletonCollection<T>> where T : SkeletonItem
	{
		private readonly HashDictionaryInt<T, int> _reverse = new HashDictionaryInt<T, int>();

		public IdGenerator IdGenerator => ParentSkeleton.IdGenerator;

		public FileSkeleton ParentSkeleton
		{
			get;
			set;
		}

		public T GetById(int id)
		{
			SkeletonCollectionKey key = SkeletonCollectionKey.From(id);
			if (!Contains(key))
			{
				return null;
			}
			return base[key];
		}

		public T GetOrAdd(T elem)
		{
			if (_reverse.ContainsKey(elem))
			{
				return base[_reverse[elem]];
			}
			lock (this)
			{
				elem.Id = IdGenerator.GetNext<T>();
				Add(elem);
				return elem;
			}
		}

		public T GetOrAddWithExistingId(T elem)
		{
			if (_reverse.ContainsKey(elem))
			{
				return base[_reverse[elem]];
			}
			lock (this)
			{
				Add(elem);
				return elem;
			}
		}

		protected override SkeletonCollectionKey GetKeyForItem(T item)
		{
			return SkeletonCollectionKey.From(item.Id);
		}

		protected override void ClearItems()
		{
			_reverse.Clear();
			base.ClearItems();
		}

		protected override void InsertItem(int index, T item)
		{
			if (!_reverse.ContainsKey(item))
			{
				_reverse.Add(item, index);
			}
			if (item.Id == 0)
			{
				item.Id = IdGenerator.GetNext<T>();
			}
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			_reverse.Remove(base[index]);
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, T item)
		{
			_reverse.Remove(base[index]);
			_reverse.Add(item, index);
			base.SetItem(index, item);
		}

		public bool Equals(SkeletonCollection<T> other)
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
	}
}
