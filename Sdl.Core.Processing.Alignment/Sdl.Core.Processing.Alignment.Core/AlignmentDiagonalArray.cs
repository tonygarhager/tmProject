using System;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignmentDiagonalArray : IAlignmentArray
	{
		private readonly AlignmentArrayElement[,] _diagonal0;

		private readonly AlignmentArrayElement[,] _diagonal1;

		public int Width
		{
			get;
			private set;
		}

		public int Height
		{
			get;
			private set;
		}

		public int DiagonalThickness
		{
			get;
			private set;
		}

		public AlignmentArrayElement this[int x, int y]
		{
			get
			{
				CheckAlignmentElementIndex(x, y);
				MapArrayToDiagonal0(x, y, out int x2, out int y2);
				if (x2 >= 0 && x2 < DiagonalThickness)
				{
					return _diagonal0[x2, y2];
				}
				MapArrayToDiagonal1(x, y, out int x3, out int y3);
				if (y3 >= 0 && y3 < DiagonalThickness)
				{
					return _diagonal1[x3, y3];
				}
				return null;
			}
			set
			{
				CheckAlignmentElementIndex(x, y);
				MapArrayToDiagonal0(x, y, out int x2, out int y2);
				if (x2 >= 0 && x2 < DiagonalThickness)
				{
					_diagonal0[x2, y2] = value;
					return;
				}
				MapArrayToDiagonal1(x, y, out int x3, out int y3);
				if (y3 >= 0 && y3 < DiagonalThickness)
				{
					_diagonal1[x3, y3] = value;
					return;
				}
				throw new InvalidOperationException("Cannot set the value of an alignment element off the diagonal.");
			}
		}

		public AlignmentDiagonalArray(int width, int height, int diagonalThickness)
		{
			if (width < 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height < 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			if (diagonalThickness < 1)
			{
				throw new ArgumentOutOfRangeException("diagonalThickness");
			}
			Width = width;
			Height = height;
			DiagonalThickness = diagonalThickness;
			_diagonal0 = new AlignmentArrayElement[DiagonalThickness, Height];
			_diagonal1 = new AlignmentArrayElement[Width, DiagonalThickness];
		}

		private void CheckAlignmentElementIndex(int x, int y)
		{
			if (x < 0 || x >= Width)
			{
				throw new IndexOutOfRangeException("x");
			}
			if (y < 0 || y >= Height)
			{
				throw new IndexOutOfRangeException("y");
			}
		}

		public bool IsValid(int x, int y)
		{
			MapArrayToDiagonal0(x, y, out int x2, out int _);
			if (x2 >= 0 && x2 < DiagonalThickness)
			{
				return true;
			}
			MapArrayToDiagonal1(x, y, out int _, out int y3);
			if (y3 >= 0 && y3 < DiagonalThickness)
			{
				return true;
			}
			return false;
		}

		public bool IsValidAndDefined(int x, int y)
		{
			if (IsValid(x, y))
			{
				return this[x, y] != null;
			}
			return false;
		}

		private void MapArrayToDiagonal0(int x, int y, out int x0, out int y0)
		{
			x0 = x - y * Width / Height + DiagonalThickness / 2;
			y0 = y;
		}

		private void MapArrayToDiagonal1(int x, int y, out int x1, out int y1)
		{
			x1 = x;
			y1 = y - x * Height / Width + DiagonalThickness / 2;
		}
	}
}
