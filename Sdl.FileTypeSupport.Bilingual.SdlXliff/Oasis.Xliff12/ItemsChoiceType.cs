using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Oasis.Xliff12
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[XmlType(Namespace = "urn:oasis:names:tc:xliff:document:1.2", IncludeInSchema = false)]
	public enum ItemsChoiceType
	{
		[XmlEnum("count-group")]
		countgroup,
		glossary,
		note,
		reference,
		tool
	}
}
