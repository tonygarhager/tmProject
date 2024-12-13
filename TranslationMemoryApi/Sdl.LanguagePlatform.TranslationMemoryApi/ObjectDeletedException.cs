using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Exception thrown when trying to access an object that has been deleted.
	/// </summary>
	[Serializable]
	public class ObjectDeletedException : InvalidOperationException
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ObjectDeletedException()
			: base("Cannot access a deleted object.")
		{
		}

		/// <summary>
		/// Constructor with message.
		/// </summary>
		/// <param name="message"></param>
		public ObjectDeletedException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Constructor with message and inner exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ObjectDeletedException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ObjectDeletedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
