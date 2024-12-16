using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public abstract class RemoteTranslationMemory : INotifyPropertyChanged, IEditableObject
	{
		internal TranslationMemoryEntity Entity
		{
			get;
			set;
		}

		protected TranslationMemoryEntity BackupEntity
		{
			get;
			private set;
		}

		[Required(ErrorMessage = "Required Field")]
		[StringLength(150, ErrorMessage = "Name too long!")]
		public string Name
		{
			get
			{
				VerifyNotDeleted();
				return Entity.Name;
			}
			set
			{
				VerifyNotDeleted();
				char[] invalidPathChars = Path.GetInvalidPathChars();
				if (invalidPathChars.Any((char invalidChar) => value.Contains(invalidChar)))
				{
					throw new InvalidOperationException("Name contains invalid characters");
				}
				PropertyValueValidator.Validate(this, value);
				Entity.Name = value;
				OnPropertyChanged("Name");
			}
		}

		[StringLength(255, ErrorMessage = "Description too long!")]
		public string Description
		{
			get
			{
				VerifyNotDeleted();
				return Entity.Description;
			}
			set
			{
				VerifyNotDeleted();
				PropertyValueValidator.Validate(this, value);
				Entity.Description = value;
				OnPropertyChanged("Description");
			}
		}

		public Guid Id
		{
			get
			{
				VerifyNotDeleted();
				return Entity.UniqueId.Value;
			}
		}

		[StringLength(1024, ErrorMessage = "Copyright too long!")]
		public string Copyright
		{
			get
			{
				VerifyNotDeleted();
				return Entity.Copyright;
			}
			set
			{
				VerifyNotDeleted();
				PropertyValueValidator.Validate(this, value);
				Entity.Copyright = value;
				OnPropertyChanged("Copyright");
			}
		}

		public DateTime CreationDate
		{
			get
			{
				VerifyNotDeleted();
				if (Entity.CreationDate.HasValue)
				{
					return Entity.CreationDate.Value;
				}
				return default(DateTime);
			}
		}

		public BuiltinRecognizers Recognizers
		{
			get
			{
				VerifyNotDeleted();
				return Entity.Recognizers.Value;
			}
			set
			{
				VerifyNotDeleted();
				Entity.Recognizers = value;
				OnPropertyChanged("Recognizers");
			}
		}

		public FuzzyIndexes FuzzyIndexes
		{
			get
			{
				VerifyNotDeleted();
				return Entity.FuzzyIndexes.Value;
			}
			set
			{
				VerifyNotDeleted();
				Entity.FuzzyIndexes = value;
				OnPropertyChanged("FuzzyIndexes");
			}
		}

		public bool IsNewObject
		{
			get
			{
				if (Entity.Id != null)
				{
					return Entity.Id.Value == null;
				}
				return true;
			}
		}

		public bool IsDeleted => Entity == null;

		private event PropertyChangedEventHandler PropertyChangedPrivate;

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

		protected RemoteTranslationMemory(TranslationMemoryEntity entity)
		{
			Entity = entity;
		}

		protected void OnPropertyChanged(string property)
		{
			this.PropertyChangedPrivate?.Invoke(this, new PropertyChangedEventArgs(property));
		}

		protected void VerifyNotDeleted()
		{
			VerifyNotDeleted("The translation memory has been deleted.");
		}

		private void VerifyNotDeleted(string message)
		{
			if (IsDeleted)
			{
				throw new ObjectDeletedException(message);
			}
		}

		void IEditableObject.BeginEdit()
		{
			if (Entity != null && BackupEntity == null)
			{
				BackupEntity = Entity.Clone();
				BackupEntity.MarkAsClean();
			}
		}

		void IEditableObject.CancelEdit()
		{
			if (Entity != null && BackupEntity != null)
			{
				Entity = BackupEntity;
				BackupEntity = null;
			}
		}

		void IEditableObject.EndEdit()
		{
			BackupEntity = null;
		}
	}
}
