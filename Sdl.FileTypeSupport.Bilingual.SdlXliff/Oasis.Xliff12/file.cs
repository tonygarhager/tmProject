using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
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
	public class file
	{
		private header headerField;

		private object[] bodyField;

		private string originalField;

		private string sourcelanguageField;

		private string datatypeField;

		private string toolidField;

		private DateTime dateField;

		private bool dateFieldSpecified;

		private string spaceField;

		private string categoryField;

		private string targetlanguageField;

		private string productnameField;

		private string productversionField;

		private string buildnumField;

		private XmlAttribute[] anyAttrField;

		public header header
		{
			get
			{
				return headerField;
			}
			set
			{
				headerField = value;
			}
		}

		[XmlArrayItem("bin-unit", typeof(binunit), IsNullable = false)]
		[XmlArrayItem("group", typeof(group), IsNullable = false)]
		[XmlArrayItem("trans-unit", typeof(transunit), IsNullable = false)]
		public object[] body
		{
			get
			{
				return bodyField;
			}
			set
			{
				bodyField = value;
			}
		}

		[XmlAttribute]
		public string original
		{
			get
			{
				return originalField;
			}
			set
			{
				originalField = value;
			}
		}

		[XmlAttribute("source-language", DataType = "language")]
		public string sourcelanguage
		{
			get
			{
				return sourcelanguageField;
			}
			set
			{
				sourcelanguageField = value;
			}
		}

		[XmlAttribute]
		public string datatype
		{
			get
			{
				return datatypeField;
			}
			set
			{
				datatypeField = value;
			}
		}

		[XmlAttribute("tool-id")]
		public string toolid
		{
			get
			{
				return toolidField;
			}
			set
			{
				toolidField = value;
			}
		}

		[XmlAttribute]
		public DateTime date
		{
			get
			{
				return dateField;
			}
			set
			{
				dateField = value;
			}
		}

		[XmlIgnore]
		public bool dateSpecified
		{
			get
			{
				return dateFieldSpecified;
			}
			set
			{
				dateFieldSpecified = value;
			}
		}

		[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
		public string space
		{
			get
			{
				return spaceField;
			}
			set
			{
				spaceField = value;
			}
		}

		[XmlAttribute]
		public string category
		{
			get
			{
				return categoryField;
			}
			set
			{
				categoryField = value;
			}
		}

		[XmlAttribute("target-language", DataType = "language")]
		public string targetlanguage
		{
			get
			{
				return targetlanguageField;
			}
			set
			{
				targetlanguageField = value;
			}
		}

		[XmlAttribute("product-name")]
		public string productname
		{
			get
			{
				return productnameField;
			}
			set
			{
				productnameField = value;
			}
		}

		[XmlAttribute("product-version")]
		public string productversion
		{
			get
			{
				return productversionField;
			}
			set
			{
				productversionField = value;
			}
		}

		[XmlAttribute("build-num")]
		public string buildnum
		{
			get
			{
				return buildnumField;
			}
			set
			{
				buildnumField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return anyAttrField;
			}
			set
			{
				anyAttrField = value;
			}
		}
	}
}
