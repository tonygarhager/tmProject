using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryImpl.Storage;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	internal class ObjectFactory
	{
		internal static TranslationMemorySetup CreateTranslationMemory(Sdl.LanguagePlatform.TranslationMemoryImpl.Storage.TranslationMemory storageTm, bool isreadonly, ResourceManager mgr)
		{
			TranslationMemorySetup translationMemorySetup = new TranslationMemorySetup
			{
				ResourceId = new PersistentObjectToken(storageTm.Id, storageTm.Guid)
				{
					Guid = storageTm.Guid
				},
				Name = storageTm.Name,
				Description = storageTm.Description,
				Copyright = storageTm.Copyright,
				Recognizers = storageTm.Recognizers,
				TokenizerFlags = storageTm.TokenizerFlags,
				WordCountFlags = storageTm.WordCountFlags,
				FGASupport = storageTm.FGASupport,
				UsesLegacyHashes = (storageTm.DataVersion == 0),
				TextContextMatchType = storageTm.TextContextMatchType,
				IdContextMatch = storageTm.IdContextMatch,
				CreationUser = storageTm.CreationUser,
				CreationDate = storageTm.CreationDate,
				ExpirationDate = storageTm.ExpirationDate,
				LanguageDirection = storageTm.LanguageDirection,
				FuzzyIndexes = storageTm.FuzzyIndexes,
				LastRecomputeDate = storageTm.LastRecomputeDate,
				LastRecomputeSize = storageTm.LastRecomputeSize,
				IsReadOnly = isreadonly,
				CanReverseLanguageDirection = false
			};
			translationMemorySetup.FieldDeclarations = new FieldDefinitions(mgr.GetFields(translationMemorySetup.ResourceId));
			translationMemorySetup.CanReportReindexRequired = storageTm.CanReportReindexRequired;
			return translationMemorySetup;
		}
	}
}
