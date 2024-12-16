namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface ILegacyTranslationMemory
	{
		string DisplayName
		{
			get;
		}

		string Url
		{
			get;
		}

		void Check();

		ILegacyTranslationMemorySetup GetSetup();
	}
}
