namespace Sdl.Core.LanguageProcessing.Stemming
{
	public interface IStemmer
	{
		string Signature
		{
			get;
		}

		string Stem(string word);
	}
}
