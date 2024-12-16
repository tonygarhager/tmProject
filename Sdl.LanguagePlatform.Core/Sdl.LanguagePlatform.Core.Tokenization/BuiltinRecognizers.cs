using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Tokenization
{
	[Flags]
	[DataContract]
	public enum BuiltinRecognizers
	{
		[EnumMember]
		RecognizeNone = 0x0,
		[EnumMember]
		RecognizeDates = 0x1,
		[EnumMember]
		RecognizeTimes = 0x2,
		[EnumMember]
		RecognizeNumbers = 0x4,
		[EnumMember]
		RecognizeAcronyms = 0x8,
		[EnumMember]
		RecognizeVariables = 0x10,
		[EnumMember]
		RecognizeMeasurements = 0x20,
		[EnumMember]
		RecognizeAlphaNumeric = 0x40,
		[EnumMember]
		RecognizeAll = 0x7F
	}
}
