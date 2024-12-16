namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface IFileBasedTranslationMemoryCreator : ITranslationMemoryCreator
	{
		string TranslationMemoryDirectory
		{
			get;
			set;
		}
	}
}
