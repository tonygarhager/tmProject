using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Abstract base class for scheduled operations, which are long running tasks that are executed
	/// on the Execution Server. 
	/// </summary>
	public abstract class ScheduledOperation
	{
		internal ScheduledOperationEntity ScheduledOperationEntity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the current status of the operation.
		/// </summary>
		public ScheduledOperationStatus Status => (ScheduledOperationStatus)ScheduledOperationEntity.Status;

		/// <summary>
		/// Gets the error message in case the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ScheduledOperation.Status" /> is <see cref="F:Sdl.LanguagePlatform.TranslationMemoryApi.ScheduledOperationStatus.Error" />.
		/// </summary>
		public string ErrorMessage => ScheduledOperationEntity.ErrorMessage;

		/// <summary>
		/// Gets the current percent complete.
		/// </summary>
		public int PercentComplete => ScheduledOperationEntity.PercentComplete;

		/// <summary>
		/// Gets when the operation was queued.
		/// </summary>
		public DateTime? QueuedOn
		{
			get
			{
				if (ScheduledOperationEntity.QueuedOn.HasValue)
				{
					return ScheduledOperationEntity.QueuedOn.Value.ToLocalTime();
				}
				return null;
			}
		}

		/// <summary>
		/// Gets when the operation was started by the Execution Server.
		/// </summary>
		public DateTime? StartedOn
		{
			get
			{
				if (ScheduledOperationEntity.StartedOn.HasValue)
				{
					return ScheduledOperationEntity.StartedOn.Value.ToLocalTime();
				}
				return null;
			}
		}

		/// <summary>
		/// Gets when the operation was completed.
		/// </summary>
		public DateTime? CompletedOn
		{
			get
			{
				if (ScheduledOperationEntity.CompletedOn.HasValue)
				{
					return ScheduledOperationEntity.CompletedOn.Value.ToLocalTime();
				}
				return null;
			}
		}

		/// <summary>
		/// Gets when the operation was cancelled if the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ScheduledOperation.Status" /> is <see cref="F:Sdl.LanguagePlatform.TranslationMemoryApi.ScheduledOperationStatus.Cancelled" />.
		/// </summary>
		public DateTime? CancelledOn
		{
			get
			{
				if (ScheduledOperationEntity.CancelledOn.HasValue)
				{
					return ScheduledOperationEntity.CancelledOn.Value.ToLocalTime();
				}
				return null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this operation has finished.
		/// </summary>
		/// <remarks>A finished operation has one of the following statues; Completed, 
		/// Aborted, Cancelled, Recovered or Error.</remarks>
		public bool IsFinished
		{
			get
			{
				if (Status != ScheduledOperationStatus.Completed && Status != ScheduledOperationStatus.Aborted && Status != ScheduledOperationStatus.Cancelled && Status != ScheduledOperationStatus.Recovered)
				{
					return Status == ScheduledOperationStatus.Error;
				}
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this operation is running.
		/// </summary>
		/// <remarks>A running operation has one of the following statues; Aborting, 
		/// Allocated or Cancelling.</remarks>
		public bool IsRunning
		{
			get
			{
				if (Status != ScheduledOperationStatus.Aborting && Status != ScheduledOperationStatus.Allocated)
				{
					return Status == ScheduledOperationStatus.Cancelling;
				}
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this operation is pending.
		/// </summary>
		/// <remarks>A pending operation has one of the following statues; NotSet, 
		/// Queued, Cancel, Abort or Recovery.</remarks>
		public bool IsPending
		{
			get
			{
				if (Status != 0 && Status != ScheduledOperationStatus.Queued && Status != ScheduledOperationStatus.Cancel && Status != ScheduledOperationStatus.Abort)
				{
					return Status == ScheduledOperationStatus.Recovery;
				}
				return true;
			}
		}

		/// <summary>
		/// Gets the unique ID of the Execution Server work item that represents this operation.
		/// </summary>
		public Guid? WorkItemId => ScheduledOperationEntity.WorkItemUniqueId;

		internal ScheduledOperation()
		{
		}

		internal ScheduledOperation(ScheduledOperationEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException();
			}
			ScheduledOperationEntity = entity;
		}

		/// <summary>
		/// Queues the operation for execution on the Execution Server.
		/// </summary>
		public abstract void Queue();

		/// <summary>
		/// Refreshes the status of the operation.
		/// </summary>
		public abstract void Refresh();
	}
}
