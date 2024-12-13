using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// IItemCollection interface represents an item collection; an item collection is
	/// a list that fires events when an item is added or removed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IItemCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		/// <summary>
		/// ItemAdded event is fired when an item is added.
		/// </summary>
		event EventHandler<ItemCollectionAddedEventArgs<T>> ItemAdded;

		/// <summary>
		/// ItemRemoved event is fired when an item is removed.
		/// </summary>
		event EventHandler<ItemCollectionRemovedEventArgs<T>> ItemRemoved;
	}
}
