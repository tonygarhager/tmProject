using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// When changing recognizer settings or when changing variables, abbreviations or ordinal followers, 
	/// it might be necessary to re-index the Translation Memory in order to make sure these settings are 
	/// applied to existing Translation Units. This class represents a scheduled re-indexing of a server-based
	/// translation memory on the Execution Server. This class can be used to schedule a re-indexing of a 
	/// server-based translation memory and also to monitor the operation's status while it is running.
	/// </summary>
	public class ScheduledReindexOperation : ScheduledOperation
	{
		/// <summary>
		/// Gets or sets the server-based translation memory to re-index.
		/// </summary>
		public ServerBasedTranslationMemory TranslationMemory
		{
			get;
			set;
		}

		internal ScheduledReindexOperation(ScheduledOperationEntity entity)
			: base(entity)
		{
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ScheduledReindexOperation()
			: this(new ScheduledOperationEntity())
		{
		}

		/// <summary>
		/// Schedules the operation for execution by the execution server.
		/// </summary>
		public override void Queue()
		{
			if (base.Status == ScheduledOperationStatus.NotSet)
			{
				base.ScheduledOperationEntity = TranslationMemory.TranslationProviderServer.Service.ScheduleReindexTranslationMemory(TranslationMemory.Entity.Id);
			}
		}

		/// <summary>
		/// Refreshes the status of this operation with up-to-date information from the server.
		/// </summary>
		public override void Refresh()
		{
			if (base.ScheduledOperationEntity.WorkItemUniqueId.HasValue && !base.IsFinished && (base.IsRunning || base.IsPending))
			{
				base.ScheduledOperationEntity = TranslationMemory.TranslationProviderServer.Service.GetScheduledOperation(base.ScheduledOperationEntity.WorkItemUniqueId.Value);
			}
		}
	}
}
