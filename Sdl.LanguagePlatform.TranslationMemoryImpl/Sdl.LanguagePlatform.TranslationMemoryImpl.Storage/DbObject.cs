using System;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl.Storage
{
	internal abstract class DbObject
	{
		public int Id
		{
			get;
			set;
		}

		public Guid Guid
		{
			get;
			set;
		}

		protected DbObject(int id, Guid guid)
		{
			Id = id;
			Guid = guid;
		}
	}
}
