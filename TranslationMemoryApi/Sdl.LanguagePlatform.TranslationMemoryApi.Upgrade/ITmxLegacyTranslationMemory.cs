namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a TMX file which can be used as input to a migration project.
	/// </summary>
	public interface ITmxLegacyTranslationMemory : ILegacyTranslationMemory
	{
		/// <summary>
		/// The absolute path of the TMX file.
		/// </summary>
		string TmxFilePath
		{
			get;
		}
	}
}
