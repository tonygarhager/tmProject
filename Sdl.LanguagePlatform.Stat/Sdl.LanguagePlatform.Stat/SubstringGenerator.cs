using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat
{
	public class SubstringGenerator<T> : IEnumerable<List<T>>, IEnumerable
	{
		private readonly List<T> _sequence;

		private int _minLength;

		private int _maxLength;

		public SubstringGenerator(List<T> sequence)
			: this(sequence, 0, 0)
		{
		}

		public SubstringGenerator(List<T> sequence, int minLength, int maxLength)
		{
			if (minLength < 0)
			{
				throw new ArgumentOutOfRangeException("minLength", "Must be >= 0");
			}
			if (maxLength < 0)
			{
				throw new ArgumentOutOfRangeException("maxLength", "Must be >= 0");
			}
			if (maxLength > 0 && maxLength < minLength)
			{
				throw new ArgumentOutOfRangeException("maxLength", "Must be >= minLength");
			}
			_sequence = (sequence ?? throw new ArgumentNullException("sequence"));
			if (minLength == 0)
			{
				minLength = 1;
			}
			_minLength = minLength;
			_maxLength = maxLength;
		}

		public IEnumerator<List<T>> GetEnumerator()
		{
			int length = _sequence.Count;
			int p = 0;
			while (p < length)
			{
				int remainingLength = length - p;
				if (_maxLength > 0 && remainingLength > _maxLength)
				{
					remainingLength = _maxLength;
				}
				int num;
				for (int i = _minLength; i <= remainingLength; i = num)
				{
					yield return _sequence.GetRange(p, i);
					num = i + 1;
				}
				num = p + 1;
				p = num;
			}
		}

		public IEnumerator<List<T>> Enumerate(int minLength)
		{
			_minLength = Math.Max(1, minLength);
			return GetEnumerator();
		}

		public IEnumerator<List<T>> Enumerate(int minLength, int maxLength)
		{
			_minLength = Math.Max(1, minLength);
			_maxLength = Math.Max(_minLength, maxLength);
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
