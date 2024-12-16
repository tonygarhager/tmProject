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
	public class ph
	{
		private object[] itemsField;

		private string ctypeField;

		private string crcField;

		private AttrType_assoc assocField;

		private bool assocFieldSpecified;

		private string idField;

		private string xidField;

		private string equivtextField;

		private XmlAttribute[] anyAttrField;

		[XmlText(typeof(string))]
		[XmlElement("sub", typeof(sub))]
		public object[] Items
		{
			get
			{
				return itemsField;
			}
			set
			{
				itemsField = value;
			}
		}

		[XmlAttribute]
		public string ctype
		{
			get
			{
				return ctypeField;
			}
			set
			{
				ctypeField = value;
			}
		}

		[XmlAttribute]
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
		public AttrType_assoc assoc
		{
			get
			{
				return assocField;
			}
			set
			{
				assocField = value;
			}
		}

		[XmlIgnore]
		public bool assocSpecified
		{
			get
			{
				return assocFieldSpecified;
			}
			set
			{
				assocFieldSpecified = value;
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
