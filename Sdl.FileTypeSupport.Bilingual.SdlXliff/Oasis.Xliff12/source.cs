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
	public class source
	{
		private object[] itemsField;

		private string langField;

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
