using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Native
{
	[Serializable]
	public class PersistentFileConversionProperties : AbstractMetaDataContainer, IPersistentFileConversionProperties, IMetaDataContainer, ICloneable, ISupportsPersistenceId
	{
		private SniffInfo _FileSnifferInfo;

		private IList<IDependencyFileProperties> _DependencyFiles = new List<IDependencyFileProperties>();

		[NonSerialized]
		private int _persistenceId;

		public FileTypeDefinitionId FileTypeDefinitionId
		{
			get
			{
				string metaData = GetMetaData("SDL:FileTypeDefinitionId");
				if (metaData != null)
				{
					return new FileTypeDefinitionId(metaData);
				}
				return default(FileTypeDefinitionId);
			}
			set
			{
				SetMetaDataProperty("SDL:FileTypeDefinitionId", value.Id);
			}
		}

		public string CreationTool
		{
			get
			{
				return GetMetaData("SDL:CreationTool");
			}
			set
			{
				SetMetaDataProperty("SDL:CreationTool", value);
			}
		}

		public DateTime CreationDate
		{
			get
			{
				return DateTime.Parse(GetMetaData("SDL:CreationDate"), CultureInfo.InvariantCulture);
			}
			set
			{
				SetMetaData("SDL:CreationDate", value.ToString(CultureInfo.InvariantCulture));
			}
		}

		public string CreationToolVersion
		{
			get
			{
				return GetMetaData("SDL:CreationToolVersion");
			}
			set
			{
				SetMetaDataProperty("SDL:CreationToolVersion", value);
			}
		}

		public SniffInfo FileSnifferInfo
		{
			get
			{
				return _FileSnifferInfo;
			}
			set
			{
				_FileSnifferInfo = value;
			}
		}

		public string this[string key]
		{
			get
			{
				return GetMetaData(key);
			}
			set
			{
				SetMetaData(key, value);
			}
		}

		public string OriginalFilePath
		{
			get
			{
				return GetMetaData("SDL:OriginalFilePath");
			}
			set
			{
				SetMetaDataProperty("SDL:OriginalFilePath", value);
			}
		}

		public string InputFilePath
		{
			get
			{
				string metaData = GetMetaData("SDL:InputFilePath");
				if (string.IsNullOrEmpty(metaData))
				{
					metaData = GetMetaData("SDL:OriginalFilePath");
				}
				return metaData;
			}
			set
			{
				SetMetaDataProperty("SDL:InputFilePath", value);
			}
		}

		public IList<IDependencyFileProperties> DependencyFiles => _DependencyFiles;

		public Codepage OriginalEncoding
		{
			get
			{
				string metaData = GetMetaData("SDL:OriginalEncoding");
				return new Codepage(metaData);
			}
			set
			{
				if (value == null)
				{
					RemoveMetaData("SDL:OriginalEncoding");
				}
				SetMetaDataProperty("SDL:OriginalEncoding", value.Name);
			}
		}

		public Codepage PreferredTargetEncoding
		{
			get
			{
				string metaData = GetMetaData("SDL:PreferredTargetEncoding");
				return new Codepage(metaData);
			}
			set
			{
				if (value == null)
				{
					RemoveMetaData("SDL:PreferredTargetEncoding");
				}
				SetMetaDataProperty("SDL:PreferredTargetEncoding", value.Name);
			}
		}

		public Language SourceLanguage
		{
			get
			{
				string metaData = GetMetaData("SDL:SourceLanguage");
				return new Language(metaData);
			}
			set
			{
				if (value == null)
				{
					RemoveMetaData("SDL:SourceLanguage");
				}
				SetMetaDataProperty("SDL:SourceLanguage", value.IsoAbbreviation);
			}
		}

		public Language TargetLanguage
		{
			get
			{
				string metaData = GetMetaData("SDL:TargetLanguage");
				return new Language(metaData);
			}
			set
			{
				if (value == null)
				{
					RemoveMetaData("SDL:TargetLanguage");
				}
				SetMetaDataProperty("SDL:TargetLanguage", value.IsoAbbreviation);
			}
		}

		public FileId FileId
		{
			get
			{
				return new FileId(GetMetaData("SDL:FileId"));
			}
			set
			{
				SetMetaDataProperty("SDL:FileId", value.Id);
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

		public PersistentFileConversionProperties()
		{
			FileId = new FileId(Guid.NewGuid().ToString());
			CreationDate = DateTime.Now;
		}

		protected PersistentFileConversionProperties(PersistentFileConversionProperties other)
			: base(other)
		{
			if (other._FileSnifferInfo != null)
			{
				_FileSnifferInfo = (SniffInfo)other._FileSnifferInfo.Clone();
			}
			if (other._DependencyFiles.Count != 0)
			{
				foreach (IDependencyFileProperties dependencyFile in other._DependencyFiles)
				{
					_DependencyFiles.Add((IDependencyFileProperties)dependencyFile.Clone());
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			PersistentFileConversionProperties persistentFileConversionProperties = (PersistentFileConversionProperties)obj;
			if (!base.Equals((object)persistentFileConversionProperties))
			{
				return false;
			}
			if (_FileSnifferInfo == null != (persistentFileConversionProperties._FileSnifferInfo == null))
			{
				return false;
			}
			if (_FileSnifferInfo != null && !_FileSnifferInfo.Equals(persistentFileConversionProperties._FileSnifferInfo))
			{
				return false;
			}
			if (persistentFileConversionProperties._DependencyFiles.Count != _DependencyFiles.Count)
			{
				return false;
			}
			foreach (IDependencyFileProperties dependencyFile in _DependencyFiles)
			{
				if (!persistentFileConversionProperties._DependencyFiles.Contains(dependencyFile))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = _DependencyFiles.Count;
			foreach (IDependencyFileProperties dependencyFile in _DependencyFiles)
			{
				if (dependencyFile != null)
				{
					num ^= dependencyFile.GetHashCode();
				}
			}
			return base.GetHashCode() ^ ((_FileSnifferInfo != null) ? _FileSnifferInfo.GetHashCode() : 0) ^ num;
		}

		public override string ToString()
		{
			string text = FileId.ToString();
			if (!string.IsNullOrEmpty(OriginalFilePath))
			{
				text = text + ": " + OriginalFilePath;
			}
			else if (!string.IsNullOrEmpty(InputFilePath))
			{
				text = text + ": " + InputFilePath;
			}
			return text;
		}

		private void SetMetaDataProperty(string key, string value)
		{
			if (value == null)
			{
				RemoveMetaData(key);
			}
			else
			{
				SetMetaData(key, value);
			}
		}

		public object Clone()
		{
			return new PersistentFileConversionProperties(this);
		}
	}
}
