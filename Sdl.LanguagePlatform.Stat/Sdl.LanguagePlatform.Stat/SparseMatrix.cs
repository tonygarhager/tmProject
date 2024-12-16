using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat
{
	[Serializable]
	public class SparseMatrix<T>
	{
		private readonly SparseArray<SparseArray<T>> _data;

		public IList<int> RowKeys => _data.Keys;

		public T this[int row, int column]
		{
			get
			{
				if (!_data.HasValue(row))
				{
					return default(T);
				}
				SparseArray<T> sparseArray = _data[row];
				if (sparseArray.HasValue(column))
				{
					return sparseArray[column];
				}
				return default(T);
			}
			set
			{
				SparseArray<T> sparseArray;
				if (!_data.HasValue(row))
				{
					sparseArray = new SparseArray<T>();
					_data[row] = sparseArray;
				}
				else
				{
					sparseArray = _data[row];
				}
				sparseArray[column] = value;
			}
		}

		public int RowCount => _data.Count;

		public SparseMatrix()
		{
			_data = new SparseArray<SparseArray<T>>();
		}

		public SparseMatrix(SparseMatrix<T> other)
		{
			_data = other._data;
		}

		public IList<int> ColumnKeys(int row)
		{
			return _data[row].Keys;
		}

		public bool HasValue(int row, int column)
		{
			if (_data.HasValue(row))
			{
				return _data[row].HasValue(column);
			}
			return false;
		}

		public bool TryGetValue(int row, int column, out T value)
		{
			if (_data.TryGetValue(row, out SparseArray<T> value2))
			{
				return value2.TryGetValue(column, out value);
			}
			value = default(T);
			return false;
		}

		public bool Equals(SparseMatrix<T> other)
		{
			if (RowCount != other.RowCount)
			{
				return false;
			}
			for (int i = 0; i < RowCount; i++)
			{
				if (KeyAt(i) != other.KeyAt(i))
				{
					return false;
				}
				SparseArray<T> sparseArray = ColumnAt(i);
				SparseArray<T> other2 = other.ColumnAt(i);
				if (!sparseArray.Equals(other2))
				{
					return false;
				}
			}
			return true;
		}

		public int KeyAt(int p)
		{
			return _data.KeyAt(p);
		}

		public void Clear()
		{
			_data.Clear();
		}

		public SparseArray<T> ColumnAt(int p)
		{
			return _data.ValueAt(p);
		}

		public void DeleteIf(Predicate<T> predicate)
		{
			for (int num = _data.Count - 1; num >= 0; num--)
			{
				_data.ValueAt(num).DeleteIf(predicate);
			}
			_data.DeleteIf((SparseArray<T> e) => e == null || e.Count == 0);
		}
	}
}
