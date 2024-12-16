namespace Sdl.LanguagePlatform.ServerBasedTranslationMemory.Contracts
{
	public static class TranslationMemoryBackgroundTasks
	{
		public const string ImportTranslationUnits = "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks.ImportTranslationUnitsWorkItem, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks";

		public const string ExportTranslationUnits = "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks.ExportTranslationUnitsWorkItem, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks";

		public const string RecomputeFuzzyIndexStatistics = "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks.RecomputeStatisticsWorkItem, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks";

		public const string ReindexTranslationMemory = "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks.ReindexWorkItem, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks";

		public const string ApplyFieldGroupChangeSet = "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks.ApplyFieldGroupChangeSetWorkItem, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks";

		public const string ApplyLanguageResourceChangeSet = "Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks.ApplyLanguageResourceChangeSetWorkItem, Sdl.LanguagePlatform.ServerBasedTranslationMemory.Tasks";
	}
}
