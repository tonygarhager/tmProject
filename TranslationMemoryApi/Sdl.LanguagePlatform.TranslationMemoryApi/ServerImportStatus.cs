namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	///
	/// </summary>
	public enum ServerImportStatus
	{
		/// <summary>
		///
		/// </summary>
		NoStatus = 0,
		/// <summary>
		///
		/// </summary>
		Queued = 1,
		/// <summary>
		///
		/// </summary>
		InProgress = 0x10,
		/// <summary>
		///
		/// </summary>
		Failed = 0x1000,
		/// <summary>
		///
		/// </summary>
		Canceled = 0x400,
		/// <summary>
		///
		/// </summary>
		Finished = 0x100
	}
}
