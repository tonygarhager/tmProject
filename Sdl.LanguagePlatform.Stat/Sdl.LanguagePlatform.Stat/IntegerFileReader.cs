using Sdl.LanguagePlatform.Core;
using System;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class IntegerFileReader : IDisposable
	{
		private BinaryReader _dataReader;

		private int _currentBlock;

		private int[] _buffer;

		private int _fullBlockCount;

		private int _lastBlockItemCount;

		private const int DefaultBufferSize = 16384;

		public string DataFileName
		{
			get;
		}

		public bool IsOpen => _dataReader != null;

		public int BufferSize
		{
			get;
			private set;
		}

		public bool Exists => File.Exists(DataFileName);

		public int this[int position]
		{
			get
			{
				if (_dataReader == null)
				{
					throw new LanguagePlatformException(ErrorCode.DataComponentNotOpen);
				}
				if (position < 0 || position >= Items)
				{
					throw new ArgumentOutOfRangeException("position");
				}
				int num = position / BufferSize;
				int num2 = position % BufferSize;
				if (num != _currentBlock)
				{
					LoadBlock(num);
				}
				return _buffer[num2];
			}
		}

		public int Items
		{
			get;
			private set;
		}

		public IntegerFileReader(string fileName, int bufferSize)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			DataFileName = fileName;
			if (bufferSize < -1)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			Items = -1;
			_currentBlock = -1;
			BufferSize = bufferSize;
		}

		public IntegerFileReader(string fileName)
			: this(fileName, 16384)
		{
		}

		private void LoadBlock(int block)
		{
			if (block < 0 || block > _fullBlockCount)
			{
				throw new ArgumentOutOfRangeException("block");
			}
			_dataReader.BaseStream.Seek(block * BufferSize * 4, SeekOrigin.Begin);
			int num = (block == _fullBlockCount) ? _lastBlockItemCount : BufferSize;
			for (int i = 0; i < num; i++)
			{
				_buffer[i] = _dataReader.ReadInt32();
			}
			_currentBlock = block;
		}

		public void Open()
		{
			if (_dataReader != null)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentAlreadyInUse);
			}
			_dataReader = new BinaryReader(File.OpenRead(DataFileName));
			Items = (int)(_dataReader.BaseStream.Length / 4);
			if (BufferSize == 0)
			{
				BufferSize = 16384;
			}
			else if (BufferSize < 0 || BufferSize > Items)
			{
				BufferSize = Items;
			}
			_fullBlockCount = Items / BufferSize;
			_lastBlockItemCount = Items % BufferSize;
			_buffer = new int[BufferSize];
			LoadBlock(0);
		}

		public void Close()
		{
			if (_dataReader != null)
			{
				_dataReader.Close();
				_dataReader = null;
			}
			Items = -1;
			_currentBlock = -1;
			_buffer = null;
		}

		public void Dispose()
		{
			if (_dataReader?.BaseStream != null)
			{
				Close();
			}
			Items = -1;
			_currentBlock = -1;
			_buffer = null;
		}

		public static int[] Load(string fileName)
		{
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				long length = fileStream.Length;
				int num = (int)length / 4;
				BinaryReader binaryReader = new BinaryReader(fileStream);
				int[] array = new int[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = binaryReader.ReadInt32();
				}
				return array;
			}
		}
	}
}
