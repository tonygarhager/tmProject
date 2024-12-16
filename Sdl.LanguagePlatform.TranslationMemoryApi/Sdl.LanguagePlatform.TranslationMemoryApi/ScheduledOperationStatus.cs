namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public enum ScheduledOperationStatus
	{
		NotSet = 0,
		Queued = 1,
		Recovery = 2,
		Cancel = 4,
		Abort = 8,
		Allocated = 0x10,
		Recovering = 0x20,
		Cancelling = 0x40,
		Aborting = 0x80,
		Completed = 0x100,
		Recovered = 0x200,
		Cancelled = 0x400,
		Aborted = 0x800,
		Error = 0x1000
	}
}
