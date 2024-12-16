using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	[DataContract]
	public class FileContainer : Container
	{
		[DataMember]
		public string Path
		{
			get;
			set;
		}

		[DataMember]
		public bool CreateIfNotExists
		{
			get;
			set;
		}

		[DataMember]
		public string Password
		{
			get;
			set;
		}

		public FileContainer()
			: this(null, createIfNotExists: false, null)
		{
		}

		public FileContainer(string path)
			: this(path, createIfNotExists: false, null)
		{
		}

		public FileContainer(string path, bool createIfNotExists)
			: this(path, createIfNotExists, null)
		{
		}

		public FileContainer(string path, bool createIfNotExists, string password)
		{
			Path = path;
			CreateIfNotExists = createIfNotExists;
			Password = password;
		}
	}
}
