using System;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class DoubleSparseMatrix
	{
		private SparseMatrix<double> _data;

		private readonly string _fileName;

		public bool Exists => File.Exists(_fileName);

		public bool IsEmpty => _data.RowCount == 0;

		public double this[int sW, int tW]
		{
			get
			{
				return _data[sW, tW];
			}
			set
			{
				_data[sW, tW] = value;
			}
		}

		public DoubleSparseMatrix(string fileName)
		{
			_data = new SparseMatrix<double>();
			_fileName = fileName;
		}

		public void Cutoff(double minValue)
		{
			_data.DeleteIf((double v) => v < minValue);
		}

		public void Save()
		{
			if (string.IsNullOrEmpty(_fileName))
			{
				throw new ArgumentException("No filename supplied");
			}
			SparseMatrixIO.Write(_data, _fileName);
		}

		public void Load()
		{
			if (string.IsNullOrEmpty(_fileName))
			{
				throw new ArgumentException("No filename supplied");
			}
			SparseMatrixIO.Load(ref _data, _fileName);
		}
	}
}
