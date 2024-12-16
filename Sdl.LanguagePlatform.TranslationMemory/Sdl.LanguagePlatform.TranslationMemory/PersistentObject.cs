using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class PersistentObject
	{
		[DataMember]
		public PersistentObjectToken ResourceId
		{
			get;
			set;
		}

		public PersistentObject()
			: this(new PersistentObjectToken())
		{
		}

		public PersistentObject(PersistentObjectToken token)
		{
			ResourceId = token;
		}

		public PersistentObject(int id, Guid guid)
			: this(new PersistentObjectToken(id, guid))
		{
		}
	}
}
