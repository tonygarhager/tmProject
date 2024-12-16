using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Sdl.FileTypeSupport.Bilingual.SdlXliff
{
	internal class XmlValidationLogger
	{
		private readonly string _xmlLogFilePath;

		public List<XmlValidationLogEntry> LogEntries
		{
			get;
			set;
		} = new List<XmlValidationLogEntry>();


		public XmlValidationLogger(string xmlLogFilePath)
		{
			_xmlLogFilePath = xmlLogFilePath;
		}

		public virtual void WriteLogFile()
		{
			FileStream fileStream = new FileStream(_xmlLogFilePath, FileMode.Create);
			XmlWriter xmlWriter = XmlWriter.Create(fileStream);
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("xliff-validation-report");
			foreach (XmlValidationLogEntry logEntry in LogEntries)
			{
				xmlWriter.WriteStartElement("log-entry");
				xmlWriter.WriteStartElement("message");
				xmlWriter.WriteString(logEntry.Message);
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("line-number");
				xmlWriter.WriteString(logEntry.LineNo.ToString());
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("line-position");
				xmlWriter.WriteString(logEntry.LinePos.ToString());
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			xmlWriter.Close();
			fileStream.Close();
		}
	}
}
