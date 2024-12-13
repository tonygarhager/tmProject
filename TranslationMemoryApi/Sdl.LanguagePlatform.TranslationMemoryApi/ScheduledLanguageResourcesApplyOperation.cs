using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts.Entities;
using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents the asynchronous process of applying a change in a server-based language resources template (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate" />)
	/// to all the translation memories associated with it. See <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.CurrentLangResApplyOperation" />.
	/// The operation is scheduled when <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ServerBasedLanguageResourcesTemplate.Save" /> is called.
	/// </summary>
	public class ScheduledLanguageResourcesApplyOperation : ScheduledOperation
	{
		/// <summary>
		/// Gets or sets the server-based translation memory to re-index.
		/// </summary>
		public ServerBasedLanguageResourcesTemplate LanguageResourcesTemplate
		{
			get;
			internal set;
		}

		internal ScheduledLanguageResourcesApplyOperation(ScheduledOperationEntity entity)
			: base(entity)
		{
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		internal ScheduledLanguageResourcesApplyOperation()
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
				base.ScheduledOperationEntity = LanguageResourcesTemplate.TranslationProviderServer.Service.GetScheduledOperation(base.ScheduledOperationEntity.WorkItemUniqueId.Value);
			}
		}
	}
}
