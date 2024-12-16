using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class ScheduledLanguageResourcesApplyOperation : ScheduledOperation
	{
		public ServerBasedLanguageResourcesTemplate LanguageResourcesTemplate
		{
			get;
			internal set;
		}

		internal ScheduledLanguageResourcesApplyOperation(ScheduledOperationEntity entity)
			: base(entity)
		{
		}

		internal ScheduledLanguageResourcesApplyOperation()
			: this(new ScheduledOperationEntity())
		{
		}

		public override void Queue()
		{
			throw new InvalidOperationException("This operation has already been queued");
		}

		public override void Refresh()
		{
			if (base.ScheduledOperationEntity.WorkItemUniqueId.HasValue && !base.IsFinished && base.IsRunning)
			{
				//base.ScheduledOperationEntity = LanguageResourcesTemplate.TranslationProviderServer.Service.GetScheduledOperation(base.ScheduledOperationEntity.WorkItemUniqueId.Value);
			}
		}
	}
}
