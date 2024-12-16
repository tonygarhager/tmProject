using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.BilingualApi
{
	public interface IAbstractMarkupDataContainer : IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId
	{
		IEnumerable<IAbstractMarkupData> AllSubItems
		{
			get;
		}

		bool CanBeSplit
		{
			get;
		}

		IEnumerable<Location> Locations
		{
			get;
		}

		IAbstractMarkupDataContainer Split(int splitBeforeItemIndex);

		void MoveAllItemsTo(IAbstractMarkupDataContainer destinationContainer);

		void MoveAllItemsTo(IAbstractMarkupDataContainer destinationContainer, int insertAtIndex);

		void MoveItemsTo(IAbstractMarkupDataContainer destinationContainer, int startIndex, int count);

		void MoveItemsTo(IAbstractMarkupDataContainer destinationContainer, int destinationIndex, int startIndex, int count);

		IEnumerable<Location> GetLocationsFrom(Location startingFrom);

		Location Find(Predicate<Location> match);

		IAbstractMarkupData Find(Predicate<IAbstractMarkupData> match);

		Location Find(Location startAt, Predicate<Location> match);

		void ForEachSubItem(Action<IAbstractMarkupData> action);
	}
}
