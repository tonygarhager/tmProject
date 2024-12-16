using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public interface ITranslationMemory2015 : ITranslationMemory, ITranslationProvider
	{
		TokenizerFlags TokenizerFlags
		{
			get;
			set;
		}

		WordCountFlags WordCountFlags
		{
			get;
			set;
		}
	}
}
