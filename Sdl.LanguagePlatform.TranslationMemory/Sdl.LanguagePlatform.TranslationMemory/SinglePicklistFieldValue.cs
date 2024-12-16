using Sdl.LanguagePlatform.Core;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class SinglePicklistFieldValue : FieldValue
	{
		[DataMember]
		public PicklistItem Value
		{
			get;
			set;
		}

		public override FieldValueType ValueType
		{
			get
			{
				return FieldValueType.SinglePicklist;
			}
			set
			{
			}
		}

		public SinglePicklistFieldValue()
		{
		}

		public SinglePicklistFieldValue(SinglePicklistFieldValue other)
			: this(other.Name, other.Value)
		{
		}

		public SinglePicklistFieldValue(string name)
			: base(name)
		{
		}

		public SinglePicklistFieldValue(string name, PicklistItem v)
			: base(name)
		{
			Value = v;
		}

		public override string GetValueString()
		{
			return "\"" + StringUtilities.EscapeString(Value.Name) + "\"";
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
			SinglePicklistFieldValue singlePicklistFieldValue = obj as SinglePicklistFieldValue;
			if (singlePicklistFieldValue == null)
			{
				return false;
			}
			if (Value == null || singlePicklistFieldValue.Value == null)
			{
				if (Value == null)
				{
					return singlePicklistFieldValue.Value == null;
				}
				return false;
			}
			return string.Equals(Value.Name, singlePicklistFieldValue.Value.Name, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Merge(FieldValue rhs)
		{
			if (rhs == null)
			{
				throw new ArgumentNullException();
			}
			SinglePicklistFieldValue singlePicklistFieldValue = rhs as SinglePicklistFieldValue;
			if (singlePicklistFieldValue == null)
			{
				throw new ArgumentException("Can't compare different field types");
			}
			if (Equals(rhs))
			{
				return false;
			}
			Value = singlePicklistFieldValue.Value;
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
			return new SinglePicklistFieldValue(this);
		}

		public override string ToString()
		{
			return Value.Name;
		}

		public override void Parse(string s)
		{
			Value = new PicklistItem(s);
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
