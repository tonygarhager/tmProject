namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the possible statuses of a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ScheduledOperation" />.
	/// </summary>
	public enum ScheduledOperationStatus
	{
		/// <summary>
		/// Status not set (default).
		/// </summary>
		NotSet = 0,
		/// <summary>
		/// The task has been added to the work item queue
		/// </summary>
		Queued = 1,
		/// <summary>
		/// The work item has been flagged as needing recovery
		/// </summary>
		Recovery = 2,
		/// <summary>
		/// The work items has been flagged as needing to be cancelled.
		/// </summary>
		Cancel = 4,
		/// <summary>
		/// The work item has been flagged as needing to be aborted
		/// </summary>
		Abort = 8,
		/// <summary>
		/// The work item has been allocated to an execution service.
		/// </summary>
		Allocated = 0x10,
		/// <summary>
		/// The work item is in the process of being recovered.
		/// </summary>
		Recovering = 0x20,
		/// <summary>
		/// The work item is in the process of being cancelled.
		/// </summary>
		Cancelling = 0x40,
		/// <summary>
		/// The work item is in the process of being aborted.
		/// </summary>
		Aborting = 0x80,
		/// <summary>
		/// The work item has been completed 
		/// </summary>
		Completed = 0x100,
		/// <summary>
		/// The work item has been recovered
		/// </summary>
		Recovered = 0x200,
		/// <summary>
		/// The work item has been cancelled
		/// </summary>
		Cancelled = 0x400,
		/// <summary>
		/// The work item has been aborted
		/// </summary>
		Aborted = 0x800,
		/// <summary>
		/// Error
		/// </summary>
		Error = 0x1000
	}
}
