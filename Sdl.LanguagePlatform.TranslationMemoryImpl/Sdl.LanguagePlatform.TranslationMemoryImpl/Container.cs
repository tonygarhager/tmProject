using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[DataContract]
	[KnownType(typeof(FileContainer))]
	[KnownType(typeof(DatabaseContainer))]
	[KnownType(typeof(InMemoryContainer))]
	[KnownType(typeof(NamedConnectionStringContainer))]
	public abstract class Container
	{
		[DataMember]
		public string UserNameOverride
		{
			get;
			set;
		}
	}
}
