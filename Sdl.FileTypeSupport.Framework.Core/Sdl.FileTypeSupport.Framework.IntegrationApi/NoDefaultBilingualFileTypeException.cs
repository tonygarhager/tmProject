using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public class NoDefaultBilingualFileTypeException : FileTypeSupportException
	{
		public NoDefaultBilingualFileTypeException()
		{
		}

		public NoDefaultBilingualFileTypeException(string message)
			: base(message)
		{
		}

		public NoDefaultBilingualFileTypeException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected NoDefaultBilingualFileTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
