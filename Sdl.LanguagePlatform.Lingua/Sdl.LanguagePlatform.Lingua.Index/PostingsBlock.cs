using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class PostingsBlock
	{
		public static readonly int MinPostingsBlockSize = 1000;

		public static readonly int MaxPostingsBlockSize = MinPostingsBlockSize * 3;

		private int _current;

		private readonly List<int> _keys;

		public bool IsEmpty
		{
			get
			{
				if (_keys != null)
				{
					return _keys.Count == 0;
				}
				return true;
			}
		}

		public int Current
		{
			get
			{
				if (_current >= _keys.Count)
				{
					return -1;
				}
				return _keys[_current];
			}
		}

		public int Id
		{
			get;
		}

		public int Feature
		{
			get;
		}

		public int FirstKey
		{
			get
			{
				if (_keys.Count == 0)
				{
					throw new Exception("No keys");
				}
				return _keys[0];
			}
		}

		public int LastKey
		{
			get
			{
				if (_keys.Count == 0)
				{
					throw new Exception("No keys");
				}
				return _keys[_keys.Count - 1];
			}
		}

		public int Count => _keys.Count;

		public bool IsDirty
		{
			get;
			set;
		}

		public PostingsBlock(int id, int feature, int firstKey, int lastKey, int count, byte[] data)
		{
			Id = id;
			Feature = feature;
			_current = 0;
			_keys = new List<int>();
			UncompressKeys(data);
			if (_keys.Count > 0)
			{
				_ = 0;
				_ = 0;
			}
		}

		private PostingsBlock(int id, int feature, List<int> keys)
		{
			Id = id;
			Feature = feature;
			IsDirty = true;
			_current = 0;
			_keys = keys;
		}

		private byte[] GetCompressedKeys()
		{
			List<byte> list = new List<byte>();
			List<byte> list2 = new List<byte>();
			int num = 0;
			foreach (int key in _keys)
			{
				int num2 = key - num;
				do
				{
					byte item = (byte)(num2 % 128);
					list2.Add(item);
					num2 >>= 7;
				}
				while (num2 > 0);
				list2[0] += 128;
				for (int num3 = list2.Count - 1; num3 >= 0; num3--)
				{
					list.Add(list2[num3]);
				}
				list2.Clear();
				num = key;
			}
			return list.ToArray();
		}

		private void UncompressKeys(IReadOnlyList<byte> data)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			_keys.Clear();
			while (0 < data.Count)
			{
				num <<= 7;
				num += data[0];
				if (data[0] >= 128)
				{
					num -= 128;
					num2 += num;
					_keys.Add(num2);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
		}

		public PostingsBlock Split()
		{
			int count = _keys.Count;
			int num = _keys.Count / 2;
			List<int> range = _keys.GetRange(num, _keys.Count - num);
			_keys.RemoveRange(num, _keys.Count - num);
			IsDirty = true;
			return new PostingsBlock(0, Feature, range);
		}

		public bool Next()
		{
			_current++;
			return _current < _keys.Count;
		}

		public void Reset()
		{
			_current = 0;
		}

		public bool AddKey(int key)
		{
			int num = _keys.BinarySearch(key);
			if (num >= 0)
			{
				return false;
			}
			_keys.Insert(~num, key);
			IsDirty = true;
			return true;
		}

		public bool DeleteKey(int key)
		{
			int num = _keys.BinarySearch(key);
			if (num < 0)
			{
				return false;
			}
			_keys.RemoveAt(num);
			IsDirty = true;
			return true;
		}

		public byte[] GetData()
		{
			return GetCompressedKeys();
		}
	}
}
