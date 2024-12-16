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
	public class tmx
	{
		private header headerField;

		private tu[] bodyField;

		private string versionField;

		public header header
		{
			get
			{
				return headerField;
			}
			set
			{
				headerField = value;
			}
		}

		[XmlArrayItem("tu", IsNullable = false)]
		public tu[] body
		{
			get
			{
				return bodyField;
			}
			set
			{
				bodyField = value;
			}
		}

		[XmlAttribute]
		public string version
		{
			get
			{
				return versionField;
			}
			set
			{
				versionField = value;
			}
		}

		public tmx()
		{
			versionField = "1.4";
		}
	}
}
