using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Oasis.Xliff12
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class header
	{
		private ElemType_ExternalReference sklField;

		private phasegroup phasegroupField;

		private object[] itemsField;

		private ItemsChoiceType[] itemsElementNameField;

		private XmlElement[] anyField;

		public ElemType_ExternalReference skl
		{
			get
			{
				return sklField;
			}
			set
			{
				sklField = value;
			}
		}

		[XmlElement("phase-group")]
		public phasegroup phasegroup
		{
			get
			{
				return phasegroupField;
			}
			set
			{
				phasegroupField = value;
			}
		}

		[XmlElement("count-group", typeof(countgroup))]
		[XmlElement("glossary", typeof(ElemType_ExternalReference))]
		[XmlElement("note", typeof(note))]
		[XmlElement("reference", typeof(ElemType_ExternalReference))]
		[XmlElement("tool", typeof(tool))]
		[XmlChoiceIdentifier("ItemsElementName")]
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

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ItemsChoiceType[] ItemsElementName
		{
			get
			{
				return itemsElementNameField;
			}
			set
			{
				itemsElementNameField = value;
			}
		}

		[XmlAnyElement]
		public XmlElement[] Any
		{
			get
			{
				return anyField;
			}
			set
			{
				anyField = value;
			}
		}
	}
}
