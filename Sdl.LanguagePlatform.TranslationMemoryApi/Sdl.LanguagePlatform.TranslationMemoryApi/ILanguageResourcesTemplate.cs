using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ILanguageResourcesTemplate
	{
		string Name
		{
			get;
			set;
		}

		string Description
		{
			get;
			set;
		}

		LanguageResourceBundleCollection LanguageResourceBundles
		{
			get;
		}

		BuiltinRecognizers? Recognizers
		{
			get;
			set;
		}

		WordCountFlags? WordCountFlags
		{
			get;
			set;
		}

		TokenizerFlags? TokenizerFlags
		{
			get;
			set;
		}
	}
}
