using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Segmentation
{
	[DataContract]
	public enum ContextType
	{
		[EnumMember]
		Unknown,
		[EnumMember]
		MatchContext,
		[EnumMember]
		AbbreviationException,
		[EnumMember]
		OrdinalFollowerException,
		[EnumMember]
		LowercaseFollowerException,
		[EnumMember]
		OtherException,
		[EnumMember]
		Other
	}
}
