namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a legacy translation memory, which can be used in a migration project (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject" />).
	/// Explicit inheritors:
	/// <list>
	/// <item><see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IExportableLegacyTranslationMemory" /></item>
	/// <item><see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITmxLegacyTranslationMemory" /></item>
	/// </list>
	/// </summary>
	public interface ILegacyTranslationMemory
	{
		/// <summary>
		/// Gets a friendly description of the translation memory, which can be used to display to the user.
		/// </summary>
		string DisplayName
		{
			get;
		}

		/// <summary>
		/// Gets the url representing the location of the legacy translation memory.
		/// </summary>
		string Url
		{
			get;
		}

		/// <summary>
		/// Checks whether the legacy translation memory can be accessed. Throws an exception if this fails.
		/// </summary>
		void Check();

		/// <summary>
		/// Retrieves the setup information from the legacy translation memory.
		/// </summary>
		/// <remarks>
		/// This setup information may contain invalid legacy fields. It describes the legacy translation memory setup
		/// as it is read from the legacy translation memory and should not be altered for export - that is the responsibility of
		/// the OutputTranslationMemory during populate (output) setup.
		/// </remarks>
		/// <returns>A <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyTranslationMemorySetup" /> object which represents the legacy setup information.</returns>
		ILegacyTranslationMemorySetup GetSetup();
	}
}
