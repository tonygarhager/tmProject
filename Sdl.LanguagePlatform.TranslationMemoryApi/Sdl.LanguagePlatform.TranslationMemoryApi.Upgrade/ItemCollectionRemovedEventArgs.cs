namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class ItemCollectionRemovedEventArgs<T> : ItemCollectionEventArgs<T>
	{
		public ItemCollectionRemovedEventArgs(T item)
			: base(item)
		{
		}
	}
}
