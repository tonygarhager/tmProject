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
	public class ude
	{
		private map[] mapField;

		private string nameField;

		private string baseField;

		[XmlElement("map")]
		public map[] map
		{
			get
			{
				return mapField;
			}
			set
			{
				mapField = value;
			}
		}

		[XmlAttribute]
		public string name
		{
			get
			{
				return nameField;
			}
			set
			{
				nameField = value;
			}
		}

		[XmlAttribute]
		public string @base
		{
			get
			{
				return baseField;
			}
			set
			{
				baseField = value;
			}
		}
	}
}
