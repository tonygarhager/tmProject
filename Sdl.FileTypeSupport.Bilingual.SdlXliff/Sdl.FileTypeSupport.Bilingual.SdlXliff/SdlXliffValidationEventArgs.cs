using System;
using System.Xml.Schema;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	public class SdlXliffValidationEventArgs : EventArgs
	{
		public XmlSeverityType Severity
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public int Line
		{
			get;
			set;
		}

		public int Offset
		{
			get;
			set;
		}

		public string FilePath
		{
			get;
			set;
		}

		public SdlXliffValidationEventArgs(XmlSeverityType severity, string message, int line, int offset, string filePath)
		{
			Severity = severity;
			Message = message;
			Line = line;
			Offset = offset;
			FilePath = filePath;
		}
	}
}
