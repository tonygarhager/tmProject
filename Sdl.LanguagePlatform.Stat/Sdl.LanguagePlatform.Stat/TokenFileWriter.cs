using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class TokenFileWriter
	{
		private readonly IntegerFileWriter _dataWriter;

		private readonly IntegerFileWriter _indexWriter;

		private int _offset;

		public bool IsOpen => _dataWriter.IsOpen;

		public TokenFileWriter(DataLocation location, CultureInfo culture)
		{
			_dataWriter = new IntegerFileWriter(location.FindComponent(DataFileType.TokenFile, culture).FileName);
			_indexWriter = new IntegerFileWriter(location.FindComponent(DataFileType.TokenFileIndex, culture).FileName);
		}

		public void Create()
		{
			_dataWriter.Create();
			_indexWriter.Create();
			_offset = 0;
		}

		public void Write(IntSegment sentence)
		{
			if (sentence == null || sentence.Count == 0)
			{
				throw new ArgumentNullException("sentence");
			}
			foreach (int element in sentence.Elements)
			{
				_dataWriter.Write(element);
			}
			_indexWriter.Write(_offset);
			_offset += sentence.Elements.Count;
		}

		public void Close()
		{
			_dataWriter.Close();
			_indexWriter.Close();
		}

		public void Dispose()
		{
			_dataWriter?.Dispose();
			_indexWriter?.Dispose();
		}
	}
}
