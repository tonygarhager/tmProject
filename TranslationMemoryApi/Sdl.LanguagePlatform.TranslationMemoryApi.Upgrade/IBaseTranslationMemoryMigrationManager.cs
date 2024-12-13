namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents the entry point to the translation memory migration API and the provided functionality.
	/// It provides methods to create a migration project (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject" />), and obtain
	/// representations of various legacy translation memories (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyTranslationMemory" /> which
	/// are the input of a migration project, and
	/// provides methods to obtain translation memory creators (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITranslationMemoryCreator" />), which
	/// are used to create the output TM of a migration project.
	/// </summary>
	public interface IBaseTranslationMemoryMigrationManager
	{
		/// <summary>
		/// Creates a new, empty migration project.
		/// </summary>
		/// <returns>A new migration project.</returns>
		IMigrationProject CreateMigrationProject();

		/// <summary>
		/// Obtains a representation of a TMX-file-based translation memory to be used as input to a migration project.
		/// <remarks>The file should end in a "tmx" extension, but can optionally be compressed 
		/// (gzip, extension .gz).</remarks>
		/// </summary>
		/// <param name="tmxFilePath">The absolute path to the TMX file.</param>
		/// <returns>A new <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITmxLegacyTranslationMemory" /> object.</returns>
		ITmxLegacyTranslationMemory GetTmxLegacyTranslationMemory(string tmxFilePath);

		/// <summary>
		/// Obtains a translation memory creator, which can be used to create file-based non-legacy translation memories (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.TranslationMemoryCreator" />).
		/// </summary>
		/// <remarks>Before this creator can be used, the required properties on it must be set.</remarks>
		/// <returns>A new <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IFileBasedTranslationMemoryCreator" /> object.</returns>
		IFileBasedTranslationMemoryCreator CreateFileBasedTranslationMemoryCreator();
	}
}
