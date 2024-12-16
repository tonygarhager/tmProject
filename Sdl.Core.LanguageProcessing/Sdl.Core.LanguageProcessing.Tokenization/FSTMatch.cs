using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class FSTMatch : Match
	{
		public string Output
		{
			get;
			set;
		}

		public FSTMatch(int index, int length, string output)
			: base(index, length)
		{
			Output = output;
		}
	}
}
