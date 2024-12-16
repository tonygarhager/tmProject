using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public class NotBilingualFileTypeException : FileTypeSupportException
	{
		public NotBilingualFileTypeException()
		{
		}

		public NotBilingualFileTypeException(string message)
			: base(message)
		{
		}

		public NotBilingualFileTypeException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected NotBilingualFileTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
