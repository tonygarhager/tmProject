using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class DuplicateIterator : TranslationMemoryIterator
	{
		[DataContract]
		public class DuplicateIteratorPosition
		{
			[DataMember]
			public long Hash;

			[DataMember]
			public int TUId;

			public DuplicateIteratorPosition()
				: this(long.MinValue, 0)
			{
			}

			public DuplicateIteratorPosition(long hash, int tuId)
			{
				Hash = hash;
				TUId = tuId;
			}
		}

		[DataMember]
		public DuplicateIteratorPosition PositionFrom
		{
			get;
			set;
		}

		[DataMember]
		public DuplicateIteratorPosition PositionTo
		{
			get;
			set;
		}

		public DuplicateIterator()
		{
			Reset();
		}

		public DuplicateIterator(int maxCount)
			: base(maxCount)
		{
			Reset();
		}

		public override void Reset()
		{
			DuplicateIteratorPosition duplicateIteratorPosition3 = PositionFrom = (PositionTo = new DuplicateIteratorPosition());
		}
	}
}
