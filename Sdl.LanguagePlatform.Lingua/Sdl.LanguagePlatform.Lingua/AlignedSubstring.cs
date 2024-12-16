using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Lingua
{
	[DataContract]
	public class AlignedSubstring
	{
		[DataMember]
		public Substring Source;

		[DataMember]
		public Substring Target;

		[DataMember]
		public int Score;

		[DataMember]
		public int Length;

		public AlignedSubstring(Substring src, Substring trg)
			: this(src, trg, 0, 0)
		{
		}

		public AlignedSubstring(Substring src, Substring trg, int score)
			: this(src, trg, score, 0)
		{
		}

		public AlignedSubstring(Substring src, Substring trg, int score, int length)
		{
			Source = src;
			Target = trg;
			Score = score;
			Length = length;
		}

		public AlignedSubstring(int sourcePos, int sourceLen, int targetPos, int targetLen)
			: this(new Substring(sourcePos, sourceLen), new Substring(targetPos, targetLen), 0, 0)
		{
		}

		public AlignedSubstring(int sourcePos, int sourceLen, int targetPos, int targetLen, int score)
			: this(new Substring(sourcePos, sourceLen), new Substring(targetPos, targetLen), score, 0)
		{
		}

		public AlignedSubstring(int sourcePos, int sourceLen, int targetPos, int targetLen, int score, int length)
			: this(new Substring(sourcePos, sourceLen), new Substring(targetPos, targetLen), score, length)
		{
		}

		public override string ToString()
		{
			return $"({Source.Start},{Source.Start + Source.Length - 1}-{Target.Start},{Target.Start + Target.Length - 1},{Score})";
		}
	}
}
