using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IInputLanguageDirectionData
	{
		IInputTranslationMemory TranslationMemory
		{
			get;
		}

		ILegacyLanguageDirectionData LanguageDirectionData
		{
			get;
		}

		string InvalidTranslationUnitsExportPath
		{
			get;
			set;
		}

		bool ImportPlainText
		{
			get;
			set;
		}

		ImportSettings.ImportTUProcessingMode ImportCompatibilitySettings
		{
			get;
			set;
		}

		ImportSettings.TUUpdateMode ImportTuUpdateModeSettings
		{
			get;
			set;
		}

		bool IsImportComplete
		{
			get;
		}

		ImportStatistics ImportStatistics
		{
			get;
		}
	}
}
