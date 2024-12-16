using Sdl.LanguagePlatform.Core;
using System.Globalization;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class IntFieldValue : FieldValue
	{
		[DataMember]
		public int Value
		{
			get;
			set;
		}

		public override FieldValueType ValueType
		{
			get
			{
				return FieldValueType.Integer;
			}
			set
			{
			}
		}

		public IntFieldValue()
		{
		}

		public IntFieldValue(string name)
			: base(name)
		{
		}

		public IntFieldValue(IntFieldValue other)
			: this(other.Name, other.Value)
		{
		}

		public IntFieldValue(string name, int v)
			: base(name)
		{
			Value = v;
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
			IntFieldValue intFieldValue = obj as IntFieldValue;
			if (intFieldValue == null)
			{
				return false;
			}
			return Value == intFieldValue.Value;
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
			Value = ((IntFieldValue)rhs).Value;
			return true;
		}

		public override bool Add(FieldValue rhs)
		{
			IntFieldValue intFieldValue = rhs as IntFieldValue;
			if (intFieldValue == null)
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}
			Value += intFieldValue.Value;
			return true;
		}

		public override bool Substract(FieldValue rhs)
		{
			IntFieldValue intFieldValue = rhs as IntFieldValue;
			if (intFieldValue == null)
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}
			Value -= intFieldValue.Value;
			return true;
		}

		public override FieldValue Duplicate()
		{
			return new IntFieldValue(this);
		}

		public override string GetValueString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public override void Parse(string s)
		{
			Value = int.Parse(s);
		}

		public override bool Add(string s)
		{
			return false;
		}

		public override void Clear()
		{
			Value = 0;
		}
	}
}
