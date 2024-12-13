using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Provides a method to update a translation memory's field declarations, 
	/// applying a given field change set.
	/// </summary>
	internal class TMFieldUpdater
	{
		private ITranslationMemoryService _service;

		private PersistentObjectToken _translationMemoryId;

		private FieldDefinitions _originalFields;

		private Container _container;

		private bool _synchronizeIds;

		/// <summary>
		/// Gets the service instance which is used for the update
		/// </summary>
		public ITranslationMemoryService Service => _service;

		/// <summary>
		/// Gets the ID of the translation memory to update
		/// </summary>
		public PersistentObjectToken TranslationMemoryId => _translationMemoryId;

		private FieldDefinitions CurrentTmFields
		{
			get
			{
				if (_originalFields == null)
				{
					_originalFields = RemoveSystemFields(_service.GetFields(_container, _translationMemoryId));
				}
				return _originalFields;
			}
		}

		/// <summary>
		/// Initializes a new instance with the specified values.
		/// </summary>
		/// <param name="service">The service to use to update the fields</param>
		/// <param name="container">The container which contains the TM</param>
		/// <param name="tmId">The ID of the translation memory to update</param>
		/// <param name="synchronizeIds">True to synchronizes local field and entity ids with values returned from server side.</param>
		public TMFieldUpdater(ITranslationMemoryService service, Container container, PersistentObjectToken tmId, bool synchronizeIds)
		{
			_container = container;
			_service = service;
			_translationMemoryId = tmId;
			_synchronizeIds = synchronizeIds;
		}

		private static FieldDefinitions RemoveSystemFields(FieldDefinitions fieldDefinitions)
		{
			FieldDefinitions fieldDefinitions2 = new FieldDefinitions();
			foreach (Field fieldDefinition in fieldDefinitions)
			{
				if (!fieldDefinition.IsSystemField)
				{
					fieldDefinitions2.Add(fieldDefinition);
				}
			}
			return fieldDefinitions2;
		}

		/// <summary>
		/// Updates the configured translation memory's field declarations according to the 
		/// changes contained in the two paramters.
		/// </summary>
		/// <param name="originalFields">The original translation memory's field declarations</param>
		/// <param name="newFields">The new field declarations, which contain the updated declarations</param>
		public void UpdateTmFields(EntityCollection<FieldEntity> originalFields, EntityCollection<FieldEntity> newFields)
		{
			FieldGroupChangeSet synchroniser = FieldGroupChangeSet.GenerateChangeSet(originalFields, newFields);
			UpdateTm(synchroniser, newFields);
		}

		public void AddTmFields(ICollection<FieldEntity> newFields)
		{
			FieldGroupChangeSet synchroniser = FieldGroupChangeSet.GenerateAddFieldsChangeSet(newFields);
			UpdateTm(synchroniser, newFields);
		}

		/// <summary>
		/// Updates the configured translation memory with the specified field group change set.
		/// </summary>
		/// <param name="synchroniser">A field group change set to apply to the TM</param>
		public void UpdateTm(FieldGroupChangeSet synchroniser)
		{
			UpdateTm(synchroniser, null);
		}

		private void UpdateTm(FieldGroupChangeSet synchroniser, ICollection<FieldEntity> newFieldEntities)
		{
			DeletePicklistValues(synchroniser.DeletedPicklistValues);
			RenamePicklistValues(synchroniser.RenamedPickListValues);
			AddPicklistValues(synchroniser.AddedPicklistValues, newFieldEntities);
			DeleteFields(synchroniser.DeletedFields);
			Dictionary<PersistentObjectToken, PersistentObjectToken> oldIdsToNewIds = AddFields(synchroniser.AddedFields);
			if (_synchronizeIds && synchroniser.AddedFields != null && synchroniser.AddedFields.Count > 0)
			{
				UpdateLocalFieldAndEntityIds(oldIdsToNewIds, synchroniser.AddedFields, newFieldEntities);
			}
			RenameFields(synchroniser.RenamedFields);
		}

		private void AddPicklistValues(Dictionary<string, List<string>> values, ICollection<FieldEntity> newFields)
		{
			foreach (KeyValuePair<string, List<string>> value2 in values)
			{
				string key = value2.Key;
				List<string> value = value2.Value;
				PersistentObjectToken[] picklistItemTokens = _service.AddPicklistValues(_container, _translationMemoryId, GetCurrentTmFieldId(key), value.ToArray());
				if (newFields != null)
				{
					FieldEntity fieldEntity = GetFieldEntity(newFields, key);
					if (fieldEntity != null)
					{
						UpdatePicklistItemIds(fieldEntity.PickListItems, value, picklistItemTokens);
					}
				}
			}
		}

		public FieldEntity GetFieldEntity(ICollection<FieldEntity> fields, string fieldName)
		{
			foreach (FieldEntity field in fields)
			{
				if (object.Equals(field.Name, fieldName))
				{
					return field;
				}
			}
			return null;
		}

		private void UpdatePicklistItemIds(ICollection<PicklistItemEntity> picklistItems, IList<string> picklistItemNames, IList<PersistentObjectToken> picklistItemTokens)
		{
			foreach (PicklistItemEntity picklistItem in picklistItems)
			{
				int num = picklistItemNames.IndexOf(picklistItem.Name);
				if (num > -1 && (picklistItem.Id == null || picklistItem.Id.IntValue == 0))
				{
					PersistentObjectToken persistentObjectToken = picklistItemTokens[num];
					picklistItem.Id = new Identity(persistentObjectToken.Id);
					picklistItem.UniqueId = persistentObjectToken.Guid;
				}
			}
		}

		private void RenamePicklistValues(Dictionary<string, Dictionary<string, string>> values)
		{
			foreach (KeyValuePair<string, Dictionary<string, string>> value in values)
			{
				foreach (KeyValuePair<string, string> item in value.Value)
				{
					_service.RenamePicklistValue(_container, _translationMemoryId, GetCurrentTmFieldId(value.Key), item.Key, item.Value);
				}
			}
		}

		private void DeletePicklistValues(Dictionary<string, List<string>> values)
		{
			foreach (KeyValuePair<string, List<string>> value in values)
			{
				foreach (string item in value.Value)
				{
					_service.RemovePicklistValue(_container, _translationMemoryId, GetCurrentTmFieldId(value.Key), item);
				}
			}
		}

		private void DeleteFields(List<string> deletedFields)
		{
			foreach (string deletedField in deletedFields)
			{
				_service.RemoveField(_container, _translationMemoryId, GetCurrentTmFieldId(deletedField));
			}
		}

		private Dictionary<PersistentObjectToken, PersistentObjectToken> AddFields(FieldDefinitions addedFields)
		{
			Dictionary<PersistentObjectToken, PersistentObjectToken> dictionary = new Dictionary<PersistentObjectToken, PersistentObjectToken>();
			foreach (Field addedField in addedFields)
			{
				PersistentObjectToken value = _service.AddField(_container, _translationMemoryId, addedField);
				dictionary.Add(new PersistentObjectToken(addedField.ResourceId.Id, addedField.ResourceId.Guid), value);
			}
			return dictionary;
		}

		private void RenameFields(Dictionary<string, string> renamedFields)
		{
			foreach (string key in renamedFields.Keys)
			{
				string newName = renamedFields[key];
				_service.RenameField(_container, _translationMemoryId, GetCurrentTmFieldId(key), newName);
			}
		}

		private PersistentObjectToken GetCurrentTmFieldId(string fieldName)
		{
			Field field = CurrentTmFields[fieldName];
			if (field == null)
			{
				throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Could not find field {0} in database", fieldName));
			}
			return field.ResourceId;
		}

		/// <summary>
		/// Synchronizes local field and entity ids with values returned from server side.
		/// </summary>
		/// <param name="oldIdsToNewIds">Maps old ids to new ids returned from server.</param>
		/// <param name="fieldsToUpdate">Fields to be updated entities.</param>
		/// <param name="entitiesToUpdate">Entities to be updated.</param>
		private static void UpdateLocalFieldAndEntityIds(Dictionary<PersistentObjectToken, PersistentObjectToken> oldIdsToNewIds, FieldDefinitions fieldsToUpdate, IEnumerable<FieldEntity> entitiesToUpdate)
		{
			foreach (KeyValuePair<PersistentObjectToken, PersistentObjectToken> kvp in oldIdsToNewIds)
			{
				if (kvp.Key.Guid != Guid.Empty && kvp.Value != null && kvp.Value.Guid != Guid.Empty && kvp.Key.Id == 0)
				{
					if (fieldsToUpdate != null)
					{
						Field field = fieldsToUpdate.Lookup(kvp.Key.Guid);
						if (field != null)
						{
							field.ResourceId.Id = kvp.Value.Id;
							field.ResourceId.Guid = kvp.Value.Guid;
						}
					}
					if (entitiesToUpdate != null)
					{
						FieldEntity fieldEntity = entitiesToUpdate.FirstOrDefault(delegate(FieldEntity e)
						{
							Guid? uniqueId = e.UniqueId;
							Guid guid = kvp.Key.Guid;
							if (!uniqueId.HasValue)
							{
								return false;
							}
							return !uniqueId.HasValue || uniqueId.GetValueOrDefault() == guid;
						});
						if (fieldEntity != null)
						{
							fieldEntity.Id = new Identity(kvp.Value.Id);
							fieldEntity.UniqueId = kvp.Value.Guid;
						}
					}
				}
			}
		}
	}
}
