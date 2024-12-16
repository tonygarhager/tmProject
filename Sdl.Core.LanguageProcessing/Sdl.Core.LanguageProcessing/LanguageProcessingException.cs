using System;

namespace Sdl.Core.LanguageProcessing
{
	public class LanguageProcessingException : Exception
	{
		public LanguageProcessingException()
		{
		}

		public LanguageProcessingException(string message)
			: base(message)
		{
		}

		public LanguageProcessingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
