using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// CascadeException class represents a cascade exception.
	/// </summary>
	public class CascadeException : Exception
	{
		/// <summary>
		/// CascadeMessage represents the underlying cascade message that caused the exception.
		/// </summary>
		public CascadeMessage CascadeMessage
		{
			get;
			private set;
		}

		/// <summary>
		/// Constructor that takes the given cascade message.
		/// </summary>
		/// <param name="cascadeMessage">cascade message</param>
		public CascadeException(CascadeMessage cascadeMessage)
			: base(cascadeMessage.Code.ToString(), cascadeMessage.Exception)
		{
			CascadeMessage = cascadeMessage;
		}
	}
}
