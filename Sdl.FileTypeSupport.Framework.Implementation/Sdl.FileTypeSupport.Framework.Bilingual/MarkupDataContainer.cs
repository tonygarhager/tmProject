using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class MarkupDataContainer : IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, ICloneable
	{
		[NonSerialized]
		private int _uniqueId;

		protected List<IAbstractMarkupData> _Content = new List<IAbstractMarkupData>();

		protected internal IAbstractMarkupDataContainer _PublicContainer;

		public IAbstractMarkupDataContainer PublicContainer
		{
			get
			{
				return _PublicContainer;
			}
			set
			{
				if (_PublicContainer != value)
				{
					_PublicContainer = value;
					IAbstractMarkupData[] array = _Content.ToArray();
					_Content.Clear();
					IAbstractMarkupData[] array2 = array;
					foreach (IAbstractMarkupData abstractMarkupData in array2)
					{
						abstractMarkupData.Parent = null;
						_PublicContainer.Add(abstractMarkupData);
					}
				}
			}
		}

		public IEnumerable<IAbstractMarkupData> AllSubItems
		{
			get
			{
				foreach (IAbstractMarkupData node in _Content)
				{
					yield return node;
					IAbstractMarkupDataContainer abstractMarkupDataContainer = node as IAbstractMarkupDataContainer;
					if (abstractMarkupDataContainer != null)
					{
						foreach (IAbstractMarkupData allSubItem in abstractMarkupDataContainer.AllSubItems)
						{
							yield return allSubItem;
						}
					}
				}
			}
		}

		public virtual bool CanBeSplit => true;

		public virtual IEnumerable<Location> Locations
		{
			get
			{
				for (int i = 0; i < _Content.Count; i++)
				{
					yield return new Location(new LevelLocation(_PublicContainer, i));
					IAbstractMarkupDataContainer abstractMarkupDataContainer = _Content[i] as IAbstractMarkupDataContainer;
					if (abstractMarkupDataContainer != null)
					{
						foreach (Location location in abstractMarkupDataContainer.Locations)
						{
							location.Levels.Insert(0, new LevelLocation(_PublicContainer, i));
							yield return location;
						}
					}
				}
				yield return new Location(new LevelLocation(_PublicContainer, _Content.Count));
			}
		}

		public virtual IAbstractMarkupData this[int index]
		{
			get
			{
				CheckIndexValue(index);
				return _Content[index];
			}
			set
			{
				RemoveAt(index);
				Insert(index, value);
			}
		}

		public virtual int Count => _Content.Count;

		bool ICollection<IAbstractMarkupData>.IsReadOnly => ((ICollection<IAbstractMarkupData>)_Content).IsReadOnly;

		[XmlIgnore]
		int ISupportsUniqueId.UniqueId
		{
			get
			{
				return _uniqueId;
			}
			set
			{
				_uniqueId = value;
			}
		}

		public MarkupDataContainer()
		{
			_PublicContainer = this;
		}

		public MarkupDataContainer(IAbstractMarkupDataContainer publicContainer)
		{
			_PublicContainer = publicContainer;
		}

		public MarkupDataContainer(MarkupDataContainer other)
		{
			_PublicContainer = this;
			foreach (IAbstractMarkupData item in other)
			{
				Add((IAbstractMarkupData)item.Clone());
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			MarkupDataContainer markupDataContainer = (MarkupDataContainer)obj;
			if (_Content == null != (markupDataContainer._Content == null))
			{
				return false;
			}
			if (_Content != null)
			{
				if (_Content.Count != markupDataContainer._Content.Count)
				{
					return false;
				}
				for (int i = 0; i < _Content.Count; i++)
				{
					IAbstractMarkupData abstractMarkupData = _Content[i];
					IAbstractMarkupData abstractMarkupData2 = markupDataContainer._Content[i];
					if (abstractMarkupData == null != (abstractMarkupData2 == null))
					{
						return false;
					}
					if (abstractMarkupData != null && !abstractMarkupData.Equals(abstractMarkupData2))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (_Content != null)
			{
				num ^= _Content.Count;
				{
					foreach (IAbstractMarkupData item in _Content)
					{
						if (item != null)
						{
							num ^= item.GetHashCode();
						}
					}
					return num;
				}
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IAbstractMarkupData item in _Content)
			{
				stringBuilder.Append(item.ToString());
			}
			return stringBuilder.ToString();
		}

		public virtual IAbstractMarkupDataContainer Split(int splitBeforeItemIndex)
		{
			if (splitBeforeItemIndex < 0 || splitBeforeItemIndex > _Content.Count)
			{
				throw new ArgumentOutOfRangeException("splitBeforeItemIndex");
			}
			MarkupDataContainer markupDataContainer = new MarkupDataContainer();
			MoveItemsTo(markupDataContainer, splitBeforeItemIndex, _Content.Count - splitBeforeItemIndex);
			return markupDataContainer;
		}

		public virtual IEnumerable<Location> GetLocationsFrom(Location startingFrom)
		{
			if (startingFrom == null)
			{
				throw new ArgumentNullException("startingFrom");
			}
			if (!startingFrom.IsValid)
			{
				throw new ArgumentException(StringResources.MarkupDataContainer_InvalidStartLocationError);
			}
			Location startLocation = (Location)startingFrom.Clone();
			while (startLocation.Levels.Count > 0 && startLocation.Levels[0].Parent != _PublicContainer)
			{
				startLocation.Levels.RemoveAt(0);
			}
			if (startLocation.Depth == 0)
			{
				throw new ArgumentOutOfRangeException("startingFrom", StringResources.MarkupDataContainer_WrongCollectionError);
			}
			for (int i = startLocation.Levels[0].Index; i < _Content.Count; i++)
			{
				if (startLocation.Depth == 1)
				{
					yield return new Location(new LevelLocation(_PublicContainer, i));
					continue;
				}
				IAbstractMarkupDataContainer abstractMarkupDataContainer = _Content[i] as IAbstractMarkupDataContainer;
				if (abstractMarkupDataContainer != null)
				{
					foreach (Location item in abstractMarkupDataContainer.GetLocationsFrom(startLocation))
					{
						item.Levels.Insert(0, new LevelLocation(_PublicContainer, i));
						yield return item;
					}
				}
			}
			yield return new Location(new LevelLocation(_PublicContainer, _Content.Count));
		}

		public virtual Location Find(Predicate<Location> match)
		{
			foreach (Location location in Locations)
			{
				if (match(location))
				{
					return location;
				}
			}
			return null;
		}

		public virtual Location Find(Location startAt, Predicate<Location> match)
		{
			foreach (Location item in GetLocationsFrom(startAt))
			{
				if (match(item))
				{
					return item;
				}
			}
			return null;
		}

		public void MoveAllItemsTo(IAbstractMarkupDataContainer destinationContainer)
		{
			MoveAllItemsTo(destinationContainer, destinationContainer.Count);
		}

		public void MoveItemsTo(IAbstractMarkupDataContainer destinationContainer, int startIndex, int count)
		{
			MoveItemsTo(destinationContainer, destinationContainer.Count, startIndex, count);
		}

		public IAbstractMarkupData Find(Predicate<IAbstractMarkupData> match)
		{
			foreach (IAbstractMarkupData allSubItem in AllSubItems)
			{
				if (match(allSubItem))
				{
					return allSubItem;
				}
			}
			return null;
		}

		public void MoveAllItemsTo(IAbstractMarkupDataContainer destinationContainer, int insertAtIndex)
		{
			if (destinationContainer == null)
			{
				throw new ArgumentNullException("container");
			}
			if (insertAtIndex < 0 || insertAtIndex > destinationContainer.Count)
			{
				throw new ArgumentOutOfRangeException("insertAtIndex");
			}
			IAbstractMarkupData[] array = _Content.ToArray();
			_Content.Clear();
			IAbstractMarkupData[] array2 = array;
			foreach (IAbstractMarkupData abstractMarkupData in array2)
			{
				abstractMarkupData.Parent = null;
				destinationContainer.Insert(insertAtIndex++, abstractMarkupData);
			}
		}

		public void MoveItemsTo(IAbstractMarkupDataContainer destinationContainer, int destinationIndex, int startIndex, int count)
		{
			if (destinationContainer == null)
			{
				throw new ArgumentNullException("destinationContainer");
			}
			if (count != 0)
			{
				if (startIndex < 0 || startIndex >= _Content.Count)
				{
					throw new ArgumentOutOfRangeException("startIndex");
				}
				if (count < 0 || count > _Content.Count - startIndex)
				{
					throw new ArgumentOutOfRangeException("count");
				}
				if (destinationIndex < 0 || destinationIndex > destinationContainer.Count)
				{
					throw new ArgumentOutOfRangeException("destinationIndex");
				}
				for (int i = 0; i < count; i++)
				{
					IAbstractMarkupData item = _Content[startIndex];
					RemoveAt(startIndex);
					destinationContainer.Insert(destinationIndex++, item);
				}
			}
		}

		public void ForEachSubItem(Action<IAbstractMarkupData> action)
		{
			foreach (IAbstractMarkupData allSubItem in AllSubItems)
			{
				action(allSubItem);
			}
		}

		public virtual int IndexOf(IAbstractMarkupData item)
		{
			int num = 0;
			foreach (IAbstractMarkupData item2 in _Content)
			{
				if (item == item2)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public virtual void Insert(int index, IAbstractMarkupData item)
		{
			if (item.Parent != null)
			{
				throw new FileTypeSupportException(StringResources.MarkupDataContainer_AlreadyInCollectionError);
			}
			_Content.Insert(index, item);
			item.Parent = _PublicContainer;
		}

		public virtual void RemoveAt(int index)
		{
			CheckIndexValue(index);
			IAbstractMarkupData abstractMarkupData = _Content[index];
			_Content.RemoveAt(index);
			if (abstractMarkupData != null)
			{
				abstractMarkupData.Parent = null;
			}
		}

		private void CheckIndexValue(int index)
		{
			if (index < 0 || index >= _Content.Count)
			{
				throw new ArgumentOutOfRangeException("index", string.Format(StringResources.MarkupDataContainer_IndexRangeError, index, _Content.Count));
			}
		}

		public virtual void Add(IAbstractMarkupData item)
		{
			if (item.Parent != null)
			{
				throw new FileTypeSupportException(StringResources.MarkupDataContainer_AlreadyInCollectionError);
			}
			_Content.Add(item);
			item.Parent = _PublicContainer;
		}

		public virtual void Clear()
		{
			IAbstractMarkupData[] array = _Content.ToArray();
			_Content.Clear();
			IAbstractMarkupData[] array2 = array;
			foreach (IAbstractMarkupData abstractMarkupData in array2)
			{
				abstractMarkupData.Parent = null;
			}
		}

		public virtual bool Contains(IAbstractMarkupData item)
		{
			foreach (IAbstractMarkupData item2 in _Content)
			{
				if (item2 == item)
				{
					return true;
				}
			}
			return false;
		}

		public virtual void CopyTo(IAbstractMarkupData[] array, int arrayIndex)
		{
			_Content.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(IAbstractMarkupData item)
		{
			int num = IndexOf(item);
			if (num != -1)
			{
				RemoveAt(num);
				return true;
			}
			return false;
		}

		public virtual IEnumerator<IAbstractMarkupData> GetEnumerator()
		{
			foreach (IAbstractMarkupData item in _Content)
			{
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public virtual object Clone()
		{
			return new MarkupDataContainer(this);
		}
	}
}
