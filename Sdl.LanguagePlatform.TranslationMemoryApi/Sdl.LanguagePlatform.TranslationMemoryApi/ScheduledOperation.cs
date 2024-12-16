using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public abstract class ScheduledOperation
	{
		internal ScheduledOperationEntity ScheduledOperationEntity
		{
			get;
			set;
		}

		public ScheduledOperationStatus Status => (ScheduledOperationStatus)ScheduledOperationEntity.Status;

		public string ErrorMessage => ScheduledOperationEntity.ErrorMessage;

		public int PercentComplete => ScheduledOperationEntity.PercentComplete;

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

		public abstract void Queue();

		public abstract void Refresh();
	}
}
