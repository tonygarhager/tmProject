using System.IO;
using System.Xml;

namespace Sdl.FileTypeSupport.Framework.Native
{
	public class XmlStringWriter : XmlTextWriter
	{
		public XmlStringWriter(TextWriter w)
			: base(w)
		{
		}

		public override void WriteStartDocument()
		{
		}

		public override void WriteStartDocument(bool standalone)
		{
		}
	}
}
