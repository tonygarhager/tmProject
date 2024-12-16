using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory
{
	[DataContract]
	[KnownType(typeof(PicklistField))]
	public class Field : PersistentObject, ICloneable, IField
	{
		private string _name;

		private FieldType _fieldType;

		private static readonly FieldDefinitions SpecialFields;

		public static readonly string StructureContextFieldName;

		public static readonly string TextContextFieldName;

		[DataMember]
		public FieldValueType ValueType
		{
			get;
			set;
		}

		public virtual IList<string> PicklistItemNames => new List<string>();

		[DataMember]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				CheckName(value);
				_name = value;
			}
		}

		[DataMember]
		public FieldType FieldType
		{
			get
			{
				return GetFieldType(_name);
			}
			set
			{
				_fieldType = value;
			}
		}

		public bool IsSystemField => GetFieldType(_name) == FieldType.System;

		static Field()
		{
			StructureContextFieldName = "StructureContext";
			TextContextFieldName = "TextContext";
			PicklistField picklistField = new PicklistField("confirmationlevel", FieldValueType.SinglePicklist);
			foreach (object value in Enum.GetValues(typeof(ConfirmationLevel)))
			{
				picklistField.Picklist.Add(value.ToString());
			}
			picklistField.FieldType = FieldType.Pseudo;
			SpecialFields = new FieldDefinitions
			{
				new Field(StructureContextFieldName, FieldValueType.Unknown, FieldType.System),
				new Field(TextContextFieldName, FieldValueType.MultipleString, FieldType.System),
				new Field("chd", FieldValueType.DateTime, FieldType.System),
				new Field("chu", FieldValueType.SingleString, FieldType.System),
				new Field("usd", FieldValueType.DateTime, FieldType.System),
				new Field("usu", FieldValueType.SingleString, FieldType.System),
				new Field("usc", FieldValueType.Integer, FieldType.System),
				new Field("crd", FieldValueType.DateTime, FieldType.System),
				new Field("cru", FieldValueType.SingleString, FieldType.System),
				new Field("src", FieldValueType.SingleString, FieldType.Pseudo),
				new Field("trg", FieldValueType.SingleString, FieldType.Pseudo),
				new Field("sourceplainlength", FieldValueType.Integer, FieldType.Pseudo),
				new Field("targetplainlength", FieldValueType.Integer, FieldType.Pseudo),
				new Field("sourcetagcount", FieldValueType.Integer, FieldType.Pseudo),
				new Field("targettagcount", FieldValueType.Integer, FieldType.Pseudo),
				new Field("x-origin", FieldValueType.SingleString, FieldType.System),
				new Field("x-format", FieldValueType.SingleString, FieldType.System),
				picklistField
			};
		}

		public Field()
		{
		}

		public Field(Field other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			ValueType = other.ValueType;
			_name = other._name;
		}

		public Field(string name, FieldValueType valueType)
			: this(name, valueType, FieldType.User)
		{
		}

		internal Field(string name, FieldValueType valueType, FieldType fieldType)
		{
			ValueType = valueType;
			Name = name;
			_fieldType = fieldType;
		}

		protected virtual void CheckName(string name)
		{
			if (!IsValidName(name))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidFieldName);
			}
		}

		public static string RemoveIllegalChars(string val)
		{
			if (string.IsNullOrEmpty(val))
			{
				throw new ArgumentNullException();
			}
			return val.Trim();
		}

		public static bool IsValidName(string val)
		{
			if (string.IsNullOrEmpty(val))
			{
				return false;
			}
			if (!char.IsWhiteSpace(val[0]))
			{
				return !char.IsWhiteSpace(val[val.Length - 1]);
			}
			return false;
		}

		public static Field LookupSpecialField(string name)
		{
			return SpecialFields.Lookup(name);
		}

		public static bool IsSystemFieldName(string name)
		{
			return GetFieldType(name) == FieldType.System;
		}

		public static bool IsPseudoFieldName(string name)
		{
			return GetFieldType(name) == FieldType.Pseudo;
		}

		public static bool IsReservedName(string name)
		{
			return GetFieldType(name) != FieldType.User;
		}

		public static FieldType GetFieldType(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new LanguagePlatformException(ErrorCode.TMInvalidFieldName);
			}
			return SpecialFields.Lookup(name.Trim())?._fieldType ?? FieldType.User;
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
			Field other = (Field)obj;
			bool flag = EqualsDeclaration(other);
			if (!flag || (ValueType != FieldValueType.SinglePicklist && ValueType != FieldValueType.MultiplePicklist))
			{
				return flag;
			}
			PicklistField obj2 = this as PicklistField;
			if (obj2 != null)
			{
				PicklistField picklistField = obj as PicklistField;
				if (picklistField != null)
				{
					return obj2.Picklist.Equals(picklistField.Picklist);
				}
			}
			throw new LanguagePlatformException(ErrorCode.DAFieldTypesInconsistent);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool EqualsDeclaration(Field other)
		{
			if (ValueType == other.ValueType)
			{
				return _name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public FieldValue CreateValue()
		{
			switch (ValueType)
			{
			case FieldValueType.SingleString:
				return new SingleStringFieldValue(_name);
			case FieldValueType.MultipleString:
				return new MultipleStringFieldValue(_name);
			case FieldValueType.DateTime:
				return new DateTimeFieldValue(_name);
			case FieldValueType.SinglePicklist:
				return new SinglePicklistFieldValue(_name);
			case FieldValueType.MultiplePicklist:
				return new MultiplePicklistFieldValue(_name);
			case FieldValueType.Integer:
				return new IntFieldValue(_name);
			default:
				return null;
			}
		}

		public virtual object Clone()
		{
			return Duplicate();
		}

		public virtual Field Duplicate()
		{
			return new Field(this);
		}
	}
}
