using Sdl.Core.Processing.Alignment.Common;
using Sdl.Core.Processing.Alignment.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Processing.Alignment
{
	internal class AlignmentQualityCalculator
	{
		internal readonly AlignmentCost MaximumGoodCost = new AlignmentCost(0.33333333333333331);

		internal readonly AlignmentCost MaximumAverageCost = new AlignmentCost(2.0 / 3.0);

		public AlignmentQualityCalculator(IEnumerable<AlignmentData> alignments)
		{
			if (alignments == null)
			{
				throw new ArgumentNullException("alignments");
			}
			IList<AlignmentData> list = alignments.OrderBy((Func<AlignmentData, double>)((AlignmentData alignment) => alignment.Cost)).ToList();
			int num = list.Count() / 3;
			if (num < list.Count)
			{
				MaximumGoodCost = list[num].Cost;
			}
			int num2 = 2 * list.Count() / 3;
			if (num2 < list.Count)
			{
				MaximumAverageCost = list[num2].Cost;
			}
		}

		public AlignmentQuality CalculateQuality(AlignmentData alignment)
		{
			if (alignment == null)
			{
				throw new ArgumentNullException("alignment");
			}
			if ((double)alignment.Cost <= (double)MaximumGoodCost)
			{
				return AlignmentQuality.Good;
			}
			if ((double)alignment.Cost <= (double)MaximumAverageCost)
			{
				return AlignmentQuality.Average;
			}
			return AlignmentQuality.Bad;
		}
	}
}
