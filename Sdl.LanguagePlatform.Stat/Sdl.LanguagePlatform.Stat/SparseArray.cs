using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat
{
	[Serializable]
	public class SparseArray<T>
	{
		private readonly List<int> _keys;

		private readonly List<T> _values;

		public T this[int key]
		{
			get
			{
				int num = _keys.BinarySearch(key);
				if (num < 0)
				{
					return default(T);
				}
				return _values[num];
			}
			set
			{
				int num = _keys.BinarySearch(key);
				if (num >= 0)
				{
					_values[num] = value;
					return;
				}
				_keys.Insert(~num, key);
				_values.Insert(~num, value);
			}
		}

		public int Count => _keys.Count;

		public IList<int> Keys => _keys;

		public SparseArray()
		{
			_keys = new List<int>();
			_values = new List<T>();
		}

		public bool TryGetValue(int key, out T value)
		{
			int num = _keys.BinarySearch(key);
			if (num >= 0)
			{
				value = _values[num];
				return true;
			}
			value = default(T);
			return false;
		}

		public bool Equals(SparseArray<T> other)
		{
			if (_keys.Count != other._keys.Count)
			{
				return false;
			}
			for (int i = 0; i < _keys.Count; i++)
			{
				if (_keys[i] != other._keys[i])
				{
					return false;
				}
				if (!_values[i].Equals(other._values[i]))
				{
					return false;
				}
			}
			return true;
		}

		public void Clear()
		{
			_keys.Clear();
			_values.Clear();
		}

		public void Do(Action<T> action)
		{
			foreach (T value in _values)
			{
				action(value);
			}
		}

		public void Do(KeyValueAction<T> action)
		{
			for (int i = 0; i < _values.Count; i++)
			{
				action(_keys[i], _values[i]);
			}
		}

		public int KeyAt(int p)
		{
			return _keys[p];
		}

		public T ValueAt(int p)
		{
			return _values[p];
		}

		public void DeleteIf(Predicate<T> predicate)
		{
			for (int num = _values.Count - 1; num >= 0; num--)
			{
				if (predicate(_values[num]))
				{
					_keys.RemoveAt(num);
					_values.RemoveAt(num);
				}
			}
		}

		public bool HasValue(int key)
		{
			return _keys.BinarySearch(key) >= 0;
		}

		public void Remove(int key)
		{
			int num = _keys.BinarySearch(key);
			if (num >= 0)
			{
				_keys.RemoveAt(num);
				_values.RemoveAt(num);
			}
		}
	}
}
