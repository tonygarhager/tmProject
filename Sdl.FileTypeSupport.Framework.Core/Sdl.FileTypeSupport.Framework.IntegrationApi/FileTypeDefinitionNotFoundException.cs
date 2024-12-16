using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public class FileTypeDefinitionNotFoundException : FileTypeSupportException
	{
		private FileTypeDefinitionId _filterDefinitionId;

		public FileTypeDefinitionId FileTypeDefinitionId
		{
			get
			{
				return _filterDefinitionId;
			}
			set
			{
				_filterDefinitionId = value;
			}
		}

		public FileTypeDefinitionNotFoundException()
		{
		}

		public FileTypeDefinitionNotFoundException(string message, FileTypeDefinitionId id)
			: base(message)
		{
			_filterDefinitionId = id;
		}

		public FileTypeDefinitionNotFoundException(string message, FileTypeDefinitionId id, Exception inner)
			: base(message, inner)
		{
			_filterDefinitionId = id;
		}

		protected FileTypeDefinitionNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			if (!base.Equals(obj))
			{
				return false;
			}
			FileTypeDefinitionNotFoundException ex = (FileTypeDefinitionNotFoundException)obj;
			if (ex._filterDefinitionId != _filterDefinitionId)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ _filterDefinitionId.GetHashCode();
		}
	}
}
