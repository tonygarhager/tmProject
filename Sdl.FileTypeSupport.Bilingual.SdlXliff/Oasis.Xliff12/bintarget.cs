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
	[XmlRoot("bin-target", Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class bintarget
	{
		private object itemField;

		private string mimetypeField;

		private string stateField;

		private string statequalifierField;

		private string phasenameField;

		private string restypeField;

		private string resnameField;

		private XmlAttribute[] anyAttrField;

		[XmlElement("external-file", typeof(externalfile))]
		[XmlElement("internal-file", typeof(internalfile))]
		public object Item
		{
			get
			{
				return itemField;
			}
			set
			{
				itemField = value;
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
