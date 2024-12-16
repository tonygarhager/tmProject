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
	[XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2", IsNullable = false)]
	public class phase
	{
		private note[] noteField;

		private string phasenameField;

		private string processnameField;

		private string companynameField;

		private string toolidField;

		private DateTime dateField;

		private bool dateFieldSpecified;

		private string jobidField;

		private string contactnameField;

		private string contactemailField;

		private string contactphoneField;

		[XmlElement("note")]
		public note[] note
		{
			get
			{
				return noteField;
			}
			set
			{
				noteField = value;
			}
		}

		[XmlAttribute("phase-name")]
		public string phasename
		{
			get
			{
				return phasenameField;
			}
			set
			{
				phasenameField = value;
			}
		}

		[XmlAttribute("process-name")]
		public string processname
		{
			get
			{
				return processnameField;
			}
			set
			{
				processnameField = value;
			}
		}

		[XmlAttribute("company-name")]
		public string companyname
		{
			get
			{
				return companynameField;
			}
			set
			{
				companynameField = value;
			}
		}

		[XmlAttribute("tool-id")]
		public string toolid
		{
			get
			{
				return toolidField;
			}
			set
			{
				toolidField = value;
			}
		}

		[XmlAttribute]
		public DateTime date
		{
			get
			{
				return dateField;
			}
			set
			{
				dateField = value;
			}
		}

		[XmlIgnore]
		public bool dateSpecified
		{
			get
			{
				return dateFieldSpecified;
			}
			set
			{
				dateFieldSpecified = value;
			}
		}

		[XmlAttribute("job-id")]
		public string jobid
		{
			get
			{
				return jobidField;
			}
			set
			{
				jobidField = value;
			}
		}

		[XmlAttribute("contact-name")]
		public string contactname
		{
			get
			{
				return contactnameField;
			}
			set
			{
				contactnameField = value;
			}
		}

		[XmlAttribute("contact-email")]
		public string contactemail
		{
			get
			{
				return contactemailField;
			}
			set
			{
				contactemailField = value;
			}
		}

		[XmlAttribute("contact-phone")]
		public string contactphone
		{
			get
			{
				return contactphoneField;
			}
			set
			{
				contactphoneField = value;
			}
		}
	}
}
