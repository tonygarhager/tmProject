namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// ItemCollectionAddedEventArgs class represents item collection added event arguments.
	/// </summary>
	/// <typeparam name="T">item type</typeparam>
	public class ItemCollectionAddedEventArgs<T> : ItemCollectionEventArgs<T>
	{
		/// <summary>
		/// Constructor that takes the given item.
		/// </summary>
		/// <param name="item">item</param>
		public ItemCollectionAddedEventArgs(T item)
			: base(item)
		{
		}
	}
}
