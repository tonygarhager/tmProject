using Sdl.LanguagePlatform.TranslationMemory;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.FGA
{
	[Serializable]
	public class SerializablePersistentObjectToken
	{
		public PersistentObjectToken PersistentObjectToken => new PersistentObjectToken(Id, Guid);

		public Guid Guid
		{
			get;
		}

		public int Id
		{
			get;
		}

		public SerializablePersistentObjectToken(PersistentObjectToken token)
		{
			Guid = token.Guid;
			Id = token.Id;
		}
	}
}
