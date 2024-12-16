using Sdl.FileTypeSupport.Framework;
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Oasis.Xliff12
{
	[XmlRoot("internal-file", Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class internalfile : IXmlSerializable
	{
		private const string FilePrefix = "file://";

		private string formField;

		private string crcField;

		private FileJanitor _valueFileJanitor;

		[XmlAttribute]
		public string form
		{
			get
			{
				return formField;
			}
			set
			{
				formField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string crc
		{
			get
			{
				return crcField;
			}
			set
			{
				crcField = value;
			}
		}

		[XmlText]
		public string Value
		{
			get
			{
				if (_valueFileJanitor == null || string.IsNullOrEmpty(_valueFileJanitor.FilePath))
				{
					return null;
				}
				return "file://" + _valueFileJanitor.FilePath;
			}
			set
			{
				if (!value.StartsWith("file://"))
				{
					throw new FileTypeSupportException("Internal error: internalfile value parameter must be a filename string!");
				}
				string text = value.Substring("file://".Length);
				if (string.IsNullOrEmpty(text) || !File.Exists(text))
				{
					throw new FileTypeSupportException($"Internal error: internalfile value file '{text}' does not exist!");
				}
				if (_valueFileJanitor != null)
				{
					_valueFileJanitor.DeleteFile();
				}
				_valueFileJanitor = new FileJanitor(text);
			}
		}

		public FileJanitor ValueFileJanitor
		{
			get
			{
				return _valueFileJanitor;
			}
			set
			{
				_valueFileJanitor = value;
			}
		}

		public XmlSchema GetSchema()
		{
			throw new NotImplementedException("Unexpected!");
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("internal-file");
			string text;
			do
			{
				text = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");
			}
			while (File.Exists(text));
			using (FileStream fileStream = File.Open(text, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
			{
				if (fileStream == null)
				{
					throw new FileTypeSupportException("Internal error: internalfile failed to create .zip from embedded file content: " + text);
				}
				byte[] array = new byte[1048576];
				int num = 0;
				while ((num = reader.ReadContentAsBase64(array, 0, array.Length)) > 0)
				{
					fileStream.Write(array, 0, num);
				}
				fileStream.Close();
			}
			reader.ReadEndElement();
			_valueFileJanitor = new FileJanitor(text);
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("form", form);
			using (FileStream fileStream = File.Open(_valueFileJanitor.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				if (fileStream == null)
				{
					throw new FileTypeSupportException("Internal error: internalfile failed to open the .zip file for an embedded file: " + _valueFileJanitor.FilePath);
				}
				byte[] array = new byte[57];
				bool flag = true;
				int num = 0;
				do
				{
					if (!flag)
					{
						writer.WriteString(Environment.NewLine);
					}
					else
					{
						flag = false;
					}
					num = fileStream.Read(array, 0, array.Length);
					writer.WriteBase64(array, 0, num);
				}
				while (num > 0);
				fileStream.Close();
			}
		}

		public static XmlSchemaComplexType GetSchema(XmlSchemaSet xs)
		{
			throw new NotImplementedException("GetSchema(XmlSchemaSet) not implemented, as we believe it is not needed!");
		}
	}
}
