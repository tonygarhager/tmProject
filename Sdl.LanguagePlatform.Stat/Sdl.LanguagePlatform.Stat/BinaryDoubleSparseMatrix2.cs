using System;
using System.Globalization;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	public class BinaryDoubleSparseMatrix2
	{
		protected int[] RowKeys;

		protected int[] RowLengths;

		protected int[] RowOffsets;

		protected int[] ColumnKeys;

		protected double[] CellData;

		public bool IsEmpty
		{
			get
			{
				if (RowKeys != null)
				{
					return RowKeys.Length == 0;
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
				return CellData[offset];
			}
		}

		public BinaryDoubleSparseMatrix2()
		{
			RowKeys = null;
			RowLengths = null;
			RowOffsets = null;
		}

		protected void AssignMatrix(SparseMatrix<double> inMemoryMatrix)
		{
			Dispose();
			RowKeys = new int[inMemoryMatrix.RowCount];
			RowLengths = new int[inMemoryMatrix.RowCount];
			RowOffsets = new int[inMemoryMatrix.RowCount];
			int num = 0;
			for (int i = 0; i < inMemoryMatrix.RowCount; i++)
			{
				RowKeys[i] = inMemoryMatrix.KeyAt(i);
				SparseArray<double> sparseArray = inMemoryMatrix.ColumnAt(i);
				RowLengths[i] = sparseArray.Count;
				num += sparseArray.Count;
				if (i > 0)
				{
					RowOffsets[i] = RowOffsets[i - 1] + RowLengths[i - 1];
				}
			}
			ColumnKeys = new int[num];
			CellData = new double[num];
			num = 0;
			for (int j = 0; j < inMemoryMatrix.RowCount; j++)
			{
				SparseArray<double> sparseArray2 = inMemoryMatrix.ColumnAt(j);
				for (int k = 0; k < sparseArray2.Count; k++)
				{
					ColumnKeys[num] = sparseArray2.KeyAt(k);
					CellData[num] = sparseArray2.ValueAt(k);
					num++;
				}
			}
		}

		public BinaryDoubleSparseMatrix2(SparseMatrix<double> inMemoryMatrix)
		{
			AssignMatrix(inMemoryMatrix);
		}

		private int GetOffset(int sW, int tW)
		{
			if (RowKeys == null)
			{
				throw new Exception("Not initialized");
			}
			int num = Array.BinarySearch(RowKeys, sW);
			if (num < 0)
			{
				return -1;
			}
			int num2 = RowOffsets[num];
			int num3 = num2 + RowLengths[num];
			if (num3 <= num2)
			{
				return -1;
			}
			int num4 = Array.BinarySearch(ColumnKeys, num2, RowLengths[num], tW);
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

		public bool Exists(DataLocation2 location, CultureInfo srcCulture, CultureInfo trgCulture)
		{
			return File.Exists(location.GetComponentFileName(DataFileType.BilingualChiSquareScores, srcCulture, trgCulture));
		}

		public void Dispose()
		{
			RowKeys = null;
			RowLengths = null;
			RowOffsets = null;
			ColumnKeys = null;
			CellData = null;
		}
	}
}
