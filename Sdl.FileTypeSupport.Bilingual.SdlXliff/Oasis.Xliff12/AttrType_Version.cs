using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Oasis.Xliff12
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[XmlType(Namespace = "urn:oasis:names:tc:xliff:document:1.2")]
	public enum AttrType_Version
	{
		[XmlEnum("1.2")]
		Item12,
		[XmlEnum("1.1")]
		Item11,
		[XmlEnum("1.0")]
		Item10
	}
}
