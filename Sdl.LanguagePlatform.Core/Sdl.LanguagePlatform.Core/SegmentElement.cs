using Sdl.LanguagePlatform.Core.Tokenization;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	[KnownType(typeof(Tag))]
	[KnownType(typeof(Text))]
	[KnownType(typeof(Token))]
	public abstract class SegmentElement
	{
		public enum Similarity
		{
			None,
			IdenticalType,
			IdenticalValueAndType
		}

		public abstract SegmentElement Duplicate();

		public abstract Similarity GetSimilarity(SegmentElement other);

		public virtual Similarity GetSimilarity(SegmentElement other, bool allowCompatibility)
		{
			return GetSimilarity(other);
		}

		public abstract void AcceptSegmentElementVisitor(ISegmentElementVisitor visitor);

		public abstract int GetWeakHashCode();
	}
}
