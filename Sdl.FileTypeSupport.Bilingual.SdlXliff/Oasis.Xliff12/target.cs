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
	public class target
	{
		private object[] itemsField;

		private string stateField;

		private string statequalifierField;

		private string phasenameField;

		private string langField;

		private string resnameField;

		private string coordField;

		private string fontField;

		private string cssstyleField;

		private string styleField;

		private string exstyleField;

		private AttrType_YesNo equivtransField;

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
		public string state
		{
			get
			{
				return stateField;
			}
			set
			{
				stateField = value;
			}
		}

		[XmlAttribute("state-qualifier")]
		public string statequalifier
		{
			get
			{
				return statequalifierField;
			}
			set
			{
				statequalifierField = value;
			}
		}

		[XmlAttribute("phase-name", DataType = "NMTOKEN")]
		public string phasename
		{
			get
			{
				return phasenameField;
			}
			set
			{
				phasenameField = value;
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

		[XmlAttribute]
		public string resname
		{
			get
			{
				return resnameField;
			}
			set
			{
				resnameField = value;
			}
		}

		[XmlAttribute]
		public string coord
		{
			get
			{
				return coordField;
			}
			set
			{
				coordField = value;
			}
		}

		[XmlAttribute]
		public string font
		{
			get
			{
				return fontField;
			}
			set
			{
				fontField = value;
			}
		}

		[XmlAttribute("css-style")]
		public string cssstyle
		{
			get
			{
				return cssstyleField;
			}
			set
			{
				cssstyleField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string style
		{
			get
			{
				return styleField;
			}
			set
			{
				styleField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string exstyle
		{
			get
			{
				return exstyleField;
			}
			set
			{
				exstyleField = value;
			}
		}

		[XmlAttribute("equiv-trans")]
		[DefaultValue(AttrType_YesNo.yes)]
		public AttrType_YesNo equivtrans
		{
			get
			{
				return equivtransField;
			}
			set
			{
				equivtransField = value;
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

		public target()
		{
			equivtransField = AttrType_YesNo.yes;
		}
	}
}
