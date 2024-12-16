using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	internal static class FieldsMerger
	{
		public static void MergeFieldLists(IList<FieldDefinition> targetFields, IEnumerable<FieldDefinition> sourceFields, IDictionary<FieldIdentifier, FieldIdentifier> fieldIdentifierMappings)
		{
			foreach (FieldDefinition sourceField in sourceFields)
			{
				FieldDefinition fieldDefinition = sourceField.Clone();
				FieldValueType valueType = fieldDefinition.ValueType;
				string name = fieldDefinition.Name;
				FieldIdentifier fieldIdentifier = new FieldIdentifier(valueType, name);
				FieldIdentifier fieldIdentifier2 = fieldIdentifierMappings[fieldIdentifier];
				string fieldName = fieldIdentifier2.FieldName;
				FieldValueType fieldValueType = fieldIdentifier2.FieldValueType;
				fieldDefinition.Name = fieldName;
				fieldDefinition.ValueType = fieldValueType;
				CleanPicklistFieldItems(fieldDefinition);
				MergeFieldItem(targetFields, fieldDefinition, fieldIdentifierMappings, fieldIdentifier);
			}
		}

		private static void MergeFieldItem(IList<FieldDefinition> targetFields, FieldDefinition sourceField, IDictionary<FieldIdentifier, FieldIdentifier> fieldIdentifierMappings, FieldIdentifier originalSourceFieldIdentifier)
		{
			FieldDefinition fieldDefinition = targetFields.SingleOrDefault((FieldDefinition targetField) => targetField.Name == sourceField.Name && targetField.ValueType == sourceField.ValueType);
			if (fieldDefinition != null)
			{
				if (fieldDefinition.ValueType == FieldValueType.DateTime || fieldDefinition.ValueType == FieldValueType.Integer)
				{
					string uniqueFieldName = GetUniqueFieldName(GetFieldNames(targetFields), sourceField.Name);
					FieldIdentifier fieldIdentifier2 = fieldIdentifierMappings[originalSourceFieldIdentifier] = new FieldIdentifier(originalSourceFieldIdentifier.FieldValueType, uniqueFieldName);
					sourceField.Name = uniqueFieldName;
					MergeFieldItem(targetFields, sourceField, fieldIdentifierMappings, originalSourceFieldIdentifier);
				}
				else
				{
					MergeFields(fieldDefinition, sourceField);
				}
				return;
			}
			FieldDefinition fieldDefinition2 = targetFields.SingleOrDefault((FieldDefinition targetField) => targetField.Name == sourceField.Name && targetField.ValueType != sourceField.ValueType);
			if (fieldDefinition2 != null)
			{
				string @string = StringResources.ResourceManager.GetString("FieldValueType_" + sourceField.ValueType.ToString());
				string text = sourceField.Name + "_" + @string;
				FieldIdentifier fieldIdentifier4 = fieldIdentifierMappings[originalSourceFieldIdentifier] = new FieldIdentifier(originalSourceFieldIdentifier.FieldValueType, text);
				sourceField.Name = text;
				MergeFieldItem(targetFields, sourceField, fieldIdentifierMappings, originalSourceFieldIdentifier);
			}
			else
			{
				targetFields.Add(sourceField);
			}
		}

		private static IList<string> GetFieldNames(IList<FieldDefinition> fields)
		{
			IList<string> list = new List<string>();
			foreach (FieldDefinition field in fields)
			{
				list.Add(field.Name);
			}
			return list;
		}

		private static string GetUniqueFieldName(IList<string> fieldNames, string fieldName)
		{
			string text = fieldName;
			int num = 2;
			while (fieldNames.Contains(text, StringComparer.OrdinalIgnoreCase))
			{
				text = fieldName + "_" + num.ToString();
				num++;
			}
			return text;
		}

		private static void MergeFields(FieldDefinition targetField, FieldDefinition sourceField)
		{
			foreach (PicklistItemDefinition picklistItem in sourceField.PicklistItems)
			{
				if (!targetField.PicklistItems.Contains(picklistItem.Name))
				{
					targetField.PicklistItems.Add(picklistItem.Name);
				}
			}
		}

		private static void CleanPicklistFieldItems(FieldDefinition field)
		{
			foreach (PicklistItemDefinition picklistItem in field.PicklistItems)
			{
				if (!Field.IsValidName(picklistItem.Name))
				{
					picklistItem.Name = Field.RemoveIllegalChars(picklistItem.Name);
				}
			}
		}
	}
}
