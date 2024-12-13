using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	/// <summary>
	/// Represents a single input translation memory language direction, as well as resource data and 
	/// migration settings within a migration project. It is part of of a possibly multilingual input 
	/// translation memory.
	/// <para>
	/// This object is specific to an output translation memory within the migration project 
	/// (see <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade.IOutputTranslationMemory.InputLanguageDirections" />).
	/// </para>
	/// </summary>
	public interface IInputLanguageDirectionData
	{
		/// <summary>
		/// Gets the possibly multilingual input translation memory from which this input 
		/// language direction data originates.
		/// </summary>
		IInputTranslationMemory TranslationMemory
		{
			get;
		}

		/// <summary>
		/// Gets the data associated with a specific language direction in the legacy translation memory.
		/// </summary>
		ILegacyLanguageDirectionData LanguageDirectionData
		{
			get;
		}

		/// <summary>
		/// Gets or sets the absolute patch where invalid translation units will be written to if they 
		/// fail to import into the output translation memory.
		/// </summary>
		string InvalidTranslationUnitsExportPath
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets whether the translation unit content in this language direction should be imported into the
		/// output translation memory as plain text.
		/// </summary>
		bool ImportPlainText
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the import compatibility settings that determines how the content from this language direction should
		/// be imported into the output translation memory.
		/// </summary>
		ImportSettings.ImportTUProcessingMode ImportCompatibilitySettings
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the import TU update settings that determines how the content from this language direction should
		/// be imported into the output translation memory.
		/// </summary>
		ImportSettings.TUUpdateMode ImportTuUpdateModeSettings
		{
			get;
			set;
		}

		/// <summary>
		/// Gets whether the content of this language direction has successfully been imported into the output translation memory.
		/// </summary>
		bool IsImportComplete
		{
			get;
		}

		/// <summary>
		/// Gets the import statistics from the last import operation into the output translation memory.
		/// </summary>
		ImportStatistics ImportStatistics
		{
			get;
		}
	}
}
