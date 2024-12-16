using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Core
{
	public class CharacterSet
	{
		private struct Range
		{
			public readonly char Lower;

			public readonly char Upper;

			public Range(char lower, char upper)
			{
				Lower = lower;
				Upper = upper;
			}
		}

		private StringBuilder _individualMembers;

		private List<UnicodeCategory> _categories;

		private List<Range> _ranges;

		public bool Negated
		{
			get;
			set;
		}

		public bool Contains(char c)
		{
			if (_individualMembers != null && _individualMembers.ToString().IndexOf(c) >= 0)
			{
				return !Negated;
			}
			if (_ranges != null)
			{
				for (int i = 0; i < _ranges.Count; i++)
				{
					if (c >= _ranges[i].Lower && c <= _ranges[i].Upper)
					{
						return !Negated;
					}
				}
			}
			if (_categories == null)
			{
				return Negated;
			}
			UnicodeCategory cat = char.GetUnicodeCategory(c);
			if (_categories.Any((UnicodeCategory t) => t == cat))
			{
				return !Negated;
			}
			return Negated;
		}

		public void Add(char c)
		{
			if (_individualMembers == null)
			{
				_individualMembers = new StringBuilder();
			}
			if (_individualMembers.ToString().IndexOf(c) < 0)
			{
				_individualMembers.Append(c);
			}
		}

		public void Add(char lower, char upper)
		{
			if (lower == upper)
			{
				Add(lower);
				return;
			}
			if (lower > upper)
			{
				Add(upper, lower);
				return;
			}
			if (_ranges == null)
			{
				_ranges = new List<Range>();
			}
			_ranges.Add(new Range(lower, upper));
		}

		public void Add(UnicodeCategory category)
		{
			if (_categories == null)
			{
				_categories = new List<UnicodeCategory>();
			}
			foreach (UnicodeCategory category2 in _categories)
			{
				if (category2 == category)
				{
					return;
				}
			}
			_categories.Add(category);
		}

		public void Add(CharacterSet other)
		{
			if (other == null)
			{
				throw new ArgumentNullException();
			}
			if (other._individualMembers != null)
			{
				for (int i = 0; i < other._individualMembers.Length; i++)
				{
					Add(other._individualMembers[i]);
				}
			}
			if (other._ranges != null)
			{
				for (int j = 0; j < other._ranges.Count; j++)
				{
					Add(other._ranges[j].Lower, other._ranges[j].Upper);
				}
			}
			if (other._categories != null)
			{
				foreach (UnicodeCategory category in other._categories)
				{
					Add(category);
				}
			}
		}

		public string Signature()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			if (Negated)
			{
				stringBuilder.Append("^");
			}
			if (_individualMembers != null)
			{
				stringBuilder.Append(_individualMembers);
			}
			if (_ranges != null)
			{
				foreach (Range range in _ranges)
				{
					stringBuilder.Append($"{range.Lower}-{range.Upper}");
				}
			}
			if (_categories != null)
			{
				foreach (UnicodeCategory category in _categories)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(CharacterProperties.GetUnicodeCategoryName(category));
				}
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			if (Negated)
			{
				stringBuilder.Append("^");
			}
			if (_individualMembers != null)
			{
				stringBuilder.Append(_individualMembers);
			}
			if (_ranges != null)
			{
				foreach (Range range in _ranges)
				{
					stringBuilder.Append($"{range.Lower}-{range.Upper}");
				}
			}
			if (_categories != null)
			{
				foreach (UnicodeCategory category in _categories)
				{
					stringBuilder.Append("\\p{" + CharacterProperties.GetUnicodeCategoryName(category) + "}");
				}
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}
