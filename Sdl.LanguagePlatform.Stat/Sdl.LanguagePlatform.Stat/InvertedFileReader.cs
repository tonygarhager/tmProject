using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class InvertedFileReader : IDisposable
	{
		private readonly IntegerFileReader _dataReader;

		private int[] _index;

		private readonly string _indexFileName;

		private int _dataItems;

		public bool Exists => _dataReader.Exists;

		public bool IsOpen => _index != null;

		public InvertedFileReader(DataLocation location, CultureInfo culture)
		{
			_dataReader = new IntegerFileReader(location.GetComponentFileName(DataFileType.TokenInvertedFile, culture));
			_indexFileName = location.GetComponentFileName(DataFileType.TokenInvertedFileIndex, culture);
		}

		public void Open()
		{
			_dataReader.Open();
			_dataItems = _dataReader.Items;
			_index = IntegerFileReader.Load(_indexFileName);
		}

		public int GetPostingCount(int id)
		{
			if (id < 0 || id >= _index.Length)
			{
				throw new ArgumentOutOfRangeException("id");
			}
			if (id == _index.Length - 1)
			{
				return (_dataItems - _index[id]) / 2;
			}
			return (_index[id + 1] - _index[id]) / 2;
		}

		public Posting GetTokenPostingAt(int id, int p)
		{
			if (p < 0 || p >= GetPostingCount(id))
			{
				throw new ArgumentOutOfRangeException("p");
			}
			Posting result = default(Posting);
			result.Segment = _dataReader[_index[id] + p * 2];
			result.Position = _dataReader[_index[id] + p * 2 + 1];
			return result;
		}

		public List<int> GetSegmentPostings(int id)
		{
			int postingCount = GetPostingCount(id);
			if (postingCount == 0)
			{
				return null;
			}
			List<int> list = new List<int>();
			int num = -1;
			for (int i = 0; i < postingCount; i++)
			{
				int num2 = _dataReader[_index[id] + i * 2];
				if (num2 > num)
				{
					list.Add(num2);
					num = num2;
				}
			}
			return list;
		}

		public void Close()
		{
			_dataReader.Close();
			_index = null;
		}

		public void Dispose()
		{
			_dataReader.Dispose();
			_index = null;
		}
	}
}
