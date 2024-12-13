using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// ItemCollection class represents an item collection; an item collection is
	/// a list that fires events when an item is added or removed.
	/// </summary>
	/// <typeparam name="T">item type</typeparam>
	public class ItemCollection<T> : Collection<T>, IItemCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		protected bool _suspendAddedRemovedEvents;

		/// <summary>
		/// ItemAdded event is fired when an item is added.
		/// </summary>
		public event EventHandler<ItemCollectionAddedEventArgs<T>> ItemAdded;

		/// <summary>
		/// ItemRemoved event is fired when an item is removed.
		/// </summary>
		public event EventHandler<ItemCollectionRemovedEventArgs<T>> ItemRemoved;

		/// <summary>
		/// Clears the items.
		/// </summary>
		protected override void ClearItems()
		{
			IList<T> list = new List<T>(this);
			base.ClearItems();
			foreach (T item in list)
			{
				OnItemRemoved(item);
			}
		}

		/// <summary>
		/// Inserts the given item at the given index.
		/// </summary>
		/// <param name="index">index</param>
		/// <param name="item">item</param>
		protected override void InsertItem(int index, T item)
		{
			base.InsertItem(index, item);
			OnItemAdded(item);
		}

		/// <summary>
		/// Removes the item at the given index.
		/// </summary>
		/// <param name="index">index</param>
		protected override void RemoveItem(int index)
		{
			T item = base[index];
			base.RemoveItem(index);
			OnItemRemoved(item);
		}

		/// <summary>
		/// Sets the item at the given index.
		/// </summary>
		/// <param name="index">index</param>
		/// <param name="item">item</param>
		protected override void SetItem(int index, T item)
		{
			T item2 = base[index];
			base.SetItem(index, item);
			OnItemRemoved(item2);
			OnItemAdded(item);
		}

		/// <summary>
		/// Fires the ItemAdded event for the given item.
		/// </summary>
		/// <param name="item">item</param>
		private void OnItemAdded(T item)
		{
			if (this.ItemAdded != null && !_suspendAddedRemovedEvents)
			{
				this.ItemAdded(this, new ItemCollectionAddedEventArgs<T>(item));
			}
		}

		/// <summary>
		/// Fires the ItemRemoved event for the given item.
		/// </summary>
		/// <param name="item">item</param>
		private void OnItemRemoved(T item)
		{
			if (this.ItemRemoved != null && !_suspendAddedRemovedEvents)
			{
				this.ItemRemoved(this, new ItemCollectionRemovedEventArgs<T>(item));
			}
		}

		/// <summary>
		/// Override to check if two objects have the same values
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ItemCollection<T> itemCollection = obj as ItemCollection<T>;
			if (itemCollection == null)
			{
				return false;
			}
			if (base.Count != itemCollection.Count)
			{
				return false;
			}
			for (int i = 0; i < base.Count; i++)
			{
				if (!base[i].Equals(itemCollection[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
