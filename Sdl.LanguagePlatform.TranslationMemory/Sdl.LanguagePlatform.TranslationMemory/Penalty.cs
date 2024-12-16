using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class Penalty
	{
		[DataMember]
		public PenaltyType PenaltyType
		{
			get;
			set;
		}

		[DataMember]
		public int Malus
		{
			get;
			set;
		}

		public static bool CanApplyMultipleTimes(PenaltyType pt)
		{
			return pt == PenaltyType.TagMismatch;
		}

		public Penalty()
		{
			Malus = 0;
			PenaltyType = PenaltyType.Unknown;
		}

		public Penalty(PenaltyType t, int malus)
		{
			Malus = malus;
			PenaltyType = t;
		}
	}
}
