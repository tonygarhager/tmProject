using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.Threading;

namespace Sdl.LanguagePlatform.TranslationMemoryImpl
{
	public interface IReindexableTranslationMemoryService
	{
		bool? GetReindexRequired(Container container, PersistentObjectToken tmId);

		int GetTuCountForReindex(Container container, PersistentObjectToken tmId);

		void SelectiveReindexTranslationUnits(Container container, PersistentObjectToken tmId, CancellationToken token, IProgress<int> progress);
	}
}
