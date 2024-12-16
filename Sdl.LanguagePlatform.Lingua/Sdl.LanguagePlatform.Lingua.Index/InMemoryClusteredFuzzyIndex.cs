using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class InMemoryClusteredFuzzyIndex : IFuzzyIndex
	{
		private const double DefaultMinOverlap = 0.7;

		private readonly IOverlapComputer _overlapComputer;

		private readonly ICentroidComputer _centroidComputer;

		private readonly double _minOverlap;

		private int _nextClusterId;

		private readonly Dictionary<int, InMemoryFeatureVectorCluster> _clusters;

		private readonly Dictionary<int, int> _featureVectorToClusterIndex;

		private InMemoryFuzzyIndex _unassignedVectors;

		public bool RequiresRebalancing
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int ClusterCount => _clusters.Count;

		public int MaxClusters
		{
			get;
		}

		public InMemoryClusteredFuzzyIndex()
			: this(null, null, 0.7, 0)
		{
		}

		public InMemoryClusteredFuzzyIndex(IOverlapComputer overlapComputer, ICentroidComputer centroidComputer, double minOverlap, int maxClusters)
		{
			_overlapComputer = (overlapComputer ?? new DiceFeatureVectorOverlapComputer());
			_centroidComputer = (centroidComputer ?? new SimpleFeatureVectorCentroidComputer());
			if (minOverlap <= 0.0 || minOverlap > 1.0)
			{
				_minOverlap = 0.7;
			}
			else
			{
				_minOverlap = minOverlap;
			}
			_clusters = new Dictionary<int, InMemoryFeatureVectorCluster>();
			_unassignedVectors = new InMemoryFuzzyIndex();
			_featureVectorToClusterIndex = new Dictionary<int, int>();
			_nextClusterId = 1;
			MaxClusters = ((maxClusters >= 0) ? maxClusters : 0);
		}

		public void Add(int key, IntFeatureVector fv)
		{
			bool flag = MaxClusters == 0 || _clusters.Count < MaxClusters;
			double winningOverlap;
			InMemoryFeatureVectorCluster inMemoryFeatureVectorCluster = FindBestCluster(fv, out winningOverlap);
			if (inMemoryFeatureVectorCluster == null || (winningOverlap < _minOverlap && flag))
			{
				inMemoryFeatureVectorCluster = new InMemoryFeatureVectorCluster(_nextClusterId, _overlapComputer, _centroidComputer, _minOverlap);
				_clusters.Add(_nextClusterId, inMemoryFeatureVectorCluster);
				inMemoryFeatureVectorCluster.Centroid.Set(fv.Features);
				_nextClusterId++;
			}
			inMemoryFeatureVectorCluster.Add(key, fv);
			_featureVectorToClusterIndex.Add(key, inMemoryFeatureVectorCluster.Id);
		}

		public void Delete(int key)
		{
			if (_featureVectorToClusterIndex.TryGetValue(key, out int value))
			{
				if (value == 0)
				{
					_unassignedVectors.Delete(key);
					return;
				}
				if (_clusters.TryGetValue(value, out InMemoryFeatureVectorCluster value2))
				{
					value2.Delete(key);
					return;
				}
				throw new Exception("No cluster found");
			}
			throw new Exception("Key not indexed");
		}

		public List<Hit> Search(IntFeatureVector fv, int maxResults, int minScore, int lastKey, ScoringMethod scoringMethod, Predicate<int> validateItemCallback, bool descendingOrder)
		{
			int searchedClusters;
			return Search(fv, ClusterSearchMode.BestOnly, maxResults, minScore, lastKey, scoringMethod, validateItemCallback, out searchedClusters);
		}

		public List<Hit> Search(IntFeatureVector fv, ClusterSearchMode mode, int maxResults, int minScore, int lastKey, ScoringMethod scoringMethod, Predicate<int> validateItemCallback, out int searchedClusters)
		{
			searchedClusters = 0;
			if (mode == ClusterSearchMode.BestOnly)
			{
				double winningOverlap;
				InMemoryFeatureVectorCluster inMemoryFeatureVectorCluster = FindBestCluster(fv, out winningOverlap);
				if (inMemoryFeatureVectorCluster != null)
				{
					searchedClusters++;
					return inMemoryFeatureVectorCluster.Search(fv, maxResults, minScore, lastKey, scoringMethod, validateItemCallback, descendingOrder: true);
				}
				if (_unassignedVectors == null)
				{
					return null;
				}
				searchedClusters++;
				return _unassignedVectors.Search(fv, maxResults, minScore, lastKey, scoringMethod, validateItemCallback, descendingOrder: true);
			}
			List<Hit> list = null;
			foreach (KeyValuePair<int, InMemoryFeatureVectorCluster> cluster in _clusters)
			{
				double overlap = GetOverlap(cluster.Value.Centroid, fv);
				if (overlap >= _minOverlap)
				{
					List<Hit> list2 = cluster.Value.Search(fv, maxResults, minScore, lastKey, scoringMethod, validateItemCallback, descendingOrder: true);
					searchedClusters++;
					if (mode == ClusterSearchMode.FirstOnly)
					{
						return list2;
					}
					if (list2 != null && list2.Count > 0)
					{
						if (mode == ClusterSearchMode.FirstWithHits)
						{
							return list2;
						}
						if (list == null)
						{
							list = list2;
						}
						else
						{
							list.AddRange(list2);
						}
					}
				}
			}
			if (list == null)
			{
				return null;
			}
			list.Sort();
			if (list.Count <= maxResults)
			{
				return list;
			}
			list.RemoveRange(maxResults, list.Count - maxResults);
			return list;
		}

		private double GetOverlap(IFeatureVector centroid, IFeatureVector fv)
		{
			return _overlapComputer.ComputeOverlap(centroid, fv);
		}

		private InMemoryFeatureVectorCluster FindBestCluster(IFeatureVector fv, out double winningOverlap)
		{
			winningOverlap = 0.0;
			if (_clusters == null || _clusters.Count == 0)
			{
				return null;
			}
			InMemoryFeatureVectorCluster result = null;
			foreach (KeyValuePair<int, InMemoryFeatureVectorCluster> cluster in _clusters)
			{
				double overlap = GetOverlap(cluster.Value.Centroid, fv);
				if (overlap > winningOverlap)
				{
					result = cluster.Value;
					winningOverlap = overlap;
				}
			}
			return result;
		}

		public int GetClusterAmbiguity(IntFeatureVector fv)
		{
			return GetClusterAmbiguity(fv, _minOverlap);
		}

		public int GetClusterAmbiguity(IntFeatureVector fv, double minOverlap)
		{
			if (_clusters == null || _clusters.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (KeyValuePair<int, InMemoryFeatureVectorCluster> cluster in _clusters)
			{
				double overlap = GetOverlap(cluster.Value.Centroid, fv);
				if (overlap >= minOverlap)
				{
					num++;
				}
			}
			return num;
		}

		private void DeleteEmptyClusters()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, InMemoryFeatureVectorCluster> cluster in _clusters)
			{
				if (cluster.Value.Count == 0)
				{
					list.Add(cluster.Key);
				}
			}
			foreach (int item in list)
			{
				_clusters.Remove(item);
			}
		}

		private void RecomputeCentroids()
		{
			foreach (KeyValuePair<int, InMemoryFeatureVectorCluster> cluster in _clusters)
			{
				if (cluster.Value.IsOutOfBalance)
				{
					cluster.Value.RecomputeCentroid();
				}
			}
		}

		public int Rebalance()
		{
			if (_unassignedVectors == null)
			{
				_unassignedVectors = new InMemoryFuzzyIndex();
			}
			int num = 0;
			foreach (KeyValuePair<int, InMemoryFeatureVectorCluster> cluster in _clusters)
			{
				if (cluster.Value.IsOutOfBalance)
				{
					cluster.Value.RecomputeCentroid();
				}
				List<KeyValuePair<int, IntFeatureVector>> list = cluster.Value.RemoveDistantVectors();
				if (list != null)
				{
					foreach (KeyValuePair<int, IntFeatureVector> item in list)
					{
						_unassignedVectors.Add(item.Key, item.Value);
					}
				}
			}
			DeleteEmptyClusters();
			foreach (KeyValuePair<int, IntFeatureVector> unassignedVector in _unassignedVectors)
			{
				Add(unassignedVector.Key, unassignedVector.Value);
				num++;
			}
			RecomputeCentroids();
			return num;
		}

		public double GetAverageVariance()
		{
			int num = 0;
			double num2 = 0.0;
			foreach (KeyValuePair<int, InMemoryFeatureVectorCluster> cluster in _clusters)
			{
				double variance = cluster.Value.GetVariance();
				num2 += variance;
				num++;
			}
			if (num == 0)
			{
				return 0.0;
			}
			return num2 / (double)num;
		}
	}
}
