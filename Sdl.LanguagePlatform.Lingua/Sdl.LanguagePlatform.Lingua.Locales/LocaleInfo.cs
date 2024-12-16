using System;

namespace Sdl.LanguagePlatform.Lingua.Locales
{
	public class LocaleInfo
	{
		public int LCID
		{
			get;
			internal set;
		}

		public string Name
		{
			get;
			internal set;
		}

		public string Code
		{
			get;
			internal set;
		}

		public string RegionQualifiedCode
		{
			get;
			internal set;
		}

		public string CanonicalCode
		{
			get;
			internal set;
		}

		internal LocaleInfo(int lcid, string name, string code)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty(code))
			{
				throw new ArgumentNullException("code");
			}
			LCID = lcid;
			Name = name;
			Code = code;
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
			LocaleInfo other = obj as LocaleInfo;
			return Equals(other, relaxedCodeEquality: false);
		}

		public bool Equals(LocaleInfo other, bool relaxedCodeEquality)
		{
			if (other == null)
			{
				return false;
			}
			bool flag = LCID == other.LCID;
			bool flag2 = string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
			bool flag3 = string.Equals(Code, other.Code, StringComparison.OrdinalIgnoreCase);
			if (!flag3 && relaxedCodeEquality)
			{
				flag3 = AreCodesCompatible(this, other);
			}
			return (flag && flag2) & flag3;
		}

		public static bool AreCodesCompatible(LocaleInfo left, LocaleInfo right)
		{
			if (left == null || right == null)
			{
				throw new ArgumentNullException();
			}
			bool flag = string.Equals(left.Code, right.Code, StringComparison.OrdinalIgnoreCase);
			if (!flag && left.RegionQualifiedCode != null && string.Equals(left.RegionQualifiedCode, right.Code, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			if (!flag && right.RegionQualifiedCode != null && string.Equals(left.Code, right.RegionQualifiedCode, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			if (!flag && right.RegionQualifiedCode != null && left.RegionQualifiedCode != null && string.Equals(left.RegionQualifiedCode, right.RegionQualifiedCode, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			return flag;
		}

		public static bool AreCodesMapped(LocaleInfo left, LocaleInfo right)
		{
			if (left == null || right == null)
			{
				throw new ArgumentNullException();
			}
			bool flag = string.Equals(left.Code, right.Code, StringComparison.OrdinalIgnoreCase);
			if (!flag && left.CanonicalCode != null && string.Equals(left.CanonicalCode, right.Code, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			if (!flag && right.CanonicalCode != null && string.Equals(left.Code, right.CanonicalCode, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			if (!flag && right.CanonicalCode != null && left.CanonicalCode != null && string.Equals(left.CanonicalCode, right.CanonicalCode, StringComparison.OrdinalIgnoreCase))
			{
				flag = true;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static int CompareByLCIDThenByCode(LocaleInfo a, LocaleInfo b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}
			int num = a.LCID - b.LCID;
			if (num == 0)
			{
				num = string.Compare(a.Code, b.Code, StringComparison.OrdinalIgnoreCase);
			}
			return num;
		}

		public static int CompareByRelaxedCode(LocaleInfo a, LocaleInfo b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}
			int num = string.Compare(a.Code, b.Code, StringComparison.OrdinalIgnoreCase);
			if (num == 0)
			{
				return num;
			}
			if (AreCodesCompatible(a, b) || AreCodesMapped(a, b))
			{
				num = 0;
			}
			return num;
		}
	}
}
