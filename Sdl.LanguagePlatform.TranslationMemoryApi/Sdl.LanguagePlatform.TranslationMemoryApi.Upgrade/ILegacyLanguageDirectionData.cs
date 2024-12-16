namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface ILegacyLanguageDirectionData
	{
		LegacyLanguage SourceLanguage
		{
			get;
		}

		LegacyLanguage TargetLanguage
		{
			get;
		}

		int TranslationUnitCount
		{
			get;
		}

		ILegacyLanguageResources[] AvailableLanguageResources
		{
			get;
		}

		ILegacyLanguageResources SuggestedLanguageResources
		{
			get;
		}
	}
}
