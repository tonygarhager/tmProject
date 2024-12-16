using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	public abstract class CachedBlockedFuzzyIndexStorage : IFuzzyDataReader, IFeatureVectorStorage, IPostingsBlockReader, IDisposable
	{
		public static readonly int DefaultMaxCacheSize = 5000;

		private readonly IFuzzyIndexStorage _storage;

		private readonly Dictionary<int, CacheItem> _cache;

		protected CachedBlockedFuzzyIndexStorage(IFuzzyIndexStorage storage)
		{
			_storage = storage;
			_cache = new Dictionary<int, CacheItem>();
		}

		public void Flush()
		{
			foreach (KeyValuePair<int, CacheItem> item in _cache)
			{
				if (item.Value.HasDeletedBlocks)
				{
					foreach (int deletedBlock in item.Value.GetDeletedBlocks())
					{
						_storage.DeleteBlock(deletedBlock);
					}
				}
				if (item.Value.HasDirtyBlocks)
				{
					foreach (PostingsBlock dirtyBlock in item.Value.GetDirtyBlocks())
					{
						_storage.WriteBlock(dirtyBlock);
					}
				}
			}
		}

		public void Dispose()
		{
			Flush();
			_cache.Clear();
		}

		public AbstractPostingsIterator GetIterator(int feature, bool orderDescending = true)
		{
			return new BlockedStoragePostingsIterator(this, feature);
		}

		public bool ContainsFeature(int feature)
		{
			return GetPostingsCount(feature) > 0;
		}

		public int GetPostingsCount(int feature)
		{
			return GetPostingsInfo(feature)?.PostingsCount ?? 0;
		}

		public IntFeatureVector GetFeatureVector(int key)
		{
			return _storage.GetFeatureVector(key);
		}

		public void Add(int key, IntFeatureVector fv)
		{
			if (fv != null)
			{
				_storage.Add(key, fv);
				foreach (int item in fv)
				{
					AddPosting(item, key);
				}
			}
		}

		public void Delete(int key)
		{
			IntFeatureVector featureVector = GetFeatureVector(key);
			if (featureVector != null)
			{
				_storage.Delete(key);
				foreach (int item in featureVector)
				{
					DeletePosting(item, key);
				}
			}
		}

		private void AddPosting(int feature, int key)
		{
			PostingsBlock postingsBlock = ReadBlockContaining(feature, key);
			if (postingsBlock == null)
			{
				throw new NotImplementedException();
			}
			if (!postingsBlock.AddKey(key) || postingsBlock.Count <= PostingsBlock.MaxPostingsBlockSize)
			{
				return;
			}
			throw new NotImplementedException();
		}

		private void DeletePosting(int feature, int key)
		{
			PostingsBlock postingsBlock = ReadBlockContaining(feature, key);
			if (postingsBlock != null && postingsBlock.DeleteKey(key))
			{
				if (postingsBlock.IsEmpty)
				{
					throw new NotImplementedException();
				}
				if (postingsBlock.Count < PostingsBlock.MinPostingsBlockSize)
				{
					throw new NotImplementedException();
				}
			}
		}

		private PostingsInfo GetPostingsInfo(int feature, bool create)
		{
			if (_cache.TryGetValue(feature, out CacheItem value))
			{
				return value.PostingsInfo;
			}
			PostingsInfo postingsInfo = _storage.GetPostingsInfo(feature);
			if (postingsInfo == null)
			{
				if (!create)
				{
					return null;
				}
				postingsInfo = new PostingsInfo(feature, 0, 0, 0, 0);
			}
			_cache.Add(feature, new CacheItem(postingsInfo));
			return postingsInfo;
		}

		public PostingsInfo GetPostingsInfo(int feature)
		{
			return GetPostingsInfo(feature, create: false);
		}

		public List<PostingsInfo> GetPostingsInfo(IntFeatureVector features)
		{
			List<PostingsInfo> list = new List<PostingsInfo>();
			foreach (int feature in features)
			{
				list.Add(GetPostingsInfo(feature));
			}
			return list;
		}

		public PostingsBlock ReadBlockAfter(int feature, int last)
		{
			PostingsInfo postingsInfo = GetPostingsInfo(feature);
			if (postingsInfo == null)
			{
				return null;
			}
			if (postingsInfo.BlockCount == 0)
			{
				return null;
			}
			throw new NotImplementedException();
		}

		public PostingsBlock ReadBlockBefore(int feature, int next)
		{
			PostingsInfo postingsInfo = GetPostingsInfo(feature);
			if (postingsInfo == null)
			{
				return null;
			}
			if (postingsInfo.BlockCount == 0)
			{
				return null;
			}
			throw new NotImplementedException();
		}

		public PostingsBlock ReadBlockContaining(int feature, int key)
		{
			PostingsInfo postingsInfo = GetPostingsInfo(feature);
			if (postingsInfo == null)
			{
				return null;
			}
			if (postingsInfo.BlockCount == 0)
			{
				return null;
			}
			throw new NotImplementedException();
		}
	}
}
