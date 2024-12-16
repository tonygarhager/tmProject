using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core.Segmentation
{
	[DataContract]
	public enum RuleType
	{
		[EnumMember]
		Unknown,
		[EnumMember]
		FullStopRule,
		[EnumMember]
		MarksRule,
		[EnumMember]
		ColonRule,
		[EnumMember]
		SemicolonRule,
		[EnumMember]
		CombinedRule,
		[EnumMember]
		Other
	}
}
