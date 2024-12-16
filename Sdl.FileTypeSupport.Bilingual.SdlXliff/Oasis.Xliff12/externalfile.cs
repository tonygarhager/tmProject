using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Oasis.Xliff12
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot("external-file", Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class externalfile
	{
		private string hrefField;

		private string crcField;

		private string uidField;

		[XmlAttribute]
		public string href
		{
			get
			{
				return hrefField;
			}
			set
			{
				hrefField = value;
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

		[XmlAttribute(DataType = "NMTOKEN")]
		public string uid
		{
			get
			{
				return uidField;
			}
			set
			{
				uidField = value;
			}
		}
	}
}
