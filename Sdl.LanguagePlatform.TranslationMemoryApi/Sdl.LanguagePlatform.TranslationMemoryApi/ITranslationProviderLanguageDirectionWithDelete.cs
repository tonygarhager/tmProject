using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationProviderLanguageDirectionWithDelete
	{
		bool DeleteTranslationUnit(TranslationUnit translationUnit);

		int DeleteTranslationUnits(TranslationUnit[] translationUnits);
	}
}
