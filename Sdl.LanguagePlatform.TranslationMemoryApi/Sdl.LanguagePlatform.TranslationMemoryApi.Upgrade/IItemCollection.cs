using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IItemCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		event EventHandler<ItemCollectionAddedEventArgs<T>> ItemAdded;

		event EventHandler<ItemCollectionRemovedEventArgs<T>> ItemRemoved;
	}
}
