using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[Serializable]
	public class CultureString : ICloneable
	{
		private CultureInfo _culture;

		private string _text;

		[XmlIgnore]
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = (value ?? throw new ArgumentNullException());
			}
		}

		[XmlAttribute(AttributeName = "xml:lang")]
		public string CultureName
		{
			get
			{
				return _culture.Name;
			}
			set
			{
				_culture = (string.IsNullOrEmpty(value) ? CultureInfo.InvariantCulture : CultureInfoExtensions.GetCultureInfo(value));
			}
		}

		[XmlText]
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public CultureString()
		{
			_text = string.Empty;
			_culture = CultureInfo.InvariantCulture;
		}

		public CultureString(CultureInfo culture, string text)
		{
			_text = text;
			_culture = culture;
		}

		public CultureString(CultureString other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			_text = other._text;
			_culture = other._culture;
		}

		public object Clone()
		{
			return new CultureString(this);
		}
	}
}
