using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class DateTimeFieldValue : FieldValue
	{
		private DateTime _value;

		[DataMember]
		public DateTime Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = DateTimeUtilities.Normalize(value);
			}
		}

		public override FieldValueType ValueType
		{
			get
			{
				return FieldValueType.DateTime;
			}
			set
			{
			}
		}

		public DateTimeFieldValue()
		{
		}

		public DateTimeFieldValue(string name)
			: base(name)
		{
		}

		public DateTimeFieldValue(DateTimeFieldValue other)
			: this(other.Name, other._value)
		{
		}

		public DateTimeFieldValue(string name, DateTime v)
			: base(name)
		{
			_value = DateTimeUtilities.Normalize(v);
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
			DateTimeFieldValue dateTimeFieldValue = obj as DateTimeFieldValue;
			if (dateTimeFieldValue == null)
			{
				return false;
			}
			return DateTime.Equals(_value, dateTimeFieldValue._value);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Merge(FieldValue rhs)
		{
			if (Equals(rhs))
			{
				return false;
			}
			_value = ((DateTimeFieldValue)rhs)._value;
			return true;
		}

		public override bool Add(FieldValue rhs)
		{
			DateTimeFieldValue dateTimeFieldValue = rhs as DateTimeFieldValue;
			if (dateTimeFieldValue == null)
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}
			_value = _value.Add(new TimeSpan(dateTimeFieldValue.Value.Ticks));
			return true;
		}

		public override bool Substract(FieldValue rhs)
		{
			DateTimeFieldValue dateTimeFieldValue = rhs as DateTimeFieldValue;
			if (dateTimeFieldValue == null)
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}
			_value = _value.Subtract(new TimeSpan(dateTimeFieldValue.Value.Ticks));
			return true;
		}

		public override FieldValue Duplicate()
		{
			return new DateTimeFieldValue(this);
		}

		public override string GetValueString()
		{
			return "\"" + StringUtilities.EscapeString(_value.ToString("R", CultureInfo.InvariantCulture)) + "\"";
		}

		public override string ToString()
		{
			return _value.ToString("R");
		}

		public override void Parse(string s)
		{
			_value = DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
		}

		public override bool Add(string s)
		{
			return false;
		}

		public override void Clear()
		{
			_value = DateTimeUtilities.Normalize(DateTime.MinValue);
		}
	}
}
