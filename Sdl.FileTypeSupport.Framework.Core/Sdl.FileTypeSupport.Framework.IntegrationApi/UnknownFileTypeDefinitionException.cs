using System;
using System.Runtime.Serialization;

namespace Sdl.FileTypeSupport.Framework.IntegrationApi
{
	[Serializable]
	public class UnknownFileTypeDefinitionException : FileTypeSupportException
	{
		public UnknownFileTypeDefinitionException()
		{
		}

		public UnknownFileTypeDefinitionException(string message)
			: base(message)
		{
		}

		public UnknownFileTypeDefinitionException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected UnknownFileTypeDefinitionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
