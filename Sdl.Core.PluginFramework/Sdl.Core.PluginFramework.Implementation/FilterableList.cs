using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.Core.PluginFramework.Implementation
{
	internal class FilterableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private class FilteredList<S> : IFilteredList<S>, IList<S>, ICollection<S>, IEnumerable<S>, IEnumerable, IDisposable
		{
			private FilterableList<S> _parentList;

			private IListFilter<S> _filter;

			private List<S> _lazyInnerList;

			private List<int> _lazyParentIndices;

			private List<S> InnerList
			{
				get
				{
					EnsureInnerList();
					return _lazyInnerList;
				}
			}

			private List<int> ParentIndices
			{
				get
				{
					EnsureInnerList();
					return _lazyParentIndices;
				}
			}

			public S this[int index]
			{
				get
				{
					return InnerList[index];
				}
				set
				{
					_parentList[GetParentIndex(index)] = value;
				}
			}

			public int Count => InnerList.Count;

			public bool IsReadOnly => false;

			public FilteredList(FilterableList<S> parentList, IListFilter<S> filter)
			{
				_parentList = parentList;
				_filter = filter;
				_parentList.ContentsChanged += _parentList_ContentsChanged;
			}

			private void _parentList_ContentsChanged(object sender, EventArgs e)
			{
				_lazyInnerList = null;
			}

			private void EnsureInnerList()
			{
				if (_lazyInnerList != null)
				{
					return;
				}
				_lazyInnerList = new List<S>();
				_lazyParentIndices = new List<int>();
				for (int i = 0; i < _parentList.Count; i++)
				{
					S item = _parentList[i];
					if (_filter.ShouldInclude(item))
					{
						_lazyInnerList.Add(item);
						_lazyParentIndices.Add(i);
					}
				}
			}

			public void Refresh()
			{
				_lazyInnerList = null;
			}

			public int IndexOf(S item)
			{
				return InnerList.IndexOf(item);
			}

			public void Insert(int index, S item)
			{
				_parentList.Insert(GetParentIndex(index), item);
			}

			public void RemoveAt(int index)
			{
				_parentList.RemoveAt(GetParentIndex(index));
			}

			public void Add(S item)
			{
				_parentList.Add(item);
			}

			public void Clear()
			{
				List<int> parentIndices = ParentIndices;
				for (int num = parentIndices.Count - 1; num >= 0; num--)
				{
					_parentList.RemoveAt(parentIndices[num]);
				}
			}

			public bool Contains(S item)
			{
				return InnerList.Contains(item);
			}

			public void CopyTo(S[] array, int arrayIndex)
			{
				InnerList.CopyTo(array, arrayIndex);
			}

			public bool Remove(S item)
			{
				return _parentList.Remove(item);
			}

			public IEnumerator<S> GetEnumerator()
			{
				return InnerList.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable)InnerList).GetEnumerator();
			}

			public void Dispose()
			{
				_parentList.ContentsChanged -= _parentList_ContentsChanged;
				GC.SuppressFinalize(this);
			}

			private int GetParentIndex(int childIndex)
			{
				if (childIndex >= 0 && childIndex < InnerList.Count)
				{
					return ParentIndices[childIndex];
				}
				if (childIndex == InnerList.Count)
				{
					return _parentList.Count;
				}
				throw new ArgumentOutOfRangeException("childIndex");
			}
		}

		private List<T> _innerList;

		public T this[int index]
		{
			get
			{
				return _innerList[index];
			}
			set
			{
				_innerList[index] = value;
				OnContentsChanged();
			}
		}

		public int Count => _innerList.Count;

		public bool IsReadOnly => false;

		private event EventHandler ContentsChanged;

		public FilterableList()
		{
			_innerList = new List<T>();
		}

		public IFilteredList<T> GetFilteredList(IListFilter<T> filter)
		{
			return new FilteredList<T>(this, filter);
		}

		public int IndexOf(T item)
		{
			return _innerList.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_innerList.Insert(index, item);
			OnContentsChanged();
		}

		public void RemoveAt(int index)
		{
			_innerList.RemoveAt(index);
			OnContentsChanged();
		}

		public void Add(T item)
		{
			_innerList.Add(item);
			OnContentsChanged();
		}

		public void Clear()
		{
			_innerList.Clear();
			OnContentsChanged();
		}

		public bool Contains(T item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			bool result = _innerList.Remove(item);
			OnContentsChanged();
			return result;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_innerList).GetEnumerator();
		}

		private void OnContentsChanged()
		{
			if (this.ContentsChanged != null)
			{
				this.ContentsChanged(this, EventArgs.Empty);
			}
		}
	}
}
