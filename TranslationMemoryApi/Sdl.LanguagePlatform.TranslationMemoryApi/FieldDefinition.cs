using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a custom field that can be associated with a translation memory or a fields template and for which values (<see cref="T:Sdl.LanguagePlatform.TranslationMemory.FieldValue" />)
	/// can be associated with translation units in a translation memory (see <see cref="P:Sdl.LanguagePlatform.TranslationMemory.TranslationUnit.FieldValues" />). These field values can subsequently be used in filter 
	/// expressions (see <see cref="T:Sdl.LanguagePlatform.TranslationMemory.FilterExpression" />) when searching or updating translation units.
	/// </summary>
	public class FieldDefinition : IField, INotifyPropertyChanged
	{
		/// <summary>
		/// The maximum length of a field name.
		/// </summary>
		public static readonly int MaximumNameLength = 400;

		/// <summary>
		/// The maximum length of a text field value.
		/// </summary>
		public static readonly int MaximumTextFieldValueLength = 2000;

		private FieldEntity _entity;

		private PicklistItemDefinitionCollection _picklist;

		/// <summary>
		/// Gets or sets the name of the field.
		/// </summary>
		/// <remarks>Note that you have to save the translation memory of the fields template to persist the change after setting this property, depending on whether
		/// the field belongs to a translation memory or a fields template.</remarks>
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

		/// <summary>
		/// Gets or sets the type of value that can be associated with this field.
		/// </summary>
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

		/// <inheritdoc />
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

		/// <summary>
		/// Gets the picklist items representing the possible values for the field, in case this field is of type <see cref="F:Sdl.LanguagePlatform.TranslationMemory.FieldValueType.SinglePicklist" />
		/// or <see cref="F:Sdl.LanguagePlatform.TranslationMemory.FieldValueType.MultiplePicklist" />.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Thrown when trying to access this property for a field that is not of type <see cref="F:Sdl.LanguagePlatform.TranslationMemory.FieldValueType.SinglePicklist" />
		/// or <see cref="F:Sdl.LanguagePlatform.TranslationMemory.FieldValueType.MultiplePicklist" />.</exception>
		/// <remarks>Note that you have to save the translation memory of the fields template to persist the change after adding or removing picklist items, 
		/// depending on whether the field belongs to a translation memory or a fields template.</remarks>
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

		/// <summary>
		/// Gets a value indicating whether this instance is new object.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is new object; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Gets a unique Id for this field definition.
		/// </summary>
		public Guid Id => _entity.UniqueId.Value;

		/// <summary>
		/// Gets a value indicating whether this instance is picklist.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is picklist; otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
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

		/// <summary>
		/// Creates a new empty field definition.
		/// </summary>
		/// <remarks>In order to add the new field to a translation memory or fields template, add it to the corresponding fields collection (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.FieldDefinitions" />
		/// or <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.FieldDefinitions" />) and save the translation memory or fields template to persist the change (<see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> or <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Save" />)</remarks>
		public FieldDefinition()
		{
			_entity = new FieldEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.ValueType = FieldValueType.Unknown;
			InitializePicklist(isReadOnly: false);
		}

		/// <summary>
		/// Creates a new field definition with the specified name and value type.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <param name="valueType">The type of value this field can hold.</param>
		/// <remarks>In order to add the new field to a translation memory or fields template, add it to the corresponding fields collection (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.FieldDefinitions" />
		/// or <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.FieldDefinitions" />) and save the translation memory or fields template to persist the change (<see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> or <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Save" />)</remarks>
		public FieldDefinition(string name, FieldValueType valueType)
		{
			_entity = new FieldEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.ValueType = valueType;
			_entity.Name = name;
			InitializePicklist(isReadOnly: false);
		}

		/// <summary>
		/// Creates a new picklist field definition with the specified name and value type.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <param name="valueType">The type of value this field can hold. This has to be <see cref="F:Sdl.LanguagePlatform.TranslationMemory.FieldValueType.SinglePicklist" />
		/// or <see cref="F:Sdl.LanguagePlatform.TranslationMemory.FieldValueType.MultiplePicklist" />.</param>
		/// <param name="picklistItems">An array of picklist values.</param>
		/// <remarks>In order to add the new field to a translation memory or fields template, add it to the corresponding fields collection (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.FieldDefinitions" />
		/// or <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.FieldDefinitions" />) and save the translation memory or fields template to persist the change (<see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationMemory.Save" /> or <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Save" />)</remarks>
		/// <exception cref="T:System.ArgumentException">Thrown if <paramref name="valueType" /> is not <see cref="F:Sdl.LanguagePlatform.TranslationMemory.FieldValueType.SinglePicklist" />
		/// or <see cref="F:Sdl.LanguagePlatform.TranslationMemory.FieldValueType.MultiplePicklist" />.</exception>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.FieldDefinition" /> class.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="isReadOnly">if set to <c>true</c> [is read only].</param>
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

		/// <summary>
		/// Converts this field definition to a <see cref="T:Sdl.LanguagePlatform.TranslationMemory.Field" />.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Creates a deep copy of this field definition.
		/// </summary>
		/// <remarks>The field definition returned </remarks>
		/// <returns>A copy of this field definition.</returns>
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

		/// <summary>
		/// Creates a FieldValue instance, with the same name and type as this field declaration.
		/// </summary>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemory.FieldValue" /> instance which is compatible with this field
		/// declaration.</returns>
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
