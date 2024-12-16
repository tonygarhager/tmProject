using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class WeightedFeatureVector : IFeatureVector
	{
		private readonly List<int> _features;

		private readonly List<double> _weights;

		public double this[int feature]
		{
			get
			{
				int num = _features.BinarySearch(feature);
				if (num >= 0)
				{
					return _weights[num];
				}
				return 0.0;
			}
			set
			{
				int num = _features.BinarySearch(feature);
				if (num < 0)
				{
					_features.Insert(~num, feature);
					_weights.Insert(~num, value);
				}
				else
				{
					_weights[num] = value;
				}
			}
		}

		public int Denominator
		{
			get;
			set;
		}

		public int Count => _features.Count;

		public WeightedFeatureVector()
		{
			_features = new List<int>();
			_weights = new List<double>();
			Denominator = 1;
		}

		public WeightedFeatureVector(WeightedFeatureVector other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			Set(other);
		}

		public void Set(IList<int> features)
		{
			Clear();
			foreach (int feature in features)
			{
				Add(feature, 1.0);
			}
		}

		public void Set(IntFeatureVector fv)
		{
			Clear();
			foreach (int feature in fv.Features)
			{
				_features.Add(feature);
				_weights.Add(1.0);
			}
		}

		public int GetFeatureAt(int idx)
		{
			return _features[idx];
		}

		public double GetWeightAt(int idx)
		{
			return _weights[idx];
		}

		public void SetWeightAt(int idx, double w)
		{
			_weights[idx] = w;
		}

		public void Add(int f, double w)
		{
			int num = _features.BinarySearch(f);
			if (num >= 0)
			{
				throw new ArgumentException("Feature alrady present");
			}
			_features.Insert(~num, f);
			_weights.Insert(~num, w);
		}

		public void Remove(int f)
		{
			int num = _features.BinarySearch(f);
			if (num < 0)
			{
				throw new ArgumentException("Feature not present");
			}
			_features.RemoveAt(num);
			_weights.RemoveAt(num);
		}

		public void Set(WeightedFeatureVector other)
		{
			Clear();
			for (int i = 0; i < other.Count; i++)
			{
				_features.Add(other._features[i]);
				_weights.Add(other._weights[i]);
			}
			Denominator = other.Denominator;
		}

		public void Clean(double threshold)
		{
			if (threshold <= 0.0)
			{
				Clear();
				return;
			}
			for (int num = _features.Count - 1; num >= 0; num--)
			{
				if (_weights[num] < threshold)
				{
					_features.RemoveAt(num);
					_weights.RemoveAt(num);
				}
			}
		}

		public void Clear()
		{
			_features.Clear();
			_weights.Clear();
			Denominator = 1;
		}
	}
}
