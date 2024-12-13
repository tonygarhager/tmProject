using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a legacy translation memory that is part of a migration project (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IMigrationProject" />).
	/// This has a reference to legacy translation memory (see <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyTranslationMemory" />, provides access to theis translation memory's
	/// setup information and holds status information about whether the necessary content has been exported to TMX yet (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.TmxFileStatus" />
	/// and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.TmxFilePath" />).
	/// </summary>
	public interface IInputTranslationMemory
	{
		/// <summary>
		/// Gets the legacy translation memory which this input translation memory represents.
		/// </summary>
		ILegacyTranslationMemory TranslationMemory
		{
			get;
		}

		/// <summary>
		/// Gets the setup information from the legacy translation memory. This information is initially retrieved
		/// through <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ILegacyTranslationMemory.GetSetup" /> and then cached for easy access throughout the migration process.
		/// </summary>
		ILegacyTranslationMemorySetup Setup
		{
			get;
		}

		/// <summary>
		/// Gets or sets the absolute TMX file path to which to export the legacy translation memory's content.
		/// </summary>
		/// <remarks>Setting this property will update the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.TmxFileStatus" /> property, depending on whether the file exists.</remarks>
		/// <exception cref="T:System.ArgumentNullException">Thrown when setting this property to null or an empty string.</exception>
		/// <exception cref="T:System.InvalidOperationException">Thrown when attempting to set this property if 
		/// the legacy translation memory is a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITmxLegacyTranslationMemory" />.</exception>
		string TmxFilePath
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the status of the TMX export file specified in <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.TmxFilePath" />. A call to <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.ExportTmxFile(System.EventHandler{Sdl.LanguagePlatform.TranslationMemoryApi.BatchExportedEventArgs})" /> will update the value
		/// of this property.
		/// </summary>
		TmxFileStatus TmxFileStatus
		{
			get;
		}

		/// <summary>
		/// Exports the legacy TM's data to the path set in the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.TmxFilePath" /> property.
		/// </summary>
		/// <param name="progressEventHandler">A progress event handler which can be used to 
		/// monitor progress or cancel the export operation.</param>
		/// <exception cref="T:System.InvalidOperationException">Thrown when attempting to call this method if 
		/// the legacy translation memory is a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITmxLegacyTranslationMemory" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">Thrown when calling this method before 
		/// the <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IInputTranslationMemory.TmxFilePath" /> property is set.</exception>
		void ExportTmxFile(EventHandler<BatchExportedEventArgs> progressEventHandler);
	}
}
