using Sdl.Core.LanguageProcessing;

namespace Sdl.LanguagePlatform.Lingua
{
	public class StringIntTrie : Trie<char, int>
	{
		public void Add(string s, int key)
		{
			Add(s.ToCharArray(), key);
		}

		public bool Contains(string s, out int key)
		{
			return Contains(s.ToCharArray(), out key);
		}
	}
}
