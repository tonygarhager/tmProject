using Sdl.Core.Api.DataAccess;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.ComponentModel;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class PicklistItemDefinition : INotifyPropertyChanged
	{
		public static readonly int MaximumNameLength = 400;

		private PicklistItemEntity _entity;

		private bool _isReadOnly;

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

		public Guid Id => _entity.UniqueId.Value;

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

		public PicklistItemDefinition()
		{
			_entity = new PicklistItemEntity();
			_entity.UniqueId = Guid.NewGuid();
		}

		public PicklistItemDefinition(string name)
		{
			_entity = new PicklistItemEntity();
			_entity.UniqueId = Guid.NewGuid();
			_entity.Name = name;
		}

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

		internal PicklistItem AsPicklistItem()
		{
			return new PicklistItem(Name);
		}

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
