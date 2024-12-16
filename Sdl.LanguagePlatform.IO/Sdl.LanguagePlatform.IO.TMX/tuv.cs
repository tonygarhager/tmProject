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
	public class tuv
	{
		private object[] itemsField;

		private seg segField;

		private string langField;

		private string oencodingField;

		private string datatypeField;

		private string usagecountField;

		private string lastusagedateField;

		private string creationtoolField;

		private string creationtoolversionField;

		private string creationdateField;

		private string creationidField;

		private string changedateField;

		private string otmfField;

		private string changeidField;

		private string lang1Field;

		[XmlElement("note", typeof(note))]
		[XmlElement("prop", typeof(prop))]
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

		public seg seg
		{
			get
			{
				return segField;
			}
			set
			{
				segField = value;
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

		[XmlAttribute]
		public string datatype
		{
			get
			{
				return datatypeField;
			}
			set
			{
				datatypeField = value;
			}
		}

		[XmlAttribute]
		public string usagecount
		{
			get
			{
				return usagecountField;
			}
			set
			{
				usagecountField = value;
			}
		}

		[XmlAttribute]
		public string lastusagedate
		{
			get
			{
				return lastusagedateField;
			}
			set
			{
				lastusagedateField = value;
			}
		}

		[XmlAttribute]
		public string creationtool
		{
			get
			{
				return creationtoolField;
			}
			set
			{
				creationtoolField = value;
			}
		}

		[XmlAttribute]
		public string creationtoolversion
		{
			get
			{
				return creationtoolversionField;
			}
			set
			{
				creationtoolversionField = value;
			}
		}

		[XmlAttribute]
		public string creationdate
		{
			get
			{
				return creationdateField;
			}
			set
			{
				creationdateField = value;
			}
		}

		[XmlAttribute]
		public string creationid
		{
			get
			{
				return creationidField;
			}
			set
			{
				creationidField = value;
			}
		}

		[XmlAttribute]
		public string changedate
		{
			get
			{
				return changedateField;
			}
			set
			{
				changedateField = value;
			}
		}

		[XmlAttribute("o-tmf")]
		public string otmf
		{
			get
			{
				return otmfField;
			}
			set
			{
				otmfField = value;
			}
		}

		[XmlAttribute]
		public string changeid
		{
			get
			{
				return changeidField;
			}
			set
			{
				changeidField = value;
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
	}
}
