namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// ItemCollectionRemovedEventArgs class represents the item collection removed event arguments.
	/// </summary>
	/// <typeparam name="T">item type</typeparam>
	public class ItemCollectionRemovedEventArgs<T> : ItemCollectionEventArgs<T>
	{
		/// <summary>
		/// Constructor that takes the given item.
		/// </summary>
		/// <param name="item">item</param>
		public ItemCollectionRemovedEventArgs(T item)
			: base(item)
		{
		}
	}
}
