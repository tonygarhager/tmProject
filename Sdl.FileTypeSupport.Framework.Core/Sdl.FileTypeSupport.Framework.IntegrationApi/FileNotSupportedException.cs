using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public class FileNotSupportedException : FileTypeSupportException
	{
		private string _filePath;

		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				_filePath = value;
			}
		}

		public FileNotSupportedException()
		{
		}

		public FileNotSupportedException(string message, string filePath)
			: base(message)
		{
			_filePath = filePath;
		}

		public FileNotSupportedException(string message, string filePath, Exception inner)
			: base(message, inner)
		{
			_filePath = filePath;
		}

		protected FileNotSupportedException(SerializationInfo info, StreamingContext context)
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
			FileNotSupportedException ex = (FileNotSupportedException)obj;
			if (ex._filePath != _filePath)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_filePath != null) ? _filePath.GetHashCode() : 0);
		}
	}
}
