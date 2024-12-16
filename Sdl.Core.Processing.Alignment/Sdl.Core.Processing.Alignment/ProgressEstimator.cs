using System;

namespace Sdl.Core.Processing.Alignment
{
	internal class ProgressEstimator
	{
		public static byte CalculateProgressInArray(int y, int x, int width, int height)
		{
			if (width * height == 0)
			{
				return 0;
			}
			double num = (double)x * (double)height + (double)y;
			double num2 = (double)height * (double)width;
			double num3 = 100.0 * num / num2;
			return (byte)num3;
		}

		public static byte CalculateProgressForIntervals(int currentIntervalSize, int numberProcessedItems, int totalNumberItems, byte currentIntervalProgress)
		{
			if (currentIntervalProgress > 100)
			{
				return 0;
			}
			if (totalNumberItems == 0)
			{
				return 0;
			}
			int num = (byte)(currentIntervalProgress * currentIntervalSize / 100);
			return (byte)Math.Min(100 * (numberProcessedItems + num) / totalNumberItems, 100);
		}
	}
}
