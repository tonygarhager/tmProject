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
	public class header
	{
		private object[] itemsField;

		private string creationtoolField;

		private string creationtoolversionField;

		private headerSegtype segtypeField;

		private string otmfField;

		private string adminlangField;

		private string srclangField;

		private string datatypeField;

		private string oencodingField;

		private string creationdateField;

		private string creationidField;

		private string changedateField;

		private string changeidField;

		[XmlElement("note", typeof(note))]
		[XmlElement("prop", typeof(prop))]
		[XmlElement("ude", typeof(ude))]
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
		public headerSegtype segtype
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
		public string adminlang
		{
			get
			{
				return adminlangField;
			}
			set
			{
				adminlangField = value;
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
	}
}
