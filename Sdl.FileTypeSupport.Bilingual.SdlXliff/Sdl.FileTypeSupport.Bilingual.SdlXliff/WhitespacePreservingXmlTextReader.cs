using System.IO;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class WhitespacePreservingXmlTextReader : XmlTextReader
	{
		private long _transUnitCount;

		private bool _firstElementRead;

		public long TransUnitCount => _transUnitCount;

		public override XmlNodeType NodeType
		{
			get
			{
				XmlNodeType xmlNodeType = base.NodeType;
				if (!_firstElementRead && xmlNodeType == XmlNodeType.Element)
				{
					_firstElementRead = true;
				}
				if (_firstElementRead && (xmlNodeType == XmlNodeType.SignificantWhitespace || xmlNodeType == XmlNodeType.Whitespace))
				{
					xmlNodeType = XmlNodeType.Text;
				}
				return xmlNodeType;
			}
		}

		public WhitespacePreservingXmlTextReader(TextReader r)
			: base(r)
		{
		}

		public WhitespacePreservingXmlTextReader(Stream s)
			: base(s)
		{
		}

		public WhitespacePreservingXmlTextReader(string filePath)
			: base(filePath)
		{
		}

		public override bool Read()
		{
			bool num = base.Read();
			if (num && NodeType == XmlNodeType.Element && Name == "trans-unit")
			{
				_transUnitCount++;
			}
			return num;
		}
	}
}
