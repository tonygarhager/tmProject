using Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl;
using System;
using System.Globalization;
using System.Linq;

namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal class TMLanguageResourceUpdater
	{
		private ITranslationMemoryService _service;

		private PersistentObjectToken _translationMemoryId;

		private Container _container;

		public ITranslationMemoryService Service => _service;

		public PersistentObjectToken TranslationMemoryId => _translationMemoryId;

		public TMLanguageResourceUpdater(ITranslationMemoryService service, Container container, PersistentObjectToken tmId)
		{
			_container = container;
			_service = service;
			_translationMemoryId = tmId;
		}

		public void UpdateTm(LanguageResourceGroupChangeSet changeSet)
		{
			LanguageResource[] translationMemoryLanguageResources = _service.GetTranslationMemoryLanguageResources(_container, _translationMemoryId, includeData: false);
			foreach (LanguageResourceUpdate languageResourceUpdate in changeSet.LanguageResourceUpdates)
			{
				switch (languageResourceUpdate.UpdateType)
				{
				case LanguageResourceUpdateType.Add:
				{
					LanguageResource languageResource3 = new LanguageResource();
					languageResource3.CultureName = languageResourceUpdate.CultureName;
					languageResource3.Data = languageResourceUpdate.Data;
					languageResource3.Type = languageResourceUpdate.Type;
					PersistentObjectToken resourceId = _service.CreateLanguageResource(_container, languageResource3);
					_service.AssignLanguageResourceToTranslationMemory(_container, resourceId, _translationMemoryId);
					break;
				}
				case LanguageResourceUpdateType.Update:
				{
					LanguageResource languageResource2 = translationMemoryLanguageResources.FirstOrDefault((LanguageResource lr) => lr.CultureName.Equals(languageResourceUpdate.CultureName, StringComparison.OrdinalIgnoreCase) && lr.Type == languageResourceUpdate.Type);
					if (languageResource2 == null)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "No language resource for language {0} and type {1} found to update.", languageResourceUpdate.CultureName, languageResourceUpdate.Type));
					}
					languageResource2.CultureName = languageResourceUpdate.CultureName;
					languageResource2.Data = languageResourceUpdate.Data;
					languageResource2.Type = languageResourceUpdate.Type;
					_service.UpdateLanguageResource(_container, languageResource2);
					break;
				}
				case LanguageResourceUpdateType.Delete:
				{
					LanguageResource languageResource = translationMemoryLanguageResources.FirstOrDefault((LanguageResource lr) => lr.CultureName.Equals(languageResourceUpdate.CultureName, StringComparison.OrdinalIgnoreCase) && lr.Type == languageResourceUpdate.Type);
					_service.UnassignLanguageResourceFromTranslationMemory(_container, languageResource.ResourceId, _translationMemoryId);
					_service.DeleteLanguageResource(_container, languageResource.ResourceId);
					break;
				}
				default:
					throw new ArgumentException("Unexpected language resource update type: " + languageResourceUpdate.UpdateType.ToString());
				}
			}
		}
	}
}
