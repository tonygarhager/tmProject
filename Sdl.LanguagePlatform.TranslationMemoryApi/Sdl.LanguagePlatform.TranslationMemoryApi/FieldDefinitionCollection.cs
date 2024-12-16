using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class FieldDefinitionCollection : ObservableCollection<FieldDefinition>, IFieldDefinitions
	{
		private EntityCollection<FieldEntity> _entities;

		private readonly bool _isReadOnly;

		public FieldDefinition this[string name] => this.FirstOrDefault((FieldDefinition f) => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

		internal FieldDefinition this[Guid id] => this.FirstOrDefault((FieldDefinition fd) => fd.Entity.UniqueId.Value == id);

		internal EntityCollection<FieldEntity> Entities => _entities;

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

		public IField LookupIField(string name)
		{
			return this[name];
		}

		public IField LookupIField(Guid guid)
		{
			return this[guid];
		}

		public void AddRange(IEnumerable<FieldDefinition> fieldDefinitions)
		{
			foreach (FieldDefinition fieldDefinition in fieldDefinitions)
			{
				InsertItem(base.Count, fieldDefinition);
			}
		}

		protected override void InsertItem(int index, FieldDefinition item)
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
