using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	public enum FaultStatus
	{
		[EnumMember]
		Success,
		[EnumMember]
		Warning,
		[EnumMember]
		Error,
		[EnumMember]
		Fatal
	}
}
