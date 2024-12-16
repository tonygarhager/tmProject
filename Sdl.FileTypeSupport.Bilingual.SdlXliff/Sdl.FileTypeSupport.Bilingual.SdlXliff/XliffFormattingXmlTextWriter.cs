using System.IO;
using System.Text;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class XliffFormattingXmlTextWriter : XmlTextWriter
	{
		public XliffFormattingXmlTextWriter(TextWriter output)
			: base(output)
		{
		}

		public XliffFormattingXmlTextWriter(string filename, Encoding encoding)
			: base(filename, encoding)
		{
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			if (localName == "trans-unit")
			{
				WriteWhitespace("\r\n");
			}
			base.WriteStartElement(prefix, localName, ns);
		}
	}
}
