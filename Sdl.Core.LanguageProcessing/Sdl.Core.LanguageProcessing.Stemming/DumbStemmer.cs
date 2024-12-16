using Sdl.LanguagePlatform.Core;

namespace Sdl.Core.LanguageProcessing.Stemming
{
	public class DumbStemmer : IStemmer
	{
		public string Signature => "DumbStemmer1";

		public string Stem(string word)
		{
			return CharacterProperties.ToBase(word).ToLowerInvariant();
		}
	}
}
