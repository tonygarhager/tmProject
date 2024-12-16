using System;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Lingua.Index
{
	internal class CacheItem
	{
		private readonly List<int> _deletedBlocks;

		public int Feature
		{
			get;
			set;
		}

		public PostingsInfo PostingsInfo
		{
			get;
			set;
		}

		public List<PostingsBlock> CachedBlocks
		{
			get;
			set;
		}

		public bool HasDirtyBlocks
		{
			get;
			set;
		}

		public bool HasDeletedBlocks => _deletedBlocks.Count > 0;

		public CacheItem(PostingsInfo pi)
		{
			PostingsInfo = pi;
			Feature = pi.Feature;
			CachedBlocks = new List<PostingsBlock>();
			_deletedBlocks = new List<int>();
		}

		public IEnumerable<int> GetDeletedBlocks()
		{
			return _deletedBlocks;
		}

		public IEnumerable<PostingsBlock> GetDirtyBlocks()
		{
			throw new NotImplementedException();
		}
	}
}
