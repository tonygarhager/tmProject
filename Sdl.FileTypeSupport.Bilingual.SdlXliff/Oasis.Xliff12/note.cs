using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Oasis.Xliff12
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class note
	{
		private string langField;

		private string priorityField;

		private string fromField;

		private AttrType_annotates annotatesField;

		private string valueField;

		[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string lang
		{
			get
			{
				return langField;
			}
			set
			{
				langField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue("1")]
		public string priority
		{
			get
			{
				return priorityField;
			}
			set
			{
				priorityField = value;
			}
		}

		[XmlAttribute]
		public string from
		{
			get
			{
				return fromField;
			}
			set
			{
				fromField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(AttrType_annotates.general)]
		public AttrType_annotates annotates
		{
			get
			{
				return annotatesField;
			}
			set
			{
				annotatesField = value;
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

		public note()
		{
			priorityField = "1";
			annotatesField = AttrType_annotates.general;
		}
	}
}
