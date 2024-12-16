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
	[XmlRoot("phase-group", Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class phasegroup
	{
		private phase[] phaseField;

		[XmlElement("phase")]
		public phase[] phase
		{
			get
			{
				return phaseField;
			}
			set
			{
				phaseField = value;
			}
		}
	}
}
