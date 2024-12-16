using System;
using System.Globalization;

namespace Sdl.Core.LanguageProcessing.Tokenization
{
	public class SeparatorCombination : ICloneable
	{
		public string GroupSeparators
		{
			get;
			set;
		}

		public string DecimalSeparators
		{
			get;
			set;
		}

		public SeparatorCombination()
		{
		}

		public SeparatorCombination(CultureInfo culture, bool augmentGroupSeparators)
		{
			if (culture == null)
			{
				throw new ArgumentNullException();
			}
			if (culture.IsNeutralCulture)
			{
				throw new ArgumentException("Invalid argument (neutral culture)");
			}
			Init(culture.NumberFormat.NumberGroupSeparator, culture.NumberFormat.NumberDecimalSeparator, augmentGroupSeparators);
		}

		public SeparatorCombination(string groupSeparators, string decimalSeparators, bool augmentGroupSeparators)
		{
			Init(groupSeparators, decimalSeparators, augmentGroupSeparators);
		}

		private void Init(string groupSeparators, string decimalSeparators, bool augmentGroupSeparators)
		{
			GroupSeparators = groupSeparators;
			DecimalSeparators = decimalSeparators;
			if (GroupSeparators != null && augmentGroupSeparators)
			{
				bool flag = GroupSeparators.IndexOf(' ') >= 0;
				bool flag2 = GroupSeparators.IndexOf('\u00a0') >= 0;
				if (flag && !flag2)
				{
					GroupSeparators += "\u00a0";
				}
				if (!flag && flag2)
				{
					GroupSeparators += " ";
				}
			}
		}

		public bool IsSwappable()
		{
			if (string.IsNullOrEmpty(GroupSeparators) || string.IsNullOrEmpty(DecimalSeparators) || GroupSeparators.Length != 1 || DecimalSeparators.Length != 1)
			{
				return false;
			}
			char c = GroupSeparators[0];
			char c2 = DecimalSeparators[0];
			if ((c == '.' || c == ',') && (c2 == '.' || c2 == ','))
			{
				return c != c2;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			SeparatorCombination separatorCombination = (SeparatorCombination)obj;
			if (string.Equals(GroupSeparators, separatorCombination.GroupSeparators))
			{
				return string.Equals(DecimalSeparators, separatorCombination.DecimalSeparators);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = GroupSeparators.GetHashCode();
			hashCode &= 0xFFFF;
			int hashCode2 = DecimalSeparators.GetHashCode();
			hashCode2 &= 0xFFFF;
			hashCode2 <<= 16;
			return hashCode + hashCode2;
		}

		public object Clone()
		{
			string groupSeparators = (GroupSeparators == null) ? null : new string(GroupSeparators.ToCharArray());
			string decimalSeparators = (DecimalSeparators == null) ? null : new string(DecimalSeparators.ToCharArray());
			return new SeparatorCombination(groupSeparators, decimalSeparators, augmentGroupSeparators: false);
		}
	}
}
