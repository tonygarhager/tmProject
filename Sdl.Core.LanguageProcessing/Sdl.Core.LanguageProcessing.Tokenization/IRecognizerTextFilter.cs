namespace Sdl.Core.LanguageProcessing.Tokenization
{
	internal interface IRecognizerTextFilter
	{
		bool ExcludeText(string s);
	}
}
