using System;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	[Serializable]
	public class TranslationProviderAuthenticationException : InvalidOperationException
	{
		public TranslationProviderAuthenticationException()
			: base("Invalid credentials.")
		{
		}

		public TranslationProviderAuthenticationException(string message)
			: base(message)
		{
		}

		public TranslationProviderAuthenticationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected TranslationProviderAuthenticationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
