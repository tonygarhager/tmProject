using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Oasis.Xliff12
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class ex
	{
		private string ridField;

		private string idField;

		private string xidField;

		private string equivtextField;

		private XmlAttribute[] anyAttrField;

		[XmlAttribute(DataType = "NMTOKEN")]
		public string rid
		{
			get
			{
				return ridField;
			}
			set
			{
				ridField = value;
			}
		}

		[XmlAttribute]
		public string id
		{
			get
			{
				return idField;
			}
			set
			{
				idField = value;
			}
		}

		[XmlAttribute]
		public string xid
		{
			get
			{
				return xidField;
			}
			set
			{
				xidField = value;
			}
		}

		[XmlAttribute("equiv-text")]
		public string equivtext
		{
			get
			{
				return equivtextField;
			}
			set
			{
				equivtextField = value;
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
