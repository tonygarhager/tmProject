using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public interface IBilingualPhraseCounter
	{
		void CountBilingualPhrase(IList<int> srcPhrase, IList<int> trgPhrase);

		BilingualDictionaryFile FinishCounting();
	}
}
