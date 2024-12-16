using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public abstract class ItemCollectionEventArgs<T> : EventArgs
	{
		private readonly T _item;

		public T Item => _item;

		protected ItemCollectionEventArgs(T item)
		{
			_item = item;
		}
	}
}
