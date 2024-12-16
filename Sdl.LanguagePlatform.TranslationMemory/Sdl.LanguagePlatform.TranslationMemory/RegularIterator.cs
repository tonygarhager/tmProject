using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class RegularIterator : TranslationMemoryIterator
	{
		[DataMember]
		public int PositionFrom
		{
			get;
			set;
		}

		[DataMember]
		public int PositionTo
		{
			get;
			set;
		}

		public RegularIterator()
		{
			Reset();
		}

		public RegularIterator(int maxCount)
			: base(maxCount)
		{
			Reset();
		}

		public sealed override void Reset()
		{
			int num3 = PositionFrom = (PositionTo = 0);
		}
	}
}
