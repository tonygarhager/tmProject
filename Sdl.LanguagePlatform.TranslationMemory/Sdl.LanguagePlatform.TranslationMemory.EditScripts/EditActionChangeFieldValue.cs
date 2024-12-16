using Sdl.LanguagePlatform.Core;
using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	[DataContract]
	public class EditActionChangeFieldValue : EditAction
	{
		[DataMember]
		public Mode Mode
		{
			get;
			set;
		}

		[DataMember]
		public FieldValue FieldValue
		{
			get;
			set;
		}

		public EditActionChangeFieldValue(FieldValue v, Mode m)
		{
			FieldValue = (v ?? throw new ArgumentNullException("v"));
			Mode = m;
		}

		public override bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			if (FieldValue == null)
			{
				if (throwIfInvalid)
				{
					throw new Exception("No field value supplied");
				}
				return false;
			}
			IField field = fields.LookupIField(FieldValue.Name);
			if (field == null)
			{
				if (throwIfInvalid)
				{
					throw new Exception("Unknown field");
				}
				return false;
			}
			if (FieldValue.ValueType == field.ValueType)
			{
				return true;
			}
			if (throwIfInvalid)
			{
				throw new Exception("Incompatible field value types");
			}
			return false;
		}

		public override bool Apply(TranslationUnit tu)
		{
			if (tu == null)
			{
				throw new ArgumentNullException();
			}
			if (FieldValue == null)
			{
				throw new ArgumentException("No field value");
			}
			FieldValue value = tu.GetValue(FieldValue.Name);
			if (value == null)
			{
				if (Mode == Mode.Substract)
				{
					return false;
				}
			}
			else if (value.ValueType != FieldValue.ValueType)
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptIncompatibleFieldValueTypes);
			}
			if (Mode == Mode.Assign || value == null || (Mode == Mode.Add && (FieldValue.ValueType == FieldValueType.SinglePicklist || FieldValue.ValueType == FieldValueType.SingleString)))
			{
				tu.SetValue(FieldValue, addIfMissing: true);
				return true;
			}
			if (!IsModeCompatible(Mode, value.ValueType))
			{
				throw new LanguagePlatformException(ErrorCode.EditScriptInvalidOperationForFieldValueType);
			}
			switch (value.ValueType)
			{
			case FieldValueType.SingleString:
			case FieldValueType.SinglePicklist:
				throw new LanguagePlatformException(ErrorCode.InternalError);
			case FieldValueType.MultipleString:
			case FieldValueType.MultiplePicklist:
				switch (Mode)
				{
				case Mode.Add:
					return value.Merge(FieldValue);
				case Mode.Substract:
					return value.Substract(FieldValue);
				default:
					throw new LanguagePlatformException(ErrorCode.InternalError);
				}
			case FieldValueType.DateTime:
			case FieldValueType.Integer:
				switch (Mode)
				{
				case Mode.Add:
					return value.Add(FieldValue);
				case Mode.Substract:
					return value.Substract(FieldValue);
				default:
					throw new LanguagePlatformException(ErrorCode.InternalError);
				}
			default:
				throw new Exception("Invalid case");
			}
		}

		private static bool IsModeCompatible(Mode mode, FieldValueType fvt)
		{
			if (mode == Mode.Substract)
			{
				if (fvt != FieldValueType.SinglePicklist && fvt != FieldValueType.SingleString)
				{
					return fvt != FieldValueType.Unknown;
				}
				return false;
			}
			return fvt != FieldValueType.Unknown;
		}
	}
}
