using System;
using System.Runtime.Serialization;

namespace Sdl.Core.Processing.Alignment.Core
{
	internal class BilingualDictionaryParseException : Exception
	{
		public BilingualDictionaryParseException()
		{
		}

		public BilingualDictionaryParseException(string message)
			: base(message)
		{
		}

		public BilingualDictionaryParseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected BilingualDictionaryParseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
