using System;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal static class AlignmentArrayFactory
	{
		public const double DiagonalThicknessRatio = 0.1;

		public const int DiagonalMinimumThickness = 250;

		public static IAlignmentArray CreateAlignmentElementArray(int width, int height)
		{
			return CreateAlignmentElementArray(width, height, 0);
		}

		public static IAlignmentArray CreateAlignmentElementArray(int width, int height, int maximumDiagonalThickness)
		{
			int num = CalculateDiagonalThickness(width, height);
			if (maximumDiagonalThickness > 0 && num > maximumDiagonalThickness)
			{
				num = maximumDiagonalThickness;
			}
			if (num < width && num < height)
			{
				return new AlignmentDiagonalArray(width, height, num);
			}
			return new AlignmentRectangularArray(width, height);
		}

		private static int CalculateDiagonalThickness(int width, int height)
		{
			int num = (int)((double)Math.Max(width, height) * 0.1 / 2.0);
			if (num <= 250)
			{
				return 250;
			}
			return num;
		}
	}
}
