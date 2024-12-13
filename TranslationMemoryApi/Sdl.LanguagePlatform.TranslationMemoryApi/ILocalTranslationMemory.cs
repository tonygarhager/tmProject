namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents base class for bilingual file-based (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.FileBasedTranslationMemory" />) and in-memory translation memories (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.InMemoryTranslationMemory" />).
	/// </summary>
	public interface ILocalTranslationMemory
	{
		/// <summary>
		/// Gets the one language direction contained in this translation memory.
		/// </summary>
		ITranslationMemoryLanguageDirection LanguageDirection
		{
			get;
		}
	}
}
