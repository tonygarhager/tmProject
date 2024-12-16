using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface IAdvancedContextTranslationMemoryLanguageDirection
	{
		TranslationUnit[] GetTranslationUnitsWithContextContent(ref RegularIterator iterator);
	}
}
