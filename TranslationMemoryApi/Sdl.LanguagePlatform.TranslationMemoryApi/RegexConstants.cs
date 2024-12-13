namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	internal static class RegexConstants
	{
		internal const string MinValidName = "[^\\\\/\"<>\\|\\*\\?%]+";

		internal const string MinValidNameErrorMessage = "The following characters ^ \\ / \\\"  < > | * ? %  are not allowed";

		internal const string VALID_WORD = "^[\\w\\s-']*";

		internal const string VALID_DATABASE_NAME = "^[\\p{L}_][\\w_@#\\$]*$";
	}
}
