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
	public class count
	{
		private string counttypeField;

		private string phasenameField;

		private string unitField;

		private string valueField;

		[XmlAttribute("count-type")]
		public string counttype
		{
			get
			{
				return counttypeField;
			}
			set
			{
				counttypeField = value;
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

		[XmlAttribute]
		[DefaultValue("word")]
		public string unit
		{
			get
			{
				return unitField;
			}
			set
			{
				unitField = value;
			}
		}

		[XmlText]
		public string Value
		{
			get
			{
				return valueField;
			}
			set
			{
				valueField = value;
			}
		}

		public count()
		{
			unitField = "word";
		}
	}
}
