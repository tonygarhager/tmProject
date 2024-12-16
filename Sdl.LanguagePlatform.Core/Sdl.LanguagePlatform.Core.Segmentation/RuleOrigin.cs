using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Segmentation
{
	[DataContract]
	public enum RuleOrigin
	{
		[EnumMember]
		Unknown,
		[EnumMember]
		System,
		[EnumMember]
		Migration,
		[EnumMember]
		User,
		[EnumMember]
		Other
	}
}
