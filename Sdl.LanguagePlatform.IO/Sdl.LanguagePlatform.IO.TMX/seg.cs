using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.IO.TMX
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "http://www.lisa.org/tmx14", IsNullable = false)]
	public class seg
	{
		private object[] itemsField;

		private string[] textField;

		[XmlElement("bpt", typeof(bpt))]
		[XmlElement("ept", typeof(ept))]
		[XmlElement("hi", typeof(hi))]
		[XmlElement("it", typeof(it))]
		[XmlElement("ph", typeof(ph))]
		[XmlElement("ut", typeof(ut))]
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

		[XmlText]
		public string[] Text
		{
			get
			{
				return textField;
			}
			set
			{
				textField = value;
			}
		}
	}
}
