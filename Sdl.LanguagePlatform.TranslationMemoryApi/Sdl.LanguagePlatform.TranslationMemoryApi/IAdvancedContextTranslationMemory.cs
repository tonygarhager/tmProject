using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface IAdvancedContextTranslationMemory
	{
		bool UsesLegacyHashes
		{
			get;
			set;
		}

		bool UsesIdContextMatching
		{
			get;
		}

		TextContextMatchType TextContextMatchType
		{
			get;
		}
	}
}
