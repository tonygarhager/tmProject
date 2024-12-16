using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	[DataContract]
	public class EditActionDeleteFieldValue : EditAction
	{
		private List<string> _FieldNames;

		[DataMember]
		public IList<string> FieldNames
		{
			get
			{
				return _FieldNames;
			}
			set
			{
				if (_FieldNames == null)
				{
					_FieldNames = new List<string>();
				}
				else
				{
					_FieldNames.Clear();
				}
				if (value != null)
				{
					foreach (string item in value)
					{
						if (item != null)
						{
							_FieldNames.Add(item);
						}
					}
				}
			}
		}

		public EditActionDeleteFieldValue()
		{
			_FieldNames = new List<string>();
		}

		public EditActionDeleteFieldValue(string fieldName)
			: this(new string[1]
			{
				fieldName
			})
		{
		}

		public EditActionDeleteFieldValue(IList<string> fieldNames)
		{
			_FieldNames = new List<string>();
			if (fieldNames != null)
			{
				foreach (string fieldName in fieldNames)
				{
					if (fieldName != null)
					{
						_FieldNames.Add(fieldName);
					}
				}
			}
		}

		public override bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			foreach (string fieldName in _FieldNames)
			{
				if (fieldName == null || fields.LookupIField(fieldName) == null)
				{
					if (throwIfInvalid)
					{
						throw new Exception("Field " + fieldName + " does not exist");
					}
					return false;
				}
			}
			return true;
		}

		public override bool Apply(TranslationUnit tu)
		{
			bool result = false;
			foreach (string fieldName in _FieldNames)
			{
				if (string.IsNullOrEmpty(fieldName))
				{
					throw new LanguagePlatformException(ErrorCode.EditScriptEmptyFieldName);
				}
				if (fieldName.Equals(Field.TextContextFieldName, StringComparison.OrdinalIgnoreCase))
				{
					if (tu.Contexts != null && tu.Contexts.Length >= 0)
					{
						tu.Contexts.Clear();
						result = true;
					}
				}
				else
				{
					FieldValue fieldValue = tu.FieldValues?.Lookup(fieldName);
					if (fieldValue != null)
					{
						tu.FieldValues.Remove(fieldValue);
						result = true;
					}
				}
			}
			return result;
		}
	}
}
