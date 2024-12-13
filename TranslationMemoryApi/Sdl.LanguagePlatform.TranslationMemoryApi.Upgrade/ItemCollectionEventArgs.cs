using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// ItemCollectionEventArgs class represents item collection event arguments.
	/// </summary>
	/// <typeparam name="T">item type</typeparam>
	public abstract class ItemCollectionEventArgs<T> : EventArgs
	{
		private readonly T _item;

		/// <summary>
		/// Item property represents the item that has been added or removed.
		/// </summary>
		public T Item => _item;

		/// <summary>
		/// Constructor that takes the given item.
		/// </summary>
		/// <param name="item">item</param>
		protected ItemCollectionEventArgs(T item)
		{
			_item = item;
		}
	}
}
