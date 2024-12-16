using Sdl.LanguagePlatform.Core;
using System;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal static class SparseMatrixIO
	{
		private const int Version = 2;

		public static void Write(SparseMatrix<int> data, string fileName)
		{
			WriteInternal(data, fileName);
		}

		public static void Write(SparseMatrix<float> data, string fileName)
		{
			WriteInternal(data, fileName);
		}

		public static void Write(SparseMatrix<double> data, string fileName)
		{
			WriteInternal(data, fileName);
		}

		public static void Load(ref SparseMatrix<int> data, string fileName)
		{
			LoadInternal(ref data, fileName);
		}

		public static void Load(ref SparseMatrix<float> data, string fileName)
		{
			LoadInternal(ref data, fileName);
		}

		public static void Load(ref SparseMatrix<double> data, string fileName)
		{
			LoadInternal(ref data, fileName);
		}

		private static void WriteInternal(SparseMatrix<int> data, string fileName)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			using (Stream output = File.Create(fileName))
			{
				BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(20061214);
				for (int i = 0; i < data.RowCount; i++)
				{
					int value = data.KeyAt(i);
					SparseArray<int> sparseArray = data.ColumnAt(i);
					for (int j = 0; j < sparseArray.Count; j++)
					{
						int value2 = sparseArray.KeyAt(j);
						int value3 = sparseArray.ValueAt(j);
						binaryWriter.Write(value);
						binaryWriter.Write(value2);
						binaryWriter.Write(value3);
					}
				}
				binaryWriter.Close();
			}
		}

		private static void LoadInternal(ref SparseMatrix<int> data, string fileName)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			using (Stream stream = File.OpenRead(fileName))
			{
				long length = stream.Length;
				long num = 0L;
				BinaryReader binaryReader = new BinaryReader(stream);
				if (binaryReader.ReadInt32() != 20061214)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData);
				}
				num += 4;
				data.Clear();
				while (num < length)
				{
					int row = binaryReader.ReadInt32();
					int column = binaryReader.ReadInt32();
					int value = binaryReader.ReadInt32();
					num += 8;
					num += 4;
					data[row, column] = value;
				}
				binaryReader.Close();
			}
		}

		private static void WriteInternal(SparseMatrix<double> data, string fileName)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			using (Stream output = File.Create(fileName))
			{
				BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(20061214);
				for (int i = 0; i < data.RowCount; i++)
				{
					int value = data.KeyAt(i);
					SparseArray<double> sparseArray = data.ColumnAt(i);
					for (int j = 0; j < sparseArray.Count; j++)
					{
						int value2 = sparseArray.KeyAt(j);
						double value3 = sparseArray.ValueAt(j);
						binaryWriter.Write(value);
						binaryWriter.Write(value2);
						binaryWriter.Write(value3);
					}
				}
				binaryWriter.Close();
			}
		}

		private static void LoadInternal(ref SparseMatrix<double> data, string fileName)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			using (Stream stream = File.OpenRead(fileName))
			{
				long length = stream.Length;
				long num = 0L;
				BinaryReader binaryReader = new BinaryReader(stream);
				if (binaryReader.ReadInt32() != 20061214)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData);
				}
				num += 4;
				data.Clear();
				while (num < length)
				{
					int row = binaryReader.ReadInt32();
					int column = binaryReader.ReadInt32();
					double value = binaryReader.ReadDouble();
					num += 8;
					num += 8;
					data[row, column] = value;
				}
				binaryReader.Close();
			}
		}

		private static void WriteInternal(SparseMatrix<float> data, string fileName)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			using (Stream output = File.Create(fileName))
			{
				BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(20080125);
				for (int i = 0; i < data.RowCount; i++)
				{
					int value = data.KeyAt(i);
					SparseArray<float> sparseArray = data.ColumnAt(i);
					for (int j = 0; j < sparseArray.Count; j++)
					{
						int value2 = sparseArray.KeyAt(j);
						float value3 = sparseArray.ValueAt(j);
						binaryWriter.Write(value);
						binaryWriter.Write(value2);
						binaryWriter.Write(value3);
					}
				}
				binaryWriter.Close();
			}
		}

		private static void LoadInternal(ref SparseMatrix<float> data, string fileName)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			using (Stream stream = File.OpenRead(fileName))
			{
				long length = stream.Length;
				long num = 0L;
				BinaryReader binaryReader = new BinaryReader(stream);
				if (binaryReader.ReadInt32() != 20080125)
				{
					throw new LanguagePlatformException(ErrorCode.CorruptData);
				}
				num += 4;
				data.Clear();
				while (num < length)
				{
					int row = binaryReader.ReadInt32();
					int column = binaryReader.ReadInt32();
					float value = binaryReader.ReadSingle();
					num += 8;
					num += 4;
					data[row, column] = value;
				}
				binaryReader.Close();
			}
		}
	}
}
