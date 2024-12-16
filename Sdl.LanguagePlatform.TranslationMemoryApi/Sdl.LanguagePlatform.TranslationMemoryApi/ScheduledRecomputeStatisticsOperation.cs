using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class ScheduledRecomputeStatisticsOperation : ScheduledOperation
	{
		public ServerBasedTranslationMemory TranslationMemory
		{
			get;
			set;
		}

		internal ScheduledRecomputeStatisticsOperation(ScheduledOperationEntity entity)
			: base(entity)
		{
		}

		public ScheduledRecomputeStatisticsOperation()
			: this(new ScheduledOperationEntity())
		{
		}

		public override void Queue()
		{
			if (base.Status == ScheduledOperationStatus.NotSet)
			{
				//base.ScheduledOperationEntity = TranslationMemory.TranslationProviderServer.Service.ScheduleRecomputeStatistics(TranslationMemory.Entity.Id);
			}
		}

		public override void Refresh()
		{
			if (base.ScheduledOperationEntity.WorkItemUniqueId.HasValue && !base.IsFinished && (base.IsRunning || base.IsPending))
			{
				//base.ScheduledOperationEntity = TranslationMemory.TranslationProviderServer.Service.GetScheduledOperation(base.ScheduledOperationEntity.WorkItemUniqueId.Value);
			}
		}
	}
}
