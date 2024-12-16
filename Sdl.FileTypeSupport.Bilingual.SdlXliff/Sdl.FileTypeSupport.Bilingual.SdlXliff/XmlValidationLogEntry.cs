namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class XmlValidationLogEntry
	{
		public string Message
		{
			get;
			set;
		}

		public int LineNo
		{
			get;
			set;
		}

		public int LinePos
		{
			get;
			set;
		}

		public XmlValidationLogEntry(string message, int lineNo, int linePos)
		{
			Message = message;
			LineNo = lineNo;
			LinePos = linePos;
		}
	}
}
