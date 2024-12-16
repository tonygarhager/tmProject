using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.FileTypeSupport.Framework.Bilingual
{
	[Serializable]
	public class SerializableMarkupDataContainer : MarkupDataContainer, ISerializableMarkupDataContainer, IAbstractMarkupDataContainer, IList<IAbstractMarkupData>, ICollection<IAbstractMarkupData>, IEnumerable<IAbstractMarkupData>, IEnumerable, ISupportsUniqueId, ISupportsPersistenceId
	{
		private DocumentProperties _docProperties;

		private FileProperties _fileProperties;

		[NonSerialized]
		private int _persistenceId;

		public IDocumentProperties DocProperties
		{
			get
			{
				return _docProperties;
			}
			set
			{
				DocumentProperties documentProperties = value as DocumentProperties;
				if (documentProperties == null)
				{
					throw new ArgumentException(StringResources.SerializableMarkupDataContainer_InvalidDocumentPropertiesError);
				}
				_docProperties = documentProperties;
			}
		}

		public IFileProperties FileProperties
		{
			get
			{
				return _fileProperties;
			}
			set
			{
				FileProperties fileProperties = value as FileProperties;
				if (fileProperties == null)
				{
					throw new ArgumentException(StringResources.SerializableMarkupDataContainer_InvalidFilePropertiesError);
				}
				_fileProperties = fileProperties;
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

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			SerializableMarkupDataContainer serializableMarkupDataContainer = (SerializableMarkupDataContainer)obj;
			if (!base.Equals((object)serializableMarkupDataContainer))
			{
				return false;
			}
			if (_docProperties == null != (serializableMarkupDataContainer._docProperties == null))
			{
				return false;
			}
			if (_docProperties != null && !_docProperties.Equals(serializableMarkupDataContainer._docProperties))
			{
				return false;
			}
			if (_fileProperties == null != (serializableMarkupDataContainer._fileProperties == null))
			{
				return false;
			}
			if (_fileProperties != null && !_fileProperties.Equals(serializableMarkupDataContainer._fileProperties))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_docProperties != null) ? _docProperties.GetHashCode() : 0) ^ ((_fileProperties != null) ? _fileProperties.GetHashCode() : 0);
		}
	}
}
