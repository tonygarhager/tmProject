using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryTools
{
	[Serializable]
	public class InvalidSegmentContentException : Exception
	{
		public InvalidSegmentContentException()
		{
		}

		public InvalidSegmentContentException(string message)
			: base(message)
		{
		}

		public InvalidSegmentContentException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvalidSegmentContentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
