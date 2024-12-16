namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IBaseTranslationMemoryMigrationManager
	{
		IMigrationProject CreateMigrationProject();

		ITmxLegacyTranslationMemory GetTmxLegacyTranslationMemory(string tmxFilePath);

		IFileBasedTranslationMemoryCreator CreateFileBasedTranslationMemoryCreator();
	}
}
