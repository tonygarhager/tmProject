using System;
using System.Globalization;

namespace Sdl.LanguagePlatform.Stat
{
	public class TokenFileWriter2 : IDisposable
	{
		private IntegerFileWriter _dataWriter;

		private IntegerFileWriter _indexWriter;

		private int _offset;

		public virtual bool IsOpen => _dataWriter.IsOpen;

		public TokenFileWriter2(DataLocation2 location, CultureInfo culture)
		{
			_dataWriter = new IntegerFileWriter(location.FindComponent(DataFileType.TokenFile, culture).FileName);
			_indexWriter = new IntegerFileWriter(location.FindComponent(DataFileType.TokenFileIndex, culture).FileName);
		}

		public virtual void Create()
		{
			_dataWriter.Create();
			_indexWriter.Create();
			_offset = 0;
		}

		public virtual void Write(IntSegment sentence)
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

		public virtual void Close()
		{
			_dataWriter.Close();
			_indexWriter.Close();
		}

		public virtual void Dispose()
		{
			_dataWriter?.Dispose();
			_indexWriter?.Dispose();
			_dataWriter = null;
			_indexWriter = null;
		}
	}
}
