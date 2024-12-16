using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class TagPairs : IEnumerable<PairedTag>, IEnumerable
	{
		private readonly Dictionary<int, int> _startIndex;

		private readonly Dictionary<int, int> _endIndex;

		private readonly Dictionary<int, int> _anchorIndex;

		private readonly List<PairedTag> _pairings;

		public PairedTag this[int p] => _pairings[p];

		public int Count => _pairings.Count;

		public TagPairs()
		{
			_startIndex = new Dictionary<int, int>();
			_endIndex = new Dictionary<int, int>();
			_anchorIndex = new Dictionary<int, int>();
			_pairings = new List<PairedTag>();
		}

		public void Add(int startTagPosition, int endTagPosition, int anchor)
		{
			if (!_anchorIndex.ContainsKey(anchor) && !_startIndex.ContainsKey(startTagPosition) && !_endIndex.ContainsKey(endTagPosition))
			{
				_pairings.Add(new PairedTag(startTagPosition, endTagPosition, anchor));
				int count = _pairings.Count;
				_anchorIndex.Add(anchor, count);
				_startIndex.Add(startTagPosition, count);
				_endIndex.Add(endTagPosition, count);
			}
		}

		public bool IsStartTag(int position)
		{
			return _startIndex.ContainsKey(position);
		}

		public bool IsEndTag(int position)
		{
			return _endIndex.ContainsKey(position);
		}

		public int GetStartPosition(int endPosition)
		{
			if (!_endIndex.TryGetValue(endPosition, out int value))
			{
				return -1;
			}
			return _pairings[value].Start;
		}

		public int GetEndPosition(int startPosition)
		{
			if (!_startIndex.TryGetValue(startPosition, out int value))
			{
				return -1;
			}
			return _pairings[value].End;
		}

		public PairedTag GetByAnchor(int anchor)
		{
			if (_anchorIndex.TryGetValue(anchor, out int value))
			{
				return _pairings[value];
			}
			return null;
		}

		public PairedTag GetByStartPosition(int startPosition)
		{
			return _pairings[_startIndex[startPosition]];
		}

		public PairedTag GetByEndPosition(int endPosition)
		{
			return _pairings[_endIndex[endPosition]];
		}

		public IEnumerator<PairedTag> GetEnumerator()
		{
			return _pairings.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _pairings.GetEnumerator();
		}
	}
}
