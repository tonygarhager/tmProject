namespace Sdl.LanguagePlatform.TranslationMemory
{
	public class SearchResultFieldValueAccessor : ITypedKeyValueContainer
	{
		private readonly SearchResult _searchResult;

		public SearchResultFieldValueAccessor(SearchResult r)
		{
			_searchResult = r;
		}

		public FieldValue GetValue(string fieldName)
		{
			return _searchResult.MemoryTranslationUnit.GetValue(fieldName);
		}

		public FieldValue GetValue(string fieldName, FieldValueType t)
		{
			return _searchResult.MemoryTranslationUnit.GetValue(fieldName, t);
		}

		public FieldValueType GetType(string fieldName)
		{
			return _searchResult.MemoryTranslationUnit.GetType(fieldName);
		}
	}
}
