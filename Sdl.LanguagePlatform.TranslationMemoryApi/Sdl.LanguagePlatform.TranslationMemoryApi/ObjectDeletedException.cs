using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Serializable]
	public class ObjectDeletedException : InvalidOperationException
	{
		public ObjectDeletedException()
			: base("Cannot access a deleted object.")
		{
		}

		public ObjectDeletedException(string message)
			: base(message)
		{
		}

		public ObjectDeletedException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ObjectDeletedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
