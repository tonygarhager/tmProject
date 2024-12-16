using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core.Segmentation
{
	[DataContract]
	public class LocalizedString : IEnumerable<CultureString>, IEnumerable, ICloneable
	{
		private List<CultureString> _cultureStrings;

		[XmlIgnore]
		public string Text
		{
			get
			{
				return GetText(CultureInfo.CurrentUICulture, mayFallback: true);
			}
			set
			{
				SetText(CultureInfo.CurrentUICulture, value, mayFallback: true);
			}
		}

		[XmlIgnore]
		public string InvariantText
		{
			get
			{
				return GetText(CultureInfo.InvariantCulture, mayFallback: true);
			}
			set
			{
				SetText(CultureInfo.InvariantCulture, value, mayFallback: true);
			}
		}

		[XmlIgnore]
		public int Count => _cultureStrings.Count;

		public LocalizedString()
		{
			_cultureStrings = new List<CultureString>();
		}

		public LocalizedString(LocalizedString other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			_cultureStrings = new List<CultureString>();
			foreach (CultureString cultureString in other._cultureStrings)
			{
				_cultureStrings.Add(new CultureString(cultureString));
			}
		}

		public LocalizedString(string text)
			: this()
		{
			InvariantText = text;
		}

		public string GetText(CultureInfo culture)
		{
			return GetText(culture, mayFallback: true);
		}

		public string GetText(CultureInfo culture, bool mayFallback)
		{
			int index = GetIndex(culture, mayFallback);
			if (index < 0)
			{
				return null;
			}
			return _cultureStrings[index].Text;
		}

		public void Add(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException("o");
			}
			CultureString cultureString = o as CultureString;
			if (cultureString == null)
			{
				throw new ArgumentException("Invalid argument");
			}
			if (cultureString.Culture == null)
			{
				throw new ArgumentNullException("Culture");
			}
			if (_cultureStrings == null)
			{
				_cultureStrings = new List<CultureString>();
			}
			else if (GetIndex(cultureString.Culture, mayFallback: false) >= 0)
			{
				throw new ArgumentException("Element already present");
			}
			_cultureStrings.Add(cultureString);
		}

		public void SetText(CultureInfo culture, string text)
		{
			SetText(culture, text, mayFallback: true);
		}

		public void SetText(CultureInfo culture, string text, bool mayFallback)
		{
			if (culture == null)
			{
				culture = CultureInfo.InvariantCulture;
			}
			int index = GetIndex(culture, mayFallback);
			if (index >= 0)
			{
				_cultureStrings[index].Text = text;
			}
			else
			{
				_cultureStrings.Add(new CultureString(culture, text));
			}
		}

		public void DeleteText(CultureInfo culture)
		{
			DeleteText(culture, mayFallback: true);
		}

		public void DeleteText(CultureInfo culture, bool mayFallback)
		{
			int index = GetIndex(culture, mayFallback);
			if (index >= 0)
			{
				_cultureStrings.RemoveAt(index);
			}
		}

		private int GetIndex(CultureInfo culture, bool mayFallback)
		{
			if (_cultureStrings == null)
			{
				return -1;
			}
			if (culture == null)
			{
				culture = CultureInfo.InvariantCulture;
			}
			CultureInfo cultureInfo = null;
			int result = -1;
			for (int i = 0; i < _cultureStrings.Count; i++)
			{
				CultureString cultureString = _cultureStrings[i];
				if (cultureString.Culture.Equals(culture))
				{
					return i;
				}
				if (mayFallback && (CultureInfoExtensions.AreCompatible(culture, cultureString.Culture) || cultureString.Culture.Equals(CultureInfo.InvariantCulture)) && (cultureInfo == null || cultureInfo.Equals(CultureInfo.InvariantCulture)))
				{
					cultureInfo = cultureString.Culture;
					result = i;
				}
			}
			return result;
		}

		public IEnumerator<CultureString> GetEnumerator()
		{
			return _cultureStrings.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _cultureStrings.GetEnumerator();
		}

		public object Clone()
		{
			return new LocalizedString(this);
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
