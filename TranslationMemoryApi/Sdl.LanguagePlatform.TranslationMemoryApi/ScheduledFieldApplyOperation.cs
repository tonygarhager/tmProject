using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the asynchronous process of applying a change in a server-based fields template (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate" />)
	/// to all the translation memories associated with it. See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.CurrentFieldApplyOperation" />.
	/// The operation is scheduled when <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedFieldsTemplate.Save" /> is called.
	/// </summary>
	public class ScheduledFieldApplyOperation : ScheduledOperation
	{
		/// <summary>
		/// Gets or sets the server-based translation memory to re-index.
		/// </summary>
		public ServerBasedFieldsTemplate FieldsTemplate
		{
			get;
			internal set;
		}

		internal ScheduledFieldApplyOperation(ScheduledOperationEntity entity)
			: base(entity)
		{
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		internal ScheduledFieldApplyOperation()
			: this(new ScheduledOperationEntity())
		{
		}

		/// <summary>
		/// Schedules the operation for execution by the execution server.
		/// </summary>
		public override void Queue()
		{
			throw new InvalidOperationException("This operation has already been queued");
		}

		/// <summary>
		/// Refreshes the status of this operation with up-to-date information from the server.
		/// </summary>
		public override void Refresh()
		{
			if (base.ScheduledOperationEntity.WorkItemUniqueId.HasValue && !base.IsFinished && base.IsRunning)
			{
				base.ScheduledOperationEntity = FieldsTemplate.TranslationProviderServer.Service.GetScheduledOperation(base.ScheduledOperationEntity.WorkItemUniqueId.Value);
			}
		}
	}
}
