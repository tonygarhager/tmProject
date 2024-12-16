using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class CascadeMessage
	{
		public CascadeEntry Entry
		{
			get;
			private set;
		}

		public CascadeMessageCode Code
		{
			get;
			private set;
		}

		public Exception Exception
		{
			get;
			private set;
		}

		public CascadeMessage(CascadeEntry entry, CascadeMessageCode code)
			: this(entry, code, null)
		{
		}

		public CascadeMessage(CascadeEntry entry, Exception exception)
			: this(entry, CascadeMessageCode.TranslationProviderThrewException, exception)
		{
		}

		public CascadeMessage(CascadeEntry entry, CascadeMessageCode code, Exception exception)
		{
			Entry = entry;
			Code = code;
			Exception = exception;
		}
	}
}
