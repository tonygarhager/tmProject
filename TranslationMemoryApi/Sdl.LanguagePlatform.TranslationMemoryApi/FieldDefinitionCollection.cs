using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a collection of custom fields in a translation memory (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.FieldDefinitions" />).
	/// </summary>
	public class FieldDefinitionCollection : ObservableCollection<FieldDefinition>, IFieldDefinitions
	{
		private EntityCollection<FieldEntity> _entities;

		private readonly bool _isReadOnly;

		/// <summary>
		/// Gets the <see cref="T:Sdl.LanguagePlatform.TranslationMemory.Field" /> with the specified name. The name is considered case-insensitive.
		/// </summary>
		/// <remarks>
		/// Do not use this accessor to modify field settings directly as these changes will not be 
		/// propagated to the underlying Field collection.
		/// </remarks>
		public FieldDefinition this[string name] => this.FirstOrDefault((FieldDefinition f) => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

		internal FieldDefinition this[Guid id] => this.FirstOrDefault((FieldDefinition fd) => fd.Entity.UniqueId.Value == id);

		internal EntityCollection<FieldEntity> Entities => _entities;

		/// <summary>
		/// Creates a new, empty field definition collection.
		/// </summary>
		/// <remarks>This constructor should typically only be used by implementers of <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.IFieldsTemplate" /> or <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory" />.</remarks>
		public FieldDefinitionCollection()
		{
			_entities = new EntityCollection<FieldEntity>();
			_isReadOnly = false;
		}

		internal FieldDefinitionCollection(EntityCollection<FieldEntity> entities, bool isReadOnly)
		{
			_entities = entities;
			_isReadOnly = isReadOnly;
			int num = 0;
			foreach (FieldEntity entity in entities)
			{
				base.InsertItem(num++, new FieldDefinition(entity, isReadOnly));
			}
		}

		/// <summary>
		/// Converts this collection to a collection a <see cref="T:Sdl.LanguagePlatform.TranslationMemory.Field" /> objects.
		/// </summary>
		/// <returns>A collection of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.Field" /> objects.</returns>
		internal ICollection<Field> AsFields()
		{
			List<Field> list = new List<Field>();
			using (IEnumerator<FieldDefinition> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FieldDefinition current = enumerator.Current;
					list.Add(current.AsField());
				}
				return list;
			}
		}

		/// <summary>
		/// Returns the field with the specified name, or <c>null</c> if there is no field with that 
		/// name.
		/// </summary>
		public IField LookupIField(string name)
		{
			return this[name];
		}

		/// <summary>
		/// Returns the field with the specified GUID, or <c>null</c> if there is no field with that 
		/// GUID.
		/// </summary>
		public IField LookupIField(Guid guid)
		{
			return this[guid];
		}

		/// <summary>
		/// Adds a number of fields to the collection.
		/// </summary>
		/// <param name="fieldDefinitions">The fields to be added.</param>
		public void AddRange(IEnumerable<FieldDefinition> fieldDefinitions)
		{
			foreach (FieldDefinition fieldDefinition in fieldDefinitions)
			{
				InsertItem(base.Count, fieldDefinition);
			}
		}

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
		/// <param name="item">The object to insert.</param>
		protected override void InsertItem(int index, FieldDefinition item)
		{
			_entities.Add(item.Entity);
			base.InsertItem(index, item);
		}

		/// <summary>
		/// Removes all items from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			_entities.Clear();
			base.ClearItems();
		}

		/// <summary>
		/// Removes the item at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		protected override void RemoveItem(int index)
		{
			FieldDefinition fieldDefinition = base[index];
			if (Contains(fieldDefinition))
			{
				_entities.Remove(fieldDefinition.Entity);
				base.RemoveItem(index);
			}
		}

		internal void UpdateEntities(EntityCollection<FieldEntity> newFieldEntities)
		{
			_entities = newFieldEntities;
			foreach (FieldEntity newFieldEntity in newFieldEntities)
			{
				FieldDefinition fieldDefinition = this[newFieldEntity.UniqueId.Value];
				if (fieldDefinition != null)
				{
					fieldDefinition.Entity = newFieldEntity;
				}
				else
				{
					base.Items.Add(new FieldDefinition(newFieldEntity, _isReadOnly));
				}
			}
			int i;
			for (i = base.Items.Count - 1; i > 0; i--)
			{
				FieldEntity entity = newFieldEntities.FirstOrDefault((FieldEntity ld) => ld.UniqueId.Value == base.Items[i].Entity.UniqueId.Value);
				if (entity == null)
				{
					base.Items.RemoveAt(i);
				}
			}
		}
	}
}
