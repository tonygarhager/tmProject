using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Exception thrown when trying to access an object that has been deleted.
	/// </summary>
	[Serializable]
	public class TranslationProviderAuthenticationException : InvalidOperationException
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TranslationProviderAuthenticationException()
			: base("Invalid credentials.")
		{
		}

		/// <summary>
		/// Constructor with message.
		/// </summary>
		/// <param name="message"></param>
		public TranslationProviderAuthenticationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Constructor with message and inner exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public TranslationProviderAuthenticationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected TranslationProviderAuthenticationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
