using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// CascadeMessage class represents a message from a cascade.
	/// </summary>
	public class CascadeMessage
	{
		/// <summary>
		/// Entry property represents the entry.
		/// </summary>
		public CascadeEntry Entry
		{
			get;
			private set;
		}

		/// <summary>
		/// Code property represents the code.
		/// </summary>
		public CascadeMessageCode Code
		{
			get;
			private set;
		}

		/// <summary>
		/// Exception property represents the exception.
		/// </summary>
		public Exception Exception
		{
			get;
			private set;
		}

		/// <summary>
		/// Constructor that takes the given entry and code.
		/// </summary>
		/// <param name="entry">entry</param>
		/// <param name="code">code</param>
		public CascadeMessage(CascadeEntry entry, CascadeMessageCode code)
			: this(entry, code, null)
		{
		}

		/// <summary>
		/// Constructor that takes the given entry and exception.
		/// </summary>
		/// <param name="entry">entry</param>
		/// <param name="exception">exception</param>
		public CascadeMessage(CascadeEntry entry, Exception exception)
			: this(entry, CascadeMessageCode.TranslationProviderThrewException, exception)
		{
		}

		/// <summary>
		/// Constructor that takes the given entry, code, and exception.
		/// </summary>
		/// <param name="entry">entry</param>
		/// <param name="code">code</param>
		/// <param name="exception">exception</param>
		public CascadeMessage(CascadeEntry entry, CascadeMessageCode code, Exception exception)
		{
			Entry = entry;
			Code = code;
			Exception = exception;
		}
	}
}
