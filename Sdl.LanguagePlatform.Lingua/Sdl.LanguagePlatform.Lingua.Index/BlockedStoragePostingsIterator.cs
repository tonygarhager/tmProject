namespace Sdl.LanguagePlatform.Lingua.Index
{
	public class BlockedStoragePostingsIterator : AbstractPostingsIterator
	{
		private readonly IPostingsBlockReader _reader;

		private PostingsInfo _postingsInfo;

		private PostingsBlock _currentBlock;

		private readonly int _feature;

		public override bool AtEnd
		{
			get
			{
				if (_currentBlock == null)
				{
					return true;
				}
				if (Current >= 0)
				{
					return Current > _postingsInfo.LastKey;
				}
				return true;
			}
		}

		public override int Current => _currentBlock?.Current ?? (-1);

		public override int Count
		{
			get
			{
				if (_currentBlock != null)
				{
					return _postingsInfo.PostingsCount;
				}
				return 0;
			}
		}

		public BlockedStoragePostingsIterator(IPostingsBlockReader storage, int feature)
		{
			_reader = storage;
			_feature = feature;
			GetPostingsInfo(_feature);
		}

		private void GetPostingsInfo(int feature)
		{
			_postingsInfo = _reader.GetPostingsInfo(feature);
			if (_postingsInfo.BlockCount > 0)
			{
				_currentBlock = _reader.ReadBlockAfter(_postingsInfo.Feature, 0);
				_ = _currentBlock;
			}
			else
			{
				_currentBlock = null;
			}
		}

		public override bool Next()
		{
			if (_currentBlock == null)
			{
				return _currentBlock != null;
			}
			if (_currentBlock.Next())
			{
				return _currentBlock != null;
			}
			_currentBlock = _reader.ReadBlockAfter(_feature, _currentBlock.LastKey);
			_ = _currentBlock;
			return _currentBlock != null;
		}
	}
}
