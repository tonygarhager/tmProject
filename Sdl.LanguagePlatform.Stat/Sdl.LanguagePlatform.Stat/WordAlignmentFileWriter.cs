using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class WordAlignmentFileWriter
	{
		private readonly DataFileInfo _dataFileInfo;

		private readonly DataFileInfo _indexFileInfo;

		private BinaryWriter _dataWriter;

		private BinaryWriter _indexWriter;

		private int _offset;

		public string DataFileName => _dataFileInfo.FileName;

		public string IndexFileName => _indexFileInfo.FileName;

		public bool IsOpen => _dataWriter != null;

		public WordAlignmentFileWriter(DataLocation location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			_dataFileInfo = location.FindComponent(DataFileType.WordAlignmentFile, srcCulture, trgCulture);
			_indexFileInfo = location.FindComponent(DataFileType.WordAlignmentFileIndex, srcCulture, trgCulture);
		}

		public void Create()
		{
			if (_dataWriter != null)
			{
				throw new LanguagePlatformException(ErrorCode.InternalError);
			}
			_dataWriter = new BinaryWriter(File.Create(_dataFileInfo.FileName));
			_indexWriter = new BinaryWriter(File.Create(_indexFileInfo.FileName));
			_offset = 0;
		}

		public void Write(AlignmentTable table)
		{
			if (_dataWriter == null)
			{
				throw new Exception("Need to Create() file before writing");
			}
			_indexWriter.Write(_offset);
			_dataWriter.Write(BitConverter.GetBytes(table.SourceSegmentLength));
			_dataWriter.Write(BitConverter.GetBytes(table.TargetSegmentLength));
			int num = 0;
			for (int i = 0; i < table.SourceSegmentLength; i++)
			{
				for (int j = 0; j < table.TargetSegmentLength; j++)
				{
					if (table[i, j])
					{
						num++;
					}
				}
			}
			_dataWriter.Write(BitConverter.GetBytes(num));
			_offset += 12;
			for (int k = 0; k < table.SourceSegmentLength; k++)
			{
				for (int l = 0; l < table.TargetSegmentLength; l++)
				{
					if (table[k, l])
					{
						_dataWriter.Write(BitConverter.GetBytes(k));
						_dataWriter.Write(BitConverter.GetBytes(l));
					}
				}
			}
			_offset += 8 * num;
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
			_indexWriter?.BaseStream.Dispose();
		}
	}
}
