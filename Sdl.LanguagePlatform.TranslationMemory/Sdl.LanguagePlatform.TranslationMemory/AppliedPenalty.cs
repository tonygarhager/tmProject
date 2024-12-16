using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class AppliedPenalty
	{
		[DataMember]
		public PenaltyType Type;

		[DataMember]
		public string FilterName;

		[DataMember]
		public int Malus;

		public AppliedPenalty(PenaltyType pt, int malus)
		{
			Type = pt;
			FilterName = null;
			Malus = malus;
		}

		public AppliedPenalty(string filterName, int malus)
		{
			Type = PenaltyType.FilterPenalty;
			FilterName = filterName;
			Malus = malus;
		}
	}
}
