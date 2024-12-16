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
	public class map
	{
		private string unicodeField;

		private string codeField;

		private string entField;

		private string substField;

		[XmlAttribute]
		public string unicode
		{
			get
			{
				return unicodeField;
			}
			set
			{
				unicodeField = value;
			}
		}

		[XmlAttribute]
		public string code
		{
			get
			{
				return codeField;
			}
			set
			{
				codeField = value;
			}
		}

		[XmlAttribute]
		public string ent
		{
			get
			{
				return entField;
			}
			set
			{
				entField = value;
			}
		}

		[XmlAttribute]
		public string subst
		{
			get
			{
				return substField;
			}
			set
			{
				substField = value;
			}
		}
	}
}
