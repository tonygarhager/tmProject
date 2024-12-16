using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal static class SegmentPairsRepairer
	{
		internal static void RepairSegmentPairs(IEnumerable<ISegment> leftSegments, IEnumerable<ISegment> rightSegments)
		{
			IEnumerator<ISegment> enumerator = leftSegments.GetEnumerator();
			IEnumerator<ISegment> enumerator2 = rightSegments.GetEnumerator();
			int num = 1;
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				ISegment current = enumerator.Current;
				ISegment current2 = enumerator2.Current;
				if (current != null && current2 != null)
				{
					ISegmentPairProperties properties = current.Properties;
					properties.Id = new SegmentId(num++);
					current2.Properties = properties;
				}
			}
		}
	}
}
