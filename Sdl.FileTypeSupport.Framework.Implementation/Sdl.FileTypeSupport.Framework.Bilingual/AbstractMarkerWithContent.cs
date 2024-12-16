using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public abstract class AbstractMarkerWithContent : AbstractMarkupData, IAbstractMarker, IAbstractMarkupData, ICloneable, ISupportsUniqueId, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable
	{
		private MarkupDataContainer _Content;

		public virtual IAbstractMarkupData this[int index]
		{
			get
			{
				return _Content[index];
			}
			set
			{
				_Content[index] = value;
			}
		}

		public virtual int Count => _Content.Count;

		bool ICollection<IAbstractMarkupData>.IsReadOnly => ((ICollection<IAbstractMarkupData>)_Content).IsReadOnly;

		public virtual bool CanBeSplit => _Content.CanBeSplit;

		public virtual IEnumerable<Location> Locations => _Content.Locations;

		public IEnumerable<IAbstractMarkupData> AllSubItems => _Content.AllSubItems;

		protected AbstractMarkerWithContent()
		{
			_Content = new MarkupDataContainer(this);
		}

		protected AbstractMarkerWithContent(AbstractMarkerWithContent other)
			: base(other)
		{
			_Content = (MarkupDataContainer)other._Content.Clone();
			_Content.PublicContainer = this;
		}

		protected AbstractMarkerWithContent(AbstractMarkerWithContent other, int splitBeforeItemIndex)
			: base(other)
		{
			_Content = (MarkupDataContainer)other._Content.Split(splitBeforeItemIndex);
			_Content.PublicContainer = this;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			AbstractMarkerWithContent abstractMarkerWithContent = (AbstractMarkerWithContent)obj;
			if (!_Content.Equals(abstractMarkerWithContent._Content))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _Content.GetHashCode();
		}

		public override string ToString()
		{
			return _Content.ToString();
		}

		public virtual IEnumerator<IAbstractMarkupData> GetEnumerator()
		{
			return _Content.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public virtual int IndexOf(IAbstractMarkupData item)
		{
			return _Content.IndexOf(item);
		}

		public virtual void Insert(int index, IAbstractMarkupData item)
		{
			_Content.Insert(index, item);
		}

		public virtual void RemoveAt(int index)
		{
			_Content.RemoveAt(index);
		}

		public virtual void Add(IAbstractMarkupData item)
		{
			_Content.Add(item);
		}

		public virtual void Clear()
		{
			_Content.Clear();
		}

		public virtual bool Contains(IAbstractMarkupData item)
		{
			return _Content.Contains(item);
		}

		public virtual void CopyTo(IAbstractMarkupData[] array, int arrayIndex)
		{
			_Content.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(IAbstractMarkupData item)
		{
			return _Content.Remove(item);
		}

		public abstract IAbstractMarkupDataContainer Split(int splitBeforeItemIndex);

		public virtual IEnumerable<Location> GetLocationsFrom(Location startingFrom)
		{
			return _Content.GetLocationsFrom(startingFrom);
		}

		public virtual Location Find(Predicate<Location> match)
		{
			return _Content.Find(match);
		}

		public virtual Location Find(Location startAt, Predicate<Location> match)
		{
			return _Content.Find(startAt, match);
		}

		public IAbstractMarkupData Find(Predicate<IAbstractMarkupData> match)
		{
			return _Content.Find(match);
		}

		public virtual void MoveAllItemsTo(IAbstractMarkupDataContainer container)
		{
			_Content.MoveAllItemsTo(container);
		}

		public virtual void MoveItemsTo(IAbstractMarkupDataContainer container, int startIndex, int count)
		{
			_Content.MoveItemsTo(container, startIndex, count);
		}

		public void MoveAllItemsTo(IAbstractMarkupDataContainer destinationContainer, int insertAtIndex)
		{
			_Content.MoveAllItemsTo(destinationContainer, insertAtIndex);
		}

		public void MoveItemsTo(IAbstractMarkupDataContainer destinationContainer, int destinationIndex, int startIndex, int count)
		{
			_Content.MoveItemsTo(destinationContainer, destinationIndex, startIndex, count);
		}

		public void ForEachSubItem(Action<IAbstractMarkupData> action)
		{
			_Content.ForEachSubItem(action);
		}
	}
}
