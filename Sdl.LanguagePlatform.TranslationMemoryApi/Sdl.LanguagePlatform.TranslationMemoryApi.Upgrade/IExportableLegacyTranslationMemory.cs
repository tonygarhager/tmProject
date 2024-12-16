namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IExportableLegacyTranslationMemory : ILegacyTranslationMemory
	{
		ITranslationMemoryExporter CreateExporter(string tmxFilePath);
	}
}
