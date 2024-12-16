using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class FileProperties : IFileProperties, ICloneable, ISupportsPersistenceId
	{
		private IPersistentFileConversionProperties _FileConversionProperties = new PersistentFileConversionProperties();

		[NonSerialized]
		private ICommentProperties _Comments;

		[NonSerialized]
		private int _persistenceId;

		public IPersistentFileConversionProperties FileConversionProperties
		{
			get
			{
				return _FileConversionProperties;
			}
			set
			{
				_FileConversionProperties = value;
			}
		}

		public ICommentProperties Comments
		{
			get
			{
				return _Comments;
			}
			set
			{
				_Comments = value;
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

		public FileProperties()
		{
		}

		protected FileProperties(FileProperties other)
		{
			_FileConversionProperties = (IPersistentFileConversionProperties)other._FileConversionProperties.Clone();
			if (other._Comments != null)
			{
				_Comments = (ICommentProperties)other._Comments.Clone();
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			FileProperties fileProperties = (FileProperties)obj;
			if (!_FileConversionProperties.Equals(fileProperties._FileConversionProperties))
			{
				return false;
			}
			if (_Comments == null != (fileProperties._Comments == null))
			{
				return false;
			}
			if (_Comments != null && _Comments.Xml != fileProperties._Comments.Xml)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _FileConversionProperties.GetHashCode() ^ ((_Comments != null) ? _Comments.Xml.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return _FileConversionProperties.ToString();
		}

		public bool IsStartOfFileSection()
		{
			if (FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM") == null || (FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM") != null && FileConversionProperties.GetMetaData("SDL_SUBCONTENT_FILE_START") != null))
			{
				return true;
			}
			return false;
		}

		public bool IsEndOfFileSection()
		{
			if (FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM") == null || (FileConversionProperties.GetMetaData("SDL_SUBCONTENT_STREAM") != null && FileConversionProperties.GetMetaData("SDL_SUBCONTENT_FILE_END") != null))
			{
				return true;
			}
			return false;
		}

		public object Clone()
		{
			return new FileProperties(this);
		}
	}
}
