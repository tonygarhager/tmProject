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
	public class tu
	{
		private object[] itemsField;

		private tuv[] tuvField;

		private string tuidField;

		private string oencodingField;

		private string datatypeField;

		private string usagecountField;

		private string lastusagedateField;

		private string creationtoolField;

		private string creationtoolversionField;

		private string creationdateField;

		private string creationidField;

		private string changedateField;

		private tuSegtype segtypeField;

		private bool segtypeFieldSpecified;

		private string changeidField;

		private string otmfField;

		private string srclangField;

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

		[XmlElement("tuv")]
		public tuv[] tuv
		{
			get
			{
				return tuvField;
			}
			set
			{
				tuvField = value;
			}
		}

		[XmlAttribute]
		public string tuid
		{
			get
			{
				return tuidField;
			}
			set
			{
				tuidField = value;
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

		[XmlAttribute]
		public tuSegtype segtype
		{
			get
			{
				return segtypeField;
			}
			set
			{
				segtypeField = value;
			}
		}

		[XmlIgnore]
		public bool segtypeSpecified
		{
			get
			{
				return segtypeFieldSpecified;
			}
			set
			{
				segtypeFieldSpecified = value;
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
		public string srclang
		{
			get
			{
				return srclangField;
			}
			set
			{
				srclangField = value;
			}
		}
	}
}
