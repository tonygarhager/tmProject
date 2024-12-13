namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// The paged translation memories
	/// </summary>
	public class PagedTranslationMemories
	{
		/// <summary>
		/// Zero based index of the translations memories to be retrieved
		/// </summary>
		public int Index
		{
			get;
			set;
		}

		/// <summary>
		/// The page size to be retrieved
		/// </summary>
		public int Size
		{
			get;
			set;
		}

		/// <summary>
		/// Number of total translation memories result
		/// </summary>
		public int TotalEntities
		{
			get;
			set;
		}

		/// <summary>
		/// The translation result based on the index and page size.
		/// </summary>
		public ServerBasedTranslationMemory[] TranslationMemories
		{
			get;
			set;
		}
	}
}
