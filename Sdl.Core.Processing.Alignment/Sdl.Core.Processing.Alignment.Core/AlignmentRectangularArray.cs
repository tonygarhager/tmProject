using System;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class AlignmentRectangularArray : IAlignmentArray
	{
		private int _width;

		private int _height;

		private AlignmentArrayElement[,] _elements;

		public int Width => _width;

		public int Height => _height;

		public AlignmentArrayElement this[int x, int y]
		{
			get
			{
				return _elements[x, y];
			}
			set
			{
				_elements[x, y] = value;
			}
		}

		public AlignmentRectangularArray(int width, int height)
		{
			if (width < 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height < 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			_width = width;
			_height = height;
			_elements = new AlignmentArrayElement[width, height];
		}

		public bool IsValid(int x, int y)
		{
			if (x >= 0 && x < Width && y >= 0)
			{
				return y < Height;
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
	}
}
