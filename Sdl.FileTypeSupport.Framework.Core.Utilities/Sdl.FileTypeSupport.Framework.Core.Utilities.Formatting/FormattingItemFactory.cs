using Sdl.FileTypeSupport.Framework.Formatting;

namespace Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting
{
	public class FormattingItemFactory : IFormattingItemFactory
	{
		public virtual IFormattingItem CreateFormattingItem(string name, string value)
		{
			IFormattingItem formattingItem = null;
			if (name == Bold.Name)
			{
				formattingItem = new Bold();
			}
			else if (name == Italic.Name)
			{
				formattingItem = new Italic();
			}
			else if (name == Underline.Name)
			{
				formattingItem = new Underline();
			}
			else if (name == Strikethrough.Name)
			{
				formattingItem = new Strikethrough();
			}
			else if (name == FontName.Name)
			{
				formattingItem = new FontName();
			}
			else if (name == FontSize.Name)
			{
				formattingItem = new FontSize();
			}
			else if (name == BackgroundColor.Name)
			{
				formattingItem = new BackgroundColor();
			}
			else if (name == TextColor.Name)
			{
				formattingItem = new TextColor();
			}
			else if (name == TextPosition.Name)
			{
				formattingItem = new TextPosition();
			}
			else
			{
				if (!(name == TextDirection.Name))
				{
					return new UnknownFormatting(name, value);
				}
				formattingItem = new TextDirection();
			}
			formattingItem.StringValue = value;
			return formattingItem;
		}

		public IFormattingGroup CreateFormatting()
		{
			return new FormattingGroup();
		}
	}
}
