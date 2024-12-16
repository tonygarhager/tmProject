using Sdl.LanguagePlatform.Core;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	[KnownType(typeof(RegularIterator))]
	[KnownType(typeof(DuplicateIterator))]
	public abstract class TranslationMemoryIterator
	{
		[DataMember]
		public FilterExpression Filter
		{
			get;
			set;
		}

		[DataMember]
		public int MaxCount
		{
			get;
			set;
		}

		[DataMember]
		public int MaxScan
		{
			get;
			set;
		}

		[DataMember]
		public int ProcessedTranslationUnits
		{
			get;
			set;
		}

		[DataMember]
		public int ScannedTranslationUnits
		{
			get;
			set;
		}

		[DataMember]
		public bool Forward
		{
			get;
			set;
		}

		protected TranslationMemoryIterator()
			: this(100)
		{
		}

		protected TranslationMemoryIterator(int maxCount)
		{
			if (maxCount <= 0)
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidIteratorSize, maxCount.ToString());
			}
			MaxCount = maxCount;
			MaxScan = 0;
			Forward = true;
		}

		public abstract void Reset();
	}
}
