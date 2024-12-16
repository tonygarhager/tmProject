using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Serializable]
	public class NullPropertyException : ArgumentException
	{
		public NullPropertyException()
			: base("Value is null.")
		{
		}

		public NullPropertyException(string message)
			: base(message)
		{
		}

		public NullPropertyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected NullPropertyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
