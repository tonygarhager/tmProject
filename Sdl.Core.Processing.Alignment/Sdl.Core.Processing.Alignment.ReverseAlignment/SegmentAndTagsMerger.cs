using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.ReverseAlignment
{
	internal class SegmentAndTagsMerger
	{
		public IParagraph MergeSegmentsAndTags(IParagraph paragraph, IList<ISegment> segmentsToMerge)
		{
			RemoveSegmentVisitor removeSegmentVisitor = new RemoveSegmentVisitor();
			removeSegmentVisitor.VisitParagraph(paragraph, segmentsToMerge);
			IParagraph newParagraph = removeSegmentVisitor.NewParagraph;
			AdjacentTagPairMerger adjacentTagPairMerger = new AdjacentTagPairMerger();
			adjacentTagPairMerger.MergeAdjacentTagPairs(newParagraph);
			return newParagraph;
		}
	}
}
