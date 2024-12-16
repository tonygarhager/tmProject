using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class WordAlignmentFileReader : IDisposable
	{
		private readonly DataFileInfo _dataFileInfo;

		private readonly DataFileInfo _indexFileInfo;

		private BinaryReader _dataReader;

		private int _dataFileLength;

		private int[] _index;

		public bool Exists
		{
			get
			{
				if (_dataFileInfo.Exists)
				{
					return _indexFileInfo.Exists;
				}
				return false;
			}
		}

		public bool IsOpen => _dataReader != null;

		public AlignmentTable this[int position] => GetTable(position);

		public int Items
		{
			get;
			private set;
		}

		public WordAlignmentFileReader(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_dataFileInfo = location.ExpectComponent(DataFileType.WordAlignmentFile, srcCulture, trgCulture);
			_indexFileInfo = location.ExpectComponent(DataFileType.WordAlignmentFileIndex, srcCulture, trgCulture);
		}

		public AlignmentTable GetTable(int position)
		{
			if (!IsOpen)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentNotOpen);
			}
			if (position < 0 || position >= Items)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			_dataReader.BaseStream.Seek(_index[position], SeekOrigin.Begin);
			int num = _dataReader.ReadInt32();
			int num2 = _dataReader.ReadInt32();
			if (num <= 0 || num2 <= 0)
			{
				throw new LanguagePlatformException(ErrorCode.CorruptData);
			}
			AlignmentTable alignmentTable = new AlignmentTable(num, num2);
			int num3 = _dataReader.ReadInt32();
			for (int i = 0; i < num3; i++)
			{
				int num4 = _dataReader.ReadInt32();
				int num5 = _dataReader.ReadInt32();
				if (num4 < 0 || num4 >= num || num5 < 0 || num5 >= num2)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData);
				}
				alignmentTable[num4, num5] = true;
			}
			return alignmentTable;
		}

		public void Open()
		{
			if (_dataReader != null)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentAlreadyInUse);
			}
			using (FileStream fileStream = File.OpenRead(_indexFileInfo.FileName))
			{
				int num = (int)fileStream.Length;
				Items = num / 4;
				_index = new int[Items];
				BinaryReader binaryReader = new BinaryReader(fileStream);
				int num2 = -1;
				for (int i = 0; i < Items; i++)
				{
					_index[i] = binaryReader.ReadInt32();
					num2 = _index[i];
				}
				binaryReader.Close();
			}
			FileStream fileStream2 = File.OpenRead(_dataFileInfo.FileName);
			_dataFileLength = (int)fileStream2.Length;
			_dataReader = new BinaryReader(fileStream2);
			if (_index[Items - 1] > _dataFileLength)
			{
				throw new LanguagePlatformException(ErrorCode.CorruptData);
			}
		}

		public void Close()
		{
			if (_dataReader != null)
			{
				_dataReader.Close();
				_dataReader = null;
			}
		}

		public void Dispose()
		{
			if (_dataReader?.BaseStream != null)
			{
				Close();
			}
			_index = null;
			Items = 0;
		}
	}
}
