using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class FuzzyIndexTuningSettings
	{
		[DataMember]
		public int MinScoreIncrease
		{
			get;
			set;
		}

		[DataMember]
		public int MinSearchVectorLengthSourceWordIndex
		{
			get;
			set;
		}

		[DataMember]
		public int MinSearchVectorLengthTargetWordIndex
		{
			get;
			set;
		}

		[DataMember]
		public int MinSearchVectorLengthSourceCharIndex
		{
			get;
			set;
		}

		[DataMember]
		public int MinSearchVectorLengthTargetCharIndex
		{
			get;
			set;
		}
	}
}
