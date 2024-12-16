namespace Sdl.LanguagePlatform.TranslationMemory
{
	public interface IFieldValueComparer<in T>
	{
		int Compare(T a, T b, string fieldName);
	}
}
