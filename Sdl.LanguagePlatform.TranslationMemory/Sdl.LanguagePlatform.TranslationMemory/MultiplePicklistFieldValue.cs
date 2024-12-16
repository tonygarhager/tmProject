using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class MultiplePicklistFieldValue : FieldValue
	{
		[DataMember]
		public List<PicklistItem> Values
		{
			get;
			set;
		}

		public override FieldValueType ValueType
		{
			get
			{
				return FieldValueType.MultiplePicklist;
			}
			set
			{
			}
		}

		public MultiplePicklistFieldValue()
		{
			Values = new List<PicklistItem>();
		}

		public MultiplePicklistFieldValue(MultiplePicklistFieldValue other)
			: this(other.Name, other.Values)
		{
		}

		public MultiplePicklistFieldValue(string name)
			: base(name)
		{
			Values = new List<PicklistItem>();
		}

		public MultiplePicklistFieldValue(string name, ICollection<PicklistItem> values)
			: base(name)
		{
			Values = new List<PicklistItem>();
			if (values != null)
			{
				foreach (PicklistItem value in values)
				{
					Add(value);
				}
			}
		}

		public bool Add(PicklistItem v)
		{
			if (HasValue(v))
			{
				return false;
			}
			Values.Add(v);
			return true;
		}

		public bool HasValue(PicklistItem v)
		{
			return HasValue(v.Name);
		}

		public bool HasValue(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				throw new ArgumentNullException();
			}
			if (Values != null)
			{
				return Values.Any((PicklistItem i) => i.Name.Equals(s, StringComparison.OrdinalIgnoreCase));
			}
			return false;
		}

		public bool HasValues(MultiplePicklistFieldValue other)
		{
			return other.Values.All(HasValue);
		}

		public bool Remove(PicklistItem v)
		{
			if (v == null)
			{
				throw new ArgumentNullException("v");
			}
			return Remove(v.Name);
		}

		public bool Remove(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				throw new ArgumentNullException();
			}
			for (int i = 0; i < Values.Count; i++)
			{
				if (Values[i].Name.Equals(s, StringComparison.OrdinalIgnoreCase))
				{
					Values.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public override string GetValueString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			for (int i = 0; i < Values.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("\"");
				stringBuilder.Append(StringUtilities.EscapeString(Values[i].Name));
				stringBuilder.Append("\"");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
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
			MultiplePicklistFieldValue r = obj as MultiplePicklistFieldValue;
			if (r == null)
			{
				return false;
			}
			if (Values == null || Values.Count == 0 || r.Values == null || r.Values.Count == 0)
			{
				if (Values == null || Values.Count == 0)
				{
					if (r.Values != null)
					{
						return r.Values.Count == 0;
					}
					return true;
				}
				return false;
			}
			if (Values.Count == r.Values.Count)
			{
				return Values.All((PicklistItem pli) => r.HasValue(pli));
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Merge(FieldValue rhs)
		{
			MultiplePicklistFieldValue obj = (rhs as MultiplePicklistFieldValue) ?? throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			bool flag = false;
			foreach (PicklistItem value in obj.Values)
			{
				flag |= Add(value);
			}
			return flag;
		}

		public override bool Add(FieldValue rhs)
		{
			return Merge(rhs);
		}

		public override bool Substract(FieldValue rhs)
		{
			MultiplePicklistFieldValue obj = (rhs as MultiplePicklistFieldValue) ?? throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			bool flag = false;
			foreach (PicklistItem value in obj.Values)
			{
				flag |= Remove(value.Name);
			}
			return flag;
		}

		public override FieldValue Duplicate()
		{
			return new MultiplePicklistFieldValue(this);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (PicklistItem value in Values)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(value.Name);
			}
			return stringBuilder.ToString();
		}

		public override void Parse(string s)
		{
		}

		public override bool Add(string s)
		{
			PicklistItem v = new PicklistItem(s);
			return Add(v);
		}

		public override void Clear()
		{
			Values.Clear();
		}
	}
}
