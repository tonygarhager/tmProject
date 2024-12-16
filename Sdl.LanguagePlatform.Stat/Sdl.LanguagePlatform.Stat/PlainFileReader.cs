using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.LanguagePlatform.Stat
{
	internal class PlainFileReader : IDisposable
	{
		private readonly DataFileInfo _dataFileInfo;

		private IntegerFileReader _indexFile;

		private BinaryReader _dataReader;

		public bool Exists
		{
			get
			{
				if (_dataFileInfo.Exists)
				{
					return _indexFile.Exists;
				}
				return false;
			}
		}

		public bool IsOpen => _dataReader != null;

		public string this[int position] => GetString(position);

		public int Items => _indexFile.Items;

		public PlainFileReader(DataLocation location, CultureInfo culture)
		{
			_dataFileInfo = location.ExpectComponent(DataFileType.PlainFile, culture);
			_indexFile = new IntegerFileReader(location.ExpectComponent(DataFileType.PlainFileIndex, culture).FileName, -1);
		}

		public string GetString(int position)
		{
			if (!IsOpen)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentNotOpen);
			}
			if (position < 0 || position >= _indexFile.Items)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			_dataReader.BaseStream.Seek(_indexFile[position], SeekOrigin.Begin);
			int count = _dataReader.ReadInt32();
			byte[] bytes = _dataReader.ReadBytes(count);
			return Encoding.UTF8.GetString(bytes);
		}

		public void Open()
		{
			if (_dataReader != null)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentAlreadyInUse);
			}
			_indexFile.Open();
			FileStream input = File.OpenRead(_dataFileInfo.FileName);
			_dataReader = new BinaryReader(input);
		}

		public void Close()
		{
			if (_dataReader != null)
			{
				_dataReader.Close();
				_dataReader = null;
			}
			if (_indexFile != null)
			{
				_indexFile.Close();
				_indexFile.Dispose();
				_indexFile = null;
			}
		}

		public void Dispose()
		{
			if (_dataReader?.BaseStream != null)
			{
				Close();
			}
			_indexFile?.Dispose();
			_indexFile = null;
		}
	}
}
