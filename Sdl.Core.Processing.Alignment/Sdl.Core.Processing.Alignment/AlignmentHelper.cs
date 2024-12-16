using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using System.IO;

namespace Sdl.Core.Processing.Alignment
{
	internal class AlignmentHelper
	{
		public static AlignmentType GetAlignmentType(int leftSegmentsCount, int rightSegmentsCount)
		{
			if (leftSegmentsCount == 0 && rightSegmentsCount == 1)
			{
				return AlignmentType.Alignment01;
			}
			if (leftSegmentsCount == 1 && rightSegmentsCount == 0)
			{
				return AlignmentType.Alignment10;
			}
			if (leftSegmentsCount == 1 && rightSegmentsCount == 1)
			{
				return AlignmentType.Alignment11;
			}
			if (leftSegmentsCount == 1 && rightSegmentsCount == 2)
			{
				return AlignmentType.Alignment12;
			}
			if (leftSegmentsCount == 1 && rightSegmentsCount == 3)
			{
				return AlignmentType.Alignment13;
			}
			if (leftSegmentsCount == 2 && rightSegmentsCount == 1)
			{
				return AlignmentType.Alignment21;
			}
			if (leftSegmentsCount == 2 && rightSegmentsCount == 2)
			{
				return AlignmentType.Alignment22;
			}
			if (leftSegmentsCount == 3 && rightSegmentsCount == 1)
			{
				return AlignmentType.Alignment31;
			}
			if (leftSegmentsCount >= 4 && rightSegmentsCount == 1)
			{
				return AlignmentType.AlignmentN1;
			}
			if (leftSegmentsCount == 1 && rightSegmentsCount >= 4)
			{
				return AlignmentType.Alignment1N;
			}
			return AlignmentType.Invalid;
		}

		public static string CreateRandomFolderName()
		{
			string tempPath = Path.GetTempPath();
			string text;
			do
			{
				text = ((!string.IsNullOrEmpty(tempPath)) ? (tempPath + Path.GetRandomFileName()) : Path.GetRandomFileName());
			}
			while (Directory.Exists(text));
			return text;
		}

		public static AlignmentType GetAlignmentType(AlignmentData alignment)
		{
			return alignment.AlignmentType;
		}
	}
}
