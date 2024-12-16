using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Lingua;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	public class TUOperations
	{
		public static void CreateTranslationProposal(SearchResult searchResult, Segment docSourceSegment, SearchSettings searchSettings = null)
		{
			if (searchSettings == null)
			{
				searchSettings = new SearchSettings();
			}
			new Translator(searchSettings).CreateTranslationProposal(searchResult, docSourceSegment);
		}

		public static SearchResult CreateTranslationProposal(TranslationUnit translatedSegment, Segment docSourceSegment, LanguageTools sourceLanguageTools, LanguageTools targetLanguageTools, SearchSettings searchSettings = null)
		{
			if (searchSettings == null)
			{
				searchSettings = new SearchSettings();
			}
			return new Translator(searchSettings)
			{
				SourceTokenizationContext = sourceLanguageTools.GetTokenizationContext(),
				TargetTokenizationContext = targetLanguageTools.GetTokenizationContext()
			}.CreateTranslationProposal(translatedSegment, docSourceSegment, sourceLanguageTools, targetLanguageTools);
		}
	}
}
