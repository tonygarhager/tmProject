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
	/// Represents a collection of picklist items that can be used as values for a given picklist field.
	/// </summary>
	public class PicklistItemDefinitionCollection : ObservableCollection<PicklistItemDefinition>
	{
		private EntityCollection<PicklistItemEntity> _entities;

		private bool _isReadOnly;

		private FieldEntity _parentField;

		/// <summary>
		/// Gets the picklist item with the specified name.
		/// </summary>
		/// <remarks>The name is considered case-insensitive.</remarks>
		/// <param name="name">The name of the picklist item.</param>
		/// <returns>The picklist item definition; or null if not such picklist item exists in this collection.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="name" /> is null or empty.</exception>
		public PicklistItemDefinition this[string name]
		{
			get
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentNullException("name");
				}
				return this.FirstOrDefault((PicklistItemDefinition p) => name.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
			}
		}

		internal PicklistItemDefinition this[Guid id] => this.FirstOrDefault((PicklistItemDefinition p) => id == p.Entity.UniqueId.Value);

		internal PicklistItemDefinitionCollection(FieldEntity fieldEntity, bool isReadOnly)
		{
			_parentField = fieldEntity;
			_entities = fieldEntity.PickListItems;
			_isReadOnly = isReadOnly;
			foreach (PicklistItemEntity entity in _entities)
			{
				base.InsertItem(base.Count, new PicklistItemDefinition(entity, isReadOnly));
			}
		}

		/// <summary>
		/// Inserts an item into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
		/// <param name="item">The object to insert.</param>
		protected override void InsertItem(int index, PicklistItemDefinition item)
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
			PicklistItemDefinition picklistItemDefinition = base[index];
			if (Contains(picklistItemDefinition))
			{
				_entities.Remove(picklistItemDefinition.Entity);
				base.RemoveItem(index);
			}
		}

		/// <summary>
		/// Converts tis collection to a collection of <see cref="T:Sdl.LanguagePlatform.TranslationMemory.PicklistItem" /> objects.
		/// </summary>
		/// <returns></returns>
		internal ICollection<PicklistItem> AsPicklistItems()
		{
			List<PicklistItem> list = new List<PicklistItem>();
			using (IEnumerator<PicklistItemDefinition> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PicklistItemDefinition current = enumerator.Current;
					PicklistItem picklistItem = new PicklistItem(current.Name);
					if (current.Entity.Id != null && current.Entity.Id.Value != null && current.Entity.Id.Value is int)
					{
						picklistItem.ID = (int)current.Entity.Id.Value;
					}
					list.Add(picklistItem);
				}
				return list;
			}
		}

		/// <summary>
		/// Returns true if this collection contains a picklist item with the specified name.
		/// </summary>
		/// <param name="name">The picklist item name. This is considered case insensitive.</param>
		/// <returns>True if this collection contains a picklist item with the same name as the specified picklist item.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="name" /> is null or empty.</exception>
		public bool Contains(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return this.FirstOrDefault((PicklistItemDefinition p) => name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)) != null;
		}

		/// <summary>
		/// Adds a new picklist item definition with the specified name.
		/// </summary>
		/// <param name="name">The picklist item name.</param>
		/// <returns>The newly added picklist item definition.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="name" /> is null or empty.</exception>
		public PicklistItemDefinition Add(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			PicklistItemDefinition picklistItemDefinition = new PicklistItemDefinition(name);
			Add(picklistItemDefinition);
			return picklistItemDefinition;
		}

		/// <summary>
		/// Remove the picklist item definition with the specified name from this collection.
		/// </summary>
		/// <param name="name">The name of the picklist item definition to remove. This is considered case-insensitive.</param>
		/// <returns>True if the item was removed; false if the collection did not contain the specified item.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="name" /> is null or empty.</exception>
		public bool Remove(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			PicklistItemDefinition picklistItemDefinition = this[name];
			if (picklistItemDefinition == null)
			{
				return false;
			}
			Remove(picklistItemDefinition);
			_entities.Remove(picklistItemDefinition.Entity);
			return true;
		}

		internal void UpdateEntities(EntityCollection<PicklistItemEntity> newPicklistItemEntities)
		{
			_entities = newPicklistItemEntities;
			foreach (PicklistItemEntity newPicklistItemEntity in newPicklistItemEntities)
			{
				PicklistItemDefinition picklistItemDefinition = this[newPicklistItemEntity.UniqueId.Value];
				if (picklistItemDefinition != null)
				{
					picklistItemDefinition.Entity = newPicklistItemEntity;
				}
				else
				{
					base.Items.Add(new PicklistItemDefinition(newPicklistItemEntity, _isReadOnly));
				}
			}
			int i;
			for (i = base.Items.Count - 1; i > 0; i--)
			{
				PicklistItemEntity entity = newPicklistItemEntities.FirstOrDefault((PicklistItemEntity ld) => ld.UniqueId.Value == base.Items[i].Entity.UniqueId.Value);
				if (entity == null)
				{
					base.Items.RemoveAt(i);
				}
			}
		}
	}
}
