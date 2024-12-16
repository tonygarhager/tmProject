using System;
using System.IO;

namespace Sdl.LanguagePlatform.Stat
{
	internal class FloatSparseMatrix
	{
		private SparseMatrix<float> _data;

		private readonly string _fileName;

		public bool Exists => File.Exists(_fileName);

		public bool IsEmpty => _data.RowCount == 0;

		public float this[int sW, int tW]
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

		public FloatSparseMatrix(string fileName)
		{
			_data = new SparseMatrix<float>();
			_fileName = fileName;
		}

		public void Cutoff(float minValue)
		{
			_data.DeleteIf((float v) => v < minValue);
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
