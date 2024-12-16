using Sdl.LanguagePlatform.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	public class FieldValues : IEnumerable<FieldValue>, IEnumerable
	{
		[DataMember]
		public List<FieldValue> Values
		{
			get;
			private set;
		}

		public int Count => Values.Count;

		public FieldValue this[string name] => Lookup(name);

		public FieldValue this[int index] => Values[index];

		public FieldValues()
		{
			Values = new List<FieldValue>();
		}

		public FieldValues(FieldValues other)
		{
			Values = new List<FieldValue>();
			foreach (FieldValue value in other.Values)
			{
				Values.Add(value.Duplicate());
			}
		}

		public void Add(FieldValue fv)
		{
			if (fv == null)
			{
				throw new ArgumentNullException("fv");
			}
			if (string.IsNullOrEmpty(fv.Name))
			{
				throw new ArgumentNullException("Name");
			}
			if (!Field.IsValidName(fv.Name))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidFieldName);
			}
			if (!Exists(fv.Name))
			{
				Values.Add(fv);
			}
		}

		public void Insert(int index, FieldValue fv)
		{
			if (fv == null)
			{
				throw new ArgumentNullException("fv");
			}
			if (string.IsNullOrEmpty(fv.Name))
			{
				throw new ArgumentNullException("Name");
			}
			if (!Field.IsValidName(fv.Name))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidFieldName);
			}
			if (!Exists(fv.Name))
			{
				Values.Insert(index, fv);
			}
		}

		public void Clear()
		{
			Values.Clear();
		}

		public bool Merge(FieldValues values)
		{
			bool flag = false;
			foreach (FieldValue value in values)
			{
				FieldValue fieldValue = Lookup(value.Name, FieldValueType.Unknown);
				if (fieldValue == null)
				{
					Values.Add(CreateValue(value));
					flag = true;
				}
				else if (fieldValue.ValueType == value.ValueType)
				{
					flag = (fieldValue.Merge(value) | flag);
				}
			}
			return flag;
		}

		public bool Assign(FieldValues values)
		{
			Clear();
			return Merge(values);
		}

		public FieldValue Lookup(string name)
		{
			return Lookup(name, FieldValueType.Unknown);
		}

		public FieldValue Lookup(string name, FieldValueType expectedValueType)
		{
			int index = GetIndex(name);
			if (index < 0)
			{
				return null;
			}
			FieldValue fieldValue = Values[index];
			if (expectedValueType != 0 && expectedValueType != fieldValue.ValueType)
			{
				throw new LanguagePlatformException(ErrorCode.TMIncompatibleFieldTypes);
			}
			return fieldValue;
		}

		private int GetIndex(string fieldName)
		{
			if (Values == null || Values.Count == 0)
			{
				return -1;
			}
			for (int i = 0; i < Values.Count; i++)
			{
				if (Values[i].Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		private static FieldValue CreateValue(string name, FieldValueType type)
		{
			switch (type)
			{
			case FieldValueType.SingleString:
				return new SingleStringFieldValue(name);
			case FieldValueType.MultipleString:
				return new MultipleStringFieldValue(name);
			case FieldValueType.DateTime:
				return new DateTimeFieldValue(name);
			case FieldValueType.SinglePicklist:
				return new SinglePicklistFieldValue(name);
			case FieldValueType.MultiplePicklist:
				return new MultiplePicklistFieldValue(name);
			case FieldValueType.Integer:
				return new IntFieldValue(name);
			default:
				throw new ArgumentException("Illegal enum value");
			}
		}

		private static FieldValue CreateValue(FieldValue fv)
		{
			switch (fv.ValueType)
			{
			case FieldValueType.SingleString:
				return new SingleStringFieldValue((SingleStringFieldValue)fv);
			case FieldValueType.MultipleString:
				return new MultipleStringFieldValue((MultipleStringFieldValue)fv);
			case FieldValueType.DateTime:
				return new DateTimeFieldValue((DateTimeFieldValue)fv);
			case FieldValueType.SinglePicklist:
				return new SinglePicklistFieldValue((SinglePicklistFieldValue)fv);
			case FieldValueType.MultiplePicklist:
				return new MultiplePicklistFieldValue((MultiplePicklistFieldValue)fv);
			case FieldValueType.Integer:
				return new IntFieldValue((IntFieldValue)fv);
			default:
				throw new ArgumentException("Illegal enum value");
			}
		}

		public FieldValue LookupOrCreate(string name, FieldValueType valueType)
		{
			FieldValue fieldValue = Lookup(name, valueType);
			if (fieldValue != null)
			{
				return fieldValue;
			}
			fieldValue = CreateValue(name, valueType);
			Values.Add(fieldValue);
			return fieldValue;
		}

		public bool Exists(string name)
		{
			return Lookup(name) != null;
		}

		public bool Remove(FieldValue item)
		{
			if (item == null)
			{
				throw new ArgumentNullException();
			}
			if (item.Name == null)
			{
				throw new ArgumentNullException("Name");
			}
			return Remove(item.Name);
		}

		public bool Remove(string name)
		{
			int index = GetIndex(name);
			if (index < 0)
			{
				return false;
			}
			Values.RemoveAt(index);
			return true;
		}

		public void RemoveAt(int index)
		{
			Values.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public IEnumerator<FieldValue> GetEnumerator()
		{
			return Values.GetEnumerator();
		}
	}
}
