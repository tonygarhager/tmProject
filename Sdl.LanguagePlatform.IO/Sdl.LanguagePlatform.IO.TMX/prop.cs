using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.IO.TMX
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "http://www.lisa.org/tmx14", IsNullable = false)]
	public class prop
	{
		private string typeField;

		private string langField;

		private string oencodingField;

		private string lang1Field;

		private string valueField;

		[XmlAttribute]
		public string type
		{
			get
			{
				return typeField;
			}
			set
			{
				typeField = value;
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

		[XmlAttribute("o-encoding")]
		public string oencoding
		{
			get
			{
				return oencodingField;
			}
			set
			{
				oencodingField = value;
			}
		}

		[XmlAttribute("lang")]
		public string lang1
		{
			get
			{
				return lang1Field;
			}
			set
			{
				lang1Field = value;
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
	}
}
