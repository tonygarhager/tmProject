using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public enum SubsegmentMatchType
	{
		[EnumMember]
		TM_TDB = 1,
		[EnumMember]
		ACS,
		[EnumMember]
		DTA
	}
}
