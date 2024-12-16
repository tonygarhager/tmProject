using System.Collections.Generic;

namespace Sdl.LanguagePlatform.Stat.WordAlignment
{
	public interface IBilingualPhraseCounter2
	{
		void CountBilingualPhrase(IList<int> srcPhrase, IList<int> trgPhrase);

		BilingualDictionaryFile2 FinishCounting();
	}
}
