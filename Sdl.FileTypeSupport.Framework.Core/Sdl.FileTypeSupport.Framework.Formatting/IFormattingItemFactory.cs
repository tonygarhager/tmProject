namespace Sdl.FileTypeSupport.Framework.Formatting
{
	public interface IFormattingItemFactory
	{
		IFormattingItem CreateFormattingItem(string name, string value);

		IFormattingGroup CreateFormatting();
	}
}
