using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "LanguageResourceUpdateType")]
	public enum LanguageResourceUpdateType
	{
		[EnumMember]
		Add,
		[EnumMember]
		Update,
		[EnumMember]
		Delete
	}
}
