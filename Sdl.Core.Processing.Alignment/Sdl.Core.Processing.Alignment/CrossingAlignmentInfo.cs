using Sdl.Core.Processing.Alignment.Core;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment
{
	internal class CrossingAlignmentInfo
	{
		private readonly List<AlignmentData> _tgtAlignments = new List<AlignmentData>();

		private readonly AlignmentData _srcAlignment;

		public AlignmentData SrcAlignment => _srcAlignment;

		public List<AlignmentData> TgtAlignments => _tgtAlignments;

		public CrossingAlignmentInfo(AlignmentData srcAlignment)
		{
			_srcAlignment = srcAlignment;
		}
	}
}
