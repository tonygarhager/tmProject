using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class FieldDefinition : IField, INotifyPropertyChanged
	{
		public static readonly int MaximumNameLength = 400;

		public static readonly int MaximumTextFieldValueLength = 2000;

		private FieldEntity _entity;

		private PicklistItemDefinitionCollection _picklist;

		public string Name
		{
			get
			{
				return _entity.Name;
			}
			set
			{
				_entity.Name = value;
				OnPropertyChanged("Name");
			}
		}

		public FieldValueType ValueType
		{
			get
			{
				return _entity.ValueType.Value;
			}
			set
			{
				_entity.ValueType = value;
				OnPropertyChanged("ValueType");
			}
		}

		public IList<string> PicklistItemNames
		{
			get
			{
				List<string> list = new List<string>();
				if (IsPicklist)
				{
					foreach (PicklistItemDefinition picklistItem in PicklistItems)
					{
						list.Add(picklistItem.Name);
					}
					return list;
				}
				return list;
			}
		}

		public PicklistItemDefinitionCollection PicklistItems => _picklist;

		internal FieldEntity Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
				_picklist.UpdateEntities(_entity.PickListItems);
			}
		}

		public bool IsNewObject
		{
			get
			{
				if (_entity.Id != null)
				{
					return _entity.Id.Value == null;
				}
				return true;
			}
		}

		public Guid Id => _entity.UniqueId.Value;

		public bool IsPicklist
		{
			get
			{
				if (ValueType != FieldValueType.SinglePicklist)
				{
					return ValueType == FieldValueType.MultiplePicklist;
				}
				return true;
			}
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				PropertyChangedPrivate += value;
			}
			remove
			{
				PropertyChangedPrivate -= value;
			}
		}

		private event PropertyChangedEventHandler PropertyChangedPrivate;

		public FieldDefinition()
		{
			_entity = new FieldEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.ValueType = FieldValueType.Unknown;
			InitializePicklist(isReadOnly: false);
		}

		public FieldDefinition(string name, FieldValueType valueType)
		{
			_entity = new FieldEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.ValueType = valueType;
			_entity.Name = name;
			InitializePicklist(isReadOnly: false);
		}

		public FieldDefinition(string name, FieldValueType valueType, string[] picklistItems)
		{
			if (valueType != FieldValueType.SinglePicklist && valueType != FieldValueType.MultiplePicklist)
			{
				throw new ArgumentException("ValueType should be FieldValueType.SinglePicklist or FieldValueType.MultiplePicklist.", "valueType");
			}
			_entity = new FieldEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.ValueType = valueType;
			_entity.Name = name;
			InitializePicklist(isReadOnly: false);
			foreach (string name2 in picklistItems)
			{
				_picklist.Add(name2);
			}
		}

		internal FieldDefinition(FieldEntity entity, bool isReadOnly)
		{
			_entity = entity;
			InitializePicklist(isReadOnly);
		}

		private void InitializePicklist(bool isReadOnly)
		{
			_picklist = new PicklistItemDefinitionCollection(_entity, isReadOnly);
			_picklist.CollectionChanged += _picklist_CollectionChanged;
		}

		private void _picklist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged("PicklistItems");
		}

		public FieldDefinition(Field field, bool isReadOnly)
		{
			_entity = new FieldEntity();
			_entity.UniqueId = ((field.ResourceId != null && field.ResourceId.Guid != Guid.Empty) ? field.ResourceId.Guid : Guid.NewGuid());
			if (field.ResourceId != null && field.ResourceId.Id != 0)
			{
				_entity.Id = new Identity(field.ResourceId.Id);
			}
			_entity.Name = field.Name;
			_entity.ValueType = field.ValueType;
			_picklist = new PicklistItemDefinitionCollection(_entity, isReadOnly);
			PicklistField picklistField = field as PicklistField;
			if (picklistField != null)
			{
				foreach (PicklistItem item in picklistField.Picklist)
				{
					_picklist.Add(new PicklistItemDefinition(item, isReadOnly));
				}
			}
		}

		internal Field AsField()
		{
			Field field = null;
			field = ((ValueType != FieldValueType.SinglePicklist && ValueType != FieldValueType.MultiplePicklist) ? new Field(Name, ValueType) : new PicklistField(Name, ValueType, PicklistItems.AsPicklistItems()));
			if (_entity.Id != null && _entity.Id.Value != null)
			{
				field.ResourceId = new PersistentObjectToken(0, _entity.UniqueId.Value);
			}
			return field;
		}

		public FieldDefinition Clone()
		{
			FieldDefinition fieldDefinition = new FieldDefinition(Name, ValueType);
			if (IsPicklist)
			{
				foreach (PicklistItemDefinition picklistItem in PicklistItems)
				{
					fieldDefinition.PicklistItems.Add(picklistItem.Clone());
				}
				return fieldDefinition;
			}
			return fieldDefinition;
		}

		public FieldValue CreateValue()
		{
			switch (ValueType)
			{
			case FieldValueType.SingleString:
				return new SingleStringFieldValue(Name);
			case FieldValueType.MultipleString:
				return new MultipleStringFieldValue(Name);
			case FieldValueType.DateTime:
				return new DateTimeFieldValue(Name);
			case FieldValueType.SinglePicklist:
				return new SinglePicklistFieldValue(Name);
			case FieldValueType.MultiplePicklist:
				return new MultiplePicklistFieldValue(Name);
			case FieldValueType.Integer:
				return new IntFieldValue(Name);
			default:
				return null;
			}
		}

		private void OnPropertyChanged(string property)
		{
			if (this.PropertyChangedPrivate != null)
			{
				this.PropertyChangedPrivate(this, new PropertyChangedEventArgs(property));
			}
		}
	}
}
