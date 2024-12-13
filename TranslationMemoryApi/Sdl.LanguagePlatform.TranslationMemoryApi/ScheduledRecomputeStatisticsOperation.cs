using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents a scheduled recomputation of the fuzzy index statistics of a server-based translation memory on the Execution Server.
	/// This class can be used to schedule a recomputation of the fuzzy index statistics of a server-based translation memory
	/// and also to monitor the operation's status while it is running.
	/// </summary>
	public class ScheduledRecomputeStatisticsOperation : ScheduledOperation
	{
		/// <summary>
		/// Gets or sets the server-based translation memory for which to recompute the fuzzy index statistics.
		/// </summary>
		public ServerBasedTranslationMemory TranslationMemory
		{
			get;
			set;
		}

		internal ScheduledRecomputeStatisticsOperation(ScheduledOperationEntity entity)
			: base(entity)
		{
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ScheduledRecomputeStatisticsOperation()
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
				base.ScheduledOperationEntity = TranslationMemory.TranslationProviderServer.Service.ScheduleRecomputeStatistics(TranslationMemory.Entity.Id);
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
