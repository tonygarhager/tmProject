using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class DependencyFileProperties : IDependencyFileProperties, ICloneable, ISupportsPersistenceId
	{
		private string _id;

		private string _currentFilePath;

		private string _originalFilePath;

		private DateTime _originalLastChangeDate;

		private string _pathRelativeToConverted;

		private string _description;

		private DependencyFileUsage _expectedUsage;

		private DependencyFileLinkOption _preferredLinkage;

		[NonSerialized]
		private IDisposable _disposableObject;

		[NonSerialized]
		private FileJanitor _managdZipFile;

		[NonSerialized]
		private int _persistenceId;

		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public bool FileExists
		{
			get
			{
				if (!string.IsNullOrEmpty(_currentFilePath))
				{
					return File.Exists(_currentFilePath);
				}
				return false;
			}
		}

		public string CurrentFilePath
		{
			get
			{
				return _currentFilePath;
			}
			set
			{
				_currentFilePath = value;
				_managdZipFile = null;
			}
		}

		public FileJanitor ZippedCurrentFile
		{
			get
			{
				if (_managdZipFile == null)
				{
					if (string.IsNullOrEmpty(_currentFilePath))
					{
						return null;
					}
					string text = FileSerializer.ZipExternalFile(_currentFilePath);
					if (string.IsNullOrEmpty(text))
					{
						throw new Exception("Failed to crate ZippedCurrentFile.");
					}
					_managdZipFile = new FileJanitor(text);
				}
				return _managdZipFile;
			}
		}

		public string OriginalFilePath
		{
			get
			{
				return _originalFilePath;
			}
			set
			{
				_originalFilePath = value;
			}
		}

		public DateTime OriginalLastChangeDate
		{
			get
			{
				return _originalLastChangeDate;
			}
			set
			{
				_originalLastChangeDate = value;
			}
		}

		public string PathRelativeToConverted
		{
			get
			{
				return _pathRelativeToConverted;
			}
			set
			{
				_pathRelativeToConverted = value;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public DependencyFileUsage ExpectedUsage
		{
			get
			{
				return _expectedUsage;
			}
			set
			{
				_expectedUsage = value;
			}
		}

		public DependencyFileLinkOption PreferredLinkage
		{
			get
			{
				return _preferredLinkage;
			}
			set
			{
				_preferredLinkage = value;
			}
		}

		public IDisposable DisposableObject
		{
			get
			{
				return _disposableObject;
			}
			set
			{
				_disposableObject = value;
			}
		}

		[XmlIgnore]
		public int PersistenceId
		{
			get
			{
				return _persistenceId;
			}
			set
			{
				_persistenceId = value;
			}
		}

		public DependencyFileProperties()
		{
			_id = Guid.NewGuid().ToString();
		}

		public DependencyFileProperties(string currentFilePath)
			: this()
		{
			_currentFilePath = currentFilePath;
		}

		protected DependencyFileProperties(DependencyFileProperties other)
		{
			_id = other._id;
			_currentFilePath = other._currentFilePath;
			_originalFilePath = other._originalFilePath;
			_originalLastChangeDate = other._originalLastChangeDate;
			_pathRelativeToConverted = other._pathRelativeToConverted;
			_description = other._description;
			_expectedUsage = other._expectedUsage;
			_preferredLinkage = other._preferredLinkage;
			_disposableObject = other._disposableObject;
			_managdZipFile = other._managdZipFile;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			DependencyFileProperties dependencyFileProperties = (DependencyFileProperties)obj;
			if (_id != dependencyFileProperties._id)
			{
				return false;
			}
			if (_currentFilePath != dependencyFileProperties._currentFilePath)
			{
				return false;
			}
			if (_description != dependencyFileProperties._description)
			{
				return false;
			}
			if (_disposableObject != dependencyFileProperties._disposableObject)
			{
				return false;
			}
			if (_expectedUsage != dependencyFileProperties._expectedUsage)
			{
				return false;
			}
			if (_originalFilePath != dependencyFileProperties._originalFilePath)
			{
				return false;
			}
			if (_originalLastChangeDate != dependencyFileProperties._originalLastChangeDate)
			{
				return false;
			}
			if (_pathRelativeToConverted != dependencyFileProperties._pathRelativeToConverted)
			{
				return false;
			}
			if (_preferredLinkage != dependencyFileProperties._preferredLinkage)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return ((_id != null) ? _id.GetHashCode() : 0) ^ ((_currentFilePath != null) ? _currentFilePath.GetHashCode() : 0) ^ ((_originalFilePath != null) ? _originalFilePath.GetHashCode() : 0) ^ _originalLastChangeDate.GetHashCode() ^ ((_pathRelativeToConverted != null) ? _pathRelativeToConverted.GetHashCode() : 0) ^ ((_description != null) ? _description.GetHashCode() : 0) ^ _expectedUsage.GetHashCode() ^ _preferredLinkage.GetHashCode() ^ ((_disposableObject != null) ? _disposableObject.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return _currentFilePath;
		}

		public object Clone()
		{
			return new DependencyFileProperties(this);
		}
	}
}
