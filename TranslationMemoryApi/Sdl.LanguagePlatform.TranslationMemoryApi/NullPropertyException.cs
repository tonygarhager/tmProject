using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Exception thrown when trying to set required properties to null.
	/// </summary>
	[Serializable]
	public class NullPropertyException : ArgumentException
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public NullPropertyException()
			: base("Value is null.")
		{
		}

		/// <summary>
		/// Constructor with message.
		/// </summary>
		/// <param name="message"></param>
		public NullPropertyException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Constructor with message and inner exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public NullPropertyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected NullPropertyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
