using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class InMemoryFuzzyIndex : IFuzzyIndex, IEnumerable<KeyValuePair<int, IntFeatureVector>>, IEnumerable
	{
		private class InMemoryDataStorage : IFuzzyDataReader, IEnumerable<KeyValuePair<int, IntFeatureVector>>, IEnumerable
		{
			private readonly Dictionary<int, PostingsList> _index;

			private readonly Dictionary<int, IntFeatureVector> _featureVectors;

			public int Count => _featureVectors.Count;

			public InMemoryDataStorage()
			{
				_index = new Dictionary<int, PostingsList>();
				_featureVectors = new Dictionary<int, IntFeatureVector>();
			}

			public AbstractPostingsIterator GetIterator(int feature, bool orderDescending)
			{
				if (!_index.TryGetValue(feature, out PostingsList value))
				{
					return null;
				}
				return new InMemoryPostingsIterator(value, orderDescending);
			}

			public void Add(int key, IntFeatureVector fv)
			{
				if (fv != null && fv.Count != 0)
				{
					if (_featureVectors.ContainsKey(key))
					{
						throw new LanguagePlatformException(ErrorCode.DAIndexDuplicateKey);
					}
					foreach (int item in fv)
					{
						if (!_index.TryGetValue(item, out PostingsList value))
						{
							value = new PostingsList(item);
							_index.Add(item, value);
						}
						if (value.Keys.Count == 0 || key > value.Keys[value.Keys.Count - 1])
						{
							value.Keys.Add(key);
						}
						else
						{
							int num = value.Keys.BinarySearch(key);
							if (num < 0)
							{
								value.Keys.Insert(~num, key);
							}
						}
					}
					_featureVectors.Add(key, fv);
				}
			}

			public bool ContainsFeature(int feature)
			{
				return _index.ContainsKey(feature);
			}

			public bool ContainsKey(int key)
			{
				return _featureVectors.ContainsKey(key);
			}

			public int GetPostingsCount(int feature)
			{
				if (!_index.TryGetValue(feature, out PostingsList value))
				{
					return 0;
				}
				return value.Keys.Count;
			}

			public IntFeatureVector GetFeatureVector(int key)
			{
				if (!_featureVectors.TryGetValue(key, out IntFeatureVector value))
				{
					return null;
				}
				return value;
			}

			public void Delete(int key)
			{
				if (_featureVectors.TryGetValue(key, out IntFeatureVector value))
				{
					foreach (int item in value)
					{
						if (_index.TryGetValue(item, out PostingsList value2))
						{
							int num = value2.Keys.BinarySearch(key);
							if (num >= 0)
							{
								value2.Keys.RemoveAt(num);
							}
						}
					}
					_featureVectors.Remove(key);
				}
			}

			public IntFeatureVector Remove(int key)
			{
				if (_featureVectors.TryGetValue(key, out IntFeatureVector value))
				{
					Delete(key);
				}
				return value;
			}

			public IEnumerator<KeyValuePair<int, IntFeatureVector>> GetEnumerator()
			{
				return _featureVectors.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _featureVectors.GetEnumerator();
			}
		}

		private class PostingsList
		{
			public int Feature;

			public readonly List<int> Keys;

			public PostingsList(int feature)
			{
				Keys = new List<int>();
				Feature = feature;
			}
		}

		private class InMemoryPostingsIterator : AbstractPostingsIterator
		{
			private readonly List<int> _column;

			private int _current;

			private int _currentValue;

			private bool _atEnd;

			private readonly bool _descendingOrder;

			public override bool AtEnd => _atEnd;

			public override int Current => _currentValue;

			public override int Count => _column.Count;

			public InMemoryPostingsIterator(PostingsList h, bool descendingOrder)
			{
				_column = h.Keys;
				_descendingOrder = descendingOrder;
				if (_descendingOrder)
				{
					_current = _column.Count - 1;
					_atEnd = (_current < 0);
				}
				else
				{
					_current = 0;
					_atEnd = (_current >= _column.Count);
				}
				if (!_atEnd)
				{
					_currentValue = _column[_current];
				}
			}

			public override bool Next()
			{
				if (_descendingOrder)
				{
					_current--;
					_atEnd = (_current < 0);
				}
				else
				{
					_current++;
					_atEnd = (_current >= _column.Count);
				}
				if (!_atEnd)
				{
					_currentValue = _column[_current];
				}
				return _atEnd;
			}
		}

		private InMemoryDataStorage _storage;

		private FuzzySearcher _searcher;

		public int Count => _storage.Count;

		public InMemoryFuzzyIndex()
		{
			Init();
		}

		private void Init()
		{
			_storage = new InMemoryDataStorage();
			_searcher = new FuzzySearcher(_storage);
		}

		public void Clear()
		{
			_storage = null;
			_searcher = null;
			Init();
		}

		public void Add(int key, IntFeatureVector fv)
		{
			_storage.Add(key, fv);
		}

		public void Add(int key, List<int> fv)
		{
			_storage.Add(key, new IntFeatureVector(fv));
		}

		public void Delete(int key)
		{
			_storage.Delete(key);
		}

		public IntFeatureVector Remove(int key)
		{
			return _storage.Remove(key);
		}

		public bool ContainsKey(int key)
		{
			return _storage.ContainsKey(key);
		}

		public List<Hit> Search(IntFeatureVector fv, int maxResults, int minScore, int lastKey, ScoringMethod scoringMethod, Predicate<int> validateItemCallback, bool descendingOrder)
		{
			return _searcher.Search(fv, maxResults, minScore, lastKey, scoringMethod, validateItemCallback, descendingOrder);
		}

		public List<Hit> Search(IList<int> fv, int maxResults, int minScore, int lastKey, ScoringMethod scoringMethod, Predicate<int> validateItemCallback, bool descendingOrder)
		{
			IntFeatureVector fv2 = new IntFeatureVector(fv);
			return _searcher.Search(fv2, maxResults, minScore, lastKey, scoringMethod, validateItemCallback, descendingOrder);
		}

		public IEnumerator<KeyValuePair<int, IntFeatureVector>> GetEnumerator()
		{
			return _storage.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _storage.GetEnumerator();
		}
	}
}
