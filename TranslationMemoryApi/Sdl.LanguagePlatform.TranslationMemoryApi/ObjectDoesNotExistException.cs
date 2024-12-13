using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Thrown when the requested object does not exist.
	/// </summary>
	[Serializable]
	public class ObjectDoesNotExistException : InvalidOperationException
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ObjectDoesNotExistException()
			: base("Object does not exist.")
		{
		}

		/// <summary>
		/// Constructor with error message.
		/// </summary>
		/// <param name="message">The error message.</param>
		public ObjectDoesNotExistException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Constructor with error message and inner exception.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="inner">Inner exception.</param>
		public ObjectDoesNotExistException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ObjectDoesNotExistException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
