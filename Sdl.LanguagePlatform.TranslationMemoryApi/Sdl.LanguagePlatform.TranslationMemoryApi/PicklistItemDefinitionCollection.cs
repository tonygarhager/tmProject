using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class PicklistItemDefinitionCollection : ObservableCollection<PicklistItemDefinition>
	{
		private EntityCollection<PicklistItemEntity> _entities;

		private bool _isReadOnly;

		private FieldEntity _parentField;

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

		protected override void InsertItem(int index, PicklistItemDefinition item)
		{
			_entities.Add(item.Entity);
			base.InsertItem(index, item);
		}

		protected override void ClearItems()
		{
			_entities.Clear();
			base.ClearItems();
		}

		protected override void RemoveItem(int index)
		{
			PicklistItemDefinition picklistItemDefinition = base[index];
			if (Contains(picklistItemDefinition))
			{
				_entities.Remove(picklistItemDefinition.Entity);
				base.RemoveItem(index);
			}
		}

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

		public bool Contains(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return this.FirstOrDefault((PicklistItemDefinition p) => name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)) != null;
		}

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
