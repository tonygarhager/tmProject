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
	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class context
	{
		private string contexttypeField;

		private AttrType_YesNo matchmandatoryField;

		private string crcField;

		private string valueField;

		[XmlAttribute("context-type")]
		public string contexttype
		{
			get
			{
				return contexttypeField;
			}
			set
			{
				contexttypeField = value;
			}
		}

		[XmlAttribute("match-mandatory")]
		[DefaultValue(AttrType_YesNo.no)]
		public AttrType_YesNo matchmandatory
		{
			get
			{
				return matchmandatoryField;
			}
			set
			{
				matchmandatoryField = value;
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
				return valueField;
			}
			set
			{
				valueField = value;
			}
		}

		public context()
		{
			matchmandatoryField = AttrType_YesNo.no;
		}
	}
}
