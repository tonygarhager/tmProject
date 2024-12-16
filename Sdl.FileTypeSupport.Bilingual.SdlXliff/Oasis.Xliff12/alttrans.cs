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
	[XmlRoot("alt-trans", Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class alttrans
	{
		private source sourceField;

		private segsource segsourceField;

		private target targetField;

		private contextgroup[] contextgroupField;

		private note[] noteField;

		private XmlElement[] anyField;

		private string matchqualityField;

		private string toolidField;

		private string crcField;

		private string langField;

		private string originField;

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

		private string midField;

		private string coordField;

		private string fontField;

		private string cssstyleField;

		private string styleField;

		private string exstyleField;

		private string phasenameField;

		private string alttranstypeField;

		private XmlAttribute[] anyAttrField;

		public source source
		{
			get
			{
				return sourceField;
			}
			set
			{
				sourceField = value;
			}
		}

		[XmlElement("seg-source")]
		public segsource segsource
		{
			get
			{
				return segsourceField;
			}
			set
			{
				segsourceField = value;
			}
		}

		public target target
		{
			get
			{
				return targetField;
			}
			set
			{
				targetField = value;
			}
		}

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

		[XmlAttribute("match-quality")]
		public string matchquality
		{
			get
			{
				return matchqualityField;
			}
			set
			{
				matchqualityField = value;
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
		public string origin
		{
			get
			{
				return originField;
			}
			set
			{
				originField = value;
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

		[XmlAttribute(DataType = "NMTOKEN")]
		public string mid
		{
			get
			{
				return midField;
			}
			set
			{
				midField = value;
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

		[XmlAttribute]
		[DefaultValue("proposal")]
		public string alttranstype
		{
			get
			{
				return alttranstypeField;
			}
			set
			{
				alttranstypeField = value;
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

		public alttrans()
		{
			alttranstypeField = "proposal";
		}
	}
}
