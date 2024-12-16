using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	[DataContract]
	public class EditActionRenameField : EditAction
	{
		private string _fromName;

		private string _toName;

		[DataMember]
		public string FromName
		{
			get
			{
				return _fromName;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException();
				}
				_fromName = value;
			}
		}

		[DataMember]
		public string ToName
		{
			get
			{
				return _toName;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException();
				}
				value = value.Trim();
				if (!Field.IsValidName(value))
				{
					throw new LanguagePlatformException(ErrorCode.TMInvalidFieldName);
				}
				if (Field.IsReservedName(value))
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptSystemField);
				}
				_toName = value;
			}
		}

		[DataMember]
		public Dictionary<string, string> PicklistValueMap
		{
			get;
			set;
		}

		public EditActionRenameField(string fromName, string toName)
		{
			if (string.IsNullOrEmpty(fromName))
			{
				throw new ArgumentNullException("fromName");
			}
			if (string.IsNullOrEmpty(toName))
			{
				throw new ArgumentNullException("toName");
			}
			FromName = fromName;
			ToName = toName;
		}

		public override bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			if (fields.LookupIField(_fromName) == null)
			{
				if (throwIfInvalid)
				{
					throw new Exception("Field with the old name does not exist");
				}
				return false;
			}
			if (fields.LookupIField(_toName) != null)
			{
				return true;
			}
			if (throwIfInvalid)
			{
				throw new Exception("Field with the new name does not exist");
			}
			return false;
		}

		public override bool Apply(TranslationUnit tu)
		{
			if (tu == null)
			{
				throw new ArgumentNullException();
			}
			if (tu.FieldValues == null || tu.FieldValues.Count == 0)
			{
				return false;
			}
			FieldValue fieldValue = tu.FieldValues.Lookup(_fromName);
			if (fieldValue == null)
			{
				return false;
			}
			bool num = !string.Equals(_fromName, _toName, StringComparison.OrdinalIgnoreCase);
			bool result = false;
			if (num)
			{
				if (tu.FieldValues.Lookup(_toName) != null)
				{
					tu.FieldValues.Remove(fieldValue);
					return true;
				}
				tu.FieldValues.Remove(fieldValue);
				fieldValue.Name = _toName;
				tu.FieldValues.Add(fieldValue);
				result = true;
			}
			if ((fieldValue.ValueType != FieldValueType.SinglePicklist && fieldValue.ValueType != FieldValueType.MultiplePicklist) || PicklistValueMap == null || PicklistValueMap.Count <= 0)
			{
				return result;
			}
			SinglePicklistFieldValue singlePicklistFieldValue = fieldValue as SinglePicklistFieldValue;
			MultiplePicklistFieldValue multiplePicklistFieldValue = fieldValue as MultiplePicklistFieldValue;
			if (singlePicklistFieldValue != null)
			{
				if (!PicklistValueMap.TryGetValue(singlePicklistFieldValue.Value.Name, out string value))
				{
					return result;
				}
				singlePicklistFieldValue.Value.Name = value;
				result = true;
			}
			else if (multiplePicklistFieldValue != null)
			{
				foreach (PicklistItem value3 in multiplePicklistFieldValue.Values)
				{
					if (PicklistValueMap.TryGetValue(value3.Name, out string value2))
					{
						value3.Name = value2;
						result = true;
					}
				}
				return result;
			}
			return result;
		}
	}
}
