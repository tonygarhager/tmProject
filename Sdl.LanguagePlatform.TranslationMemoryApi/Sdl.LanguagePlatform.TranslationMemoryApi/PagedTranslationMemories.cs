namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class PagedTranslationMemories
	{
		public int Index
		{
			get;
			set;
		}

		public int Size
		{
			get;
			set;
		}

		public int TotalEntities
		{
			get;
			set;
		}

		public ServerBasedTranslationMemory[] TranslationMemories
		{
			get;
			set;
		}
	}
}
