namespace Sdl.Core.LanguageProcessing.Segmentation.Serialization
{
	internal class TextToken : Token
	{
		public string Text
		{
			get;
		}

		public TextToken(string text)
		{
			Text = text;
		}
	}
}
