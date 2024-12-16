using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.IO.TMX
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[XmlType(AnonymousType = true)]
	public enum headerSegtype
	{
		block,
		paragraph,
		sentence,
		phrase
	}
}
