using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Serializable]
	public class ObjectNotSavedException : InvalidOperationException
	{
		public ObjectNotSavedException()
			: base("The object has to be saved before performing this operation.")
		{
		}

		public ObjectNotSavedException(string message)
			: base(message)
		{
		}

		public ObjectNotSavedException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ObjectNotSavedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
