using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Sdl.LanguagePlatform.Stat
{
	public class PlainFileWriter : IDisposable
	{
		private readonly DataFileInfo _dataFileInfo;

		private BinaryWriter _dataWriter;

		private IntegerFileWriter _indexWriter;

		private int _offset;

		public string DataFileName => _dataFileInfo.FileName;

		public bool IsOpen => _dataWriter != null;

		public PlainFileWriter(DataLocation location, CultureInfo culture)
		{
			_dataFileInfo = location.FindComponent(DataFileType.PlainFile, culture);
			_indexWriter = new IntegerFileWriter(location.FindComponent(DataFileType.PlainFileIndex, culture).FileName);
		}

		public void Create()
		{
			if (_dataWriter != null)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentAlreadyInUse);
			}
			_dataWriter = new BinaryWriter(File.Create(_dataFileInfo.FileName));
			_indexWriter.Create();
			_offset = 0;
		}

		public void Write(string plain)
		{
			if (plain == null)
			{
				throw new ArgumentNullException("plain");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(plain);
			_dataWriter.Write(bytes.Length);
			_dataWriter.Write(bytes);
			_indexWriter.Write(_offset);
			_offset += 4 + bytes.Length;
		}

		public void Close()
		{
			if (_dataWriter == null)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentNotOpen);
			}
			_dataWriter.Close();
			_indexWriter.Close();
			_dataWriter = null;
			_indexWriter = null;
		}

		public void Dispose()
		{
			_dataWriter?.BaseStream.Dispose();
			_indexWriter?.Dispose();
		}
	}
}
