using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.IO
{
	internal class IllegalCharacterFilter : TextReader
	{
		private TextReader _WrappedReader;

		private const int _BLOCK_SIZE = 4096;

		private const int _BUFFER_SIZE = 24576;

		private char[] _Buffer;

		private int _BufferUsed;

		private int _BufferStart;

		private bool _EOF;

		private char[] _Block;

		public IllegalCharacterFilter(TextReader wrappedReader)
		{
			_WrappedReader = wrappedReader;
			_Buffer = new char[24576];
			_BufferUsed = (_BufferStart = 0);
			_Block = new char[4096];
		}

		public override void Close()
		{
			base.Close();
			_WrappedReader?.Close();
		}

		private void ReadFromWrappedStream()
		{
			_BufferUsed = 0;
			int num = _WrappedReader.Read(_Block, 0, 4096);
			_EOF = (num < 4096);
			for (int i = 0; i < num; i++)
			{
				char c = _Block[i];
				if ((c >= ' ' || c == '\t' || c == '\n' || c == '\r') && c != '\uffff')
				{
					_Buffer[_BufferUsed++] = c;
					continue;
				}
				int num2 = c;
				string text = num2.ToString("x", CultureInfo.InvariantCulture);
				_Buffer[_BufferUsed++] = '&';
				_Buffer[_BufferUsed++] = '#';
				_Buffer[_BufferUsed++] = 'x';
				for (int j = 0; j < text.Length; j++)
				{
					_Buffer[_BufferUsed++] = text[j];
				}
				_Buffer[_BufferUsed++] = ';';
			}
			_ = _BufferUsed;
			_ = 4096;
			_BufferStart = 0;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_WrappedReader?.Dispose();
			_Buffer = null;
			_Block = null;
		}

		public override int Peek()
		{
			return -1;
		}

		public override int Read()
		{
			if (_BufferStart == _BufferUsed)
			{
				if (_EOF)
				{
					return -1;
				}
				ReadFromWrappedStream();
			}
			if (_BufferStart < _BufferUsed)
			{
				return _Buffer[_BufferStart++];
			}
			return -1;
		}

		public override string ReadLine()
		{
			return null;
		}

		public override int Read(char[] buffer, int index, int count)
		{
			int num = 0;
			while (count > 0)
			{
				if (_BufferStart < _BufferUsed)
				{
					int num2 = Math.Min(count, _BufferUsed - _BufferStart);
					for (int i = 0; i < num2; i++)
					{
						buffer[index++] = _Buffer[_BufferStart++];
					}
					count -= num2;
					num += num2;
				}
				else
				{
					if (_EOF)
					{
						return num;
					}
					ReadFromWrappedStream();
				}
			}
			return num;
		}

		public override string ReadToEnd()
		{
			throw new NotImplementedException("Not supposed to be called - see source code comments");
		}

		public override int ReadBlock(char[] buffer, int index, int count)
		{
			throw new NotImplementedException("Not supposed to be called - see source code comments");
		}
	}
}
