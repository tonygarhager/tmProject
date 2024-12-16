using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class TokenFileReader2 : IDisposable
	{
		private readonly IntegerFileReader _dataReader;

		private int[] _index;

		private readonly string _indexFileName;

		public bool Exists => _dataReader.Exists;

		public bool IsOpen => _dataReader.IsOpen;

		public int Segments => _index.Length;

		public int Tokens
		{
			get;
			private set;
		}

		public TokenFileReader2(DataLocation2 location, CultureInfo culture)
		{
			_dataReader = new IntegerFileReader(location.ExpectComponent(DataFileType.TokenFile, culture).FileName);
			_indexFileName = location.ExpectComponent(DataFileType.TokenFileIndex, culture).FileName;
		}

		public IntSegment GetSegmentAt(int position)
		{
			if (!IsOpen)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentNotOpen);
			}
			if (position < 0 || position >= _index.Length)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			GetSegmentRange(position, out int from, out int upto);
			IntSegment intSegment = new IntSegment();
			for (int i = from; i < upto; i++)
			{
				intSegment.Elements.Add(_dataReader[i]);
			}
			return intSegment;
		}

		public int GetTokenAt(int position)
		{
			if (!IsOpen)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentNotOpen);
			}
			if (position < 0 || position >= _dataReader.Items)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			return _dataReader[position];
		}

		public void Open()
		{
			if (IsOpen)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentAlreadyInUse);
			}
			_dataReader.Open();
			_index = IntegerFileReader.Load(_indexFileName);
			Tokens = _dataReader.Items;
		}

		public void Close()
		{
			if (IsOpen)
			{
				_dataReader.Close();
				_index = null;
			}
		}

		public void GetSegmentRange(int segment, out int from, out int upto)
		{
			if (segment < 0 || segment >= _index.Length)
			{
				throw new ArgumentOutOfRangeException("segment");
			}
			from = _index[segment];
			upto = ((segment == _index.Length - 1) ? Tokens : _index[segment + 1]);
		}

		public void Dispose()
		{
			if (IsOpen)
			{
				Close();
			}
			_dataReader.Dispose();
			_index = null;
		}
	}
}
