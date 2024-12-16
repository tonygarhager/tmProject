namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public enum ServerImportStatus
	{
		NoStatus = 0,
		Queued = 1,
		InProgress = 0x10,
		Failed = 0x1000,
		Canceled = 0x400,
		Finished = 0x100
	}
}
