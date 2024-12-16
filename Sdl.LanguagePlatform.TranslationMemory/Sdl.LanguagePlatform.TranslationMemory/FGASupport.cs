using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public enum FGASupport
	{
		[EnumMember]
		Legacy,
		[EnumMember]
		Off,
		[EnumMember]
		NonAutomatic,
		[EnumMember]
		Automatic
	}
}
