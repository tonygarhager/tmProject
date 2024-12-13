using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Thrown when an object should be saved before performing a specific operation.
	/// </summary>
	[Serializable]
	public class ObjectNotSavedException : InvalidOperationException
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ObjectNotSavedException()
			: base("The object has to be saved before performing this operation.")
		{
		}

		/// <summary>
		/// Constructor with error message.
		/// </summary>
		/// <param name="message">The error message.</param>
		public ObjectNotSavedException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Constructor with error message and inner exception.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="inner">Inner exception.</param>
		public ObjectNotSavedException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ObjectNotSavedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
