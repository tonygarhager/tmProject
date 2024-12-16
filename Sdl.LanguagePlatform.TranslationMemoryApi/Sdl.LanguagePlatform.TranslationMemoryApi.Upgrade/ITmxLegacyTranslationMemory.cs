namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface ITmxLegacyTranslationMemory : ILegacyTranslationMemory
	{
		string TmxFilePath
		{
			get;
		}
	}
}
