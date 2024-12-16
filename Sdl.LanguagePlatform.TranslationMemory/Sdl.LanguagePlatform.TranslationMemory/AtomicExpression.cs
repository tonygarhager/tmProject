using Sdl.LanguagePlatform.Core;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class AtomicExpression : FilterExpression
	{
		public enum Operator
		{
			Equal = 1,
			Smaller,
			SmallerEqual,
			Greater,
			GreaterEqual,
			NotEqual,
			Contains,
			ContainsNot,
			Matches,
			MatchesNot
		}

		private FieldValue _value;

		private Operator _op;

		[DataMember(Order = 0)]
		public FieldValue Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		[DataMember(Order = 1)]
		public Operator Op
		{
			get
			{
				return _op;
			}
			set
			{
				ArgumentException ex = new ArgumentException("The operator is not compatible with attribute type.");
				_op = value;
				switch (_op)
				{
				case Operator.Smaller:
				case Operator.SmallerEqual:
				case Operator.Greater:
				case Operator.GreaterEqual:
				{
					FieldValueType valueType = _value.ValueType;
					if (valueType == FieldValueType.MultipleString || (uint)(valueType - 4) <= 1u)
					{
						throw ex;
					}
					break;
				}
				case Operator.NotEqual:
				{
					FieldValueType valueType = _value.ValueType;
					if (valueType == FieldValueType.MultipleString || valueType == FieldValueType.MultiplePicklist)
					{
						throw ex;
					}
					break;
				}
				case Operator.Contains:
				case Operator.ContainsNot:
				{
					FieldValueType valueType = _value.ValueType;
					if ((uint)(valueType - 3) <= 1u || valueType == FieldValueType.Integer)
					{
						throw ex;
					}
					break;
				}
				case Operator.Matches:
				case Operator.MatchesNot:
					if (_value.ValueType != FieldValueType.SingleString)
					{
						throw ex;
					}
					break;
				}
			}
		}

		public AtomicExpression(FieldValue value, Operator op)
		{
			Value = value;
			Op = op;
		}

		public override string ToString()
		{
			string text;
			switch (_op)
			{
			case Operator.Equal:
				text = " = ";
				break;
			case Operator.Smaller:
				text = " < ";
				break;
			case Operator.SmallerEqual:
				text = " <= ";
				break;
			case Operator.Greater:
				text = " > ";
				break;
			case Operator.GreaterEqual:
				text = " >= ";
				break;
			case Operator.NotEqual:
				text = " != ";
				break;
			case Operator.Contains:
				text = " @ ";
				break;
			case Operator.ContainsNot:
				text = " !@ ";
				break;
			case Operator.Matches:
				text = " ~ ";
				break;
			case Operator.MatchesNot:
				text = " !~ ";
				break;
			default:
				return string.Empty;
			}
			return "\"" + StringUtilities.EscapeString(_value.Name) + "\"" + text + _value.GetValueString();
		}

		public override bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			IField field = fields?.LookupIField(_value.Name) ?? Field.LookupSpecialField(_value.Name);
			if (field == null)
			{
				if (throwIfInvalid)
				{
					throw new Exception("Unknown field: " + _value.Name);
				}
				return false;
			}
			if (field.ValueType == _value.ValueType)
			{
				return true;
			}
			if (throwIfInvalid)
			{
				throw new Exception($"Incompatible field types: {field.ValueType} vs. {_value.ValueType}");
			}
			return false;
		}

		public override bool Evaluate(ITypedKeyValueContainer values)
		{
			FieldValue value = values.GetValue(_value.Name, _value.ValueType);
			if (value == null)
			{
				return false;
			}
			ArgumentException ex = new ArgumentException("Invalid attribute type.");
			switch (_op)
			{
			case Operator.Equal:
				switch (_value.ValueType)
				{
				case FieldValueType.SingleString:
					return string.Compare(((SingleStringFieldValue)value).Value, ((SingleStringFieldValue)_value).Value, StringComparison.OrdinalIgnoreCase) == 0;
				case FieldValueType.DateTime:
					return ((DateTimeFieldValue)value).Value == ((DateTimeFieldValue)_value).Value;
				case FieldValueType.Integer:
					return ((IntFieldValue)value).Value == ((IntFieldValue)_value).Value;
				case FieldValueType.SinglePicklist:
					return string.Compare(((SinglePicklistFieldValue)value).Value.Name, ((SinglePicklistFieldValue)_value).Value.Name, StringComparison.OrdinalIgnoreCase) == 0;
				case FieldValueType.MultipleString:
					if (((MultipleStringFieldValue)value).HasValues((MultipleStringFieldValue)_value))
					{
						return ((MultipleStringFieldValue)_value).HasValues((MultipleStringFieldValue)value);
					}
					return false;
				case FieldValueType.MultiplePicklist:
					if (((MultiplePicklistFieldValue)value).HasValues((MultiplePicklistFieldValue)_value))
					{
						return ((MultiplePicklistFieldValue)_value).HasValues((MultiplePicklistFieldValue)value);
					}
					return false;
				default:
					throw ex;
				}
			case Operator.Smaller:
				switch (_value.ValueType)
				{
				case FieldValueType.SingleString:
					return string.Compare(((SingleStringFieldValue)value).Value, ((SingleStringFieldValue)_value).Value, StringComparison.OrdinalIgnoreCase) < 0;
				case FieldValueType.DateTime:
					return ((DateTimeFieldValue)value).Value < ((DateTimeFieldValue)_value).Value;
				case FieldValueType.Integer:
					return ((IntFieldValue)value).Value < ((IntFieldValue)_value).Value;
				default:
					throw ex;
				}
			case Operator.SmallerEqual:
				switch (_value.ValueType)
				{
				case FieldValueType.SingleString:
					return string.Compare(((SingleStringFieldValue)value).Value, ((SingleStringFieldValue)_value).Value, StringComparison.OrdinalIgnoreCase) <= 0;
				case FieldValueType.DateTime:
					return ((DateTimeFieldValue)value).Value <= ((DateTimeFieldValue)_value).Value;
				case FieldValueType.Integer:
					return ((IntFieldValue)value).Value <= ((IntFieldValue)_value).Value;
				default:
					throw ex;
				}
			case Operator.Greater:
				switch (_value.ValueType)
				{
				case FieldValueType.SingleString:
					return string.Compare(((SingleStringFieldValue)value).Value, ((SingleStringFieldValue)_value).Value, StringComparison.OrdinalIgnoreCase) > 0;
				case FieldValueType.DateTime:
					return ((DateTimeFieldValue)value).Value > ((DateTimeFieldValue)_value).Value;
				case FieldValueType.Integer:
					return ((IntFieldValue)value).Value > ((IntFieldValue)_value).Value;
				default:
					throw ex;
				}
			case Operator.GreaterEqual:
				switch (_value.ValueType)
				{
				case FieldValueType.SingleString:
					return string.Compare(((SingleStringFieldValue)value).Value, ((SingleStringFieldValue)_value).Value, StringComparison.OrdinalIgnoreCase) >= 0;
				case FieldValueType.DateTime:
					return ((DateTimeFieldValue)value).Value >= ((DateTimeFieldValue)_value).Value;
				case FieldValueType.Integer:
					return ((IntFieldValue)value).Value >= ((IntFieldValue)_value).Value;
				default:
					throw ex;
				}
			case Operator.NotEqual:
				switch (_value.ValueType)
				{
				case FieldValueType.SingleString:
					return string.Compare(((SingleStringFieldValue)value).Value, ((SingleStringFieldValue)_value).Value, StringComparison.OrdinalIgnoreCase) != 0;
				case FieldValueType.DateTime:
					return ((DateTimeFieldValue)value).Value != ((DateTimeFieldValue)_value).Value;
				case FieldValueType.Integer:
					return ((IntFieldValue)value).Value != ((IntFieldValue)_value).Value;
				case FieldValueType.SinglePicklist:
					return string.Compare(((SinglePicklistFieldValue)value).Value.Name, ((SinglePicklistFieldValue)_value).Value.Name, StringComparison.OrdinalIgnoreCase) != 0;
				default:
					throw ex;
				}
			case Operator.Contains:
				switch (_value.ValueType)
				{
				case FieldValueType.SingleString:
					return ((SingleStringFieldValue)value).Value.IndexOf(((SingleStringFieldValue)_value).Value, StringComparison.OrdinalIgnoreCase) != -1;
				case FieldValueType.MultipleString:
					return ((MultipleStringFieldValue)value).HasValues((MultipleStringFieldValue)_value);
				case FieldValueType.MultiplePicklist:
					return ((MultiplePicklistFieldValue)value).HasValues((MultiplePicklistFieldValue)_value);
				default:
					throw ex;
				}
			case Operator.ContainsNot:
				switch (_value.ValueType)
				{
				case FieldValueType.SingleString:
					return !((SingleStringFieldValue)value).Value.Contains(((SingleStringFieldValue)_value).Value);
				case FieldValueType.MultipleString:
					return !((MultipleStringFieldValue)value).HasValues((MultipleStringFieldValue)_value);
				case FieldValueType.MultiplePicklist:
					return !((MultiplePicklistFieldValue)value).HasValues((MultiplePicklistFieldValue)_value);
				default:
					throw ex;
				}
			case Operator.Matches:
				if (_value.ValueType == FieldValueType.SingleString)
				{
					return Regex.IsMatch(((SingleStringFieldValue)value).Value, ((SingleStringFieldValue)_value).Value);
				}
				throw ex;
			case Operator.MatchesNot:
				if (_value.ValueType == FieldValueType.SingleString)
				{
					return !Regex.IsMatch(((SingleStringFieldValue)value).Value, ((SingleStringFieldValue)_value).Value);
				}
				throw ex;
			default:
				throw new ArgumentException("Invalid operator type.");
			}
		}
	}
}
