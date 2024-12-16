namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities
{
	public class PagedTranslationMemoryEntities
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

		public TranslationMemoryEntity[] TranslationMemoryEntities
		{
			get;
			set;
		}
	}
}
