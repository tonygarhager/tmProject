using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat
{
	public class StringSubstringGenerator : IEnumerable<string>, IEnumerable
	{
		private readonly string _sequence;

		private int _minLength;

		private int _maxLength;

		public StringSubstringGenerator(string sequence)
			: this(sequence, 0, 0)
		{
		}

		public StringSubstringGenerator(string sequence, int minLength, int maxLength)
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

		public IEnumerator<string> GetEnumerator()
		{
			int length = _sequence.Length;
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
					yield return _sequence.Substring(p, i);
					num = i + 1;
				}
				num = p + 1;
				p = num;
			}
		}

		public IEnumerator<string> Enumerate(int minLength)
		{
			_minLength = Math.Max(1, minLength);
			return GetEnumerator();
		}

		public IEnumerator<string> Enumerate(int minLength, int maxLength)
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
