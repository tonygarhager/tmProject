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
	public class g
	{
		private object[] itemsField;

		private string ctypeField;

		private AttrType_YesNo cloneField;

		private string idField;

		private string xidField;

		private string equivtextField;

		private XmlAttribute[] anyAttrField;

		[XmlText(typeof(string))]
		[XmlElement("bpt", typeof(bpt))]
		[XmlElement("bx", typeof(bx))]
		[XmlElement("ept", typeof(ept))]
		[XmlElement("ex", typeof(ex))]
		[XmlElement("g", typeof(g))]
		[XmlElement("it", typeof(it))]
		[XmlElement("mrk", typeof(mrk))]
		[XmlElement("ph", typeof(ph))]
		[XmlElement("x", typeof(x))]
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
		[DefaultValue(AttrType_YesNo.yes)]
		public AttrType_YesNo clone
		{
			get
			{
				return cloneField;
			}
			set
			{
				cloneField = value;
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

		public g()
		{
			cloneField = AttrType_YesNo.yes;
		}
	}
}
