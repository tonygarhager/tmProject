using Sdl.LanguagePlatform.Core;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SingleStringFieldValue : FieldValue
	{
		[DataMember]
		public string Value
		{
			get;
			set;
		}

		public override FieldValueType ValueType
		{
			get
			{
				return FieldValueType.SingleString;
			}
			set
			{
			}
		}

		public SingleStringFieldValue()
		{
		}

		public SingleStringFieldValue(SingleStringFieldValue other)
			: this(other.Name, other.Value)
		{
		}

		public SingleStringFieldValue(string name)
			: base(name)
		{
		}

		public SingleStringFieldValue(string name, string v)
			: base(name)
		{
			Value = v;
		}

		public override string GetValueString()
		{
			if (string.IsNullOrEmpty(Value))
			{
				return "\"\"";
			}
			return "\"" + StringUtilities.EscapeString(Value) + "\"";
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
			SingleStringFieldValue singleStringFieldValue = obj as SingleStringFieldValue;
			if (singleStringFieldValue != null)
			{
				return string.Equals(Value, singleStringFieldValue.Value, StringComparison.OrdinalIgnoreCase);
			}
			return false;
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
			Value = ((SingleStringFieldValue)rhs).Value;
			return true;
		}

		public override bool Add(FieldValue rhs)
		{
			throw new LanguagePlatformException(ErrorCode.EditScriptInvalidOperationForFieldValueType);
		}

		public override bool Substract(FieldValue rhs)
		{
			throw new LanguagePlatformException(ErrorCode.EditScriptInvalidOperationForFieldValueType);
		}

		public override FieldValue Duplicate()
		{
			return new SingleStringFieldValue(this);
		}

		public override string ToString()
		{
			return Value;
		}

		public override void Parse(string s)
		{
			Value = s;
		}

		public override bool Add(string s)
		{
			return false;
		}

		public override void Clear()
		{
			Value = null;
		}
	}
}
