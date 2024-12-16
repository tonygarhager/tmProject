using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class TagPair : AbstractTag, ITagPair, IAbstractTag, IAbstractDataContent, IAbstractMarkupData, ICloneable, ISupportsUniqueId, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable
	{
		private IEndTagProperties _EndTagProperties;

		private IRevisionProperties _StartTagRevision;

		private IRevisionProperties _EndTagRevision;

		private MarkupDataContainer _Content;

		private bool _IsStartTagGhost;

		private bool _IsEndTagGhost;

		public IAbstractMarkupData this[int index]
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

		public int Count => _Content.Count;

		bool ICollection<IAbstractMarkupData>.IsReadOnly => ((ICollection<IAbstractMarkupData>)_Content).IsReadOnly;

		public bool CanBeSplit => _Content.CanBeSplit;

		public IEnumerable<Location> Locations => _Content.Locations;

		public IEnumerable<IAbstractMarkupData> AllSubItems => _Content.AllSubItems;

		public IStartTagProperties StartTagProperties
		{
			get
			{
				return (IStartTagProperties)TagProperties;
			}
			set
			{
				TagProperties = value;
			}
		}

		public IEndTagProperties EndTagProperties
		{
			get
			{
				return _EndTagProperties;
			}
			set
			{
				_EndTagProperties = value;
			}
		}

		public bool IsStartTagGhost
		{
			get
			{
				return _IsStartTagGhost;
			}
			set
			{
				_IsStartTagGhost = value;
			}
		}

		public bool IsEndTagGhost
		{
			get
			{
				return _IsEndTagGhost;
			}
			set
			{
				_IsEndTagGhost = value;
			}
		}

		public IRevisionProperties StartTagRevisionProperties
		{
			get
			{
				return _StartTagRevision;
			}
			set
			{
				_StartTagRevision = value;
			}
		}

		public IRevisionProperties EndTagRevisionProperties
		{
			get
			{
				return _EndTagRevision;
			}
			set
			{
				_EndTagRevision = value;
			}
		}

		public TagPair()
		{
			_Content = new MarkupDataContainer(this);
		}

		public TagPair(params IAbstractMarkupData[] markupData)
		{
			_Content = new MarkupDataContainer(this);
			ReadMarkupData(markupData);
		}

		protected TagPair(TagPair other)
			: base(other)
		{
			_EndTagProperties = other._EndTagProperties;
			_Content = (MarkupDataContainer)other._Content.Clone();
			_Content.PublicContainer = this;
			_IsStartTagGhost = other._IsStartTagGhost;
			_IsEndTagGhost = other._IsEndTagGhost;
		}

		protected TagPair(TagPair other, int splitBeforeItemIndex)
			: base(other)
		{
			StartTagProperties = other.StartTagProperties;
			_EndTagProperties = other._EndTagProperties;
			_Content = (MarkupDataContainer)other._Content.Split(splitBeforeItemIndex);
			_Content.PublicContainer = this;
			_IsStartTagGhost = other._IsStartTagGhost;
			_IsEndTagGhost = other._IsEndTagGhost;
		}

		public static TagPair Create(params object[] markupData)
		{
			TagPair tagPair = new TagPair();
			for (int i = 0; i < markupData.Length; i++)
			{
				tagPair.Add((IAbstractMarkupData)markupData[i]);
			}
			return tagPair;
		}

		public static TagPair Create()
		{
			return new TagPair();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			TagPair tagPair = (TagPair)obj;
			if (!base.Equals((object)tagPair))
			{
				return false;
			}
			if (_EndTagProperties == null != (tagPair._EndTagProperties == null))
			{
				return false;
			}
			if (_EndTagProperties != null && !_EndTagProperties.Equals(tagPair._EndTagProperties))
			{
				return false;
			}
			if (_StartTagRevision == null != (tagPair._StartTagRevision == null))
			{
				return false;
			}
			if (_StartTagRevision != null && !_StartTagRevision.Equals(tagPair._StartTagRevision))
			{
				return false;
			}
			if (_EndTagRevision == null != (tagPair._EndTagRevision == null))
			{
				return false;
			}
			if (_EndTagRevision != null && !_EndTagRevision.Equals(tagPair._EndTagRevision))
			{
				return false;
			}
			if (_Content == null != (tagPair._Content == null))
			{
				return false;
			}
			if (_Content != null && !_Content.Equals(tagPair._Content))
			{
				return false;
			}
			if (_IsStartTagGhost != tagPair._IsStartTagGhost)
			{
				return false;
			}
			if (_IsEndTagGhost != tagPair._IsEndTagGhost)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_EndTagProperties != null) ? _EndTagProperties.GetHashCode() : 0) ^ ((_StartTagRevision != null) ? _StartTagRevision.GetHashCode() : 0) ^ ((_EndTagRevision != null) ? _EndTagRevision.GetHashCode() : 0) ^ ((_Content != null) ? _Content.GetHashCode() : 0) ^ _IsStartTagGhost.GetHashCode() ^ _IsEndTagGhost.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (StartTagProperties != null)
			{
				stringBuilder.Append(StartTagProperties.ToString());
			}
			stringBuilder.Append(_Content.ToString());
			if (EndTagProperties != null)
			{
				stringBuilder.Append(EndTagProperties.ToString());
			}
			return stringBuilder.ToString();
		}

		private void ReadMarkupData(IList<IAbstractMarkupData> markupDataList)
		{
			foreach (IAbstractMarkupData markupData in markupDataList)
			{
				_Content.Add(markupData);
			}
		}

		public override object Clone()
		{
			return new TagPair(this);
		}

		public override void AcceptVisitor(IMarkupDataVisitor visitor)
		{
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.VisitTagPair(this);
		}

		public IEnumerator<IAbstractMarkupData> GetEnumerator()
		{
			return _Content.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Content.GetEnumerator();
		}

		public int IndexOf(IAbstractMarkupData item)
		{
			return _Content.IndexOf(item);
		}

		public void Insert(int index, IAbstractMarkupData item)
		{
			_Content.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_Content.RemoveAt(index);
		}

		public void Add(IAbstractMarkupData item)
		{
			_Content.Add(item);
		}

		public void Clear()
		{
			_Content.Clear();
		}

		public bool Contains(IAbstractMarkupData item)
		{
			return _Content.Contains(item);
		}

		public void CopyTo(IAbstractMarkupData[] array, int arrayIndex)
		{
			_Content.CopyTo(array, arrayIndex);
		}

		public bool Remove(IAbstractMarkupData item)
		{
			return _Content.Remove(item);
		}

		public IAbstractMarkupDataContainer Split(int splitBeforeItemIndex)
		{
			return new TagPair(this, splitBeforeItemIndex);
		}

		public IEnumerable<Location> GetLocationsFrom(Location startingFrom)
		{
			return _Content.GetLocationsFrom(startingFrom);
		}

		public Location Find(Predicate<Location> match)
		{
			return _Content.Find(match);
		}

		public Location Find(Location startAt, Predicate<Location> match)
		{
			return _Content.Find(startAt, match);
		}

		public void MoveAllItemsTo(IAbstractMarkupDataContainer container)
		{
			_Content.MoveAllItemsTo(container);
		}

		public void MoveItemsTo(IAbstractMarkupDataContainer container, int startIndex, int count)
		{
			_Content.MoveItemsTo(container, startIndex, count);
		}

		public IAbstractMarkupData Find(Predicate<IAbstractMarkupData> match)
		{
			return _Content.Find(match);
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
