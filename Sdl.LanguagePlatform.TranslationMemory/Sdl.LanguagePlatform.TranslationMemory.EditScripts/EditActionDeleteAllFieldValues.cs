using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemory.EditScripts
{
	[DataContract]
	public class EditActionDeleteAllFieldValues : EditAction
	{
		[DataMember]
		public bool DeleteSystemFields
		{
			get;
			set;
		}

		public EditActionDeleteAllFieldValues()
			: this(deleteSystemFields: false)
		{
		}

		public EditActionDeleteAllFieldValues(bool deleteSystemFields)
		{
			DeleteSystemFields = deleteSystemFields;
		}

		public override bool Validate(IFieldDefinitions fields, bool throwIfInvalid)
		{
			return true;
		}

		public override bool Apply(TranslationUnit tu)
		{
			if (DeleteSystemFields)
			{
				if ((tu.FieldValues == null || tu.FieldValues.Count <= 0) && (tu.Contexts == null || tu.Contexts.Length <= 0))
				{
					return false;
				}
				tu.FieldValues?.Clear();
				tu.Contexts?.Clear();
				return true;
			}
			if (tu.FieldValues == null || tu.FieldValues.Count == 0)
			{
				return false;
			}
			List<int> list = new List<int>();
			bool flag = false;
			for (int i = 0; i < tu.FieldValues.Count; i++)
			{
				if (Field.IsSystemFieldName(tu.FieldValues[i].Name))
				{
					flag = true;
				}
				else
				{
					list.Add(i);
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			if (!flag)
			{
				tu.FieldValues.Clear();
				return true;
			}
			bool result = false;
			for (int num = list.Count - 1; num >= 0; num--)
			{
				tu.FieldValues.RemoveAt(list[num]);
				result = true;
			}
			return result;
		}
	}
}
