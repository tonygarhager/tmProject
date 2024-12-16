using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public enum FieldValueType
	{
		[EnumMember]
		Unknown,
		[EnumMember]
		SingleString,
		[EnumMember]
		MultipleString,
		[EnumMember]
		DateTime,
		[EnumMember]
		SinglePicklist,
		[EnumMember]
		MultiplePicklist,
		[EnumMember]
		Integer
	}
}
