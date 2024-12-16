namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class BaseTranslationMemoryMigrationManager : IBaseTranslationMemoryMigrationManager
	{
		public IFileBasedTranslationMemoryCreator CreateFileBasedTranslationMemoryCreator()
		{
			return new FileBasedTranslationMemoryCreator();
		}

		public IMigrationProject CreateMigrationProject()
		{
			return new MigrationProject();
		}

		public ITmxLegacyTranslationMemory GetTmxLegacyTranslationMemory(string tmxFilePath)
		{
			return new TmxLegacyTranslationMemory(tmxFilePath);
		}
	}
}
