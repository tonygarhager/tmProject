using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	internal class InMemoryFeatureVectorCluster : IFuzzyIndex, IEnumerable<KeyValuePair<int, IntFeatureVector>>, IEnumerable
	{
		private readonly InMemoryFuzzyIndex _fuzzyIndex;

		private readonly IOverlapComputer _overlapComputer;

		private readonly ICentroidComputer _centroidComputer;

		private readonly double _minOverlap;

		private readonly double _minWeight;

		public int Id
		{
			get;
		}

		public WeightedFeatureVector Centroid
		{
			get;
		}

		public int Count => _fuzzyIndex.Count;

		public bool IsOutOfBalance
		{
			get;
			private set;
		}

		public InMemoryFeatureVectorCluster(int id, IOverlapComputer overlapComputer, ICentroidComputer centroidComputer, double minOverlap)
		{
			Id = id;
			IsOutOfBalance = false;
			_overlapComputer = overlapComputer;
			_centroidComputer = centroidComputer;
			_minOverlap = minOverlap;
			_fuzzyIndex = new InMemoryFuzzyIndex();
			Centroid = new WeightedFeatureVector();
			_minWeight = 0.001;
		}

		public void Add(int key, IntFeatureVector fv)
		{
			Add(key, fv, recomputeCentroid: false);
		}

		public void Add(int key, IntFeatureVector fv, bool recomputeCentroid)
		{
			_fuzzyIndex.Add(key, fv);
			if (recomputeCentroid)
			{
				RecomputeCentroid();
			}
			else
			{
				IsOutOfBalance = true;
			}
		}

		public void Delete(int key)
		{
			Delete(key, recomputeCentroid: false);
		}

		public void Delete(int key, bool recomputeCentroid)
		{
			_fuzzyIndex.Delete(key);
			if (recomputeCentroid)
			{
				RecomputeCentroid();
			}
			else
			{
				IsOutOfBalance = true;
			}
		}

		public IntFeatureVector Remove(int key, bool recomputeCentroid)
		{
			IntFeatureVector intFeatureVector = _fuzzyIndex.Remove(key);
			if (intFeatureVector != null)
			{
				Delete(key, recomputeCentroid);
			}
			return intFeatureVector;
		}

		public List<Hit> Search(IntFeatureVector fv, int maxResults, int minScore, int lastKey, ScoringMethod scoringMethod, Predicate<int> validateItemCallback, bool descendingOrder)
		{
			return _fuzzyIndex.Search(fv, maxResults, minScore, lastKey, scoringMethod, validateItemCallback, descendingOrder);
		}

		public void RecomputeCentroid()
		{
			if (_fuzzyIndex.Count == 0)
			{
				Centroid.Clear();
			}
			else
			{
				_centroidComputer.Compute(Centroid, _fuzzyIndex.Select((KeyValuePair<int, IntFeatureVector> x) => x.Value), _minWeight);
			}
			IsOutOfBalance = false;
		}

		public List<KeyValuePair<int, IntFeatureVector>> RemoveDistantVectors()
		{
			List<KeyValuePair<int, IntFeatureVector>> list = null;
			RecomputeCentroid();
			foreach (KeyValuePair<int, IntFeatureVector> item in _fuzzyIndex)
			{
				double num = _overlapComputer.ComputeOverlap(Centroid, item.Value);
				if (num < _minOverlap)
				{
					if (list == null)
					{
						list = new List<KeyValuePair<int, IntFeatureVector>>();
					}
					list.Add(item);
				}
			}
			if (list == null)
			{
				return null;
			}
			foreach (KeyValuePair<int, IntFeatureVector> item2 in list)
			{
				_fuzzyIndex.Delete(item2.Key);
			}
			RecomputeCentroid();
			return list;
		}

		public double GetVariance()
		{
			int num = 0;
			double num2 = 0.0;
			foreach (KeyValuePair<int, IntFeatureVector> item in _fuzzyIndex)
			{
				double num3 = _overlapComputer.ComputeOverlap(Centroid, item.Value);
				double num4 = Math.Pow(1.0 - num3, 2.0);
				num2 += num4;
				num++;
			}
			if (num == 0)
			{
				return 0.0;
			}
			return num2 / (double)num;
		}

		public IEnumerator<KeyValuePair<int, IntFeatureVector>> GetEnumerator()
		{
			return _fuzzyIndex.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _fuzzyIndex.GetEnumerator();
		}
	}
}
