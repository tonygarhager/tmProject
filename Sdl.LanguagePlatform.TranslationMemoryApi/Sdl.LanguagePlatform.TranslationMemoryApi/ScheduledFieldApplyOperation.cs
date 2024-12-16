using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	public class ScheduledFieldApplyOperation : ScheduledOperation
	{
		public ServerBasedFieldsTemplate FieldsTemplate
		{
			get;
			internal set;
		}

		internal ScheduledFieldApplyOperation(ScheduledOperationEntity entity)
			: base(entity)
		{
		}

		internal ScheduledFieldApplyOperation()
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
				//base.ScheduledOperationEntity = FieldsTemplate.TranslationProviderServer.Service.GetScheduledOperation(base.ScheduledOperationEntity.WorkItemUniqueId.Value);
			}
		}
	}
}
