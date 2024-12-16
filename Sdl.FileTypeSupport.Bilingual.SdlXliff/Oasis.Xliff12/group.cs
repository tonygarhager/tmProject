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
	public class group
	{
		private contextgroup[] contextgroupField;

		private countgroup[] countgroupField;

		private note[] noteField;

		private XmlElement[] anyField;

		private object[] itemsField;

		private string idField;

		private string datatypeField;

		private string spaceField;

		private string restypeField;

		private string resnameField;

		private string extradataField;

		private string extypeField;

		private string helpidField;

		private string menuField;

		private string menuoptionField;

		private string menunameField;

		private string coordField;

		private string fontField;

		private string cssstyleField;

		private string styleField;

		private string exstyleField;

		private AttrType_YesNo translateField;

		private string reformatField;

		private string sizeunitField;

		private string maxwidthField;

		private string minwidthField;

		private string maxheightField;

		private string minheightField;

		private string maxbytesField;

		private string minbytesField;

		private string charclassField;

		private AttrType_YesNo mergedtransField;

		private XmlAttribute[] anyAttrField;

		[XmlElement("context-group")]
		public contextgroup[] contextgroup
		{
			get
			{
				return contextgroupField;
			}
			set
			{
				contextgroupField = value;
			}
		}

		[XmlElement("count-group")]
		public countgroup[] countgroup
		{
			get
			{
				return countgroupField;
			}
			set
			{
				countgroupField = value;
			}
		}

		[XmlElement("note")]
		public note[] note
		{
			get
			{
				return noteField;
			}
			set
			{
				noteField = value;
			}
		}

		[XmlAnyElement]
		public XmlElement[] Any
		{
			get
			{
				return anyField;
			}
			set
			{
				anyField = value;
			}
		}

		[XmlElement("bin-unit", typeof(binunit))]
		[XmlElement("group", typeof(group))]
		[XmlElement("trans-unit", typeof(transunit))]
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
		public string restype
		{
			get
			{
				return restypeField;
			}
			set
			{
				restypeField = value;
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
		public string extradata
		{
			get
			{
				return extradataField;
			}
			set
			{
				extradataField = value;
			}
		}

		[XmlAttribute]
		public string extype
		{
			get
			{
				return extypeField;
			}
			set
			{
				extypeField = value;
			}
		}

		[XmlAttribute("help-id", DataType = "NMTOKEN")]
		public string helpid
		{
			get
			{
				return helpidField;
			}
			set
			{
				helpidField = value;
			}
		}

		[XmlAttribute]
		public string menu
		{
			get
			{
				return menuField;
			}
			set
			{
				menuField = value;
			}
		}

		[XmlAttribute("menu-option")]
		public string menuoption
		{
			get
			{
				return menuoptionField;
			}
			set
			{
				menuoptionField = value;
			}
		}

		[XmlAttribute("menu-name")]
		public string menuname
		{
			get
			{
				return menunameField;
			}
			set
			{
				menunameField = value;
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

		[XmlAttribute]
		[DefaultValue(AttrType_YesNo.yes)]
		public AttrType_YesNo translate
		{
			get
			{
				return translateField;
			}
			set
			{
				translateField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue("yes")]
		public string reformat
		{
			get
			{
				return reformatField;
			}
			set
			{
				reformatField = value;
			}
		}

		[XmlAttribute("size-unit")]
		[DefaultValue("pixel")]
		public string sizeunit
		{
			get
			{
				return sizeunitField;
			}
			set
			{
				sizeunitField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string maxwidth
		{
			get
			{
				return maxwidthField;
			}
			set
			{
				maxwidthField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string minwidth
		{
			get
			{
				return minwidthField;
			}
			set
			{
				minwidthField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string maxheight
		{
			get
			{
				return maxheightField;
			}
			set
			{
				maxheightField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string minheight
		{
			get
			{
				return minheightField;
			}
			set
			{
				minheightField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string maxbytes
		{
			get
			{
				return maxbytesField;
			}
			set
			{
				maxbytesField = value;
			}
		}

		[XmlAttribute(DataType = "NMTOKEN")]
		public string minbytes
		{
			get
			{
				return minbytesField;
			}
			set
			{
				minbytesField = value;
			}
		}

		[XmlAttribute]
		public string charclass
		{
			get
			{
				return charclassField;
			}
			set
			{
				charclassField = value;
			}
		}

		[XmlAttribute("merged-trans")]
		[DefaultValue(AttrType_YesNo.no)]
		public AttrType_YesNo mergedtrans
		{
			get
			{
				return mergedtransField;
			}
			set
			{
				mergedtransField = value;
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

		public group()
		{
			translateField = AttrType_YesNo.yes;
			reformatField = "yes";
			sizeunitField = "pixel";
			mergedtransField = AttrType_YesNo.no;
		}
	}
}
