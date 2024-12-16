using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class CascadeException : Exception
	{
		public CascadeMessage CascadeMessage
		{
			get;
			private set;
		}

		public CascadeException(CascadeMessage cascadeMessage)
			: base(cascadeMessage.Code.ToString(), cascadeMessage.Exception)
		{
			CascadeMessage = cascadeMessage;
		}
	}
}
