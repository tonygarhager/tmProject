using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework
{
	[Serializable]
	public class FileTypeSupportException : Exception
	{
		public FileTypeSupportException()
		{
		}

		public FileTypeSupportException(string message)
			: base(message)
		{
		}

		public FileTypeSupportException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected FileTypeSupportException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
