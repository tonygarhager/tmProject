using System;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents an output translation memory to be created within a migration project, to which the contents of one or more legacy translation
	/// memories (or language directions within such TMs) will be migrated. Holds settings required to create the new translation memory (<see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.InputLanguageDirections" />, 
	/// <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.TranslationMemoryCreator" /> and <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.Setup" />) and provides status information on the migration process.
	/// </summary>
	public interface IOutputTranslationMemory
	{
		/// <summary>
		/// The collection of input language direction data, from which content should be imported into the output translation memory.
		/// </summary>
		IInputLanguageDirectionDataCollection InputLanguageDirections
		{
			get;
		}

		/// <summary>
		/// Gets or sets a <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.ITranslationMemoryCreator" /> object, which should be usd for creating the output translation memory.
		/// </summary>
		ITranslationMemoryCreator TranslationMemoryCreator
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the setup information to be used for creating the output translation memory.
		/// </summary>
		ITranslationMemorySetupOptions Setup
		{
			get;
		}

		/// <summary>
		/// Gets the estimated number of translation units this output translation memory will contains after the 
		/// migration is complete. Can be used for progress notifications. This is <code>-1</code> if this number
		/// is not available (or cannot easily be obtained).
		/// </summary>
		int ExpectedTUCount
		{
			get;
		}

		/// <summary>
		/// Gets the newly created translation memory. Returns null before the translation memory has been created.
		/// </summary>
		ITranslationMemory TranslationMemory
		{
			get;
		}

		/// <summary>
		/// Automatically populate the setup information based on the language directions currently present in <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.InputLanguageDirections" />.
		/// This will reset any setup information in the output translation memory that is currently present.
		/// <remarks>All changes to the setup should be done before the output translation memory is created.</remarks>
		/// </summary>
		void PopulateSetup();

		/// <summary>
		/// Creates the output translation memory and import all content.
		/// </summary>
		/// <remarks>This method assumes that all the input translation memories have been exported to TMX and will throw an exception if
		/// this is not the case.</remarks>
		/// <param name="progressEventHandler">A delegate which can be used to monitor progress and to cancel the migration process. May be null.</param>
		void Process(EventHandler<ProgressEventArgs> progressEventHandler);

		/// <summary>
		/// Validates whether this output translation memory is ready to be processed. Throws an xception if it is not.
		/// </summary>
		void Validate();

		/// <summary>
		/// Determines whether the output translation memory is valid. It returns false and a user-friendly localised error message if it is not.
		/// </summary>
		/// <param name="errorMessage">error message</param>
		/// <returns>whether output translation memory valid</returns>
		bool IsValid(out string errorMessage);

		/// <summary>
		/// Creates the output translation memory if it has not been created yet.
		/// </summary>
		void CreateTranslationMemory();

		/// <summary>
		/// Imports the content of the specified input language direction data sets into the output TM.
		/// </summary>
		/// <param name="languageDirectionData">The input language direction data to import.</param>
		/// <param name="progressEventHandler">Progress delegate; can be null.</param>
		void Import(IInputLanguageDirectionData languageDirectionData, EventHandler<ProgressEventArgs> progressEventHandler);
	}
}
