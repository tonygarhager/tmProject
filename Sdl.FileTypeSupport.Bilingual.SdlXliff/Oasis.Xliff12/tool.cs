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
	public class tool
	{
		private XmlNode[] anyField;

		private string toolidField;

		private string toolnameField;

		private string toolversionField;

		private string toolcompanyField;

		private XmlAttribute[] anyAttrField;

		[XmlText]
		[XmlAnyElement]
		public XmlNode[] Any
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

		[XmlAttribute("tool-name")]
		public string toolname
		{
			get
			{
				return toolnameField;
			}
			set
			{
				toolnameField = value;
			}
		}

		[XmlAttribute("tool-version")]
		public string toolversion
		{
			get
			{
				return toolversionField;
			}
			set
			{
				toolversionField = value;
			}
		}

		[XmlAttribute("tool-company")]
		public string toolcompany
		{
			get
			{
				return toolcompanyField;
			}
			set
			{
				toolcompanyField = value;
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
