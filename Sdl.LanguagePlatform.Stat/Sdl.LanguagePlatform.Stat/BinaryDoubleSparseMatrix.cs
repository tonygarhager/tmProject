using System;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class BinaryDoubleSparseMatrix : IDisposable
	{
		private const int Marker = 20100203;

		private const int Version = 1;

		private int[] _rowKeys;

		private int[] _rowLengths;

		private int[] _rowOffsets;

		private int[] _columnKeys;

		private double[] _cellData;

		public bool IsEmpty
		{
			get
			{
				if (_rowKeys != null)
				{
					return _rowKeys.Length == 0;
				}
				return true;
			}
		}

		public double this[int sW, int tW]
		{
			get
			{
				int offset = GetOffset(sW, tW);
				if (offset < 0)
				{
					return 0.0;
				}
				return _cellData[offset];
			}
		}

		public BinaryDoubleSparseMatrix()
		{
			_rowKeys = null;
			_rowLengths = null;
			_rowOffsets = null;
		}

		public BinaryDoubleSparseMatrix(SparseMatrix<double> inMemoryMatrix)
		{
			Dispose();
			_rowKeys = new int[inMemoryMatrix.RowCount];
			_rowLengths = new int[inMemoryMatrix.RowCount];
			_rowOffsets = new int[inMemoryMatrix.RowCount];
			int num = 0;
			for (int i = 0; i < inMemoryMatrix.RowCount; i++)
			{
				_rowKeys[i] = inMemoryMatrix.KeyAt(i);
				SparseArray<double> sparseArray = inMemoryMatrix.ColumnAt(i);
				_rowLengths[i] = sparseArray.Count;
				num += sparseArray.Count;
				if (i > 0)
				{
					_rowOffsets[i] = _rowOffsets[i - 1] + _rowLengths[i - 1];
				}
			}
			_columnKeys = new int[num];
			_cellData = new double[num];
			num = 0;
			for (int j = 0; j < inMemoryMatrix.RowCount; j++)
			{
				SparseArray<double> sparseArray2 = inMemoryMatrix.ColumnAt(j);
				for (int k = 0; k < sparseArray2.Count; k++)
				{
					_columnKeys[num] = sparseArray2.KeyAt(k);
					_cellData[num] = sparseArray2.ValueAt(k);
					num++;
				}
			}
		}

		private int GetOffset(int sW, int tW)
		{
			if (_rowKeys == null)
			{
				throw new Exception("Not initialized");
			}
			int num = Array.BinarySearch(_rowKeys, sW);
			if (num < 0)
			{
				return -1;
			}
			int num2 = _rowOffsets[num];
			int num3 = num2 + _rowLengths[num];
			if (num3 <= num2)
			{
				return -1;
			}
			int num4 = Array.BinarySearch(_columnKeys, num2, _rowLengths[num], tW);
			if (num4 < 0)
			{
				return -1;
			}
			return num4;
		}

		public bool HasValue(int sW, int tW)
		{
			return GetOffset(sW, tW) >= 0;
		}

		public void Load(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("No filename supplied");
			}
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException(fileName);
			}
			using (Stream input = File.OpenRead(fileName))
			{
				using (BinaryReader rdr = new BinaryReader(input))
				{
					Deserialize(rdr);
				}
			}
		}

		public void Save(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("No filename supplied");
			}
			using (Stream output = File.Create(fileName))
			{
				using (BinaryWriter wtr = new BinaryWriter(output))
				{
					Serialize(wtr);
				}
			}
		}

		private void Serialize(BinaryWriter wtr)
		{
			wtr.Write(20100204);
			wtr.Write(_rowKeys.Length);
			wtr.Write(_columnKeys.Length);
			int[] rowKeys = _rowKeys;
			foreach (int value in rowKeys)
			{
				wtr.Write(value);
			}
			for (int j = 0; j < _rowKeys.Length; j++)
			{
				wtr.Write(_rowLengths[j]);
			}
			for (int k = 0; k < _rowKeys.Length; k++)
			{
				wtr.Write(_rowOffsets[k]);
			}
			for (int l = 0; l < _cellData.Length; l++)
			{
				wtr.Write(_columnKeys[l]);
			}
			double[] cellData = _cellData;
			foreach (double value2 in cellData)
			{
				wtr.Write(value2);
			}
		}

		private void Deserialize(BinaryReader rdr)
		{
			Dispose();
			int num = rdr.ReadInt32();
			if (num != 20100204)
			{
				throw new Exception("Data format version mismatch");
			}
			int num2 = rdr.ReadInt32();
			int num3 = rdr.ReadInt32();
			_rowKeys = new int[num2];
			_rowLengths = new int[num2];
			_rowOffsets = new int[num2];
			_columnKeys = new int[num3];
			_cellData = new double[num3];
			for (int i = 0; i < _rowKeys.Length; i++)
			{
				_rowKeys[i] = rdr.ReadInt32();
			}
			for (int j = 0; j < _rowKeys.Length; j++)
			{
				_rowLengths[j] = rdr.ReadInt32();
			}
			for (int k = 0; k < _rowKeys.Length; k++)
			{
				_rowOffsets[k] = rdr.ReadInt32();
			}
			for (int l = 0; l < _cellData.Length; l++)
			{
				_columnKeys[l] = rdr.ReadInt32();
			}
			for (int m = 0; m < _cellData.Length; m++)
			{
				_cellData[m] = rdr.ReadDouble();
			}
		}

		public void Dispose()
		{
			_rowKeys = null;
			_rowLengths = null;
			_rowOffsets = null;
			_columnKeys = null;
			_cellData = null;
		}
	}
}
