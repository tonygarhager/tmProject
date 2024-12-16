using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public class InputLanguagePair : IInputLanguageDirectionData
	{
		private readonly IInputTranslationMemory _inputTranslationMemory;

		private readonly ILegacyLanguageDirectionData _legacyLanguagePair;

		private string _invalidTranslationUnitsExportPath;

		private readonly ImportStatistics _importStatistics = new ImportStatistics();

		public IInputTranslationMemory TranslationMemory => _inputTranslationMemory;

		public ILegacyLanguageDirectionData LanguageDirectionData => _legacyLanguagePair;

		public string InvalidTranslationUnitsExportPath
		{
			get
			{
				return _invalidTranslationUnitsExportPath;
			}
			set
			{
				_invalidTranslationUnitsExportPath = value;
			}
		}

		public bool ImportPlainText
		{
			get;
			set;
		}

		public ImportSettings.ImportTUProcessingMode ImportCompatibilitySettings
		{
			get;
			set;
		}

		public ImportSettings.TUUpdateMode ImportTuUpdateModeSettings
		{
			get;
			set;
		}

		public bool IsImportComplete
		{
			get;
			set;
		}

		public ImportStatistics ImportStatistics => _importStatistics;

		public InputLanguagePair(IInputTranslationMemory inputTranslationMemory, ILegacyLanguageDirectionData legacyLanguagePair)
		{
			_inputTranslationMemory = inputTranslationMemory;
			_legacyLanguagePair = legacyLanguagePair;
		}
	}
}
