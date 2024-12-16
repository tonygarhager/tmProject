using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[Flags]
	public enum WordCountFlags
	{
		[EnumMember]
		NoFlags = 0x0,
		[EnumMember]
		BreakOnHyphen = 0x1,
		[EnumMember]
		BreakOnDash = 0x2,
		[EnumMember]
		BreakOnTag = 0x4,
		[EnumMember]
		BreakOnApostrophe = 0x8,
		[Obsolete("use logic or operators to get better version compatibility")]
		[EnumMember]
		AllFlags = 0x7,
		[EnumMember]
		DefaultFlags = 0x4
	}
}
