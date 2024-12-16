using Sdl.LanguagePlatform.Core;
using System;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class IntegerFileWriter : IDisposable
	{
		private BinaryWriter _dataWriter;

		public string DataFileName
		{
			get;
		}

		public bool IsOpen => _dataWriter != null;

		public IntegerFileWriter(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			DataFileName = fileName;
		}

		public void Create()
		{
			if (_dataWriter != null)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentAlreadyInUse);
			}
			_dataWriter = new BinaryWriter(File.Create(DataFileName));
		}

		public void Write(int i)
		{
			_dataWriter.Write(i);
		}

		public void Close()
		{
			if (_dataWriter == null)
			{
				throw new LanguagePlatformException(ErrorCode.DataComponentNotOpen);
			}
			_dataWriter.Close();
			_dataWriter = null;
		}

		public void Dispose()
		{
			_dataWriter?.BaseStream.Dispose();
		}
	}
}
