namespace Sdl.LanguagePlatform.TranslationMemoryApi.Upgrade
{
	public interface ITranslationMemoryCreator
	{
		string DisplayName
		{
			get;
		}

		int MaximumNameLength
		{
			get;
		}

		int MaximumCopyrightLength
		{
			get;
		}

		int MaximumDescriptionLength
		{
			get;
		}

		ITranslationMemory CreateEmptyTranslationMemory(ITranslationMemorySetupOptions setup);

		bool IsValid(out string errorMessage);

		bool IsValidName(string translationMemoryName, out string errorMessage);

		bool IsValidCopyright(string translationMemoryCopyright, out string errorMessage);

		bool IsValidDescription(string translationMemoryDescription, out string errorMessage);
	}
}
