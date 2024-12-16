using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class MultipleStringFieldValue : FieldValue
	{
		private HashSet<string> _values;

		[DataMember]
		public IEnumerable<string> Values
		{
			get
			{
				return _values;
			}
			set
			{
				Clear();
				if (value != null)
				{
					foreach (string item in value)
					{
						Add(item);
					}
				}
			}
		}

		public int Count
		{
			get
			{
				HashSet<string> values = _values;
				return ((values != null) ? new int?(values.Count) : null).Value;
			}
		}

		public override FieldValueType ValueType
		{
			get
			{
				return FieldValueType.MultipleString;
			}
			set
			{
			}
		}

		public MultipleStringFieldValue()
		{
			_values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public MultipleStringFieldValue(MultipleStringFieldValue other)
			: this(other.Name, other._values)
		{
		}

		public MultipleStringFieldValue(string name)
			: base(name)
		{
			_values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public MultipleStringFieldValue(string name, ICollection<string> values)
			: base(name)
		{
			_values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string value in values)
			{
				Add(value);
			}
		}

		public override string GetValueString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			bool flag = true;
			if (_values != null)
			{
				foreach (string value in _values)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("\"");
					stringBuilder.Append(StringUtilities.EscapeString(value));
					stringBuilder.Append("\"");
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public sealed override bool Add(string v)
		{
			if (_values == null)
			{
				_values = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
			return _values.Add(v);
		}

		public bool HasValue(string v)
		{
			if (_values == null)
			{
				return false;
			}
			string value = v.ToLower();
			foreach (string value2 in _values)
			{
				if (value2.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(string v)
		{
			return HasValue(v);
		}

		public bool HasValues(MultipleStringFieldValue other)
		{
			return other.Values.All(HasValue);
		}

		public bool Remove(string v)
		{
			if (_values != null)
			{
				return _values.Remove(v);
			}
			return false;
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
			MultipleStringFieldValue r = obj as MultipleStringFieldValue;
			if (r == null)
			{
				return false;
			}
			if (_values == null || _values.Count == 0 || r._values == null || r._values.Count == 0)
			{
				if (_values == null || _values.Count == 0)
				{
					if (r._values != null)
					{
						return r._values.Count == 0;
					}
					return true;
				}
				return false;
			}
			if (_values.Count == r._values.Count)
			{
				return _values.All((string v) => r.HasValue(v));
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Merge(FieldValue rhs)
		{
			MultipleStringFieldValue obj = (rhs as MultipleStringFieldValue) ?? throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			bool flag = false;
			foreach (string value in obj._values)
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
			MultipleStringFieldValue multipleStringFieldValue = rhs as MultipleStringFieldValue;
			if (multipleStringFieldValue == null)
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}
			if (_values == null)
			{
				return false;
			}
			bool flag = false;
			foreach (string value in multipleStringFieldValue._values)
			{
				flag |= Remove(value);
			}
			return flag;
		}

		public override FieldValue Duplicate()
		{
			return new MultipleStringFieldValue(this);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string value in Values)
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(value);
				}
			}
			return stringBuilder.ToString();
		}

		public override void Parse(string s)
		{
		}

		public override void Clear()
		{
			_values?.Clear();
		}
	}
}
