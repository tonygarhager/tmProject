using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[Flags]
	[DataContract]
	public enum FuzzyIndexes
	{
		[EnumMember]
		SourceWordBased = 0x1,
		[EnumMember]
		SourceCharacterBased = 0x2,
		[EnumMember]
		TargetCharacterBased = 0x4,
		[EnumMember]
		TargetWordBased = 0x8
	}
}
