using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts
{
	[DataContract(Namespace = "http://sdl.com/languageplatform/2010", Name = "FieldGroupChangeSet")]
	[KnownType(typeof(FieldDefinitions))]
	public class FieldGroupChangeSet : IExtensibleDataObject
	{
		private ExtensionDataObject _extensionDataObject;

		private FieldDefinitions _addedFields = new FieldDefinitions();

		private List<string> _deletedFields = new List<string>();

		private Dictionary<string, string> _renamedFields = new Dictionary<string, string>();

		private Dictionary<string, List<string>> _addedPicklistValues = new Dictionary<string, List<string>>();

		private Dictionary<string, List<string>> _deletedPicklistValues = new Dictionary<string, List<string>>();

		private Dictionary<string, Dictionary<string, string>> _renamedPickListValues = new Dictionary<string, Dictionary<string, string>>();

		public ExtensionDataObject ExtensionData
		{
			get
			{
				return _extensionDataObject;
			}
			set
			{
				_extensionDataObject = value;
			}
		}

		[DataMember]
		public FieldDefinitions AddedFields
		{
			get
			{
				return _addedFields;
			}
			set
			{
				_addedFields = value;
			}
		}

		[DataMember]
		public List<string> DeletedFields
		{
			get
			{
				return _deletedFields;
			}
			set
			{
				_deletedFields = value;
			}
		}

		[DataMember]
		public Dictionary<string, string> RenamedFields
		{
			get
			{
				return _renamedFields;
			}
			set
			{
				_renamedFields = value;
			}
		}

		[DataMember]
		public Dictionary<string, List<string>> AddedPicklistValues
		{
			get
			{
				return _addedPicklistValues;
			}
			set
			{
				_addedPicklistValues = value;
			}
		}

		[DataMember]
		public Dictionary<string, List<string>> DeletedPicklistValues
		{
			get
			{
				return _deletedPicklistValues;
			}
			set
			{
				_deletedPicklistValues = value;
			}
		}

		[DataMember]
		public Dictionary<string, Dictionary<string, string>> RenamedPickListValues
		{
			get
			{
				return _renamedPickListValues;
			}
			set
			{
				_renamedPickListValues = value;
			}
		}

		public bool HasChanges
		{
			get
			{
				if (_addedFields.Count <= 0 && _addedPicklistValues.Count <= 0 && _deletedFields.Count <= 0 && _deletedPicklistValues.Count <= 0 && _renamedFields.Count <= 0)
				{
					return _renamedPickListValues.Count > 0;
				}
				return true;
			}
		}

		public static FieldGroupChangeSet GenerateAddFieldsChangeSet(ICollection<FieldEntity> newFields)
		{
			FieldGroupChangeSet fieldGroupChangeSet = new FieldGroupChangeSet();
			foreach (FieldEntity newField in newFields)
			{
				fieldGroupChangeSet.AddedFields.Add(ToField(newField));
			}
			return fieldGroupChangeSet;
		}

		public static FieldGroupChangeSet GenerateChangeSet(EntityCollection<FieldEntity> originalFields, EntityCollection<FieldEntity> newFields)
		{
			FieldGroupChangeSet fieldGroupChangeSet = new FieldGroupChangeSet();
			foreach (FieldEntity newField in newFields)
			{
				FieldEntity fieldEntity = Find(originalFields, newField.UniqueId.Value);
				FieldEntity fieldEntity2 = Find(originalFields, newField.Name);
				bool flag = (!(fieldEntity != null)) ? (fieldEntity2 != null) : (fieldEntity.Name == newField.Name);
				bool flag2 = flag && fieldEntity2.ValueType == newField.ValueType;
				if (fieldEntity != null)
				{
					if (IsPickListField(newField) && IsPickListField(fieldEntity))
					{
						SyncPicklists(fieldGroupChangeSet, newField, fieldEntity);
					}
					else if (!IsPickListField(newField) && newField.PickListItems.Count > 0)
					{
						throw new InvalidOperationException("Only picklist fields can have picklist items.");
					}
					if (!flag)
					{
						fieldGroupChangeSet.RenamedFields.Add(fieldEntity.Name, newField.Name);
					}
					else if (!flag2)
					{
						fieldGroupChangeSet.DeletedFields.Add(fieldEntity2.Name);
						fieldGroupChangeSet.AddedFields.Add(ToField(newField));
					}
				}
				else if (flag)
				{
					if (!flag2)
					{
						fieldGroupChangeSet.DeletedFields.Add(fieldEntity2.Name);
						fieldGroupChangeSet.AddedFields.Add(ToField(newField));
					}
				}
				else
				{
					fieldGroupChangeSet.AddedFields.Add(ToField(newField));
				}
			}
			foreach (Identity deletedFieldId in newFields.RemovedEntities)
			{
				FieldEntity fieldEntity3 = originalFields.FirstOrDefault((FieldEntity f) => f.Id.IntValue == deletedFieldId.IntValue);
				if (fieldEntity3 != null)
				{
					fieldGroupChangeSet.DeletedFields.Add(fieldEntity3.Name);
				}
			}
			return fieldGroupChangeSet;
		}

		private void AddDeletedPicklistValue(string picklistFieldName, string picklistValue)
		{
			if (!DeletedPicklistValues.TryGetValue(picklistFieldName, out List<string> value))
			{
				value = new List<string>(1);
				DeletedPicklistValues[picklistFieldName] = value;
			}
			value.Add(picklistValue);
		}

		private void AddAddedPicklistValue(string picklistFieldName, string picklistValue)
		{
			if (!AddedPicklistValues.TryGetValue(picklistFieldName, out List<string> value))
			{
				value = new List<string>(1);
				AddedPicklistValues[picklistFieldName] = value;
			}
			value.Add(picklistValue);
		}

		private void AddRenamedPicklistValue(string picklistFieldName, string oldPicklistValue, string newPicklistValue)
		{
			if (!RenamedPickListValues.TryGetValue(picklistFieldName, out Dictionary<string, string> value))
			{
				value = new Dictionary<string, string>(1);
				RenamedPickListValues[picklistFieldName] = value;
			}
			value.Add(oldPicklistValue, newPicklistValue);
		}

		private static void SyncPicklists(FieldGroupChangeSet changeSet, FieldEntity newField, FieldEntity existingField)
		{
			foreach (PicklistItemEntity newPicklistItem in newField.PickListItems)
			{
				if (newPicklistItem.Id == null || newPicklistItem.Id.Value == null || (int)newPicklistItem.Id.Value == 0)
				{
					changeSet.AddAddedPicklistValue(existingField.Name, newPicklistItem.Name);
				}
				else
				{
					PicklistItemEntity picklistItemEntity = existingField.PickListItems.FirstOrDefault((PicklistItemEntity item) => (int)item.Id.Value == (int)newPicklistItem.Id.Value);
					if (picklistItemEntity != null)
					{
						if (newPicklistItem.Name != picklistItemEntity.Name)
						{
							changeSet.AddRenamedPicklistValue(existingField.Name, picklistItemEntity.Name, newPicklistItem.Name);
						}
					}
					else if (existingField.PickListItems.FirstOrDefault((PicklistItemEntity item) => item.Name == newPicklistItem.Name) == null)
					{
						changeSet.AddAddedPicklistValue(existingField.Name, newPicklistItem.Name);
					}
				}
			}
			foreach (Identity removedPicklistItemId in newField.PickListItems.RemovedEntities)
			{
				PicklistItemEntity picklistItemEntity2 = existingField.PickListItems.FirstOrDefault((PicklistItemEntity pli) => pli.Id.IntValue == removedPicklistItemId.IntValue);
				changeSet.AddDeletedPicklistValue(existingField.Name, picklistItemEntity2.Name);
			}
		}

		private static bool IsPickListField(FieldEntity newField)
		{
			if (newField.ValueType != FieldValueType.MultiplePicklist)
			{
				return newField.ValueType == FieldValueType.SinglePicklist;
			}
			return true;
		}

		private static FieldEntity Find(ICollection<FieldEntity> fields, string name)
		{
			return fields.FirstOrDefault((FieldEntity f) => f.Name == name);
		}

		private static FieldEntity Find(ICollection<FieldEntity> fields, Guid uniqueId)
		{
			return fields.FirstOrDefault((FieldEntity f) => f.UniqueId.Value == uniqueId);
		}

		private static Field ToField(FieldEntity fieldEntity)
		{
			if (fieldEntity.ValueType.Value == FieldValueType.SinglePicklist || fieldEntity.ValueType.Value == FieldValueType.MultiplePicklist)
			{
				PicklistField picklistField = new PicklistField();
				int id = (fieldEntity.Id != null && fieldEntity.Id.Value != null) ? ((int)fieldEntity.Id.Value) : 0;
				picklistField.ResourceId = new PersistentObjectToken(id, fieldEntity.UniqueId.Value);
				picklistField.Name = fieldEntity.Name;
				picklistField.ValueType = fieldEntity.ValueType.Value;
				{
					foreach (PicklistItemEntity pickListItem in fieldEntity.PickListItems)
					{
						int? iD = (pickListItem.Id != null && pickListItem.Id.Value != null) ? ((int?)pickListItem.Id.Value) : null;
						PicklistItem picklistItem = new PicklistItem(pickListItem.Name);
						picklistItem.ID = iD;
						picklistField.Picklist.Add(picklistItem);
					}
					return picklistField;
				}
			}
			Field field = new Field();
			int id2 = (fieldEntity.Id != null && fieldEntity.Id.Value != null) ? ((int)fieldEntity.Id.Value) : 0;
			field.ResourceId = new PersistentObjectToken(id2, fieldEntity.UniqueId.Value);
			field.Name = fieldEntity.Name;
			field.ValueType = fieldEntity.ValueType.Value;
			return field;
		}
	}
}
