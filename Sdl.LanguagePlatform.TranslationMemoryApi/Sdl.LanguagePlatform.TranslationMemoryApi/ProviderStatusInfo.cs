namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class ProviderStatusInfo
	{
		public bool Available
		{
			get;
			set;
		}

		public string StatusMessage
		{
			get;
			set;
		}

		public ProviderStatusInfo(bool available, string statusMessage)
		{
			Available = available;
			StatusMessage = statusMessage;
		}
	}
}
