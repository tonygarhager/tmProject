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
	[XmlRoot("bin-unit", Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class binunit
	{
		private binsource binsourceField;

		private bintarget bintargetField;

		private object[] itemsField;

		private XmlElement[] anyField;

		private string idField;

		private string mimetypeField;

		private AttrType_YesNo approvedField;

		private bool approvedFieldSpecified;

		private AttrType_YesNo translateField;

		private string reformatField;

		private string restypeField;

		private string resnameField;

		private string phasenameField;

		private XmlAttribute[] anyAttrField;

		[XmlElement("bin-source")]
		public binsource binsource
		{
			get
			{
				return binsourceField;
			}
			set
			{
				binsourceField = value;
			}
		}

		[XmlElement("bin-target")]
		public bintarget bintarget
		{
			get
			{
				return bintargetField;
			}
			set
			{
				bintargetField = value;
			}
		}

		[XmlElement("context-group", typeof(contextgroup))]
		[XmlElement("count-group", typeof(countgroup))]
		[XmlElement("note", typeof(note))]
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

		[XmlAttribute("mime-type")]
		public string mimetype
		{
			get
			{
				return mimetypeField;
			}
			set
			{
				mimetypeField = value;
			}
		}

		[XmlAttribute]
		public AttrType_YesNo approved
		{
			get
			{
				return approvedField;
			}
			set
			{
				approvedField = value;
			}
		}

		[XmlIgnore]
		public bool approvedSpecified
		{
			get
			{
				return approvedFieldSpecified;
			}
			set
			{
				approvedFieldSpecified = value;
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

		[XmlAttribute("phase-name")]
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

		public binunit()
		{
			translateField = AttrType_YesNo.yes;
			reformatField = "yes";
		}
	}
}
