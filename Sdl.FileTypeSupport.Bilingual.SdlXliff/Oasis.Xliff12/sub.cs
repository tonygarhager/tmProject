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
	public class sub
	{
		private object[] itemsField;

		private string datatypeField;

		private string ctypeField;

		private string xidField;

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
	}
}
