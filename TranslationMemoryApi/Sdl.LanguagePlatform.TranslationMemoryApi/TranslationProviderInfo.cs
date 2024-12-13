namespace Sdl.LanguagePlatform.TranslationMemoryApi
{
	/// <summary>
	/// Represents general information about a translation provider (<see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider" />).
	/// </summary>
	public class TranslationProviderInfo
	{
		/// <summary>
		/// Gets the name of the translation provider.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the translation method used by the translation provider.
		/// </summary>
		public TranslationMethod TranslationMethod
		{
			get;
			set;
		}
	}
}
