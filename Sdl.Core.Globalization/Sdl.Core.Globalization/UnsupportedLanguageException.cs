using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Globalization
{
	[Serializable]
	public class UnsupportedLanguageException : Exception
	{
		public UnsupportedLanguageException()
		{
		}

		public UnsupportedLanguageException(string message)
			: base(message)
		{
		}

		public UnsupportedLanguageException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected UnsupportedLanguageException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
