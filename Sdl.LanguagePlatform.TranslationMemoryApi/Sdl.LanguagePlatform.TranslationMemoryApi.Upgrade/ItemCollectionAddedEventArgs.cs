namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class ItemCollectionAddedEventArgs<T> : ItemCollectionEventArgs<T>
	{
		public ItemCollectionAddedEventArgs(T item)
			: base(item)
		{
		}
	}
}
