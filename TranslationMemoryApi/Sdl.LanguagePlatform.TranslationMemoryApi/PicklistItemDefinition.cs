using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a picklist item definition, which is a possible value that can be assigned to a picklist field.
	/// See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.FieldDefinition.PicklistItems" />.
	/// </summary>
	public class PicklistItemDefinition : INotifyPropertyChanged
	{
		/// <summary>
		/// The maximum length of a picklist item name. 
		/// </summary>
		public static readonly int MaximumNameLength = 400;

		private PicklistItemEntity _entity;

		private bool _isReadOnly;

		/// <summary>
		/// Gets or sets the name of the picklist field definition.
		/// </summary>
		/// <remarks>Note that you have to save the translation memory of the fields template to persists the change after setting this property, depending on whether
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

		internal PicklistItemEntity Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
			}
		}

		/// <summary>
		/// Gets a unique Id for this picklist item definition.
		/// </summary>
		public Guid Id => _entity.UniqueId.Value;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				PrivatePropertyChanged += value;
			}
			remove
			{
				PrivatePropertyChanged -= value;
			}
		}

		private event PropertyChangedEventHandler PrivatePropertyChanged;

		/// <summary>
		/// Creates a new picklist field definition.
		/// </summary>
		public PicklistItemDefinition()
		{
			_entity = new PicklistItemEntity();
			_entity.UniqueId = Guid.NewGuid();
		}

		/// <summary>
		/// Creates a new picklist field definition with the specified name.
		/// </summary>
		/// <param name="name">The name of the picklist field.</param>
		public PicklistItemDefinition(string name)
		{
			_entity = new PicklistItemEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.Name = name;
		}

		/// <summary>
		/// Constructor for an existing picklist field definition.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="isReadOnly">Whether the object can be modified.</param>
		internal PicklistItemDefinition(PicklistItemEntity entity, bool isReadOnly)
		{
			_entity = entity;
			_isReadOnly = isReadOnly;
		}

		internal PicklistItemDefinition(PicklistItem picklistItem, bool isReadOnly)
		{
			_entity = new PicklistItemEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.Name = picklistItem.Name;
			if (picklistItem.ID.HasValue)
			{
				_entity.Id = new Identity(picklistItem.ID.Value);
			}
			_isReadOnly = isReadOnly;
		}

		/// <summary>
		/// Ases the picklist item.
		/// </summary>
		/// <returns></returns>
		internal PicklistItem AsPicklistItem()
		{
			return new PicklistItem(Name);
		}

		/// <summary>
		/// Creates a deep copy of this picklist field definition.
		/// </summary>
		/// <returns>A deep copy of the picklist field definition.</returns>
		public PicklistItemDefinition Clone()
		{
			return new PicklistItemDefinition(Name);
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PrivatePropertyChanged != null)
			{
				this.PrivatePropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
