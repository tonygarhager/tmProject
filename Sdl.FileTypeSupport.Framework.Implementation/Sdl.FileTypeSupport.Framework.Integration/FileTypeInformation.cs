using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.FileTypeSupport.Framework.Integration
{
	[Serializable]
	public class FileTypeInformation : AbstractFileTypeDefinitionComponent, IFileTypeInformation, IFileTypeDefinitionAware, IMetaDataContainer
	{
		private FileTypeDefinitionId _fileTypeDefinitionId;

		private LocalizableString _fileTypeName;

		private LocalizableString _fileTypeDocumentName;

		private LocalizableString _fileTypeDocumentsName;

		private Version _fileTypeFrameworkVersion;

		private string _fileDialogWildcardExpression;

		private string _defaultFileExtension;

		private LocalizableString _description;

		private IconDescriptor _icon;

		private bool _enabled;

		private bool _hidden;

		private bool _removed;

		private bool _isBilingualDocumentFileType;

		private readonly MetaDataContainer _metaData;

		public LocalizableString FileTypeNameSingular
		{
			get
			{
				return FileTypeDocumentName;
			}
			set
			{
				FileTypeDocumentName = value;
			}
		}

		public bool IsBilingualDocumentFileType
		{
			get
			{
				return _isBilingualDocumentFileType;
			}
			set
			{
				_isBilingualDocumentFileType = value;
			}
		}

		public FileTypeDefinitionId FileTypeDefinitionId
		{
			get
			{
				return _fileTypeDefinitionId;
			}
			set
			{
				_fileTypeDefinitionId = value;
			}
		}

		public LocalizableString FileTypeName
		{
			get
			{
				return _fileTypeName;
			}
			set
			{
				_fileTypeName = value;
			}
		}

		public LocalizableString FileTypeDocumentName
		{
			get
			{
				if (_fileTypeDocumentName == null || string.IsNullOrEmpty(_fileTypeDocumentName.Content))
				{
					return FileTypeName;
				}
				return _fileTypeDocumentName;
			}
			set
			{
				_fileTypeDocumentName = value;
			}
		}

		public LocalizableString FileTypeDocumentsName
		{
			get
			{
				if (_fileTypeDocumentsName == null || string.IsNullOrEmpty(_fileTypeDocumentsName.Content))
				{
					return FileTypeName;
				}
				return _fileTypeDocumentsName;
			}
			set
			{
				_fileTypeDocumentsName = value;
			}
		}

		public string FileDialogWildcardExpression
		{
			get
			{
				return _fileDialogWildcardExpression;
			}
			set
			{
				_fileDialogWildcardExpression = value;
			}
		}

		public Regex Expression => BuildExpressionFromWildcard();

		public string DefaultFileExtension
		{
			get
			{
				return _defaultFileExtension;
			}
			set
			{
				_defaultFileExtension = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		public bool Hidden
		{
			get
			{
				return _hidden;
			}
			set
			{
				_hidden = value;
			}
		}

		public bool Removed
		{
			get
			{
				return _removed;
			}
			set
			{
				_removed = value;
			}
		}

		public Version FileTypeFrameworkVersion
		{
			get
			{
				if (_fileTypeFrameworkVersion == null)
				{
					return new Version(1, 0, 0, 0);
				}
				return _fileTypeFrameworkVersion;
			}
			set
			{
				_fileTypeFrameworkVersion = value;
			}
		}

		public LocalizableString Description
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

		public IconDescriptor Icon
		{
			get
			{
				return _icon;
			}
			set
			{
				_icon = value;
			}
		}

		public string[] SilverlightSettingsPageIds
		{
			get;
			set;
		}

		public string[] WinFormSettingsPageIds
		{
			get;
			set;
		}

		public bool HasMetaData => _metaData.HasMetaData;

		public IEnumerable<KeyValuePair<string, string>> MetaData => _metaData.MetaData;

		public int MetaDataCount => _metaData.MetaDataCount;

		public FileTypeInformation()
		{
			_icon = new IconDescriptor();
			_enabled = true;
			_metaData = new MetaDataContainer();
		}

		public override string ToString()
		{
			return _fileTypeDefinitionId.Id;
		}

		private Regex BuildExpressionFromWildcard()
		{
			if (string.IsNullOrEmpty(_fileDialogWildcardExpression))
			{
				return null;
			}
			string[] array = _fileDialogWildcardExpression.Split(';');
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				stringBuilder.Append("(^");
				for (int j = 0; j < text2.Length; j++)
				{
					switch (text2[j])
					{
					case '$':
					case '(':
					case ')':
					case '+':
					case '.':
					case '[':
					case '\\':
					case ']':
					case '^':
					case '{':
					case '|':
					case '}':
						stringBuilder.Append("\\");
						stringBuilder.Append(text2[j]);
						continue;
					case '*':
						stringBuilder.Append("[\\d\\D]*");
						continue;
					case '?':
						stringBuilder.Append("[\\d\\D]");
						continue;
					}
					if (char.IsLower(text2[j]))
					{
						stringBuilder.AppendFormat("[{0}{1}]", text2[j], char.ToUpper(text2[j]));
					}
					else if (char.IsUpper(text[j]))
					{
						stringBuilder.AppendFormat("[{0}{1}]", text2[j], char.ToLower(text2[j]));
					}
					else
					{
						stringBuilder.Append(text2[j]);
					}
				}
				stringBuilder.Append("$)|");
			}
			if (array.Length != 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return new Regex(stringBuilder.ToString());
		}

		public void ClearMetaData()
		{
			_metaData.ClearMetaData();
		}

		public string GetMetaData(string key)
		{
			return _metaData.GetMetaData(key);
		}

		public bool MetaDataContainsKey(string key)
		{
			return _metaData.MetaDataContainsKey(key);
		}

		public bool RemoveMetaData(string key)
		{
			return _metaData.RemoveMetaData(key);
		}

		public void SetMetaData(string key, string value)
		{
			_metaData.SetMetaData(key, value);
		}
	}
}
