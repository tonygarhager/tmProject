using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Globalization
{
	[Serializable]
	public class UnsupportedCodepageException : Exception
	{
		public UnsupportedCodepageException()
		{
		}

		public UnsupportedCodepageException(string message)
			: base(message)
		{
		}

		public UnsupportedCodepageException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected UnsupportedCodepageException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
