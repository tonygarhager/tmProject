using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Sdl.LanguagePlatform.Core
{
	[DataContract]
	public class LanguagePair : IComparable<LanguagePair>
	{
		[XmlIgnore]
		public CultureInfo SourceCulture
		{
			get;
			set;
		}

		[XmlIgnore]
		public CultureInfo TargetCulture
		{
			get;
			set;
		}

		[DataMember]
		public string SourceCultureName
		{
			get
			{
				return SourceCulture?.Name;
			}
			set
			{
				SourceCulture = CultureInfoExtensions.GetCultureInfo(value);
			}
		}

		[DataMember]
		public string TargetCultureName
		{
			get
			{
				return TargetCulture?.Name;
			}
			set
			{
				TargetCulture = CultureInfoExtensions.GetCultureInfo(value);
			}
		}

		public LanguagePair()
			: this(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture)
		{
		}

		public LanguagePair(CultureInfo srcCulture, CultureInfo trgCulture)
		{
			SourceCulture = srcCulture;
			TargetCulture = trgCulture;
		}

		public LanguagePair(string srcCultureName, string trgCultureName)
		{
			SourceCulture = CultureInfoExtensions.GetCultureInfo(srcCultureName);
			TargetCulture = CultureInfoExtensions.GetCultureInfo(trgCultureName);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			LanguagePair languagePair = obj as LanguagePair;
			if (languagePair != null && object.Equals(SourceCulture, languagePair.SourceCulture))
			{
				return object.Equals(TargetCulture, languagePair.TargetCulture);
			}
			return false;
		}

		public LanguagePair Reverse()
		{
			return new LanguagePair(TargetCulture, SourceCulture);
		}

		public bool IsCompatible(LanguagePair other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			if (SourceCulture == null || TargetCulture == null || other.SourceCulture == null || other.TargetCulture == null)
			{
				return false;
			}
			if (CultureInfoExtensions.AreCompatible(SourceCulture, other.SourceCulture))
			{
				return CultureInfoExtensions.AreCompatible(TargetCulture, other.TargetCulture);
			}
			return false;
		}

		public override int GetHashCode()
		{
			CultureInfo sourceCulture = SourceCulture;
			if (sourceCulture == null)
			{
				if (TargetCulture == null)
				{
					return 0;
				}
				return 0 ^ TargetCulture.GetHashCode();
			}
			if (TargetCulture != null)
			{
				return SourceCulture.GetHashCode() ^ TargetCulture.GetHashCode();
			}
			return -1 ^ SourceCulture.GetHashCode();
		}

		public override string ToString()
		{
			return SourceCulture.Name + "/" + TargetCulture.Name;
		}

		public static bool TryParse(string s, out LanguagePair lp)
		{
			lp = null;
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			int num = s.IndexOf('/');
			if (num < 0)
			{
				return false;
			}
			string text = s.Substring(0, num);
			string text2 = s.Substring(num + 1);
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
			{
				return false;
			}
			try
			{
				lp = new LanguagePair(text, text2);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public int CompareTo(LanguagePair other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			int num = CompareCultures(SourceCulture, other.SourceCulture);
			if (num == 0)
			{
				num = CompareCultures(TargetCulture, other.TargetCulture);
			}
			return num;
		}

		private static int CompareCultures(CultureInfo c1, CultureInfo c2)
		{
			if (c1 == null)
			{
				if (c2 == null)
				{
					return 0;
				}
				return -1;
			}
			if (c2 != null)
			{
				return string.CompareOrdinal(c1.Name, c2.Name);
			}
			return 1;
		}
	}
}
