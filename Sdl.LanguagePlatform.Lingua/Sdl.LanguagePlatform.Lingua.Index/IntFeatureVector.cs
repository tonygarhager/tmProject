using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class IntFeatureVector : IFeatureVector, IEnumerable<int>, IEnumerable
	{
		private int[] _features;

		public int this[int idx] => _features[idx];

		public int Count
		{
			get
			{
				int[] features = _features;
				if (features == null)
				{
					return 0;
				}
				return features.Length;
			}
		}

		public IList<int> Features => _features;

		public IntFeatureVector()
		{
			_features = null;
		}

		public IntFeatureVector(IntFeatureVector other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Set(other);
		}

		public IntFeatureVector(IList<int> fv)
		{
			Init(fv);
		}

		public void Set(IList<int> features)
		{
			if (features == null)
			{
				throw new ArgumentNullException();
			}
			Clear();
			Init(features);
		}

		private void Init(IList<int> features)
		{
			if (features == null || features.Count == 0)
			{
				return;
			}
			_features = new int[features.Count];
			bool flag = true;
			for (int i = 0; i < features.Count; i++)
			{
				_features[i] = features[i];
				if (flag && i > 0 && _features[i] < _features[i - 1])
				{
					flag = false;
				}
			}
			if (!flag)
			{
				Array.Sort(_features);
			}
		}

		internal static bool IsSorted(IList<int> l)
		{
			if (l.Count < 2)
			{
				return true;
			}
			for (int i = 1; i < l.Count; i++)
			{
				if (l[i] <= l[i - 1])
				{
					return false;
				}
			}
			return true;
		}

		public int GetFeatureAt(int idx)
		{
			return _features[idx];
		}

		public double GetWeightAt(int idx)
		{
			if (idx < 0 || idx >= _features.Length)
			{
				throw new IndexOutOfRangeException();
			}
			return 1.0;
		}

		public void Set(IntFeatureVector other)
		{
			Clear();
			if (other.Count > 0)
			{
				_features = new int[other.Count];
				for (int i = 0; i < other.Count; i++)
				{
					_features[i] = other._features[i];
				}
			}
		}

		public void Clear()
		{
			_features = null;
		}

		public int GetCommonFeatureCount(IntFeatureVector other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			int count = Count;
			int count2 = other.Count;
			if (count == 0 || count2 == 0)
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (num < count && num2 < count2)
			{
				if (_features[num] == other._features[num2])
				{
					num3++;
					num++;
					num2++;
				}
				else if (_features[num] < other._features[num2])
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
			return num3;
		}

		public IEnumerator<int> GetEnumerator()
		{
			int p = 0;
			while (p < Count)
			{
				yield return GetFeatureAt(p);
				int num = p + 1;
				p = num;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
