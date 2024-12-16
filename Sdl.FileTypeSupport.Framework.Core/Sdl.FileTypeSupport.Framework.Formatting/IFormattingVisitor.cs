namespace Sdl.FileTypeSupport.Framework.Formatting
{
	public interface IFormattingVisitor
	{
		void VisitBold(Bold item);

		void VisitItalic(Italic item);

		void VisitUnderline(Underline item);

		void VisitStrikethrough(Strikethrough item);

		void VisitFontName(FontName item);

		void VisitFontSize(FontSize item);

		void VisitBackgroundColor(BackgroundColor item);

		void VisitTextColor(TextColor item);

		void VisitTextPosition(TextPosition item);

		void VisitTextDirection(TextDirection item);

		void VisitUnknownFormatting(UnknownFormatting item);
	}
}
