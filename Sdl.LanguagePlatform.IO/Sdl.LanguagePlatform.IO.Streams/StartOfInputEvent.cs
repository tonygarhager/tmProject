using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.IO.Streams
{
	public class StartOfInputEvent : Event
	{
		private CultureInfo _SourceCulture;

		private CultureInfo _TargetCulture;

		protected DateTime _CreationDate;

		private string _CreationUser;

		private Dictionary<string, string> _Properties;

		[XmlIgnore]
		public CultureInfo SourceCulture
		{
			get
			{
				return _SourceCulture;
			}
			set
			{
				_SourceCulture = value;
			}
		}

		public string SourceCultureName
		{
			get
			{
				return _SourceCulture?.Name;
			}
			set
			{
				_SourceCulture = CultureInfoExtensions.GetCultureInfo(value);
			}
		}

		public Dictionary<string, string> Properties
		{
			get
			{
				return _Properties;
			}
			set
			{
				_Properties = value;
			}
		}

		[XmlIgnore]
		public CultureInfo TargetCulture
		{
			get
			{
				return _TargetCulture;
			}
			set
			{
				_TargetCulture = value;
			}
		}

		public string TargetCultureName
		{
			get
			{
				return _TargetCulture?.Name;
			}
			set
			{
				_TargetCulture = CultureInfoExtensions.GetCultureInfo(value);
			}
		}

		public DateTime CreationDate
		{
			get
			{
				return _CreationDate;
			}
			set
			{
				_CreationDate = DateTimeUtilities.Normalize(value);
			}
		}

		public string CreationUser
		{
			get
			{
				return _CreationUser;
			}
			set
			{
				_CreationUser = value;
			}
		}
	}
}
