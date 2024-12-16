using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[Flags]
	[DataContract]
	public enum TokenizerFlags
	{
		[EnumMember]
		NoFlags = 0x0,
		[EnumMember]
		BreakOnHyphen = 0x1,
		[EnumMember]
		BreakOnDash = 0x2,
		[EnumMember]
		BreakOnApostrophe = 0x4,
		[Obsolete("use logic or operators to get better version compatibility")]
		[EnumMember]
		AllFlags = 0x3,
		[EnumMember]
		DefaultFlags = 0x7
	}
}
