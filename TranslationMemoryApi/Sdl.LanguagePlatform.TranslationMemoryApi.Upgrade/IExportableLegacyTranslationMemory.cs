namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a legacy translation memory that provides functionality to export its content to a TMX file.
	/// </summary>
	public interface IExportableLegacyTranslationMemory : ILegacyTranslationMemory
	{
		/// <summary>
		/// Creates a translation memory exporter, which can be used to export content from the legacy translation memory to a TMX file.
		/// </summary>
		/// <param name="tmxFilePath">The absolute path to export to.</param>
		/// <returns>A translation memory exporter.</returns>
		ITranslationMemoryExporter CreateExporter(string tmxFilePath);
	}
}
