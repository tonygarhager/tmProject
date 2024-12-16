using System.Runtime.Serialization;

namespace Sdl.Core.Globalization
{
	[DataContract]
	public enum ConfirmationLevel
	{
		[EnumMember]
		Unspecified,
		[EnumMember]
		Draft,
		[EnumMember]
		Translated,
		[EnumMember]
		RejectedTranslation,
		[EnumMember]
		ApprovedTranslation,
		[EnumMember]
		RejectedSignOff,
		[EnumMember]
		ApprovedSignOff
	}
}
