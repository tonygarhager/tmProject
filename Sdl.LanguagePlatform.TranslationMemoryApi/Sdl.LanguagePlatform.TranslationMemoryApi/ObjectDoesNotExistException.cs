using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Serializable]
	public class ObjectDoesNotExistException : InvalidOperationException
	{
		public ObjectDoesNotExistException()
			: base("Object does not exist.")
		{
		}

		public ObjectDoesNotExistException(string message)
			: base(message)
		{
		}

		public ObjectDoesNotExistException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ObjectDoesNotExistException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
