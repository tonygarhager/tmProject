using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Core.Bcm.BcmModel.Collections
{
	public class SegmentPairCollection : IEnumerable<SegmentPair>, IEnumerable
	{
		private readonly IEnumerable<SegmentPair> _enumerable;

		public SegmentPair this[string segmentNumber] => _enumerable.First((SegmentPair x) => x.Source.SegmentNumber == segmentNumber);

		public SegmentPairCollection(IEnumerable<SegmentPair> enumerable)
		{
			_enumerable = enumerable;
		}

		public IEnumerator<SegmentPair> GetEnumerator()
		{
			return _enumerable.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
