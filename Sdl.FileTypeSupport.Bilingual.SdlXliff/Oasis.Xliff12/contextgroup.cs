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
	[XmlRoot("context-group", Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class contextgroup
	{
		private context[] contextField;

		private string nameField;

		private string crcField;

		private string purposeField;

		[XmlElement("context")]
		public context[] context
		{
			get
			{
				return contextField;
			}
			set
			{
				contextField = value;
			}
		}

		[XmlAttribute]
		public string name
		{
			get
			{
				return nameField;
			}
			set
			{
				nameField = value;
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

		[XmlAttribute]
		public string purpose
		{
			get
			{
				return purposeField;
			}
			set
			{
				purposeField = value;
			}
		}
	}
}
