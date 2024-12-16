using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.Formatting;
using System.Drawing;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff.Preview
{
	internal class XliffXmlFormattingVisitor : IFormattingVisitor
	{
		private const string FONT_WEIGHT = "font-weight";

		private const string FONT_STYLE = "font-style";

		private const string FONT_FAMILY = "font-family";

		private const string FONT_SIZE = "font-size";

		private const string TEXT_DECORATION = "text-decoration";

		private const string TEXT_COLOR = "text-color";

		private const string TEXT_POSITION = "vertical-align";

		private const string BACK_COLOR = "background-color";

		private const string TEXT_DIRECTION = "text-direction";

		private XmlWriter _XmlWriter;

		public XliffXmlFormattingVisitor(XmlWriter xmlWriter)
		{
			_XmlWriter = xmlWriter;
		}

		public void VisitBold(Bold item)
		{
			if (item.Value)
			{
				_XmlWriter.WriteAttributeString("font-weight", "bold");
			}
		}

		public void VisitItalic(Italic item)
		{
			if (item.Value)
			{
				_XmlWriter.WriteAttributeString("font-style", "italic");
			}
		}

		public void VisitUnderline(Underline item)
		{
			if (item.Value)
			{
				_XmlWriter.WriteAttributeString("text-decoration", "underline");
			}
		}

		public void VisitFontName(FontName item)
		{
			_XmlWriter.WriteAttributeString("font-family", item.Value);
		}

		public void VisitFontSize(FontSize item)
		{
			_XmlWriter.WriteAttributeString("font-size", item.Value.ToString());
		}

		public void VisitBackgroundColor(BackgroundColor item)
		{
			_XmlWriter.WriteAttributeString("background-color", ColorTranslator.ToHtml(item.Value));
		}

		public void VisitTextColor(TextColor item)
		{
			_XmlWriter.WriteAttributeString("text-color", ColorTranslator.ToHtml(item.Value));
		}

		public void VisitTextPosition(TextPosition item)
		{
			if (item.Value == TextPosition.SuperSub.Superscript)
			{
				_XmlWriter.WriteAttributeString("vertical-align", "super");
			}
			else if (item.Value == TextPosition.SuperSub.Subscript)
			{
				_XmlWriter.WriteAttributeString("vertical-align", "sub");
			}
		}

		public void VisitTextDirection(TextDirection item)
		{
			if (item.Direction == Direction.LeftToRight)
			{
				_XmlWriter.WriteAttributeString("text-direction", "ltr");
			}
			else if (item.Direction == Direction.RightToLeft)
			{
				_XmlWriter.WriteAttributeString("text-direction", "rtl");
			}
		}

		public void VisitUnknownFormatting(UnknownFormatting item)
		{
		}

		public void VisitStrikethrough(Strikethrough item)
		{
			if (item.Value)
			{
				_XmlWriter.WriteAttributeString("text-decoration", "line-through");
			}
		}
	}
}
