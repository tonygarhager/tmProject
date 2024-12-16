using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public class OneOrMoreFilesNotSupportedException : FileTypeSupportException
	{
		private List<FileNotSupportedException> _notSupportedFiles;

		public List<FileNotSupportedException> NotSupportedFiles
		{
			get
			{
				return _notSupportedFiles;
			}
			set
			{
				_notSupportedFiles = value;
			}
		}

		public OneOrMoreFilesNotSupportedException()
		{
		}

		public OneOrMoreFilesNotSupportedException(string message, List<FileNotSupportedException> notSupportedFiles)
			: base(message)
		{
			_notSupportedFiles = notSupportedFiles;
		}

		protected OneOrMoreFilesNotSupportedException(SerializationInfo info, StreamingContext context)
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
			OneOrMoreFilesNotSupportedException ex = (OneOrMoreFilesNotSupportedException)obj;
			if (_notSupportedFiles == null != (ex._notSupportedFiles == null))
			{
				return false;
			}
			if (_notSupportedFiles != null && !_notSupportedFiles.Equals(ex._notSupportedFiles))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ((_notSupportedFiles != null) ? _notSupportedFiles.GetHashCode() : 0);
		}
	}
}
