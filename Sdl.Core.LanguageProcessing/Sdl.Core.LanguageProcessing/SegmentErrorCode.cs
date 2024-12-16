using System.Runtime.Serialization;

namespace Sdl.Core.LanguageProcessing
{
	[DataContract]
	public enum SegmentErrorCode
	{
		[EnumMember]
		OK = 0,
		[EnumMember]
		Other = 9,
		[EnumMember]
		UndefinedOrInvalidLanguage = 10,
		[EnumMember]
		NeutralLanguage = 11,
		[EnumMember]
		EmptySegment = 12,
		[EnumMember]
		NoTextInSegment = 0xF,
		[EnumMember]
		TagInvalidTagAnchor = 0x10,
		[EnumMember]
		TagAnchorAlreadyUsed = 18,
		[EnumMember]
		TagAnchorNotOpen = 19,
		[EnumMember]
		TagAnchorAlreadyClosed = 20,
		[EnumMember]
		TagAnchorNotClosed = 21
	}
}
