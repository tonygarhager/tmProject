using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[DataContract]
	public class NamedConnectionStringContainer : Container
	{
		[DataMember]
		public string ConnectionStringName
		{
			get;
			set;
		}

		public NamedConnectionStringContainer()
		{
			ConnectionStringName = null;
		}

		public NamedConnectionStringContainer(string connectionStringName)
		{
			ConnectionStringName = connectionStringName;
		}
	}
}
